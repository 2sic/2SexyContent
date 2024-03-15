﻿using System.IO;
using System.Reflection;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code.Gen.Internal.Generate;
using ToSic.Sxc.Code.Internal.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeControllerReal(FileSaver fileSaver, LazySvc<IJsonService> json, LazySvc<IEnumerable<IFileGenerator>> generators) : ServiceBase("Api.CodeRl")
{
    public const string LogSuffix = "Code";

    public class HelpItem
    {
        // the name of the class
        public string Term { get; set; }
        // message from the attribute
        public string[] Help { get; set; }


        // not supported in System.Text.Json
        //// ignore some of serialization exceptions
        //// https://www.newtonsoft.com/json/help/html/serializationerrorhandling.htm
        //[OnError]
        //internal void OnError(StreamingContext context, ErrorContext errorContext)
        //{
        //    errorContext.Handled = true;
        //}
    }

    public IEnumerable<HelpItem> InlineHelp(string language)
    {
        var l = Log.Fn<IEnumerable<HelpItem>>($"InlineHelp:l:{language}", timer: true);

        if (_inlineHelp != null) return l.ReturnAsOk(_inlineHelp);

        // TODO: stv# how to use languages?

        try
        {
            _inlineHelp = AssemblyHandling.GetTypes(Log)
                .Where(t => t?.IsDefined(typeof(DocsAttribute)) ?? false)
                .Select(t => new HelpItem
                {
                    Term = t?.Name,
                    Help = t?.GetCustomAttribute<DocsAttribute>()?.GetMessages(t?.FullName)
                }).ToArray();
        }
        catch (Exception e)
        {
            l.A("Exception in inline help.");
            l.Ex(e);
        }

        return l.ReturnAsOk(_inlineHelp);
    }
    private static IEnumerable<HelpItem> _inlineHelp;

    public RichResult GenerateDataModels(int appId, string edition)
    {
        var l = Log.Fn<RichResult>($"{nameof(appId)}:{appId};{nameof(edition)}:{edition}", timer: true);

        try
        {
            fileSaver.GenerateAndSaveFiles(new() { AppId = appId, Edition = edition });

            return l.Return(new RichResult
                {
                    Ok = true,
                    Message = $"Data models generated in {edition}/AppCode/Data.",
                }
                .WithTime(l)
            );
        }
        catch (Exception e)
        {
            return l.Return(new RichResult
                {
                    Ok = false,
                    Message = $"Error generating data models in {edition}/AppCode/Data. {e.GetType().FullName} - {e.Message}",
                }
                .WithTime(l)
            );
        }
    }

    public EditionsDto GetEditions(int appId)
    {
        var l = Log.Fn<EditionsDto>($"{nameof(appId)}:{appId}");

        // get generators
        var fileGenerators = generators.Value.Select(g => new GeneratorDto(g)).ToList();

        var pathToDotAppJson = fileSaver.GetPathToDotAppJson(new() { AppId = appId });
        l.A($"path to app.json: {pathToDotAppJson}");
        if (File.Exists(pathToDotAppJson))
        {
            l.A($"has app.json");
            var editionsJson = json.Value.To<EditionsJson>(File.ReadAllText(pathToDotAppJson));

            if (editionsJson?.Editions?.Count > 0)
            {
                l.A($"has editions in app.json: {editionsJson?.Editions?.Count}");
                return l.ReturnAsOk(editionsJson.ToEditionsDto(fileGenerators));
            }
        }

        l.A("editions are not specified, so using default edition data");
        // default data
        var nothingSpecified = new EditionsDto
        {
            Ok = true,
            IsConfigured = false,
            Editions = [ new() { Name = "", Description = "Root edition", IsDefault = true } ],
            Generators = fileGenerators
        };

        return l.Return(nothingSpecified, "editions not specified in app.json");
    }
}
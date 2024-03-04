﻿using System.IO;
using System.Reflection;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code.Internal.Documentation;
using ToSic.Sxc.Code.Internal.Generate;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeControllerReal(DataClassesGenerator classesGenerator, LazySvc<IJsonService> json) : ServiceBase("Api.CodeRl")
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
        var wrapLog = Log.Fn<IEnumerable<HelpItem>>($"InlineHelp:l:{language}", timer: true);

        if (_inlineHelp != null) return wrapLog.ReturnAsOk(_inlineHelp);

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
            Log.A("Exception in inline help.");
            Log.Ex(e);
        }

        return wrapLog.ReturnAsOk(_inlineHelp);
    }
    private static IEnumerable<HelpItem> _inlineHelp;

    public RichResult GenerateDataModels(int appId, string edition)
    {
        var l = Log.Fn<RichResult>($"{nameof(appId)}:{appId};{nameof(edition)}:{edition}", timer: true);

        var dataModelGenerator = classesGenerator.Setup(appId, edition);
        dataModelGenerator.GenerateAndSaveFiles();

        return l.Return(new RichResult
            {
                Ok = true,
                Message = $"Data models generated in {edition}/AppCode/Data.",
            }
            .WithTime(l)
        );
    }

    public EditionsDto GetEditions(int appId)
    {
        var l = Log.Fn<EditionsDto>($"{nameof(appId)}:{appId}");

        var pathToDotAppJson = classesGenerator.Setup(appId).GetPathToDotAppJson();
        l.A($"path to app.json: {pathToDotAppJson}");
        if (File.Exists(pathToDotAppJson))
        {
            l.A($"has app.json");
            var editionsJson = json.Value.To<EditionsJson>(File.ReadAllText(pathToDotAppJson));

            if (editionsJson?.Editions?.Count > 0)
            {
                l.A($"has editions in app.json: {editionsJson?.Editions?.Count}");
                return l.ReturnAsOk(editionsJson.ToEditionsDto());
            }
        }

        l.A("editions are not specified, so using default edition data");
        // default data
        var nothingSpecified = new EditionsDto
        {
            Ok = true,
            IsConfigured = false,
            Editions = [ new() { Name = "", Description = "Root edition" } ]
        };

        return l.Return(nothingSpecified, "editions not specified in app.json");
    }
}
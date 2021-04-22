﻿using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code.Builder;
using ToSic.Sxc.Oqt.Server.Plumbing;
using File = System.IO.File;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /// <summary>
    /// Manage app api controller compilation and registration so we can invoke action latter.
    /// </summary>
    public class AppApiControllerManager: IHasLog
    {
        private readonly ConcurrentDictionary<string, bool> _compiledAppApiControllers;
        private readonly ApplicationPartManager _partManager;
        public AppApiControllerManager(ApplicationPartManager partManager, AppApiFileSystemWatcher appApiFileSystemWatcher)
        {
            _partManager = partManager;
            _compiledAppApiControllers = appApiFileSystemWatcher.CompiledAppApiControllers;
            Log = new Log(HistoryLogName, null, "AppApiControllerManager");
            History.Add(HistoryLogGroup, Log);
        }

        public ILog Log { get; }

        protected string HistoryLogGroup { get; } = "app-api";

        protected string HistoryLogName => "Controller.Manager";

        /// <summary>
        /// Compile and register dyncode app api controller (for new or updated app api).
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public async ValueTask<bool> PrepareController(RouteValueDictionary values)
        {
            var wrapLog = Log.Call<bool>();

            var apiFile = (string)values["apiFile"];
            var dllName = (string)values["dllName"];

            // If we have a key (that controller is compiled and registered, but not updated) controller was prepared before, so just return values.
            // Alternatively remove older version of AppApi controller (if we got updated flag from file system watcher).
            if (_compiledAppApiControllers.TryGetValue(apiFile, out var updated))
            {
                Log.Add($"_compiledAppApiControllers have value: {updated} for: {apiFile}.");
                if (updated)
                    RemoveController(dllName, apiFile);
                else
                    return wrapLog(
                        $"ok, nothing to do, AppApi Controller is already compiled and added to ApplicationPart: {apiFile}.",
                        true);
            }

            Log.Add($"We need to prepare controller for: {apiFile}.");

            // Check for AppApi file
            if (!File.Exists(apiFile))
                throw new IOException($"Error, missing AppApi file {apiFile}.");

            // note: this may look like something you could optimize/cache the result, but that's a bad idea
            // because when the file changes, the type-object will be different, so please don't optimize :)

            // Check for AppApi source code
            var apiCode = await File.ReadAllTextAsync(apiFile);
            if (string.IsNullOrWhiteSpace(apiCode))
                throw new IOException($"Error, missing AppApi code in file {apiFile}.");

            // Build new AppApi Controller
            Log.Add($"Compile assembly: {apiFile}, {dllName}");
            var assembly = new Compiler().Compile(apiFile, dllName);

            // Add new key to concurrent dictionary, before registering new AppAPi controller.
            if (!_compiledAppApiControllers.TryAdd(apiFile, false))
                throw new IOException($"Error, while adding key {apiFile} to concurrent dictionary, so will not register AppApi Controller to avoid duplicate controller routes.");

            // Register new AppApi Controller.
            AddController(dllName, assembly);

            return wrapLog($"ok, Controller is compiled and added to ApplicationParts: {apiFile}.", true);
        }

        private void AddController(string dllName, Assembly assembly)
        {
            Log.Add($"Add ApplicationPart: {dllName}");
            _partManager.ApplicationParts.Add(new CompilationReferencesProvider(assembly));
            // Notify change
            NotifyChange();
        }

        private void RemoveController(string dllName, string apiFile)
        {
            Log.Add($"In ApplicationParts, find AppApi controller: {dllName}.");
            var applicationPart = _partManager.ApplicationParts.FirstOrDefault(a => a.Name.Equals($"{dllName}.dll"));
            if (applicationPart != null)
            {
                Log.Add($"From ApplicationParts, remove AppApi controller: {dllName}.");
                _partManager.ApplicationParts.Remove(applicationPart);
                NotifyChange();

                Log.Add(_compiledAppApiControllers.TryRemove(apiFile, out var removeValue)
                    ? $"Value removed: {removeValue} for {apiFile}."
                    : $"Error, can't remove value for {apiFile}.");
            }
            else
            {
                Log.Add($"In ApplicationParts, can't find AppApi controller: {dllName}");
            }
        }

        private static void NotifyChange()
        {
            // Notify change
            AppApiActionDescriptorChangeProvider.Instance.HasChanged = true;
            AppApiActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToSic.Sxc.Code.Internal
{
    public class AssemblyResult(
        Assembly assembly = null,
        string errorMessages = null,
        string[] assemblyLocations = null,
        string safeClassName = null,
        Type mainType = default)
    {
        public Assembly Assembly { get; } = assembly;
        public string ErrorMessages { get; } = errorMessages;
        public string[] AssemblyLocations { get; } = assemblyLocations;
        public string SafeClassName { get; } = safeClassName;

        /// <summary>
        /// The main type of this assembly - typically for Razor files which usually just publish a single type.
        /// This is to speed up performance, so the user of it doesn't need to find it again. 
        /// </summary>
        public Type MainType => mainType;

        /// <summary>
        /// The list of folders which must be watched for changes when using this assembly.
        /// ATM just used for AppCode assemblies, should maybe be in a inheriting class...
        /// </summary>
        // WIP - should be more functional, this get/set is still hacky
        public IList<string> WatcherFolders { get; set; }

        public AssemblyResult((Assembly Assembly, string ErrorMessages) tuple) : this(tuple.Assembly, tuple.ErrorMessages)
        { }

    }

    public static class AssemblyResultExtensions
    {
        //public static AssemblyResult ToAssemblyResult(this (Assembly Assembly, string ErrorMessages) tuple)
        //    => new(tuple);

        public static (Assembly Assembly, string ErrorMessages) ToTuple(this AssemblyResult assemblyResult)
            => (assemblyResult.Assembly, assemblyResult.ErrorMessages);
    }


}
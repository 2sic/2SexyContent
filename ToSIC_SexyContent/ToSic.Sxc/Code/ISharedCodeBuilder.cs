﻿using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Marks objects - usually DynamicCode - which can create instances of other C# files. <br/>
    /// A special feature is that it must store a reference to the path it's in (provided by the compiler that created this instance).
    /// This is important, so that CreateInstance knows what path to start in. 
    /// </summary>
    [PublicApi]
    public interface ICreateInstance
    {
        /// <summary>
        /// Location of the current code. This is important when trying to create instances for
        /// other code in relative folders - as this is usually not known. 
        /// </summary>
        /// <returns>The real path to the currently executed code - important for dynamically compiled code like WebApis</returns>
        string CreateInstancePath { get; set; }

        /// <summary>
        /// Create an instance of code lying in a file near this
        /// </summary>
        /// <param name="virtualPath">path to the other code file to compile</param>
        /// <param name="dontRelyOnParameterOrder">dummy parameter to ensure all other parameters are called using named parameters, so that the API can change in future</param>
        /// <param name="name">Override the class name to compile - usually not required as it should match the file name</param>
        /// <param name="relativePath">optional relative path, will usually use the <see cref="CreateInstancePath"/></param>
        /// <param name="throwOnError">throw errors if compiling fails, recommended</param>
        /// <returns>An object of the class in the file</returns>
        /// <remarks>Note that the C# code which we are creating an instance of inherits from <see cref="DynamicCodeChild"/> then it will automatically be initialized to support App, AsDynamic etc.</remarks>
        dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string name = null,
            string relativePath = null,
            bool throwOnError = true);

    }
}

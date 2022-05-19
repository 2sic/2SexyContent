﻿using System;
using System.IO;
using System.Web;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn
{
    public class RazorCodeManager: HasLog
    {
        public RazorComponentBase Parent;
        public RazorCodeManager(RazorComponentBase parent): base("Rzr.Code", parent.Log)
        {
            Parent = parent;
        }

        /// <summary>
        /// The compiled code - or null
        /// </summary>
        private object _code;

        /// <summary>
        /// Determines if code has been compiled (or at least attempted)
        /// </summary>
        protected bool BuildComplete;

        /// <summary>
        /// Copy of any exception thrown when compiling the code
        /// </summary>
        protected Exception BuildException;

        /// <summary>
        ///  This tries to get the code and will show an exception if not ready. 
        /// </summary>
        public dynamic CodeOrException
        {
            get
            {
                TryToBuildCode();
                if (BuildException == null) return _code;
                throw ImproveExceptionMessage(BuildException);
            }
        }

        /// <summary>
        /// Internal accessor for the code, which does not throw exceptions but returns a null if not available
        /// </summary>
        internal dynamic CodeOrNull
        {
            get
            {
                TryToBuildCode();
                return _code;
            }
        }

        /// <summary>
        /// Try to build the code. If something fails, remember the exception in case we need it later.
        /// </summary>
        private void TryToBuildCode()
        {
            if (BuildComplete) return;
            var wrapLog = Log.Call();
            var codeFile = Parent.VirtualPath.Replace(".cshtml", ".code.cshtml");
            Log.A($"Will try to load code from '{codeFile}");
            try
            {
                var compiled  = Parent.CreateInstance(codeFile);
                if (compiled != null && !(compiled is RazorComponentCode))
                {
                    throw new Exception(
                        $"Tried to compile the .Code file, but the type is '{compiled.GetType().Name}'. " +
                        $"Expected that it inherits from '{nameof(RazorComponentCode)}'. " +
                        "Please add '@inherits ToSic.Sxc.Dnn.RazorComponentCode' to the beginning of the 'xxx.code.cshtml' file. ");
                }

                _code = compiled;
            }
            catch (Exception e)
            {
                BuildException = e;
            }

            BuildComplete = true;
            wrapLog(null);
        }

        private static Exception ImproveExceptionMessage(Exception innerException)
        {
            switch (innerException)
            {
                case FileNotFoundException _:
                    return new Exception("Tried to compile matching .Code file - but couldn't find it. \n", innerException);
                case HttpCompileException _:
                    return new Exception("Error compiling .Code file. \n", innerException);
                default:
                    return innerException;
            }
        }
    }
}

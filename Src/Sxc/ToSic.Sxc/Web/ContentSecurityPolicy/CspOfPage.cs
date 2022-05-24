﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class CspOfPage: HasLog
    {
        public CspOfPage(): base(CspConstants.LogPrefix + ".Page")
        {
        }

        public List<CspParameters> CspParameters { get; } = new List<CspParameters>();

        public void Add(IList<CspParameters> additional) => CspParameters.AddRange(additional);

        /// <summary>
        /// Name of the CSP header to be added, based on the report-only aspect
        /// </summary>
        public string HeaderName(bool isEnforced) => isEnforced ? CspConstants.CspHeaderNamePolicy : CspConstants.CspHeaderNameReport;


        public string CspHttpHeader()
        {
            try
            {
                var wrapLog = Log.Fn<string>();
                var relevant = CspParameters.Where(cs => cs != null).ToList();
                if (!relevant.Any()) return wrapLog.ReturnNull("none relevant");
                var mergedPolicy = relevant.First();

                var finalizer = new CspParameterFinalizer().Init(Log);

                if (relevant.Count == 1)
                    return wrapLog.Return(finalizer.Finalize(mergedPolicy).ToString(), "found 1");

                // Pre-copy, so we never change the original!
                mergedPolicy = new CspParameters(mergedPolicy);

                // If many, merge the settings of each additional policy list
                foreach (var cspS in relevant.Skip(1))
                    mergedPolicy.Add(cspS);

                return wrapLog.Return(finalizer.Finalize(mergedPolicy).ToString(), "merged");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


    }
}
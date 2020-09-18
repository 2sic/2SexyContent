﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Run;
using Exception = System.Exception;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class InstallationController: HasLog, IEnvironmentInstaller
    {
        public bool SaveUnimportantDetails = true;

        private readonly DnnInstallLogger _installLogger;


        /// <summary>
        /// Instance initializers...
        /// </summary>
        public InstallationController(): base("Dnn.InstCo")
        {
            _installLogger = new DnnInstallLogger(SaveUnimportantDetails);
            History.Add("installation", Log);
        }

        public IEnvironmentInstaller Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }

        internal string UpgradeModule(string version)
        {
            // Check if table "ToSIC_SexyContent_Templates" exists. 
            // If it's gone, then PROBABLY skip all upgrade-codes incl. 8.11!
            var sql = @"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_SexyContent_Templates]') AND TYPE IN(N'U')";
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
            sqlConnection.Open();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            var runDbChangesUntil811 = (int)sqlCommand.ExecuteScalar() == 1; // if there is one result row, this means the templates table still exists, we need to run changes before 08.11
            sqlConnection.Close();

            // if version is 01.00.00, the upgrade has to run because log files should be cleared
            if (!runDbChangesUntil811 && version != "01.00.00" && (new Version(version) <= new Version(8,11,0)))
            {
                _installLogger.LogStep(version, "Upgrade skipped because 00.99.00 install detected (installation of everything until and including 08.11 has been done by 00.99.00.SqlDataProvider)", true);
                _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
                return version;
            }

            _installLogger.LogStep(version, "UpgradeModule starting", false);

            // Configure Unity / eav, etc.
            Settings.EnsureSystemIsInitialized();
            // new UnityConfig().Configure();

            // Abort upgrade if it's already done - if version is 01.00.00, the module has probably been uninstalled - continue in this case.
            if (version != "01.00.00" && IsUpgradeComplete(version, "- Check on Start UpgradeModule"))
            {
                _installLogger.LogStep(version, "Apparently trying to update this version, but this versions upgrade is apparently compeleted, will abort");
                throw new Exception("2sxc upgrade for version " + version +
                                    " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");
            }
            _installLogger.LogStep(version, "version / upgrade-complete test passed");

            if (IsUpgradeRunning)
            {
                _installLogger.LogStep(version, "Apparently upgrade is running, will abort");
                throw new Exception("2sxc upgrade for version " + version +
                                    " started, but the upgrade is already running. Aborting upgrade.");
            }
            _installLogger.LogStep(version, "is-upgrade-running test passed");

            IsUpgradeRunning = true;
            _installLogger.LogStep(version, "----- Upgrade to " + version + " started -----");

            try
            {

                switch (version)
                {
                    case "01.00.00": // Make sure that log folder empty on new installations (could happen if 2sxc was already installed on a system)
                        MaybeResetUpgradeLogsToStartAgainFromV1();
                        break;
                    case "07.02.00":
                    case "07.02.02":
                    case "07.03.01":
                    case "07.03.03":
                    case "08.00.02":
                    case "08.00.04":
                    case "08.00.07":
                    case "08.01.00":
                    case "08.03.00":
                    case "08.03.02":
                    case "08.03.03":
                    case "08.03.05":
                    case "08.04.00":
                    case "08.04.03":
                    case "08.04.05":
                    case "08.05.00":
                    case "08.05.01":
                    case "08.05.02":
                    case "08.05.03":
                    case "08.05.05":
                    case "08.11.00":
                        throw new Exception("Trying to upgrade a 7 or 8 version - which isn't supported in v9.20+. Please upgrade to the latest 8.12 or 9.15before trying to upgrade to a 9.20+");

                    // case "1X.xx.xx":
                    //Helpers.ImportXmlSchemaOfVersion("1X.xx.xx", false);
                    //new V9(version, _installLogger, Log).Version09xxxx();
                    // warning!!! when you add a new case, make sure you upgrade the version number on Settings.Installation.LastVersionWithServerChanges!!!
                }
                _installLogger.LogStep(version, "version-list check / switch done", false);

                // Increase ClientDependency version upon each upgrade (System and all Portals)
                // prevents browsers caching old JS and CSS files for editing, which could cause several errors
                // only set this on the last upgraded version, to prevent crazy updating the client-resource-cache while upgrading
                if (version == Settings.Installation.UpgradeVersionList.Last())
                {
                    _installLogger.LogStep(version, "ClientResourceManager- seems to be last item in version-list, will clear");

                    ClientResourceManager.UpdateVersion();
                    _installLogger.LogStep(version, "ClientResourceManager- done clearing");

                    UpdateUpgradeCompleteStatus();
                    _installLogger.LogStep(version, "updated upgrade-complete status");
                }

                _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
                _installLogger.LogStep(version, "----- Upgrade to " + version + " completed -----");

            }
            catch (Exception e)
            {
                _installLogger.LogStep(version, "Upgrade failed - " + e.Message);
                throw;
            }
            finally
            {
                IsUpgradeRunning = false;
            }
            _installLogger.LogStep(version, "UpgradeModule done / returning");
            return version;
        }

        private void MaybeResetUpgradeLogsToStartAgainFromV1()
        {
            _installLogger.LogStep("", "Maybe reset logs start");
            // this condition only applies, if 2sxc upgrade 7 didn't happen yet
            var appState = State.Get(new AppIdentity(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId));
            if (appState.GetContentType("2SexyContent-Template") != null) return;

            _installLogger.LogStep("", "Will reset all logs now");
            _installLogger.DeleteAllLogFiles();
            _installLogger.LogStep("", "Maybe Reset logs done");
        }


    }
}
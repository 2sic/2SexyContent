﻿namespace ToSic.Sxc.Dnn.Install
{
    public partial class InstallationController
    {
        #region Static Constructors to ensure installation

        /// <summary>
        /// This static initializer will do a one-time check to see if everything is ready,
        /// so subsequent access to this property will not need to do anything any more
        /// </summary>
        static InstallationController() => UpdateUpgradeCompleteStatus();

        private static void UpdateUpgradeCompleteStatus()
            => UpgradeComplete = new InstallationController()
                .IsUpgradeComplete(Settings.Installation.LastVersionWithServerChanges, "- static check");

        internal static bool UpgradeComplete;

        #endregion
    }
}

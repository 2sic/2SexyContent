﻿using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract class AdamState: HasLog
    {
        // Temp
        public abstract AppRuntime AppRuntime { get; }

        /// <summary>
        /// Determines if the files come from the root (shared files).
        /// Is false, if they come from the item specific ADAM folder.
        /// </summary>
        public bool UseSiteRoot;

        /// <summary>
        /// The field this state is for. Will be null/empty if UsePortalRoot is true
        /// </summary>
        public string ItemField;

        /// <summary>
        /// The item guid this state is for. Will be Empty if UsePortalRoot is true.
        /// </summary>
        public Guid ItemGuid;

        internal IContentTypeAttribute Attribute;

        internal IBlock Block;

        public readonly Guid[] FeaturesForRestrictedUsers =
        {
            FeatureIds.PublicUpload,
            FeatureIds.PublicForms
        };

        public SecurityChecksBase Security;
        public MultiPermissionsTypes Permissions;

        public IApp App;


        #region Constructor / DI

        protected AdamState(string logName): base(logName ?? "Adm.State") {}

        /// <summary>
        /// Initializes the object and performs all the initial security checks
        /// </summary>
        public AdamState Init(IBlock block, int appId, string contentType, string field, Guid guid, bool usePortalRoot, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var callLog = Log.Call<AdamState>($"field:{field}, guid:{guid}");
            App = Factory.Resolve<Apps.App>().Init(appId, parentLog, block);
            Permissions = Factory.Resolve<MultiPermissionsTypes>()
                .Init(block.Context, App, contentType, Log);
            Block = block;

            // only do checks on field/guid if it's actually accessing that, if it's on the portal root, don't.
            UseSiteRoot = usePortalRoot;
            if (!usePortalRoot)
            {
                ItemField = field;
                ItemGuid = guid;
            }

            Security = Factory.Resolve<SecurityChecksBase>().Init(this, usePortalRoot, Log);
            
            SecurityCheckHelpers.ThrowIfAccessingRootButNotAllowed(usePortalRoot, Security.UserIsRestricted);

            Log.Add("check if feature enabled");
            if (Security.UserIsRestricted && !ToSic.Eav.Configuration.Features.Enabled(FeaturesForRestrictedUsers))
                throw HttpException.PermissionDenied(
                    $"low-permission users may not access this - {ToSic.Eav.Configuration.Features.MsgMissingSome(FeaturesForRestrictedUsers)}");

            Init(App, guid, field, usePortalRoot);

            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(field)) return callLog(null, this);

            Attribute = Definition(appId, contentType, field);
            if (!Security.FileTypeIsOkForThisField(out var exp))
                throw exp;
            return callLog(null, this);
        }

        #endregion

        #region Initialization methods

        private IContentTypeAttribute Definition(int appId, string contentType, string fieldName)
        {
            // try to find attribute definition - for later extra security checks
            var type = State.Get(appId).GetContentType(contentType);
            return type[fieldName];
        }

        protected abstract void Init(IApp app, Guid entityGuid, string fieldName, bool usePortalRoot);

        #endregion
    }
}

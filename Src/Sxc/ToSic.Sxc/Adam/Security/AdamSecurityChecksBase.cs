﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Files;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using IAsset = ToSic.Eav.Apps.Assets.IAsset;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract class AdamSecurityChecksBase: HasLog
    {
        #region DI / Constructor

        protected AdamSecurityChecksBase(string logPrefix) : base($"{logPrefix}.TnScCk") { }

        internal AdamSecurityChecksBase Init(AdamState adamState, bool usePortalRoot, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var callLog = Log.Call<AdamSecurityChecksBase>();
            AdamState = adamState;

            var firstChecker = AdamState.Permissions.PermissionCheckers.First().Value;
            var userMayAdminSomeFiles = firstChecker.UserMay(GrantSets.WritePublished);
            var userMayAdminSiteFiles = firstChecker.GrantedBecause == Conditions.EnvironmentGlobal ||
                                        firstChecker.GrantedBecause == Conditions.EnvironmentInstance;

            UserIsRestricted = !(usePortalRoot
                ? userMayAdminSiteFiles
                : userMayAdminSomeFiles);

            Log.Add($"adminSome:{userMayAdminSomeFiles}, restricted:{UserIsRestricted}");

            return callLog(null, this);
        }

        internal AdamState AdamState;
        public bool UserIsRestricted;

        #endregion

        #region Abstract methods to re-implement

        public abstract bool SiteAllowsExtension(string fileName);

        public abstract bool CanEditFolder(IAsset item);

        #endregion

        internal bool ExtensionIsOk(string fileName, out HttpExceptionAbstraction preparedException)
        {
            if (!SiteAllowsExtension(fileName))
            {
                preparedException = HttpException.NotAllowedFileType(fileName, "Not in whitelisted CMS file types.");
                return false;
            }

            if (FileNames.IsKnownRiskyExtension(fileName))// AdamSecurityCheckHelpers.IsKnownRiskyExtension(fileName))
            {
                preparedException = HttpException.NotAllowedFileType(fileName, "This is a known risky file type.");
                return false;
            }
            preparedException = null;
            return true;
        }


        /// <summary>
        /// Returns true if user isn't restricted, or if the restricted user is accessing a draft item
        /// </summary>
        internal bool UserIsNotRestrictedOrItemIsDraft(Guid guid, out HttpExceptionAbstraction exp)
        {
            Log.Add($"check if user is restricted ({UserIsRestricted}) or if the item '{guid}' is draft");
            exp = null;
            // check that if the user should only see drafts, he doesn't see items of normal data
            if (!UserIsRestricted || FieldPermissionOk(GrantSets.ReadPublished)) return true;

            // check if the data is public
            var itm = AdamState.AppRuntime.Entities.Get(guid);
            if (!(itm?.IsPublished ?? false)) return true;

            exp = HttpException.PermissionDenied(Log.Add("user is restricted and may not see published, but item exists and is published - not allowed"));
            return false;
        }

        internal bool FileTypeIsOkForThisField(out HttpExceptionAbstraction preparedException)
        {
            var wrapLog = Log.Call<bool>();
            var fieldDef = AdamState.Attribute;
            bool result;
            // check if this field exists and is actually a file-field or a string (wysiwyg) field
            if (fieldDef == null || !(fieldDef.Type != Eav.Constants.DataTypeHyperlink ||
                                      fieldDef.Type != Eav.Constants.DataTypeString))
            {
                preparedException = HttpException.BadRequest("Requested field '" + AdamState.ItemField + "' type doesn't allow upload");
                Log.Add($"field type:{fieldDef?.Type} - does not allow upload");
                result = false;
            }
            else
            {
                Log.Add($"field type:{fieldDef.Type}");
                preparedException = null;
                result = true;
            }
            return wrapLog(result.ToString(), result);
        }


        internal bool UserIsPermittedOnField(List<Grants> requiredPermissions, out HttpExceptionAbstraction preparedException)
        {
            // check field permissions, but only for non-publish-data
            if (UserIsRestricted && !FieldPermissionOk(requiredPermissions))
            {
                preparedException = HttpException.PermissionDenied("this field is not configured to allow uploads by the current user");
                return false;
            }
            preparedException = null;
            return true;
        }


        /// <summary>
        /// This will check if the field-definition grants additional rights
        /// Should only be called if the user doesn't have full edit-rights
        /// </summary>
        public bool FieldPermissionOk(List<Grants> requiredGrant)
        {
            var fieldPermissions = AdamState.ServiceProvider.Build<AppPermissionCheck>().ForAttribute(
                AdamState.Permissions.Context, AdamState.Context.AppState, AdamState.Attribute, Log);

            return fieldPermissions.UserMay(requiredGrant);
        }

        internal bool SuperUserOrAccessingItemFolder(string path, out HttpExceptionAbstraction preparedException)
        {
            preparedException = null;
            return !UserIsRestricted || DestinationIsInItem(AdamState.ItemGuid, AdamState.ItemField, path, out preparedException);
        }

        private static bool DestinationIsInItem(Guid guid, string field, string path, out HttpExceptionAbstraction preparedException)
        {
            var inAdam = Sxc.Adam.Security.PathIsInItemAdam(guid, field, path);
            preparedException = inAdam
                ? null
                : HttpException.PermissionDenied("Can't access a resource which is not part of this item.");
            return inAdam;
        }


        internal bool MustThrowIfAccessingRootButNotAllowed(bool usePortalRoot, out HttpExceptionAbstraction preparedException)
        {
            if (usePortalRoot && UserIsRestricted)
            {
                preparedException = HttpException.BadRequest("you may only create draft-data, so file operations outside of ADAM is not allowed");
                return true;
            }

            preparedException = null;
            return false;
        }

    }
}

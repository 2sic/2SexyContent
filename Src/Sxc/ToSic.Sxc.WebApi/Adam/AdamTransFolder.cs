﻿using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamTransFolder<TFolderId, TFileId> : AdamTransactionBase<AdamTransFolder<TFolderId, TFileId>, TFolderId, TFileId>
    {
        public AdamTransFolder(Lazy<AdamContext<TFolderId, TFileId>> adamState, IContextResolver ctxResolver) : base(adamState, ctxResolver, "Adm.TrnFld") { }

        public IList<AdamItemDto> Folder(string parentSubfolder, string newFolder)
        {
            var logCall = Log.Call<IList<AdamItemDto>>($"get folders for subfld:{parentSubfolder}, new:{newFolder}");
            if (AdamContext.Security.UserIsRestricted && !AdamContext.Security.FieldPermissionOk(GrantSets.ReadSomething))
                return null;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            var folder = AdamContext.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            if (!string.IsNullOrEmpty(parentSubfolder))
                folder = AdamContext.ContainerContext.Folder(parentSubfolder, false);

            // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
            if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(folder))
                throw HttpException.PermissionDenied("can't create new folder - permission denied");

            var newFolderPath = string.IsNullOrEmpty(parentSubfolder)
                ? newFolder
                : Path.Combine(parentSubfolder, newFolder).Replace("\\", "/");

            // now access the subfolder, creating it if missing (which is what we want
            AdamContext.ContainerContext.Folder(newFolderPath, true);

            return logCall("ok", ItemsInField(parentSubfolder));
        }
    }
}

﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using ToSic.Sxc.Views;

namespace ToSic.SexyContent.AppAssets
{
    internal class AssetEditor
    {
        public AssetEditInfo EditInfo { get; }

        private readonly bool _userIsSuperUser;
        private readonly bool _userIsAdmin;


        private readonly App _app;

        public AssetEditor(App app, int templateId, bool isSuperUser, bool isAdmin)
        {
            _app = app;
            _userIsSuperUser = isSuperUser;
            _userIsAdmin = isAdmin;

            var template = _app.ViewManager.GetTemplate(templateId);
            EditInfo = TemplateAssetsInfo(template);
        }

        public AssetEditor(App app, string path, bool isSuperUser, bool isAdmin, bool global = false)
        {
            _app = app;
            _userIsSuperUser = isSuperUser;
            _userIsAdmin = isAdmin;

            EditInfo = new AssetEditInfo(_app.AppId, _app.Name, path, global);
        }

        public AssetEditInfo EditInfoWithSource

        {
            get
            {
                EditInfo.Code = Source; // do this later, because it relies on the edit-info to exist
                return EditInfo;
            }
        }

        /// <summary>
        /// Check permissions and if not successful, give detailed explanation
        /// </summary>
        public void EnsureUserMayEditAsset(string fullPath = null)
        {
            // check super user permissions - then all is allowed
            if (_userIsSuperUser)
                return;

            // ensure current user is admin - this is the minimum of not super-user
            if(!_userIsAdmin)
                throw new AccessViolationException("current user may not edit templates, requires admin rights");

            // if not super user, check if razor (not allowed; super user only)
            if(!EditInfo.IsSafe)
                throw new AccessViolationException("current user may not edit razor templates - requires super user");

            // if not super user, check if cross-portal storage (not allowed; super user only)
            if(EditInfo.LocationScope != Settings.TemplateLocations.PortalFileSystem)
                throw new AccessViolationException("current user may not edit templates in central storage - requires super user");

            // optionally check if the file is really in the portal
            if (fullPath == null) return;

            var path = new FileInfo(fullPath);
            if(path.Directory == null)
                throw new AccessViolationException("path is null");

            if (path.Directory.FullName.IndexOf(_app.PhysicalPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                throw new AccessViolationException("current user may not edit files outside of the app-scope");
        }

        private AssetEditInfo TemplateAssetsInfo(IView view)
        {
            var t = new AssetEditInfo(_app.AppId, _app.Name, view.Path,
                view.Location == Settings.TemplateLocations.HostFileSystem)
            {
                // Template specific properties, not really available in other files
                LocationScope = view.Location,
                Type = view.Type,
                Name = view.Name,
                HasList = view.UseForList,
                TypeContent = view.ContentType,
                TypeContentPresentation = view.PresentationType,
                TypeList = view.HeaderType,
                TypeListPresentation = view.HeaderPresentationType
            };
            return t;
        }

        public string InternalPath => HttpContext.Current.Server.MapPath(
            Path.Combine(
                Internal.TemplateHelpers.GetTemplatePathRoot(EditInfo.LocationScope, _app),
                EditInfo.FileName));


        /// <summary>
        /// Read / Write the source code of the template file
        /// </summary>
        public string Source
        {
            get
            {
                EnsureUserMayEditAsset(InternalPath);
                if (File.Exists(InternalPath))
                    return File.ReadAllText(InternalPath);

                throw new FileNotFoundException("could not find file" 
                    + (_userIsSuperUser 
                    ? " for superuser - file tried '" + InternalPath + "'" 
                    : "")
                    );
            }
            set
            {
                EnsureUserMayEditAsset(InternalPath);

                if (File.Exists(InternalPath))
                    File.WriteAllText(InternalPath, value);
                else
                    throw new FileNotFoundException("could not find file"
                        + (_userIsSuperUser
                        ? " for superuser - file tried '" + InternalPath + "'"
                        : "")
                        );

            }
        }

        public bool Create(string contents)
        {
            // todo: maybe add some security for special dangerous file names like .cs, etc.?
            EditInfo.FileName = Regex.Replace(EditInfo.FileName, @"[?:\/*""<>|]", "");
            var absolutePath = InternalPath;

            // don't create if it already exits
            if (File.Exists(absolutePath)) return false;

            // ensure the web.config exists (usually missing in the global area)
            new Internal.TemplateHelpers(_app).EnsureTemplateFolderExists(EditInfo.LocationScope);

            // check if the folder to it already exists, or create it...
            var foundFolder = absolutePath.LastIndexOf("\\", StringComparison.InvariantCulture);
            if (foundFolder > -1)
            {
                var folderPath = absolutePath.Substring(0, foundFolder);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
            }

            // now create the file
            var stream = new StreamWriter(File.Create(absolutePath));
            stream.Write(contents);
            stream.Flush();
            stream.Close();

            return true;
        }

    }
}
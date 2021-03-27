﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps.Blocks
{
    public class BlockConfiguration: EntityBasedWithLog, IAppIdentity
    {
        public  int ZoneId { get; }
        public  int AppId { get; }
        internal readonly bool ShowDrafts;
        internal Guid? PreviewTemplateId;

        private readonly CmsRuntime _cmsRuntime;

        public BlockConfiguration(IEntity entity, CmsRuntime cmsRuntime, string languageCode, ILog parentLog): base(entity, languageCode, parentLog, "Blk.Config")
        {
            Log.Add("Entity is " + (entity == null ? "" : "not") + " null");
            _cmsRuntime = cmsRuntime;
            ZoneId = cmsRuntime.ZoneId;
            AppId = cmsRuntime.AppId;
            ShowDrafts = cmsRuntime.ShowDrafts;
        }
        
        internal BlockConfiguration WarnIfMissingData()
        {
            if (Entity != null) return this;
            throw new Exception("BlockConfiguration entity is null. " +
                                "This usually happens when you are duplicating a site, and have not yet imported the other content/apps. " +
                                "If that is your issue, check 2sxc.org/help?tag=export-import");
        }
        
        /// <summary>
        /// Returns true if a content group entity for this group really exists
        /// Means for example, that the app can't be changed anymore
        /// </summary>
        public bool Exists => Entity != null;

        internal bool DataIsMissing = false;
        
        
        #region View stuff
        
        public IView View
        {
            get
            {
                if (_view != null) return _view;

                // if we're previewing another template, look that up
                var templateEntity = PreviewTemplateId.HasValue
                    ? _cmsRuntime.Data.List.One(PreviewTemplateId.Value) // ToDo: Should use an indexed Guid filter
                    : Entity?.Children(ViewParts.ViewFieldInContentBlock).FirstOrDefault();

                return _view = templateEntity == null ? null : new View(templateEntity, LookupLanguages, Log);
            }
        }
        private IView _view;
        
        #endregion

        #region Retrieve the lists - either as object or by the type-indexer

        /// <summary>
        /// Content is a bit special, it must always return a list with at least one null-item
        /// </summary>
        public List<IEntity> Content
        {
            get
            {
                if (Entity == null) return new List<IEntity> {null};
                var list = Entity.Children(ViewParts.Content);
                return list.Count > 0 ? list : new List<IEntity> {null};
            }
        }

        public List<IEntity> Presentation => Entity?.Children(ViewParts.Presentation) ?? new List<IEntity>();

        public List<IEntity> Header => Entity?.Children(ViewParts.ListContent) ?? new List<IEntity>();

        public List<IEntity> HeaderPresentation => Entity?.Children(ViewParts.ListPresentation) ?? new List<IEntity>();

        public List<IEntity> this[string type]
        {
            get
            {
                switch (type.ToLowerInvariant())
                {
                    case ViewParts.ContentLower: return Content;
                    case ViewParts.PresentationLower: return Presentation;
                    case ViewParts.ListContentLower: return Header;
                    case ViewParts.ListPresentationLower: return HeaderPresentation;
                    default: throw new Exception("Type " + type + " not allowed");
                }
            }
        }

        #endregion
    }
}
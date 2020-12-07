﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Conversion;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Engines;

// note: not sure if the final namespace should be Sxc.Apps or Sxc.Views
namespace ToSic.Sxc.Apps
{
	public class ViewsRuntime: PartOf<CmsRuntime, ViewsRuntime>
    {
        #region Constructor / DI

        private IValueConverter ValueConverter => _valConverter ?? (_valConverter = _valConverterLazy.Value);
        private readonly Lazy<IValueConverter> _valConverterLazy;
        private readonly IZoneCultureResolver _cultureResolver;
        private IValueConverter _valConverter;

        public ViewsRuntime(Lazy<IValueConverter> valConverterLazy, IZoneCultureResolver cultureResolver) : base("Cms.ViewRd")
        {
            _valConverterLazy = valConverterLazy;
            _cultureResolver = cultureResolver;
        }

        #endregion

        private IDataSource _viewDs;
		private IDataSource ViewsDataSource()
		{
            if(_viewDs!= null)return _viewDs;
		    // ReSharper disable once RedundantArgumentDefaultValue
            var dataSource = Parent.Data;
			dataSource = Parent.DataSourceFactory.GetDataSource<EntityTypeFilter>(dataSource);
		    ((EntityTypeFilter) dataSource).TypeName = Configuration.TemplateContentType;
		    _viewDs = dataSource;
			return dataSource;
		}

        public IEnumerable<IView> GetAll() 
            => _all ?? (_all = ViewsDataSource().Immutable
                   .Select(p => new View(p, _cultureResolver.CurrentCultureCode, Log))
                   .OrderBy(p => p.Name));
        private IEnumerable<IView> _all;

        public IEnumerable<IView> GetRazor() => GetAll().Where(t => t.IsRazor);
        public IEnumerable<IView> GetToken() => GetAll().Where(t => !t.IsRazor);


        public IView Get(int templateId)
		{
            var templateEntity = ViewsDataSource().Immutable.One(templateId);

            if(templateEntity == null)
				throw new Exception("The template with id " + templateId + " does not exist.");

			return new View(templateEntity, _cultureResolver.CurrentCultureCode, Log);
		}

        public IView Get(Guid guid)
        {
            var templateEntity = ViewsDataSource().Immutable.One(guid);

            if (templateEntity == null)
                throw new Exception("The template with id " + guid + " does not exist.");

            return new View(templateEntity, _cultureResolver.CurrentCultureCode, Log);
        }


        internal IEnumerable<TemplateUiInfo> GetCompatibleViews(IApp app, BlockConfiguration blockConfiguration)
	    {
	        IEnumerable<IView> availableTemplates;
	        var items = blockConfiguration.Content;

            // if any items were already initialized...
	        if (items.Any(e => e != null))
	            availableTemplates = GetFullyCompatibleViews(blockConfiguration);

            // if it's only nulls, and only one (no list yet)
	        else if (items.Count <= 1)
	            availableTemplates = GetAll(); 

            // if it's a list of nulls, only allow lists
	        else
	            availableTemplates = GetAll().Where(p => p.UseForList);

	        return availableTemplates.Select(t => new TemplateUiInfo
	        {
	            TemplateId = t.Id,
	            Name = t.Name,
	            ContentTypeStaticName = t.ContentType,
	            IsHidden = t.IsHidden,
	            Thumbnail = Parent.ServiceProvider.Build<TemplateHelpers>().Init(app, Log).IconPathOrNull(t, PathTypes.Link)
	        });
	    }


        /// <summary>
        /// Get templates which match the signature of possible content-items, presentation etc. of the current template
        /// </summary>
        /// <param name="blockConfiguration"></param>
        /// <returns></returns>
	    private IEnumerable<IView> GetFullyCompatibleViews(BlockConfiguration blockConfiguration)
        {
            var isList = blockConfiguration.Content.Count > 1;

            var compatibleTemplates = GetAll().Where(t => t.UseForList || !isList);
            compatibleTemplates = compatibleTemplates
                .Where(t => blockConfiguration.Content.All(c => c == null) || blockConfiguration.Content.First(e => e != null).Type.StaticName == t.ContentType)
                .Where(t => blockConfiguration.Presentation.All(c => c == null) || blockConfiguration.Presentation.First(e => e != null).Type.StaticName == t.PresentationType)
                .Where(t => blockConfiguration.Header.All(c => c == null) || blockConfiguration.Header.First(e => e != null).Type.StaticName == t.HeaderType)
                .Where(t => blockConfiguration.HeaderPresentation.All(c => c == null) || blockConfiguration.HeaderPresentation.First(e => e != null).Type.StaticName == t.HeaderPresentationType);

            return compatibleTemplates;
        }


        // todo: check if this call could be replaced with the normal ContentTypeController.Get to prevent redundant code
        public IEnumerable<ContentTypeUiInfo> GetContentTypesWithStatus()
        {
            var templates = GetAll().ToList();
            var visible = templates.Where(t => !t.IsHidden).ToList();
            var serializer = new EntitiesToDictionary();

            return Parent.ContentTypes.All.OfScope(Settings.AttributeSetScope) 
                .Where(ct => templates.Any(t => t.ContentType == ct.StaticName)) // must exist in at least 1 template
                .OrderBy(ct => ct.Name)
                .Select(ct =>
                {
                    var metadata = ct.Metadata.Description;
                    return new ContentTypeUiInfo {
                        StaticName = ct.StaticName,
                        Name = ct.Name,
                        IsHidden = visible.All(t => t.ContentType != ct.StaticName),   // must check if *any* template is visible, otherwise tell the UI that it's hidden
                        Thumbnail = ValueConverter.ToValue(metadata?.GetBestValue<string>(View.TemplateIcon)),
                        Metadata = serializer.Convert(metadata)
                    };
                });
        }



    }

}
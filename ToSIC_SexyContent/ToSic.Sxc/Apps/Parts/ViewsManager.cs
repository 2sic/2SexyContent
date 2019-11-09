﻿using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
    /// <inheritdoc />
    /// <summary>
    /// Views manager for the app engine - in charge of importing / modifying templates at app-level
    /// </summary>
    public class ViewsManager: ManagerBase
    {
        public ViewsManager(AppManager app, ILog parentLog) : base(app, parentLog, "App.TplMng") {}

        #region Template

        /// <summary>
        /// Adds or updates a template - will create a new template if templateId is not specified
        /// </summary>
        public void CreateOrUpdate(int? templateId, string name, string path, string contentTypeStaticName,
            int? contentDemoEntity, string presentationTypeStaticName, int? presentationDemoEntity,
            string listContentTypeStaticName, int? listContentDemoEntity, string listPresentationTypeStaticName,
            int? listPresentationDemoEntity, string templateType, bool isHidden, string location, bool useForList,
            bool publishData, string streamsToPublish, int? queryEntity, string viewNameInUrl)
        {
            var values = new Dictionary<string, object>
            {
                {View.FieldName, name },
                {View.FieldPath, path },
                {View.FieldContentType, contentTypeStaticName },
                {View.FieldContentDemo, contentDemoEntity.HasValue ? new List<int> { contentDemoEntity.Value } : new List<int>() },
                {View.FieldPresentationType, presentationTypeStaticName },
                {View.FieldPresentationItem, presentationDemoEntity.HasValue ? new List<int> { presentationDemoEntity.Value } : new List<int>() },
                {View.FieldHeaderType, listContentTypeStaticName },
                {View.FieldHeaderItem, listContentDemoEntity.HasValue ? new List<int> { listContentDemoEntity.Value } : new List<int>() },
                {View.FieldHeaderPresentationType, listPresentationTypeStaticName },
                {View.FieldHeaderPresentationItem, listPresentationDemoEntity.HasValue ? new List<int> { listPresentationDemoEntity.Value } : new List<int>() },
                {View.FieldType, templateType },
                {View.FieldIsHidden, isHidden },
                {View.FieldLocation, location },
                {View.FieldUseList, useForList },
                {View.FieldPublishEnable, publishData },
                {View.FieldPublishStreams, streamsToPublish },
                {View.FieldPipeline, queryEntity.HasValue ? new List<int> { queryEntity.Value } : new List<int>() },
                {View.FieldNameInUrl, viewNameInUrl }
            };

            if (templateId.HasValue)
                AppManager.Entities.UpdateParts(templateId.Value, values);
            else
                AppManager.Entities.Create(Configuration.TemplateContentType, values);
        }



        #endregion

    }
}

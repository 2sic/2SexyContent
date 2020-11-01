﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
	public class BlocksManager: PartOf<CmsManager, BlocksManager>
	{
        public BlocksManager() : base("CG.Manage") { }

	    public Guid UpdateOrCreateContentGroup(BlockConfiguration blockConfiguration, int templateId)
		{
		    var appMan = Parent;

		    if (!blockConfiguration.Exists)
		    {
		        Log.Add($"doesn't exist, will create new CG with template#{templateId}");
		        return appMan.Entities.Create(BlocksRuntime.BlockTypeName, new Dictionary<string, object>
		        {
		            {ViewParts.TemplateContentType, new List<int> {templateId}},
		            {ViewParts.Content, new List<int>()},
		            {ViewParts.Presentation, new List<int>()},
		            {ViewParts.ListContent, new List<int>()},
		            {ViewParts.ListPresentation, new List<int>()}
		        }).Item2; // new guid
		    }
		    else
		    {
		        Log.Add($"exists, create for group#{blockConfiguration.Guid} with template#{templateId}");
		        appMan.Entities.UpdateParts(blockConfiguration.Entity.EntityId,
		            new Dictionary<string, object> {{ ViewParts.TemplateContentType, new List<int?> {templateId}}});

		        return blockConfiguration.Guid; // guid didn't change
		    }
		}

        public void AddEmptyItem(BlockConfiguration block, int? sortOrder) =>
            Parent.Entities.FieldListUpdate(block.Entity, ViewParts.ContentPair, block.VersioningEnabled,
                lists => lists.Add(sortOrder, new int?[] { null, null }));



        public int NewBlockReference(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            Log.Add($"get CB parent:{parentId}, field:{field}, order:{sortOrder}, app:{app}, guid:{guid}");
            var contentTypeName = Settings.AttributeSetStaticNameContentBlockTypeName;
            var values = new Dictionary<string, object>
            {
                {BlockFromEntity.CbPropertyTitle, ""},
                {BlockFromEntity.CbPropertyApp, app},
            };
            var newGuid = guid ?? Guid.NewGuid();
            var entityId = CreateItemAndAddToList(parentId, field, sortOrder, contentTypeName, values, newGuid);

            return entityId;
        }

        private int CreateItemAndAddToList(int parentId, string field, int index, string typeName, Dictionary<string, object> values, Guid newGuid)
        {
            var callLog = Log.Call<int>($"{nameof(parentId)}:{parentId}, {nameof(field)}:{field}, {nameof(index)}, {index}, {nameof(typeName)}:{typeName}");
            // create the new entity 
            var entityId = Parent.Entities.GetOrCreate(newGuid, typeName, values);

            #region attach to the current list of items

            var cbEnt = Parent.AppState.List.One(parentId);
            var blockList = ((IEnumerable<IEntity>)cbEnt.GetBestValue(field))?.ToList() ?? new List<IEntity>();

            var intList = blockList.Select(b => b.EntityId).ToList();
            // add only if it's not already in the list (could happen if http requests are run again)
            if (!intList.Contains(entityId))
            {
                if (index > intList.Count) index = intList.Count;
                intList.Insert(index, entityId);
            }
            var updateDic = new Dictionary<string, object> { { field, intList } };
            Parent.Entities.UpdateParts(cbEnt.EntityId, updateDic);
            #endregion

            return callLog($"{entityId}", entityId);
        }


    }
}
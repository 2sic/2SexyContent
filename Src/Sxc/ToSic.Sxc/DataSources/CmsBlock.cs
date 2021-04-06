﻿using System;
using System.Collections.Immutable;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// This data-source delivers the core data for a CMS Block. <br/>
    /// It will look up the configuration in the CMS (like the Module-Settings in DNN) to determine what data is needed for the block. <br/>
    /// Usually it will then find a reference to a ContentBlock, from which it determines what content-items are assigned. <br/>
    /// It could also find that the template specifies a query, in which case it would retrieve that. <br/>
    /// <em>Was previously called ModuleDataSource</em>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    [VisualQuery(
        NiceName = "CMS Block",
        UiHint = "Data for this CMS Block (instance/module)",
        Icon = "recent_actors",
        Type = DataSourceType.Source, 
        GlobalName = "ToSic.Sxc.DataSources.CmsBlock, ToSic.Sxc",
        ExpectsDataOfType = "7c2b2bc2-68c6-4bc3-ba18-6e6b5176ba02",
        In = new []{Eav.Constants.DefaultStreamName},
        HelpLink = "https://docs.2sxc.org/api/dot-net/ToSic.Sxc.DataSources.CmsBlock.html",
        PreviousNames = new []{ "ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent" })]
    public sealed partial class CmsBlock : DataSourceBase
    {
        private readonly Lazy<CmsRuntime> _lazyCmsRuntime;

        /// <inheritdoc />
        public override string LogId => "Sxc.CmsBDs";

        public const string InstanceLookupName = "module";
        public const string InstanceIdKey = "ModuleId";

        [PrivateApi]
        public enum Settings
        {
            InstanceId
        }

        /// <summary>
        /// The instance-id of the CmsBlock (2sxc instance, DNN ModId). <br/>
        /// It's named Instance-Id to be more neutral as we're opening it to other platforms
        /// </summary>
        public int? InstanceId
        {
            get
            {
                Configuration.Parse();
                var listIdString = Configuration[InstanceIdKey];
                return int.TryParse(listIdString, out var listId) ? listId : new int?();
            }
            set => Configuration[InstanceIdKey] = value.ToString();
        }


        public CmsBlock(Lazy<CmsRuntime> lazyCmsRuntime)
        {
            _lazyCmsRuntime = lazyCmsRuntime;
            Provide(GetContent);
            Provide(ViewParts.Header, GetHeader);
            Provide(ViewParts.ListContent, GetHeader);
			Configuration.Values.Add(InstanceIdKey, $"[Settings:{Settings.InstanceId}||[{InstanceLookupName}:{InstanceIdKey}]]");
        }

        private ImmutableArray<IEntity> GetContent()
            => GetStream(BlockConfiguration.Content, View.ContentItem,
                BlockConfiguration.Presentation, View.PresentationItem, false);

        private ImmutableArray<IEntity> GetHeader()
            => GetStream(BlockConfiguration.Header, View.HeaderItem,
                BlockConfiguration.HeaderPresentation, View.HeaderPresentationItem, true);
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane)
    /// </summary>
    [PrivateApi]
    [VisualQuery(
        ExpectsDataOfType = VqExpectsDataOfType,
        GlobalName = VqGlobalName,
        HelpLink = VqHelpLink,
        Icon = VqIcon,
        NiceName = VqNiceName, 
        Type = VqType, 
        UiHint = VqUiHint)]
    public class Pages: CmsBases.PagesBase
    {

        protected override List<TempPageInfo> GetPagesInternal()
        {
            var wrapLog = Log.Call<List<TempPageInfo>>();
            var siteId = PortalSettings.Current?.PortalId ?? -1;
            Log.Add($"Portal Id {siteId}");
            List<TabInfo> pages;
            try
            {
                pages = TabController.GetPortalTabs(siteId, 0, false, false);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return wrapLog("error", new List<TempPageInfo>());
            }

            if (pages == null || !pages.Any()) return wrapLog("null/empty", new List<TempPageInfo>());


            try
            {
                var result = pages
                    .Where(p => !p.IsSuperTab && !p.IsDeleted && !p.IsSystem)
                    .Where(DotNetNuke.Security.Permissions.TabPermissionController.CanViewPage)
                    .Select(p => new TempPageInfo
                    {
                        Id = p.TabID,
                        Guid = p.UniqueId,
                        Title = p.Title,
                        Name = p.TabName,
                        ParentId = p.ParentId,
                        Visible = p.IsVisible,
                        Path = p.TabPath,
                        Url = p.FullUrl,
                        Created = p.CreatedOnDate,
                        Modified = p.LastModifiedOnDate
                    })
                    .ToList();
                return wrapLog("found", result);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return wrapLog("error", new List<TempPageInfo>());
            }
        }
        
    }
}

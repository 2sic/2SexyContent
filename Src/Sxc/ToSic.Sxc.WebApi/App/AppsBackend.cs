﻿using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Work;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Web.LightSpeed;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppsBackend: ServiceBase
{
    private readonly WorkApps _workApps;
    private readonly CodeInfoStats _codeStats;
    private readonly IContextOfSite _context;
    private readonly Generator<AppConfigDelegate> _appConfigDelegate;

    public AppsBackend(WorkApps workApps, IContextOfSite context, Generator<AppConfigDelegate> appConfigDelegate, CodeInfoStats codeStats) : base("Bck.Apps")
    {
        ConnectServices(
            _workApps = workApps,
            _codeStats = codeStats,
            _context = context,
            _appConfigDelegate = appConfigDelegate
        );
    }
        
    public List<AppDto> Apps()
    {
        var configurationBuilder = _appConfigDelegate.New().Build();
        var list = _workApps.GetApps(_context.Site, configurationBuilder);
        return list.Select(CreateAppDto).ToList();
    }

    public List<AppDto> GetInheritableApps()
    {
        var configurationBuilder = _appConfigDelegate.New().Build();
        var list = _workApps.GetInheritableApps(_context.Site, configurationBuilder);
        return list.Select(CreateAppDto).ToList();
    }

    private AppDto CreateAppDto(IApp a)
    {
        AppMetadataDto lightspeed = null;
        var lsEntity = a.AppState.Metadata.FirstOrDefaultOfType(LightSpeedDecorator.TypeNameId);
        if (lsEntity != null)
        {
            var lsd = new LightSpeedDecorator(lsEntity);
            lightspeed = new AppMetadataDto { Id = lsd.Id, Title = lsd.Title, IsEnabled = lsd.IsEnabled };
        }
        return new AppDto
        {
            Id = a.AppId,
            IsApp = a.NameId != Eav.Constants.DefaultAppGuid &&
                    a.NameId != Eav.Constants.PrimaryAppGuid, // #SiteApp v13
            Guid = a.NameId,
            Name = a.Name,
            Folder = a.Folder,
            AppRoot = a.Path,
            IsHidden = a.Hidden,
            ConfigurationId = a.Configuration?.Id,
            Items = a.Data.List.Count(),
            Thumbnail = a.Thumbnail,
            Version = a.VersionSafe(),
            IsGlobal = a.AppState.IsShared(),
            IsInherited = a.AppState.IsInherited(),
            Lightspeed = lightspeed,
            HasCodeWarnings = _codeStats.AppHasWarnings(a.AppId),
        };
    }
}
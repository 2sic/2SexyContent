﻿using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Internal.Features;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services.Internal;
using static System.StringComparer;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Backend.Cms;

internal class LoadSettingsForGpsDefaults: ServiceBase, ILoadSettingsProvider
{
    private readonly GoogleMapsSettings _googleMapsSettings;
    private readonly LazySvc<IFeaturesService> _features;

    public LoadSettingsForGpsDefaults(GoogleMapsSettings googleMapsSettings,
        LazySvc<IFeaturesService> features) : base($"{SxcLogging.SxcLogName}.LdGpsD")
    {
        ConnectServices(
            _googleMapsSettings = googleMapsSettings,
            _features = features
        );
    }

    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) => Log.Func(l =>
    {
        var coordinates = MapsCoordinates.Defaults;

        if (_features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId))
        {
            var getMaps = parameters.ContextOfApp.AppSettings.InternalGetPath(_googleMapsSettings.SettingsIdentifier);
            coordinates = getMaps.GetFirstResultEntity() is IEntity mapsEntity
                ? _googleMapsSettings.Init(mapsEntity).DefaultCoordinates
                : MapsCoordinates.Defaults;
        }

        var result = new Dictionary<string, object>(InvariantCultureIgnoreCase)
        {
            {
                $"{_googleMapsSettings.SettingsIdentifier}.{nameof(GoogleMapsSettings.DefaultCoordinates)}", coordinates
            }
        };
        return (result, $"{result.Count}");
    });
}
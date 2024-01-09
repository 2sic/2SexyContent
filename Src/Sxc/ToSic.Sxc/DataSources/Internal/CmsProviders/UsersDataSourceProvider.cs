﻿using System.Collections.Generic;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Internal.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the UsersDataSourceProvider.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class UsersDataSourceProvider: ServiceBase
{
    protected UsersDataSourceProvider(string logName) : base(logName)
    { }

    /// <summary>
    /// The inner list retrieving the users.
    /// </summary>
    /// <returns></returns>
    [PrivateApi]
    public abstract IEnumerable<CmsUserRaw> GetUsersInternal();
}
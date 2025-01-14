﻿using ToSic.Sxc.DataSources;
using ToSic.Sxc.Models.Internal;

namespace ToSic.Sxc.Models;

/// <summary>
/// Model to return role information as provided by the <see cref="UserRoles"/> DataSource.
/// </summary>
/// <remarks>
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn RoleInfo](https://docs.dnncommunity.org/api/DotNetNuke.Security.Roles.RoleInfo.html)
/// * [Oqtane UserRole](https://docs.oqtane.org/api/Oqtane.Models.UserRole.html)
/// 
/// History
/// 
/// * Introduced in v19.01
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class UserRoleModel: DataModel, IUserRoleModel
{
    /// <inheritdoc />
    public int Id => _entity.EntityId;
    /// <inheritdoc />
    public string Name => _entity.Get<string>(nameof(Name));
    /// <inheritdoc />
    public DateTime Created => _entity.Created;
    /// <inheritdoc />
    public DateTime Modified => _entity.Modified;

}
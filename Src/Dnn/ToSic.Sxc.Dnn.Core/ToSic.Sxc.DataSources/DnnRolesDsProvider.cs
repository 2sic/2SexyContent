﻿using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of roles from the Dnn.
    /// </summary>
    public class DnnRolesDsProvider : RolesDataSourceProvider
    {
        public DnnRolesDsProvider() : base("Dnn.Roles")
        { }

        [PrivateApi]
        public override IEnumerable<RoleDataNew> GetRolesInternal() => Log.Func(l =>
        {
            var siteId = PortalSettings.Current?.PortalId ?? -1;
            l.A($"Portal Id {siteId}");
            try
            {
                var dnnRoles = RoleController.Instance.GetRoles(portalId: siteId);
                if (!dnnRoles.Any()) return (new List<RoleDataNew>(), "null/empty");

                var result = dnnRoles
                    .Select(r => new RoleDataNew
                    {
                        Id = r.RoleID,
                        // Guid = r.
                        Name = r.RoleName,
                        Created = r.CreatedOnDate,
                        Modified = r.LastModifiedOnDate,
                    })
                    .ToList();
                return (result, "found");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new List<RoleDataNew>(), "error");
            }
        });
    }
}
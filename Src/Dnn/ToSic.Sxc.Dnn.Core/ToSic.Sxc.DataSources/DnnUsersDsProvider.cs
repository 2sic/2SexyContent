﻿using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{

    public class DnnUsersDsProvider : UsersDataSourceProvider
    {
        public DnnUsersDsProvider() : base("Dnn.Users")
        { }

        public override IEnumerable<UserDataRaw> GetUsersInternal() => Log.Func(l =>
            {
                var siteId = PortalSettings.Current?.PortalId ?? -1;
                l.A($"Portal Id {siteId}");
                try
                {
                    // take all portal users (this should include superusers, but superusers are missing)
                    var dnnAllUsers =
                        UserController.GetUsers(portalId: siteId, includeDeleted: false, superUsersOnly: false);

                    // append all superusers
                    dnnAllUsers.AddRange(UserController.GetUsers(portalId: -1, includeDeleted: false,
                        superUsersOnly: true));

                    var dnnUsers = dnnAllUsers.Cast<UserInfo>().ToList();
                    if (!dnnUsers.Any()) return (new List<UserDataRaw>(), "null/empty");

                    var result = dnnUsers
                        //.Where(d => !d.IsDeleted)
                        .Select(d =>
                        {
                            var adminInfo = d.UserMayAdminThis();
                            return new UserDataRaw
                            {
                                Id = d.UserID,
                                Guid = d.UserGuid(),
                                NameId = d.UserIdentityToken(),
                                RoleIds = d.RoleList(portalId: siteId),
                                IsSystemAdmin = d.IsSuperUser,
                                IsSiteAdmin = adminInfo.IsSiteAdmin,
                                IsContentAdmin = adminInfo.IsContentAdmin,
                                //IsDesigner = d.IsDesigner(),
                                IsAnonymous = d.IsAnonymous(),
                                Created = d.CreatedOnDate,
                                Modified = d.LastModifiedOnDate,
                                //
                                Username = d.Username,
                                Email = d.Email,
                                Name = d.DisplayName
                            };
                        }).ToList();
                    return (result, "found");
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    return (new List<UserDataRaw>(), "error");
                }
            });
    }
}
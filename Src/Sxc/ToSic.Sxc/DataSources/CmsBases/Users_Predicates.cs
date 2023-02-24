﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Raw;

namespace ToSic.Sxc.DataSources
{
    public partial class Users
    {
        private List<Func<CmsUserNew, bool>> GetAllFilters() => Log.Func(l =>
        {
            var filters = new List<Func<CmsUserNew, bool>>
            {
                IncludeUsersPredicate(),
                ExcludeUsersPredicate(),
                FilterIncludeUsersOfRoles(),
                ExcludeRolesPredicate(),
                SuperUserPredicate()
            };
            filters = filters.Where(f => f != null).ToList();
            return (filters, $"{filters.Count}");
        });


        private Func<CmsUserNew, bool> IncludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(UserIds)) return null;
            var includeUserGuids = FilterKeepUserGuids();
            var includeUserIds = FilterKeepUserIds();
            if (includeUserGuids == null && includeUserIds == null) return null;
            return u => (includeUserGuids != null && includeUserGuids(u)) || (includeUserIds != null && includeUserIds(u));
        }

        private Func<CmsUserNew, bool> FilterKeepUserGuids()
        {
            var userGuidFilter = UserIds.Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty).ToList();
            return userGuidFilter.Any()
                ? (Func<CmsUserNew, bool>)(u => u.Guid != Guid.Empty && userGuidFilter.Contains(u.Guid))
                : null;
        }

        private Func<CmsUserNew, bool> FilterKeepUserIds()
        {
            var userIdFilter = UserIds.Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1).ToList();
            return userIdFilter.Any()
                ? (Func<CmsUserNew, bool>)(u => userIdFilter.Contains(u.Id))
                : null;
        }

        private Func<CmsUserNew, bool> ExcludeUsersPredicate()
        {
            if (string.IsNullOrEmpty(ExcludeUserIds)) return null;
            var excludeUserGuids = FilterExcludeUserGuids();
            var excludeUserIds = FilterExcludeUserIds();
            if (excludeUserGuids == null && excludeUserIds == null) return null;
            return u => (excludeUserGuids == null || excludeUserGuids(u)) && (excludeUserIds == null || excludeUserIds(u));
        }

        private Func<CmsUserNew, bool> FilterExcludeUserGuids()
        {
            var excludeUserGuidsFilter = ExcludeUserIds
                .Split(Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty)
                .ToList();
            return excludeUserGuidsFilter.Any()
                ? (Func<CmsUserNew, bool>)(u => u.Guid != Guid.Empty && !excludeUserGuidsFilter.Contains(u.Guid))
                : null;
        }

        private Func<CmsUserNew, bool> FilterExcludeUserIds()
        {
            var excludeUserIdsFilter = ExcludeUserIds
                .Split(Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : -1)
                .Where(u => u != -1)
                .ToList();
            return excludeUserIdsFilter.Any()
                ? (Func<CmsUserNew, bool>)(u => !excludeUserIdsFilter.Contains(u.Id))
                : null;
        }

        private Func<CmsUserNew, bool> FilterIncludeUsersOfRoles()
        {
            var rolesFilter = Roles.RolesCsvListToInt(RoleIds);
            return rolesFilter.Any()
                ? (Func<CmsUserNew, bool>)(u => u.Roles.Any(r => rolesFilter.Contains(r)))
                : null;
        }

        private Func<CmsUserNew, bool> ExcludeRolesPredicate()
        {
            var excludeRolesFilter = Roles.RolesCsvListToInt(ExcludeRoleIds);
            return excludeRolesFilter.Any()
                ? (Func<CmsUserNew, bool>)(u => !u.Roles.Any(r => excludeRolesFilter.Contains(r)))
                : null;
        }

        private Func<CmsUserNew, bool> SuperUserPredicate() => Log.Func<Func<CmsUserNew, bool>>(() =>
        {
            // If "include" == "only" return only super users
            if (IncludeSystemAdmins.EqualsInsensitive(IncludeRequired))
                return (u => u.IsSystemAdmin, IncludeRequired);

            // If "include" == true, return all
            if (IncludeSystemAdmins.EqualsInsensitive(IncludeOptional))
                return (null, $"{IncludeOptional} = any"); // skip IsSystemAdmin check will return normal and super users

            // If "include" == false - or basically any unknown value, return only normal users
            return (u => !u.IsSystemAdmin, IncludeForbidden);
        });
    }
}

﻿
using Microsoft.AspNetCore.Identity;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of users from the current platform (Dnn or Oqtane)
    /// </summary>
    [PublicApi]
    [VisualQuery(
        NiceName = VqNiceName,
        Icon = VqIcon,
        UiHint = VqUiHint,
        HelpLink = VqHelpLink,
        GlobalName = VqGlobalName,
        Type = VqType,
        ExpectsDataOfType = VqExpectsDataOfType
    )]
    public class UsersDataSource : CmsBases.UsersDataSourceBase
    {
        private readonly IUserRoleRepository _userRoles;
        private readonly SiteState _siteState;
        private readonly UserManager<IdentityUser> _identityUserManager;

        public UsersDataSource(IUserRoleRepository userRoles, SiteState siteState, UserManager<IdentityUser> identityUserManager)
        {
            _userRoles = userRoles;
            _siteState = siteState;
            _identityUserManager = identityUserManager;
        }
        
        protected override IEnumerable<UserDataSourceInfo> GetUsersInternal()
        {
            var wrapLog = Log.Fn<List<UserDataSourceInfo>>();
            var siteId = _siteState.Alias.SiteId;
            Log.A($"Portal Id {siteId}");
            try
            {
                var userRoles = _userRoles.GetUserRoles(siteId).ToList();
                var users = userRoles.Select(ur => ur.User).Distinct().ToList();
                if (!users.Any()) return wrapLog.Return(new List<UserDataSourceInfo>(), "null/empty");

                var result = users
                    .Where(u => !u.IsDeleted)
                    .Select(u => new UserDataSourceInfo
                    {
                        Id = u.UserId,
                        Guid = new Guid((_identityUserManager.FindByNameAsync(u.Username).Result).Id), // new Guid(new IdentityUser(u.User.Username).Id),
                        IdentityToken = $"{OqtConstants.UserTokenPrefix}:{u.UserId}",
                        Roles = userRoles.Where(ur => ur.UserId == u.UserId).Select(ur => ur.RoleId).ToList(),
                        IsSuperUser = userRoles.Any(ur => ur.UserId == u.UserId && ur.Role.Name == RoleNames.Host),
                        IsAdmin = userRoles.Any(ur => ur.UserId == u.UserId && ur.Role.Name == RoleNames.Admin),
                        IsDesigner = userRoles.Any(ur => ur.UserId == u.UserId && ur.Role.Name == RoleNames.Host),
                        IsAnonymous = u.UserId == -1,
                        Created = u.CreatedOn,
                        Modified = u.ModifiedOn,
                        //
                        Username = u.Username,
                        Email = u.Email,
                        Name = u.DisplayName,
                    }).ToList();
                return wrapLog.Return(result, "found");
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                return wrapLog.Return(new List<UserDataSourceInfo>(), "error");
            }
        }
    }
}
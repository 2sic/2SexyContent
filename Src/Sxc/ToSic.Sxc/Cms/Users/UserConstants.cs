﻿using ToSic.Sxc.Cms.Users.Internal;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Cms.Users;

internal class UserConstants
{
    #region Constant user objects for Unknown/Anonymous

    internal static readonly UserModel AnonymousUser = new()
    {
        Id = -1,
        Name = SxcUserConstants.Anonymous,
        Roles = [],
    };

    internal static readonly UserModel UnknownUser = new()
    {
        Id = -2,
        Name = Eav.Constants.NullNameId,
        Roles = [],
    };

    #endregion

    #region Other Constants

    internal const char Separator = ',';
    internal const int NullInteger = -1;
    public const string IncludeRequired = "required";
    public const string IncludeOptional = "true";
    public const string IncludeForbidden = "false";

    #endregion
}
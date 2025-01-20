﻿using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.DataSources.Internal;

public record UsersGetSpecsParsed(UsersGetSpecs specs)
{
    #region Configuration

    public IEnumerable<int> UserIdFilter => field ??= ParseCsvToIntList(specs.UserIds);
    public IEnumerable<Guid> UserGuidFilter => field ??= ParseCsvToGuidList(specs.UserIds);
    public IEnumerable<int> ExcludeUserIdsFilter => field ??= ParseCsvToIntList(specs.ExcludeUserIds);
    public IEnumerable<Guid> ExcludeUserGuidsFilter => field ??= ParseCsvToGuidList(specs.ExcludeUserIds);
    public IEnumerable<int> RolesFilter => field ??= ParseCsvToIntList(specs.RoleIds);
    public IEnumerable<int> ExcludeRolesFilter => field ??= ParseCsvToIntList(specs.ExcludeRoleIds);

    #endregion

    private static List<int> ParseCsvToIntList(string stringList)
        => !stringList.HasValue()
            ? []
            : stringList.Split(UsersGetSpecs.Separator)
                .Select(u => int.TryParse(u.Trim(), out var userId) ? userId : UsersGetSpecs.NullInteger)
                .Where(u => u != -1)
                .ToList();

    private static List<Guid> ParseCsvToGuidList(string stringList)
        => !stringList.HasValue()
            ? []
            : stringList.Split(UsersGetSpecs.Separator)
                .Select(u => Guid.TryParse(u.Trim(), out var userGuid) ? userGuid : Guid.Empty)
                .Where(u => u != Guid.Empty)
                .ToList();

}
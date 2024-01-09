﻿using System;
using System.Runtime.CompilerServices;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class CodeDataFactoryExtensions
{
    /// <summary>
    /// Will check if the CodeDataFactory exists and try to get the ServiceKit - or throw an error. 
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    internal static ServiceKit16 GetServiceKitOrThrow(this Internal.CodeDataFactory cdf, [CallerMemberName] string cName = default)
    {
        if (cdf == null)
            throw new NotSupportedException(
                $"Trying to use {cName}(...) in a scenario where the {nameof(cdf)} is not available.");

        var kit = cdf._DynCodeRoot.GetKit<ServiceKit16>();
        return kit ?? throw new NotSupportedException(
            $"Trying to use {cName}(...) in a scenario where the {nameof(ServiceKit16)} is not available.");
    }

}
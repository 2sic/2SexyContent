﻿using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class CustomCodeBase : ServiceForDynamicCode, ICompatibilityLevel
{
    #region Constructor / Setup

    /// <summary>
    /// Main constructor, NOT for DI may never have parameters, otherwise inheriting code will run into problems. 
    /// </summary>
    protected CustomCodeBase(string logName) : base(logName) { }

    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected internal CodeHelper SysHlp => _sysHlp ??= new CodeHelper().Init(this);
    private CodeHelper _sysHlp;


    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void ConnectToRoot(ICodeApiService codeRoot) => base.Log.Do(() =>
    {
        base.ConnectToRoot(codeRoot);
        SysHlp.ConnectToRoot(codeRoot);
    });

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract int CompatibilityLevel { get; }

    #endregion
}
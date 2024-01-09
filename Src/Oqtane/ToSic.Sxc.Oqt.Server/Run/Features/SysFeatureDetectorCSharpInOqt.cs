﻿using ToSic.Eav.Internal.Features;
using static ToSic.Eav.Internal.Features.SysFeatureSuggestions;

namespace ToSic.Sxc.Oqt.Server.Run.Features;

public class SysFeatureDetectorCSharp6: SysFeatureDetector
{
    public SysFeatureDetectorCSharp6() : base(CSharp06, true) { }
}
public class SysFeatureDetectorCSharp7: SysFeatureDetector
{
    public SysFeatureDetectorCSharp7() : base(CSharp07, true) { }
}
public class SysFeatureDetectorCSharp8: SysFeatureDetector
{
    public SysFeatureDetectorCSharp8() : base(CSharp08, true) { }
}
public class SysFeatureDetectorCSharp9: SysFeatureDetector
{
    public SysFeatureDetectorCSharp9() : base(CSharp09, true) { }
}
public class SysFeatureDetectorCSharp10: SysFeatureDetector
{
    public SysFeatureDetectorCSharp10() : base(CSharp10, true) { }
}
public class SysFeatureDetectorCSharp11: SysFeatureDetector
{
    public SysFeatureDetectorCSharp11() : base(CSharp11, true) { }
}
public class SysFeatureDetectorCSharp12: SysFeatureDetector
{
    public SysFeatureDetectorCSharp12() : base(CSharp12, false) { }
}
using SolidWorks.Interop.swconst;

internal static class FeatTypeNameUtil
{
    public const string RefPlane = nameof(RefPlane);

    public const string LibraryFeature = nameof(LibraryFeature);

    public const string OriginProfileFeature = nameof(OriginProfileFeature);

    public const string RefAxis = nameof(RefAxis);

    public const string MacroFeature = nameof(MacroFeature);

    public const string ProfileFeature = nameof(ProfileFeature);

    public const string Extrusion = nameof(Extrusion);

    public const string RevCut = nameof(RevCut);

    public const string Attribute = nameof(Attribute);

    public const string ICE = nameof(ICE);

    public const string Chamfer = nameof(Chamfer);

    public const string Component = "Reference";

    public const string CosmeticThread = nameof(CosmeticThread);

    public static bool IsCavity(string typeName)
    {
        return typeName == RevCut;
    }

    public static bool NeedToSuppressedWhenDrawing(string typeName)
    {
        return typeName == ICE || typeName == Chamfer;
    }

    public static bool IsTopCavityFeatureTypeName(string typeName)
    {
        return typeName == MacroFeature || typeName == LibraryFeature;
    }

    public static bool IsEntity(int type)
    {
        return type == (int)swSelectType_e.swSelFACES ||
            type == (int)swSelectType_e.swSelEDGES;
    }
}

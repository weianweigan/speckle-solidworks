using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;

namespace Speckle.ConnectorSolidWorks.Selection;

public enum SolidWorksFilterType
{
    Body,
    CustomProperty,
    Equation,
    Feature,

    Component,
    Mate,

    DimensionTable,
    Sketch,

    SketchSegment,
    SketchPoint,

    DrawingView,
    DrawingSketch,
    BomTable,
}

public static class SolidWorksFilterTypeUtils
{
    public static IEnumerable<SolidWorksFilterType> ByDoc(IModelDoc2 doc)
    {
        var docType = (swDocumentTypes_e)doc.GetType();

        if(doc.SketchManager.ActiveSketch != null)
        {
            return ForSketch();
        }
        else
        {
            return docType switch
            {
                swDocumentTypes_e.swDocPART => ForPartDoc(),
                swDocumentTypes_e.swDocASSEMBLY => ForAssemblyDoc(),
                swDocumentTypes_e.swDocDRAWING => ForDrawingDoc(),
                _ => throw new System.NotSupportedException($"Filter Not Support {docType}")
            };
        }
    }

    public static IEnumerable<SolidWorksFilterType> ForPartDoc()
    {
        yield return SolidWorksFilterType.Body;
        yield return SolidWorksFilterType.CustomProperty;
        yield return SolidWorksFilterType.Equation;
        yield return SolidWorksFilterType.Feature;
        yield return SolidWorksFilterType.DimensionTable;
    }

    public static IEnumerable<SolidWorksFilterType> ForAssemblyDoc()
    {
        yield return SolidWorksFilterType.CustomProperty;
        yield return SolidWorksFilterType.Equation;

        yield return SolidWorksFilterType.Mate;

        yield return SolidWorksFilterType.Component;
    }

    public static IEnumerable<SolidWorksFilterType> ForDrawingDoc()
    {
        yield return SolidWorksFilterType.DrawingView;
        yield return SolidWorksFilterType.DrawingSketch;
        yield return SolidWorksFilterType.BomTable;
    }

    public static IEnumerable<SolidWorksFilterType> ForSketch()
    {
        yield return SolidWorksFilterType.SketchPoint;
        yield return SolidWorksFilterType.SketchSegment;
    }
}
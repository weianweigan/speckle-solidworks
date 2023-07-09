using Objects.Organization;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.Objects.SolidWorks;

namespace Objects.Converter.SolidWorks.Converters;

public static class SwDocumentConverter
{
    public static Model ConvertToSpeckleModel(IModelDoc2 doc)
    {
        var modeInfo = ToModelInfo(doc);
        return new Model(modeInfo);
    }

    public static SwModelInfo ToModelInfo(IModelDoc2 modelDoc)
    {
        var documentType = (swDocumentTypes_e)modelDoc.GetType();

        var swModelInfo = new SwModelInfo();

        swModelInfo.DocumentType = documentType switch
        { 
            swDocumentTypes_e.swDocPART => DocumentType.Part.ToString(),
            swDocumentTypes_e.swDocASSEMBLY => DocumentType.Assembly.ToString(),
            swDocumentTypes_e.swDocDRAWING => DocumentType.Drawing.ToString(),
            _ => DocumentType.UnKnown.ToString()
        };

        swModelInfo.name = modelDoc.GetTitle();
        swModelInfo.number = 1.ToString();

        return swModelInfo;
    }
}

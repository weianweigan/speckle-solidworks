using Objects.Converter.SolidWorks.Converters;
using SolidWorks.Interop.sldworks;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using swconst = SolidWorks.Interop.swconst;

namespace Objects.Converter.SolidWorks;

public partial class ConverterSolidWorks
{
    public bool CanConvertToSpeckle(object @object)
    {
        return @object is IModelDoc2 or SwSeleTypeObjectPair;
    }

    public Base ConvertToSpeckle(object @object)
    {
        if (@object is IModelDoc2 doc)
        {
            return SwDocumentConverter.ConvertToSpeckleModel(doc);
        }
        else if (@object is SwSeleTypeObjectPair swSeleTypeObjectPair)
        {
            return swSeleTypeObjectPair.SelectType switch
            {
                swconst.swSelectType_e.swSelSOLIDBODIES => SwBody2Converter.ConvertToSpeckleSwBody(swSeleTypeObjectPair.SelectedObject as IBody2),
                _ => null
            };
        }
        else
        {
            throw new NotSupportedException($"Not support object type:{@object.GetType().FullName}");
        }
    }

    public List<Base> ConvertToSpeckle(List<object> objects)
    {
        return objects
            .Select(ConvertToSpeckle)
            .ToList();
    }
}

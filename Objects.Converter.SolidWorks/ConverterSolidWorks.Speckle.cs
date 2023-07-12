using Objects.Converter.SolidWorks.Converters;
using Objects.Converter.SolidWorks.Utils;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using swconst = SolidWorks.Interop.swconst;

namespace Objects.Converter.SolidWorks;

public partial class ConverterSolidWorks
{
    private HashSet<swSelectType_e> _supportTypes = new HashSet<swSelectType_e>() { 
        swSelectType_e.swSelNOTHING,
        swSelectType_e.swSelCOMPONENTS,
        swSelectType_e.swSelSOLIDBODIES
    };

    public bool CanConvertToSpeckle(object @object)
    {
        if (@object is IModelDoc2)
        {
            return true;
        }else if (@object is SwSeleTypeObjectPair objectPair)
        {
            return _supportTypes.Contains(objectPair.SelectType);
        }
        else
        {
            return false;
        }
    }

    public Base ConvertToSpeckle(object @object)
    {
        var materialValue = Doc.GetMaterialValue();

        if (@object is IModelDoc2 doc)
        {
            return SwDocumentConverter.ToSpeckleModel(doc);
        }
        else if (@object is SwSeleTypeObjectPair swSeleTypeObjectPair)
        {
            if (swSeleTypeObjectPair.SelectType == swconst.swSelectType_e.swSelNOTHING)
            {
                if (swSeleTypeObjectPair.SelectedObject is ICustomPropertyManager customPropertyManager)
                {
                    return SwCustomProperty2DataTable.ToSpeckleDataTable(customPropertyManager);
                }
                else if (swSeleTypeObjectPair.SelectedObject is IEquationMgr equationMgr)
                {
                    return SwEquationMgr2DataTable.ToSpeckleDataTable(equationMgr);
                }
                else
                {
                    SpeckleLog.Logger.Information($"Not support object type:{swSeleTypeObjectPair.SelectType} {swSeleTypeObjectPair.SelectedObject.GetType().FullName}");
                    throw new NotSupportedException($"Not support object type:{@object.GetType().FullName}");
                }
            }
            else
            {
                return swSeleTypeObjectPair.SelectType switch
                {
                    swconst.swSelectType_e.swSelSOLIDBODIES => SwBodyToBrepConverter.ToSpeckle(swSeleTypeObjectPair, materialValue),
                    swconst.swSelectType_e.swSelCOMPONENTS => SwComponent2ToComponentConverter.ToSpeckle(Doc, swSeleTypeObjectPair, materialValue),
                    _ => null
                };
            }
        }
        else
        {
            SpeckleLog.Logger.Information($"Not support object type:{@object.GetType().FullName}");
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

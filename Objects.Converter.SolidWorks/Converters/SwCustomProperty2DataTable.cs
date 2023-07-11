using Objects.Organization;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Models;
using swconst = SolidWorks.Interop.swconst;

namespace Objects.Converter.SolidWorks.Converters;

internal static class SwCustomProperty2DataTable
{
    public static DataTable ToSpeckleDataTable(
        ICustomPropertyManager customPropertyManager,
        string pid = nameof(CustomPropertyManager))
    {
        var dataTable = new DataTable()
        {
            applicationId = pid
        };

        // Define columns.
        dataTable.DefineColumn(new Base
        {
            ["Name"] = "Property Name"
        });
        dataTable.DefineColumn(new Base
        {
            ["Type"] = "Property Type"
        });
        dataTable.DefineColumn(new Base
        {
            ["Value"] = "Value"
        });

        // Add rows.
        var metadata = new Base();

        // Add header row
        dataTable.AddRow(metadata, objects: new string[]
        {
            "Property Name", 
            "Property Type", 
            "Value" 
        }, index: dataTable.headerRowIndex);

        // https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.ICustomPropertyManager~GetAll2.html
        object propertyNames = null;
        object propertyTypes = null;
        object propertyValues = null;
        object propertyResolved = null;
        // SolidWorks 2014 above
        int count = customPropertyManager.GetAll2(ref propertyNames, ref propertyTypes, ref propertyValues, ref propertyResolved);
        if (count <= 0)
        {
            return dataTable;
        }

        var names = (string[])propertyNames;
        var types = (swconst.swCustomInfoType_e[])propertyTypes;
        var values = (string[])propertyValues;
        for (int i = 0; i < count; i++)
        {
            dataTable.AddRow(metadata, objects: new string[] { 
                names[i], 
                types[i].ToString(), 
                values[i] });
        }

        return dataTable;
    }
}

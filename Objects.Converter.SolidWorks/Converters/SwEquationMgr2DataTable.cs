using Objects.Organization;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Models;

namespace Objects.Converter.SolidWorks.Converters;

internal static class SwEquationMgr2DataTable
{
    /// <summary>
    /// Convert SolidWorks EquationMgr to a Speckle DataTable.
    /// </summary>
    /// <remarks>
    /// <see href="http://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IEquationMgr_members.html"/>
    /// </remarks>
    /// <param name="equationMgr"></param>
    /// <param name="pid"></param>
    /// <returns></returns>
    public static DataTable ToSpeckleDataTable(
        IEquationMgr equationMgr, 
        string pid = nameof(EquationMgr))
    {
        var dataTable = new DataTable() { 
            applicationId = pid
        };

        // Define columns.
        var columnMetadata = new Base();
        columnMetadata["Equation"] = "SwEquation";
        // TODO
        dataTable.DefineColumn(columnMetadata);

        // Add rows.
        var metadata = new Base();

        // Add row header.
        dataTable.AddRow(
            metadata, 
            objects: new string[] { "Equation" }, 
            index: dataTable.headerRowIndex);

        int count = equationMgr.GetCount();
        if (count <= 0)
        {
            return dataTable;
        }

        for (int i = 0; i < count; i++)
        {
            // TODO: Separate equation into name and value.
            string value = equationMgr.Equation[i];
            dataTable.AddRow(metadata, objects: new string[] { value});
        }

        return dataTable;
    }
}

using Objects.Organization;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Converter.SolidWorks.Converters;

internal static class SwEquationMgr2DataTable
{
    /// <summary>
    /// Convert SolidWorks EquationMgr to a Speckle DataTable.
    /// </summary>
    /// <remarks>
    /// <see href="http://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IEquationMgr_members.html"/>
    /// </remarks>
    /// <param name="customPropertyMgr"></param>
    /// <param name="pid"></param>
    /// <returns></returns>
    public static DataTable ToSpeckleDataTable(
        IEquationMgr customPropertyMgr, 
        string pid = nameof(EquationMgr))
    {
        var dataTable = new DataTable() { 
            applicationId = pid
        };

        // Define columns.
        var columnMetadata = new Base();
        // TODO
        dataTable.DefineColumn(columnMetadata);

        // Add rows.
        var metadata = new Base();

        // TODO
        dataTable.AddRow(metadata, objects: new string[] { });

        return dataTable;
    }
}

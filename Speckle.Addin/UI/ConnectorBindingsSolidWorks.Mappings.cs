using DesktopUI2.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Speckle.ConnectorSolidWorks.UI;

public partial class ConnectorBindingsSolidWorks
{
    /// <summary>
    /// Imports new family types into Revit.
    /// </summary>
    /// <param name="Mapping"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task<Dictionary<string, List<MappingViewModel.MappingValue>>> ImportFamilyCommand(
        Dictionary<string, List<MappingViewModel.MappingValue>> Mapping)
    {
        return Task.FromResult(Mapping);
    }
}

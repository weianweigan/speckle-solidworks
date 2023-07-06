using DesktopUI2.Models.Filters;
using Speckle.ConnectorSolidWorks.Selection;
using System.Collections.Generic;
using System.Linq;

namespace Speckle.ConnectorSolidWorks.UI;

public partial class ConnectorBindingsSolidWorks
{
    public override void SelectClientObjects(
        List<string> objs, 
        bool deselect = false)
    {
        if (deselect)
        {

        }
        else
        {

        }
    }

    public override List<string>? GetSelectedObjects()
    {
        var doc = App.IActiveDoc2;

        return doc
            ?.GetSelections()
            .Select(p => p.Name)
            .ToList();
    }

    public override List<ISelectionFilter> GetSelectionFilters()
    {
        return SolidWorksSelectionFilter.CreateAll();
    }
}

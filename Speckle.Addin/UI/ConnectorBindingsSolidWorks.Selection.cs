using DesktopUI2.Models.Filters;
using Speckle.ConnectorSolidWorks.Selection;
using System.Collections.Generic;
using System.Linq;

namespace Speckle.ConnectorSolidWorks.UI;

public partial class ConnectorBindingsSolidWorks
{
    /// <summary>
    /// Select solidworks objects by their unique id.
    /// </summary>
    /// <remarks>
    /// <see href="https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IModelDocExtension~MultiSelect2.html"/>
    /// </remarks>
    /// <param name="objs"></param>
    /// <param name="deselect"></param>
    public override void SelectClientObjects(
        List<string> objs, 
        bool deselect = false)
    {
        if (objs == null)
        {
            return;
        }

        var doc = App.IActiveDoc2;
        if (doc == null)
        {
            return;
        }

        var swSeleObjs = objs
            .Select(o => SwSeleTypeObjectPair.FromJson(o))
            .ToList();

        if (deselect)
        {
            foreach (var obj in swSeleObjs)
            {
                doc.ISelectionManager.DeSelect2(obj.Index, obj.Mark);
            }
        }
        else
        {
            foreach (var obj in swSeleObjs)
            {
                obj.ReSolveFormPID(doc);
                doc.Extension.MultiSelect2(null, true, null);
            }

            doc.Extension.MultiSelect2(swSeleObjs.ToArray(), true, null);
        }
    }

    public override List<string>? GetSelectedObjects()
    {
        var doc = App.IActiveDoc2;

        IEnumerable<SwSeleTypeObjectPair>? selections = doc
            ?.GetSelections();

        return selections.Select(p => p.ToJson()).ToList();
    }

    public override List<ISelectionFilter> GetSelectionFilters()
    {
        return SolidWorksSelectionFilter.CreateAll();
    }
}

using DesktopUI2.Models.Filters;
using SolidWorks.Interop.sldworks;
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
    /// Triggered when user want to preview or send to server.
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

    /// <summary>
    /// Triggered when user use a ManualSelectionFilter, then click set current selection.
    /// </summary>
    /// <returns></returns>
    public override List<string>? GetSelectedObjects()
    {
        var doc = App.IActiveDoc2;

        IEnumerable<SwSeleTypeObjectPair>? selections = doc
            ?.GetSelections();

        return selections.Select(p => p.ToJson()).ToList();
    }

    /// <summary>
    /// Create selection filter for solidworks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Triggered when user open a stream.
    /// </para>
    /// <para>
    /// Allowed types
    /// <list type="bullet">
    /// <item>
    /// <see cref="AllSelectionFilter"/>
    /// </item>
    /// <item>
    /// <see cref="ListSelectionFilter"/>
    /// </item>
    /// <item>
    /// <see cref="ManualSelectionFilter"/>
    /// </item>
    /// <item>
    /// <see cref="PropertySelectionFilter"/>
    /// </item>
    /// <item>
    /// <see cref="TreeSelectionFilter"/>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <returns>List of ISelectionFilter</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public override List<ISelectionFilter> GetSelectionFilters()
    {
        var doc = App.IActiveDoc2;
        return CreateFilter(doc).ToList();
    }

    public IEnumerable<ISelectionFilter> CreateFilter(IModelDoc2 doc)
    {
        yield return new AllSelectionFilter
        {
            Slug = "all",
            Name = "Everything",
            Icon = "CubeScan",
            Description = "Sends all supported elements and project information in your solidworks document"
        };

        yield return new ManualSelectionFilter();

        yield return new ListSelectionFilter
        {
            Slug = "category",
            Name = "Category",
            Icon = "Category",
            Values = SolidWorksFilterTypeUtils.ByDoc(doc).Select(p => p.ToString()).ToList(),
            Description = "Adds all elements belonging to the selected categories"
        };

        // TODO: TreeSelectionFilter
    }
}

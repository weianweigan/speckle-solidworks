using DesktopUI2.Models.Filters;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.ConnectorSolidWorks.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Speckle.ConnectorSolidWorks.Utils;

internal static class SwDocFilterUtils
{
    public static IEnumerable<SwSeleTypeObjectPair> Filter(
        IModelDoc2 doc, 
        ISelectionFilter selectionFilter)
    {
        var items = selectionFilter switch
        {
            AllSelectionFilter => All(doc),
            ListSelectionFilter listSelectionFilter => ByCategory(doc, listSelectionFilter),
            ManualSelectionFilter => BySelection(doc),
            _ => throw new NotSupportedException($"Not support ISelectionFilter:{selectionFilter.GetType().FullName}")
        };

        foreach (var item in items)
        {
            item.PID = PIDUtils.GetPID(doc, item.SelectedObject);
            yield return item;
        }
    }

    private static IEnumerable<SwSeleTypeObjectPair> BySelection(IModelDoc2 doc)
    {
        return doc.GetSelections();
    }

    public static IEnumerable<SwSeleTypeObjectPair> All(IModelDoc2 doc)
    {
        if (doc is IPartDoc partDoc)
        {
            return Bodies(partDoc);
        }
        else
        {
            return Enumerable.Empty<SwSeleTypeObjectPair>();
        }
    }

    public static IEnumerable<SwSeleTypeObjectPair> ByCategory(
        IModelDoc2 doc, 
        ListSelectionFilter listSelectionFilter)
    {
        var types = listSelectionFilter.Selection
            .Select(f => Enum.Parse(typeof(SolidWorksFilterType),f))
            .ToList();

        var items = Enumerable.Empty<SwSeleTypeObjectPair>();
        foreach (var type in types)
        {
            var itemsByType = type switch
            {
                SolidWorksFilterType.Body => Bodies(doc as IPartDoc),
                _ => throw new NotSupportedException($"Not support SolidWorksFilterType:{type}")
            };
            items = items.Union(itemsByType);
        }

        return items;
    }

    public static IEnumerable<SwSeleTypeObjectPair> Bodies(IPartDoc? doc)
    {
        if (doc == null)
        {
            return Enumerable.Empty<SwSeleTypeObjectPair>();
        }

        var bodies = ((object[])doc
            .GetBodies2((int)swBodyType_e.swAllBodies, true))
            .OfType<IBody2>()
            .Where(b => {
                var bodyType = (swBodyType_e)b.GetType();
                return bodyType == swBodyType_e.swSolidBody || bodyType == swBodyType_e.swSheetBody;
            })
            .Select(b => new SwSeleTypeObjectPair(b.Name, swSelectType_e.swSelSOLIDBODIES, b));

        return bodies;
    }
} 

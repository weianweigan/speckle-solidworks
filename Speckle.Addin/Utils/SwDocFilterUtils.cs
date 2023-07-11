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
            if (item.SelectType == swSelectType_e.swSelNOTHING)
            {
                item.PID = item.Name;
            }
            else
            {
                item.PID = PIDUtils.GetPID(doc, item.SelectedObject);
            }
            yield return item;
        }
    }

    private static IEnumerable<SwSeleTypeObjectPair> BySelection(IModelDoc2 doc)
    {
        return doc.GetSelections();
    }

    public static IEnumerable<SwSeleTypeObjectPair> All(IModelDoc2 doc)
    {
        var customPropertyMgr = CustomPropertyManager(doc);
        if (customPropertyMgr != null)
        {
            yield return customPropertyMgr;
        }

        var equationMgr = EquationMgr(doc);
        if (equationMgr != null)
        {
            yield return equationMgr;
        }

        var features = doc.GetTopFeatures();

        swDocumentTypes_e docType = (swDocumentTypes_e)doc.GetType();

        if (docType == swDocumentTypes_e.swDocPART)
        {
            foreach (var item in Bodies((IPartDoc)doc))
            {
                yield return item;
            }
        }

        foreach (var feature in features)
        {
            string typeName = feature.GetTypeName2();
            if(docType == swDocumentTypes_e.swDocASSEMBLY && typeName == FeatTypeNameUtil.Component)
            {
                var comp = (IComponent2)feature.GetSpecificFeature2();
                yield return new SwSeleTypeObjectPair(comp.Name2, swSelectType_e.swSelCOMPONENTS, comp) ;
            }
            // TODO: Features
        }
    }

    public static IEnumerable<SwSeleTypeObjectPair> ByCategory(
        IModelDoc2 doc, 
        ListSelectionFilter listSelectionFilter)
    {
        swDocumentTypes_e docType = (swDocumentTypes_e)doc.GetType();

        var types = listSelectionFilter.Selection
            .Select(f => (SolidWorksFilterType)Enum.Parse(typeof(SolidWorksFilterType),f))
            .Distinct()
            .ToList();

        foreach (var type in types)
        {
            if (docType == swDocumentTypes_e.swDocPART && type == SolidWorksFilterType.Body)
            {
                foreach (var item in Bodies((IPartDoc)doc))
                {
                    yield return item;
                }
            }

            if (type == SolidWorksFilterType.CustomProperty)
            {
                var customPropertyMgr = CustomPropertyManager(doc);
                if (customPropertyMgr != null)
                {
                    yield return customPropertyMgr;
                }
            }
            if (type == SolidWorksFilterType.Equation)
            {
                var equationMgr = EquationMgr(doc);
                if (equationMgr != null)
                {
                    yield return equationMgr;
                }
            }
        }

        var features = doc.GetTopFeatures();

        bool hasComponent = types.Contains(SolidWorksFilterType.Component);
        foreach (var feature in features)
        {
            string typeName = feature.GetTypeName2();
            if (hasComponent && docType == swDocumentTypes_e.swDocASSEMBLY && typeName == FeatTypeNameUtil.Component)
            {
                var comp = (IComponent2)feature.GetSpecificFeature2();
                yield return new SwSeleTypeObjectPair(comp.Name2, swSelectType_e.swSelCOMPONENTS, comp);
            }
            // TODO: Features
        }
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

    public static SwSeleTypeObjectPair? CustomPropertyManager(IModelDoc2 doc)
    {
        var customPropertyMgr = doc.Extension.CustomPropertyManager[""];
        if (customPropertyMgr != null)
        {
            return new SwSeleTypeObjectPair(nameof(CustomPropertyManager), swSelectType_e.swSelNOTHING, customPropertyMgr);
        }
        else
        {
            return null;
        }
    }

    public static SwSeleTypeObjectPair? EquationMgr(IModelDoc2 doc)
    {
        var equationMgr = doc.GetEquationMgr();
        if (equationMgr != null)
        {
            return new SwSeleTypeObjectPair(nameof(EquationMgr), swSelectType_e.swSelNOTHING, equationMgr);
        }
        else
        {
            return null;
        }
    }
} 

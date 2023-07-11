using Objects.Converter.SolidWorks.Utils;
using Objects.Other;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.ConnectorSolidWorks.Utils;
using Speckle.Core.Models;
using Speckle.Objects.SolidWorks;
using System.Collections.Generic;
using speckleComp = Objects.BuiltElements.SolidWorks.Component;

namespace Objects.Converter.SolidWorks.Converters;

internal static class SwComponent2ToComponentConverter
{
    public static speckleComp ToSpeckle(
        IModelDoc2 doc, 
        SwSeleTypeObjectPair seleTypeObjectPair, 
        MaterialValue? materialValue)
    {
        return ToSpeckle(doc, (IComponent2)seleTypeObjectPair.SelectedObject, materialValue);
    }

    public static speckleComp ToSpeckle(
        IModelDoc2 doc,
        IComponent2 component,
        MaterialValue? materialValue)
    {
        string pid = PIDUtils.GetPID(doc, component);

        // Base information for solid works component.
        speckleComp speckleComponent = new speckleComp
        {
            applicationId = pid,
            Name = component.Name,
            IsSuppressed = component.IsSuppressed(),
            IsVirtual = component.IsVirtual,
            IsPatternInstance = component.IsPatternInstance(),
            ComponentId = component.GetID(),
            children = new ()
        };

        MaterialValue? compMaterial = component.GetMaterialValue() ?? materialValue;
        Transform transform = component.Transform2.ToSpeckleTransform();

        // Get bodies from component.
        var bodies = (object[])component.GetBodies3((int)swBodyType_e.swSolidBody, out object bodyObjects);

        // Convert bodies.
        if (bodies.Length > 0)
        {
            var bodiesList = new List<Base>();
            foreach (IBody2 body in bodies)
            {
                if ((swBodyType_e)body.GetType() != swBodyType_e.swSolidBody)
                {
                    continue;
                }

                MaterialValue? bodyMaterialValue = body.GetMaterialValue() ?? compMaterial;
                bodiesList.Add(SwBodyToBrepConverter.ToSpeckle(body, bodyMaterialValue, pid, transform));
            }
            speckleComponent.displayValue = bodiesList;
        }

        // Children.
        var children = (object[])component.GetChildren();
        foreach (IComponent2 child in children)
        {
            var childSpeckleComp = ToSpeckle(doc, child, compMaterial);
            speckleComponent.children.Add(childSpeckleComp);
        }

        // Return value.
        return speckleComponent;
    }
}

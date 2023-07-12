using Objects.Geometry;
using Objects.Other;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Core.Models;
using Speckle.Objects.SolidWorks;

namespace Objects.Converter.SolidWorks.Converters;

internal static class SwBodyToBrepConverter
{
    // Temp use mesh until brep converter are implemented
    public static bool UseMesh = true;

    public static BuiltElements.SolidWorks.Body ToSpeckle(
        SwSeleTypeObjectPair objectPair,
        MaterialValue? materialValue,
        string pid = null)
    {
        return ToSpeckle(
            objectPair.SelectedObject as IBody2, 
            materialValue,
            objectPair.PID);
    }

    public static BuiltElements.SolidWorks.Body ToSpeckle(
        IBody2 body,
        MaterialValue? materialValue,
        string pid,
        Transform transform = null)
    {
        Base display;
        if (UseMesh)
        {
            Mesh mesh = SwBody2MeshConverter.ToSpeckleMesh(body, materialValue, pid);
            if (transform != null)
            {
                mesh.TransformTo(transform, out Mesh newMesh);
                mesh = newMesh;
            }
            display = mesh;
        }
        else
        {
            var brep = ConvertToSpeckleBrep(body, materialValue, pid);
            if (transform != null)
            {
                brep.TransformTo(transform, out Brep newBrep);
                brep = newBrep;
            }
            display = brep;
        }
        //https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IBody2~GetMassProperties.html
        var massProperties = (double[])body.GetMassProperties(1);

        var bodyType = (swBodyType_e)body.GetType();
        var (volume, area, bodyTypeName) = bodyType switch
        {
            swBodyType_e.swSheetBody => (massProperties[3], massProperties[4], "SheetBody"),
            swBodyType_e.swSolidBody => (massProperties[3], massProperties[3], "SolidBody"),
            _ => (0, 0, "")
        };

        return new BuiltElements.SolidWorks.Body()
        {
            Name = body.Name,
            applicationId = pid,
            displayValue = display,
            volume = volume,
            BodyType = bodyTypeName,
            area = area
        };
    }

    public static Brep ConvertToSpeckleBrep(
        IBody2 body2, 
        MaterialValue? materialValue, 
        string pid)
    {
        Mesh mesh = SwBody2MeshConverter.ToSpeckleMesh(body2,materialValue ,pid);

        var brep = new Brep("SOLIDWORKS", mesh, applicationId: pid);

        // TODO: Construct brep faces from body faces

        return brep;
    }
}

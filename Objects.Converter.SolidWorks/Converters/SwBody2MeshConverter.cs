using Objects.Converter.SolidWorks.Utils;
using Objects.Geometry;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Objects.SolidWorks;
using System.Collections.Generic;

namespace Objects.Converter.SolidWorks.Converters;

/// <summary>
/// Converter that turns a SolidWorks Body into a Speckle Mesh.
/// </summary>
internal static class SwBody2MeshConverter
{
    /// <summary>
    /// Option for body tessellation quality. Default is true.
    /// </summary>
    public static bool ImproveQuality { get; set; } = false;

    public static BuiltElements.SolidWorks.Body ToSpeckle(
        SwSeleTypeObjectPair objectPair, MaterialValue? materialValue)
    {
        return ToSpeckle(objectPair.SelectedObject as IBody2, materialValue, objectPair.PID);
    }

    public static BuiltElements.SolidWorks.Body ToSpeckle(
        IBody2 body,
        MaterialValue? materialValue,
        string pid = null)
    {
        Mesh mesh = ToSpeckleMesh(body, materialValue, pid);

        var (volume, area, bodyTypeName) = GetBodyInformation(body);

        return new BuiltElements.SolidWorks.Body()
        {
            Name = body.Name,
            applicationId = pid,
            displayValue = mesh,
            volume = volume,
            BodyType = bodyTypeName,
            area = area
        };
    }

    public static Mesh ToSpeckleMesh(
        IBody2 body, 
        MaterialValue? materialValue, 
        string pid = null)
    {
        var tessellation = (ITessellation)body.GetTessellation(null);

        tessellation.NeedFaceFacetMap = true;
        tessellation.NeedVertexParams = true;
        tessellation.NeedVertexNormal = true;
        tessellation.ImprovedQuality = ImproveQuality;

        tessellation.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
        // Do it
        bool bResult = tessellation.Tessellate();
        if (!bResult)
        {
            Speckle.Core.Logging.SpeckleLog.Logger.Information("Tessellation failed");
            return new Mesh();
        }

        MaterialValue? bodyMaterialValue = body.GetMaterialValue() ?? materialValue;

        // Tess all faces
        var face = (IFace2)body.GetFirstFace();
        List<double> vertices = new();
        List<int> faces = new();
        List<int> colors = new();

        int triangleCount = -1;
        while (face != null)
        {
            int[] vFacetId = (int[])tessellation.GetFaceFacets(face);

            // Should always be three fins per facet
            for (int i = 0; i < vFacetId.Length; i++)
            {
                int[] vFinId = (int[])tessellation.GetFacetFins(vFacetId[i]);
                for (int j = 0; j < 3; j++)
                {
                    int[] vVertexId = (int[])tessellation.GetFinVertices(vFinId[j]);
                    // Should always be two vertices per fin
                    double[] vVertex0 = (double[])tessellation.GetVertexPoint(vVertexId[0]);
                    //double[] vVertex1 = (double[])tessellation.GetVertexPoint(vVertexId[1]);

                    vertices.Add(vVertex0[0]); vertices.Add(vVertex0[1]); vertices.Add(vVertex0[2]);
                }
                faces.Add(3);// TRIANGLE flag
                faces.Add(triangleCount + 1);
                faces.Add(++triangleCount + 2);
                faces.Add(++triangleCount + 3);

                int? color = face.ToARGB();
                if (color != null)
                {
                    colors.Add(triangleCount + 1);
                    colors.Add(++triangleCount + 2);
                    colors.Add(++triangleCount + 3);
                    colors.Add(color.Value);
                }

                triangleCount += 3;
            }
            face = (IFace2)face.GetNextFace();
        }

        var mesh = new Mesh(vertices, faces, colors, applicationId: pid);

        Other.RenderMaterial renderMaterial = bodyMaterialValue.GetRenderMaterial();
        if (renderMaterial != null)
        {
            mesh["renderMaterial"] = renderMaterial;
        }

        return mesh;
    }

    public static (double, double, string) GetBodyInformation(
    IBody2 body)
    {
        //https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IBody2~GetMassProperties.html
        var massProperties = (double[])body.GetMassProperties(1);

        var bodyType = (swBodyType_e)body.GetType();
        return bodyType switch
        {
            swBodyType_e.swSheetBody => (massProperties[3], massProperties[4], "SheetBody"),
            swBodyType_e.swSolidBody => (massProperties[3], massProperties[3], "SolidBody"),
            _ => (0, 0, "")
        };
    }
}
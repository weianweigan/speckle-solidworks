using Objects.BuiltElements.SolidWorks;
using Objects.Converter.SolidWorks.Utils;
using Objects.Geometry;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Speckle.ConnectorSolidWorks.Selection;
using System.Collections.Generic;

namespace Objects.Converter.SolidWorks.Converters;

/// <summary>
/// Converter that turns a SolidWorks Body into a Speckle Mesh.
/// </summary>
internal static class SwBody2Converter
{
    /// <summary>
    /// Option for body tessellation quality. Default is true.
    /// </summary>
    public static bool ImproveQuality { get; set; } = true;

    public static SolidWorksBody ConvertToSpeckleSwBody(SwSeleTypeObjectPair objectPair)
    {
        return ConvertToSpeckleSwBody(objectPair.SelectedObject as IBody2, objectPair.PID);
    }

    public static SolidWorksBody ConvertToSpeckleSwBody(IBody2 body, string pid = null)
    {
        var mesh = ConvertToSpeckleMesh(body, pid);

        //https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.IBody2~GetMassProperties.html
        var massProperties = (double[])body.GetMassProperties(1);

        var bodyType = (swBodyType_e)body.GetType();
        var (volume,area,bodyTypeName) = bodyType switch
        {
            swBodyType_e.swSheetBody => (massProperties[3], massProperties[4], "SheetBody"),
            swBodyType_e.swSolidBody => (massProperties[3], massProperties[3], "SolidBody"),
            _ => (0, 0, "")
        };

        return new SolidWorksBody()
        {
            displayValue = mesh,
            volume = volume,
            BodyType = bodyTypeName,
            area = area
        };
    }

    public static Mesh ConvertToSpeckleMesh(IBody2 body, string pid = null)
    {
        var tessellation = (ITessellation)body.GetTessellation(null);

        tessellation.NeedFaceFacetMap = true;
        tessellation.NeedVertexParams = true;
        tessellation.NeedVertexNormal = true;
        tessellation.ImprovedQuality = ImproveQuality;

        // How to handle matches across common edges
        tessellation.MatchType = (int)swTesselationMatchType_e.swTesselationMatchFacetTopology;
        // Do it
        bool bResult = tessellation.Tessellate();
        if (!bResult)
        {
            Speckle.Core.Logging.SpeckleLog.Logger.Information("Tessellation failed");
            return new Mesh();
        }

        // Tess all faces
        var face = (IFace2)body.GetFirstFace();
        List<double> vertices = new();
        List<int> faces = new();
        List<int> colors = new();
        while (face != null)
        {
            int[] vFacetId = (int[])tessellation.GetFaceFacets(face);

            //Should always be three fins per facet
            for (int i = 0; i < vFacetId.Length; i++)
            {
                int[] vFinId = (int[])tessellation.GetFacetFins(vFacetId[i]);
                for (int j = 0; j < 3; j++)
                {
                    int[] vVertexId = (int[])tessellation.GetFinVertices(vFinId[j]);
                    //Should always be two vertices per fin
                    //double[] vVertex1 = (double[])tessellation.GetVertexPoint(vVertexId[0]);
                    double[] vVertex2 = (double[])tessellation.GetVertexPoint(vVertexId[1]);

                    vertices.Add(vVertex2[0]); vertices.Add(vVertex2[1]); vertices.Add(vVertex2[2]);
                }
                faces.Add(i);
            }
            colors.Add(face.ToARGB());
            face = (IFace2)face.GetNextFace();
        }

        var mesh = new Mesh(vertices, faces, colors, applicationId: pid);

        Other.RenderMaterial renderMaterial = body.GetRenderMaterial();
        if (renderMaterial != null)
        {
            mesh["renderMaterial"] = renderMaterial;
        }

        return mesh;
    }
}
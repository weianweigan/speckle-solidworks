using Objects.Geometry;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Models;

namespace Objects.Converter.SolidWorks.Converters;

internal class Body2MeshConverter : ISwToSpeckle<IBody2>
{
    public Base Convert(IBody2 @object)
    {
        Mesh mesh = new Mesh();

        // TODO: Implement this

        return mesh;
    }
}
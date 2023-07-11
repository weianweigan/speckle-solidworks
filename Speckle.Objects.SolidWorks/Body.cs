using Objects.Geometry;
using Speckle.Core.Models;

namespace Objects.BuiltElements.SolidWorks;

public class Body : 
    Base, 
    IDisplayValue<Base>, 
    IHasVolume, 
    IHasArea,
    IHasBoundingBox
{
    /// <summary>
    /// Body name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Mesh or Brep for visualization.
    /// </summary>
    /// <remarks>
    /// <see cref="Mesh"/> or <see cref="Brep"/>
    /// </remarks>
    [DetachProperty]
    public Base displayValue { get; set; }

    /// <summary>
    /// BoundingBox
    /// </summary>
    public Box bbox { get; }

    /// <summary>
    /// volume of the SolidWorks body.
    /// </summary>
    public double volume { get; set; }

    /// <summary>
    /// Body type of the SolidWorks body.
    /// </summary>
    /// <remarks>
    /// Solid or Sheet.
    /// </remarks>
    public string BodyType { get; set; }

    /// <summary>
    /// Area of the SolidWorks body.
    /// </summary>
    /// <remarks>
    /// Is the sum of the areas of all faces.
    /// </remarks>
    public double area { get; set; }
}

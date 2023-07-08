using Objects.Geometry;
using Speckle.Core.Models;

namespace Objects.BuiltElements.SolidWorks;

public class SolidWorksBody : 
    Base, 
    IDisplayValue<Mesh>, 
    IHasVolume, 
    IHasBoundingBox
{
    public Mesh displayValue { get; set; }

    public Box bbox { get; }

    public double volume { get; set; }
}

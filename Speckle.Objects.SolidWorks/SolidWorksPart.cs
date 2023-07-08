using Speckle.Core.Models;
using System.Collections.Generic;

namespace Objects.BuiltElements.SolidWorks;

public sealed class SolidWorksPart : Base
{
    public List<CustomProperty> CustomProperties { get; set; } = new();

    public List<Equation> Equations { get; set; } = new();

    public List<Feature> Features { get; set; } = new();
}

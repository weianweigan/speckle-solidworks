using Objects.Geometry;
using Speckle.Core.Models;
using System.Collections.Generic;

namespace Objects.BuiltElements.SolidWorks;

public enum SketchAttachType
{
    Face,
    Plane
}

public class Sketch: Base
{
    public string Name { get; set; }

    /// <summary>
    /// The entity name which the sketch is attached to.
    /// </summary>
    public string Attachment { get; set; }

    public SketchAttachType SketchAttachType { get; set; } = SketchAttachType.Plane;

    public List<ICurve> Curves { get; set; } = new();

    public List<Point> Points { get; set; } = new();
}
using System.Collections.Generic;
using Objects.Geometry;
using Speckle.Core.Models;

namespace Objects.BuiltElements.SolidWorks;

public sealed class Component : Base, IDisplayValue<List<Base>>
{
    /// <summary>
    /// Component name
    /// </summary>
    /// <remarks>
    /// IComponent2.Name2
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    public int ComponentId { get; set; }

    public bool IsLightWeight { get; set; }

    public bool IsVirtual { get; set; }

    public bool IsSuppressed { get; set; }

    public string Configuration { get; set; }

    /// <summary>
    /// <see cref="Mesh"/> Or <see cref="Brep"/> for visualization.
    /// </summary>
    [DetachProperty]
    public List<Base> displayValue { get; set; }

    [DetachProperty]
    public List<Base> children { get; set; }

    public bool IsPatternInstance { get; set; }
}
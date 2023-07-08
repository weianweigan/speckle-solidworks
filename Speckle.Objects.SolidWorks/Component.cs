using System.Collections.Generic;
using Objects.Geometry;
using Speckle.Core.Models;

namespace Objects.BuiltElements.SolidWorks;

public sealed class Component : Base, IDisplayValue<List<Mesh>>
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

    public bool IsSuppressed { get; set; }

    public bool IsInstance { get; set; }

    public string Configuration { get; set; }

    public List<Base> CustomProperties { get; set; }

#pragma warning disable CS8618
    [DetachProperty]
    public List<Mesh> displayValue { get; set; }

    [DetachProperty]
    public List<Base> children { get; set; }
#pragma warning restore CS8618
}
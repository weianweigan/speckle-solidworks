using DesktopUI2.Models.Filters;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Speckle.ConnectorSolidWorks.Selection;

[AttributeUsage(AttributeTargets.Field)]
public class FilterAttribute : Attribute
{
    /// <summary>
    /// Material design icon.
    /// </summary>
    /// <remarks>
    /// <see href="https://pictogrammers.com/library/mdi/"/>
    /// </remarks>
    public string? Icon { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }


}

public enum SolidWorksFilterType
{
    [Filter(Icon = "cube-unfolded")]
    Feature,
    [Filter(Icon = "playlist-check")]
    CustomProperty,
    [Filter(Icon = "not-equal-variant")]
    Equation,
    [Filter(Icon = "cube-scan")]
    AssemblyComponent,
    [Filter(Icon = "cube-outline")]
    PartComponent,

    [Filter(Icon = "vector-line")]
    Dimension,
    [Filter(Icon = "vector-line")]
    Sketch,

    [Filter(Icon = "chart-line-variant")]
    SketchSegment,
    [Filter(Icon = "vector-point-plus")]
    SketchPoint,
}

/// <summary>
/// SolidWorks selection filter.
/// </summary>
public sealed class SolidWorksSelectionFilter : ISelectionFilter
{
#pragma warning disable CS8618
    public SolidWorksSelectionFilter() { }
#pragma warning restore CS8618

    public SolidWorksSelectionFilter(SolidWorksFilterType filterType, List<SwSeleTypeObjectPair> values) 
    {
        var filter = typeof(SolidWorksFilterType)
            .GetField(filterType.ToString())
            ?.GetCustomAttribute<FilterAttribute>();

        var name = filterType.ToString();
        Name = name;
        Icon = filter?.Icon ?? "";
        Type = name;
        Slug = name.ToLower();
        Summary = filter?.Description ?? name;
        Description = filter?.Description ?? name;
        Values = values;
        Selection = values.Select(p => p.Name).ToList();
    }

    /// <summary>
    /// User friendly name displayed in the UI.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Used as the discriminator for deserialization.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// MaterialDesignIcon use the demo app from the MaterialDesignInXamlToolkit to get the correct name
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Internal filter name
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Should return a succinct summary of the filter: what does it contain inside?
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Should contain a generic description of the filter and how it works.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Holds the values that the user selected from the filter. Not the actual objects.
    /// </summary>
    public List<string> Selection { get; set; }

    /// <summary>
    /// Values that prepare for sending.
    /// </summary>
    [JsonIgnore]
    public List<SwSeleTypeObjectPair > Values { get; }

    /// <summary>
    /// View associated to this filter type
    /// </summary>
    [JsonIgnore]
    public Type? ViewType { get; set; }

    public static List<ISelectionFilter> CreateAll()
    {
        return Enum.GetValues(typeof(SolidWorksFilterType))
            .OfType<SolidWorksFilterType>()
            .Select(f => (ISelectionFilter)new SolidWorksSelectionFilter(f, new()))
            .ToList();
    }
}

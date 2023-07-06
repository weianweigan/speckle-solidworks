using DesktopUI2.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Speckle.ConnectorSolidWorks.Selection;

public enum SolidWorksFilterType
{
    Feature, 
    CustomProperty, 
    Equation, 
    AssemblyComponent,
    PartComponent,

    Dimension,
    Sketch,

    SketchSegment,
    SketchPoint,
}

/// <summary>
/// SolidWorks selection filter.
/// </summary>
public sealed class SolidWorksSelectionFilter : ISelectionFilter
{
    public SolidWorksSelectionFilter(SolidWorksFilterType filterType):
        this(filterType.ToString(), filterType.ToString(), null, null, null, null, null, null)
    {
        
    }

    public SolidWorksSelectionFilter(
        string name, 
        string type, 
        string? icon, 
        string? slug, 
        string? summary, 
        string? description, 
        List<string>? selection, 
        Type? viewType)
    {
        Name = name;
        Type = type;
        Icon = icon;
        Slug = slug;
        Summary = summary;
        Description = description;
        Selection = selection;
        ViewType = viewType;
    }

    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get ; set ; }

    public string Type { get; set; }

    public string? Icon { get ; set ; }

    public string? Slug { get ; set ; }

    public string? Summary { get ; set ; }

    public string? Description { get ; set ; }

    public List<string>? Selection { get ; set ; }

    public Type? ViewType { get; set; }

    public static List<ISelectionFilter> CreateAll()
    {
        return Enum.GetValues(typeof(SolidWorksFilterType))
            .OfType<SolidWorksFilterType>()
            .Select(f => (ISelectionFilter)new SolidWorksSelectionFilter(f))
            .ToList();
    }
}

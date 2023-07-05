using DesktopUI2.Models.Filters;
using System;
using System.Collections.Generic;

namespace Speckle.ConnectorSolidWorks.Selection;

public enum SolidWorksFilterType
{
    Feature, 
    CustomProperty, 
    Equation, 
    AssemblyComponent,
    PartComponent,
    Sketch,
    Dimension,

}

/// <summary>
/// SolidWorks selection filter.
/// </summary>
public sealed class SolidWorksSelectionFilter : ISelectionFilter
{
    /// <summary>
    /// Name.
    /// </summary>
    public string Name { get ; set ; }

    public string Type { get; set; }

    public string Icon { get ; set ; }

    public string Slug { get ; set ; }

    public string Summary { get ; set ; }

    public string Description { get ; set ; }

    public List<string> Selection { get ; set ; }

    public Type ViewType { get; set; }

    public static IList<SolidWorksSelectionFilter> Create()
    {
        return new[]
        {
            new SolidWorksSelectionFilter
            {
                Name = "Part",
                Type = "Part",
                Icon = "Part",
                Slug = "part",
                Summary = "Select a part",
                Description = "Select a part",
                Selection = new List<string> { "Part" },
                ViewType = typeof(SelectionView)
            },
        }
    }
}

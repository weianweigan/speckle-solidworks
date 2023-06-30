using DesktopUI2.Models.Filters;
using System;
using System.Collections.Generic;

namespace Speckle.ConnectorSolidWorks.Selection;

public class SolidWorksSelectionFilter : ISelectionFilter
{
    public string Name { get ; set ; }

    public string Type { get; set; }

    public string Icon { get ; set ; }

    public string Slug { get ; set ; }

    public string Summary { get ; set ; }

    public string Description { get ; set ; }

    public List<string> Selection { get ; set ; }

    public Type ViewType { get; set; }
}

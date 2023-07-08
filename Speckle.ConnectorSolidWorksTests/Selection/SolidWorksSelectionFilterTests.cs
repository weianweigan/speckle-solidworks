using Xunit;
using System.Collections.Generic;
using DesktopUI2.Models.Filters;

namespace Speckle.ConnectorSolidWorks.Selection.Tests;

public class SolidWorksSelectionFilterTests
{
    [Fact()]
    public void SolidWorksSelectionFilterTest()
    {
        List<ISelectionFilter> filters = SolidWorksSelectionFilter.CreateAll();

        Assert.NotNull(filters);
        Assert.All(filters, f => Assert.NotNull(f.Icon));
    }
}
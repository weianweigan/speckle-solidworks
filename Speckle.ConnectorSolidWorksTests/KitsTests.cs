using Speckle.Core.Kits;
using Xunit;

namespace Speckle.ConnectorSolidWorksTests;

public class KitTests
{
    /// <summary>
    /// Test if the default kit is loaded correctly for solidworks converter.
    /// </summary>
    [Fact]
    public void KitLoadConverter()
    {
        ISpeckleConverter converter = KitManager.GetDefaultKit().LoadConverter("SolidWorks");

        Assert.NotNull(converter);
        Assert.True(converter.GetType().Name == "Objects.Converter.SolidWorks");
    }
}

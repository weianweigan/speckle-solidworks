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
        var type = converter.GetType();
        Assert.True(type.Name == "ConverterSolidWorks");
    }
}

using Moq;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Speckle.ConnectorSolidWorks.Selection.Tests;

public class SolidWorksFilterTypeUtilsTests
{
    [Fact]
    public void ByDocTest()
    {
        // Mock
        var mockDoc = new Mock<IModelDoc2>();
        mockDoc.Setup(p => p.GetType())
            .Returns((int)swDocumentTypes_e.swDocPART);
        mockDoc.Setup(p => p.SketchManager.ActiveSketch)
            .Returns<Sketch>(null);

        // Act
        List<SolidWorksFilterType> filters = SolidWorksFilterTypeUtils
            .ByDoc(mockDoc.Object)
            .ToList();

        // Assert
        Assert.NotNull(filters);
        Assert.True(filters.Count > 1);
    }

    [Fact]
    public void ByNotSupportDoc()
    {
        // Mock
        var mockDoc = new Mock<IModelDoc2>();
        mockDoc.Setup(p => p.GetType())
            .Returns((int)swDocumentTypes_e.swDocNONE);
        mockDoc.Setup(p => p.SketchManager.ActiveSketch)
            .Returns<Sketch>(null);

        // Act
        try
        {
            List<SolidWorksFilterType> filters = SolidWorksFilterTypeUtils
                .ByDoc(mockDoc.Object)
                .ToList();

            Assert.Fail("Error documentType passed!");
        }
        catch (System.NotSupportedException)
        {
            Assert.True(true);
        }
    }
}
using Xunit;
using SolidWorks.Interop.swconst;
using System.Text.Json;

namespace Speckle.ConnectorSolidWorks.Selection.Tests;

public class SwSeleTypeObjectPairSerialize
{
    [Fact()]
    public void SwSeleTypeObjectPairTest()
    {
        SwSeleTypeObjectPair s = new SwSeleTypeObjectPair(
            1, 
            swSelectType_e.swSelBODYFEATURES, 
            1, 
            new object(), 
            null, 
            "1");

        Assert.NotNull(s);

        //Serialize
        string text = JsonSerializer.Serialize(s);
        Assert.True(!string.IsNullOrWhiteSpace(text));
        //DeSerialize
        var newS = JsonSerializer.Deserialize<SwSeleTypeObjectPair>(text);
        Assert.NotNull(newS);

        Assert.Equal(s.Name, newS.Name);
        Assert.Equal(s.PID, newS.PID);
    }
}
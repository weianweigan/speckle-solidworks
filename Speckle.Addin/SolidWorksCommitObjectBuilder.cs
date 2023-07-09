using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Core.Models;

namespace Speckle.ConnectorSolidWorks;

public class SolidWorksCommitObjectBuilder : 
    CommitObjectBuilder<SwSeleTypeObjectPair>
{
    public override void IncludeObject(
        Base conversionResult, 
        SwSeleTypeObjectPair nativeElement)
    {
        SetRelationship(conversionResult);
    }
}

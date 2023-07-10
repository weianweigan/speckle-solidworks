using Objects.BuiltElements.SolidWorks;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Core.Models;
using System.Collections.Generic;
using System.Linq;

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

    public override void BuildCommitObject(Base rootCommitObject)
    {
        base.BuildCommitObject(rootCommitObject);

        var bodies = (IList<Base>)(rootCommitObject["bodies"] ??= new List<Base>());

        //Finally, apply collection -> host relationships
        foreach (var body in converted.Values.OfType<Body>())
        {
            bodies.Add(body);
        }
    }
}
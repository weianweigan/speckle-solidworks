using Objects.BuiltElements.SolidWorks;
using Objects.Organization;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.Core.Models;
using System.Collections.Generic;

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
        var components = (IList<Base>)(rootCommitObject["components"] ??= new List<Base>());

        foreach (var value in converted.Values)
        {
            if (value == null)
            {
                continue;
            }

            if (value is Body)
            {
                bodies.Add(value);
            }
            else if(value is DataTable dataTable)
            {
                rootCommitObject[dataTable.applicationId] = dataTable;
            }else if (value is Component)
            {
                components.Add(value);
            }
        }
    }
}
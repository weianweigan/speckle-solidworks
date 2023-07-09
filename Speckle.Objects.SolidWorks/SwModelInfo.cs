using Objects.Organization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Speckle.Objects.SolidWorks;

public enum DocumentType
{
    Part,
    Assembly,
    Drawing,
    UnKnown
}

public class SwModelInfo : ModelInfo
{
    public string DocumentType { get; set; }
}

using Speckle.ConnectorSolidWorks.Properties;
using System.ComponentModel;
using Xarial.XCad.Base.Attributes;

namespace Speckle.ConnectorSolidWorks;

[Title("Speckle")]
[Icon(typeof(Resources), nameof(Properties.Resources.logo))]
public enum SwCommands
{
    [Title("Speckle")]
    [Description("SolidWorks Connector")]
    [Icon(typeof(Resources), nameof(Properties.Resources.logo))]
    SolidWorksConnector,
}

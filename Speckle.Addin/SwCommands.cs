using Speckle.ConnectorSolidWorks.Properties;
using System.ComponentModel;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.UI.Commands.Attributes;

namespace Speckle.ConnectorSolidWorks;

[Title("Speckle")]
[Icon(typeof(Resources), nameof(Resources.logo))]
public enum SwCommands
{
    [Title("Speckle")]
    [Description("SolidWorks Connector")]
    [Icon(typeof(Resources), nameof(Resources.logo))]
    SolidWorksConnector,

    [Title(nameof(Scheduler))]
    [Description("Scheduler for SolidWorks Connector")]
    [Icon(typeof(Resources), nameof(Resources.scheduler))]
    [CommandSpacer]
    Scheduler,

    [Title("Community Forum")]
    [Description("Scheduler for SolidWorks Connector")]
    [Icon(typeof(Resources), nameof(Resources.forum))]
    CommunityForum,

    [Title(nameof(Tutorials))]
    [Description("Tutorials for SolidWorks Connector")]
    [Icon(typeof(Resources), nameof(Resources.tutorials))]
    Tutorials,

    [Title(nameof(Docs))]
    [Description("Documentation for SolidWorks Connector")]
    [Icon(typeof(Resources), nameof(Resources.docs))]
    Docs,
}

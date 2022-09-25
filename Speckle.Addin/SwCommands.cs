using Speckle.ConnectorSolidWorks.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.Base.Attributes;

namespace Speckle.ConnectorSolidWorks
{
    [Title("Speckle")]
    [Icon(typeof(Resources), nameof(Properties.Resources.logo))]
    public enum SwCommands
    {
        [Title(nameof(SolidWorksConnector))]
        [Description("SolidWorks Connector")]
        [Icon(typeof(Resources), nameof(Properties.Resources.logo))]
        SolidWorksConnector,
    }
}

using DesktopUI2.Models;
using DesktopUI2.ViewModels;
using System;
using System.Threading.Tasks;

namespace Speckle.ConnectorSolidWorks.UI
{
    public partial class ConnectorBindingsSolidWorks
    {
        public override Task<StreamState> PreviewReceive(
            StreamState state, 
            ProgressViewModel progress)
        {
            throw new NotImplementedException();
        }

        public override Task<StreamState> ReceiveStream(
            StreamState state, 
            ProgressViewModel progress)
        {
            throw new NotImplementedException();
        }
    }
}

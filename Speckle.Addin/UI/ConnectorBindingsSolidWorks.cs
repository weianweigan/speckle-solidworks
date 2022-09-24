using DesktopUI2;
using DesktopUI2.Models;
using DesktopUI2.Models.Filters;
using DesktopUI2.Models.Settings;
using DesktopUI2.ViewModels;
using Speckle.Core.Kits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speckle.Addin.UI
{
    public class ConnectorBindingsSolidWorks : ConnectorBindings
    {
        public override bool CanPreviewSend => throw new NotImplementedException();

        public override bool CanPreviewReceive => throw new NotImplementedException();

        public override string GetActiveViewName()
        {
            throw new NotImplementedException();
        }

        public override List<MenuItem> GetCustomStreamMenuItems()
        {
            throw new NotImplementedException();
        }

        public override string GetDocumentId()
        {
            throw new NotImplementedException();
        }

        public override string GetDocumentLocation()
        {
            throw new NotImplementedException();
        }

        public override string GetFileName()
        {
            throw new NotImplementedException();
        }

        public override string GetHostAppName()
        {
            throw new NotImplementedException();
        }

        public override string GetHostAppNameVersion()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetObjectsInView()
        {
            throw new NotImplementedException();
        }

        public override List<ReceiveMode> GetReceiveModes()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetSelectedObjects()
        {
            throw new NotImplementedException();
        }

        public override List<ISelectionFilter> GetSelectionFilters()
        {
            throw new NotImplementedException();
        }

        public override List<ISetting> GetSettings()
        {
            throw new NotImplementedException();
        }

        public override List<StreamState> GetStreamsInFile()
        {
            throw new NotImplementedException();
        }

        public override Task<Dictionary<string, List<MappingViewModel.MappingValue>>> ImportFamilyCommand(Dictionary<string, List<MappingViewModel.MappingValue>> Mapping)
        {
            throw new NotImplementedException();
        }

        public override Task<StreamState> PreviewReceive(StreamState state, ProgressViewModel progress)
        {
            throw new NotImplementedException();
        }

        public override void PreviewSend(StreamState state, ProgressViewModel progress)
        {
            throw new NotImplementedException();
        }

        public override Task<StreamState> ReceiveStream(StreamState state, ProgressViewModel progress)
        {
            throw new NotImplementedException();
        }

        public override void ResetDocument()
        {
            throw new NotImplementedException();
        }

        public override void SelectClientObjects(List<string> objs, bool deselect = false)
        {
            throw new NotImplementedException();
        }

        public override Task<string> SendStream(StreamState state, ProgressViewModel progress)
        {
            throw new NotImplementedException();
        }

        public override void WriteStreamsToFile(List<StreamState> streams)
        {
            throw new NotImplementedException();
        }
    }
}

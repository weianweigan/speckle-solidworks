using DesktopUI2;
using DesktopUI2.Models;
using DesktopUI2.Models.Filters;
using DesktopUI2.Models.Settings;
using DesktopUI2.ViewModels;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Kits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.XCad.SolidWorks;

namespace Speckle.Addin.UI
{
    public partial class ConnectorBindingsSolidWorks : ConnectorBindings
    {
        public ConnectorBindingsSolidWorks(ISwApplication swApp)
        {
            SwApp = swApp;
            App = SwApp.Sw;
        }

        public override bool CanPreviewSend => true;

        public override bool CanPreviewReceive => true;

        public ISldWorks App { get; }
        public ISwApplication SwApp { get; }

        public override string GetActiveViewName()
        {
            return App.IActiveDoc2?.IActiveView?.ToString();
        }

        public override List<MenuItem> GetCustomStreamMenuItems()
        {
            return new List<MenuItem>();
        }

        public override string GetDocumentId()
        {
            return App.IActiveDoc2?.GetTitle();
        }

        public override string GetDocumentLocation()
        {
            return App.IActiveDoc2?.GetPathName();
        }

        public override string GetFileName()
        {
            return App.IActiveDoc2?.GetTitle();
        }

        public override string GetHostAppName()
        {
            return "SolidWorks";
        }

        public override string GetHostAppNameVersion()
        {
            return SwApp.Version.DisplayName;
        }

        public override List<string> GetObjectsInView()
        {
            return new List<string>();
        }

        public override List<ReceiveMode> GetReceiveModes()
        {
            return new List<ReceiveMode>();
        }

        public override List<string> GetSelectedObjects()
        {
            return new List<string>();
        }

        public override List<ISelectionFilter> GetSelectionFilters()
        {
            return new List<ISelectionFilter>();
        }

        public override List<ISetting> GetSettings()
        {
            return new List<ISetting>();
        }

        public override List<StreamState> GetStreamsInFile()
        {
            return new List<StreamState>();
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

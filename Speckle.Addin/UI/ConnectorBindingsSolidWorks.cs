using DesktopUI2;
using DesktopUI2.Models;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Kits;
using System.Collections.Generic;
using System.Linq;
using Xarial.XCad.SolidWorks;

namespace Speckle.ConnectorSolidWorks.UI;

public partial class ConnectorBindingsSolidWorks : ConnectorBindings
{
    public const string APP_NAME = "SolidWorks";

    public ConnectorBindingsSolidWorks(ISwApplication swApp)
    {
        SwApp = swApp;
        App = SwApp.Sw;
    }

    public override bool CanPreviewSend => true;

    public override bool CanPreviewReceive => true;

    public ISldWorks App { get; }

    public ISwApplication SwApp { get; }

    public override string? GetActiveViewName()
    {
        return App.IActiveDoc2?.IActiveView?.ToString();
    }

    public override List<MenuItem> GetCustomStreamMenuItems()
    {
        return new List<MenuItem>();
    }

    public override string? GetDocumentId()
    {
        return App.IActiveDoc2?.GetTitle();
    }

    public override string? GetDocumentLocation()
    {
        return App.IActiveDoc2?.GetPathName();
    }

    public override string? GetFileName()
    {
        return App.IActiveDoc2?.GetTitle();
    }

    public override string? GetHostAppName()
    {
        return APP_NAME;
    }

    public override string GetHostAppNameVersion()
    {
        return SwApp.Version.DisplayName;
    }

    public override List<string>? GetObjectsInView()
    {
        //Return All Features
        var doc = App.IActiveDoc2;
        return doc
            ?.GetAllFeatures()
            .Select(p => p.Name)
            .ToList();
    }

    public override List<ReceiveMode> GetReceiveModes()
    {
        return new List<ReceiveMode>();
    }

    public override List<StreamState> GetStreamsInFile()
    {
        return new List<StreamState>();
    }

    public override void ResetDocument()
    {
        //Fire when select a stream
        var doc = App.IActiveDoc2;
        doc?.ClearSelection();
    }

    public override void WriteStreamsToFile(List<StreamState> streams)
    {

    }
}

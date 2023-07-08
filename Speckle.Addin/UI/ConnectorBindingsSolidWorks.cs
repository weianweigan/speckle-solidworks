using DesktopUI2;
using DesktopUI2.Models;
using SolidWorks.Interop.sldworks;
using Speckle.ConnectorSolidWorks.Extensions;
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

    #region Properties
    public override bool CanPreviewSend => true;

    public override bool CanPreviewReceive => true;

    public ISldWorks App { get; }

    public ISwApplication SwApp { get; }
    #endregion

    #region Methods 
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
            .Union(new[] {"CustomProperties", "Equations"})
            .ToList();
    }

    public override List<ReceiveMode> GetReceiveModes()
    {
        return new List<ReceiveMode>() { ReceiveMode.Create};
    }

    public override List<StreamState> GetStreamsInFile()
    {
        var doc = App.IActiveDoc2;

        if (doc == null)
        {
            return new();
        }

        return SolidWorksStreamStateManager.ReadState(doc);
    }

    /// <summary>
    /// Clears the document state of selections and previews
    /// </summary>
    public override void ResetDocument()
    {
        // Clear selections
        var doc = App.IActiveDoc2;
        doc?.ClearSelection();

        // TODO: Clear previews
    }

    public override void WriteStreamsToFile(List<StreamState> streams)
    {
        var doc = App.IActiveDoc2;

        if (doc == null)
        {
            return;
        }

        SolidWorksStreamStateManager.WriteStreamStateList(doc, streams, App);
    }
    #endregion
}

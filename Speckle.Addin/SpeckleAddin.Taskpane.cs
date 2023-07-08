using Avalonia;
using DesktopUI2.ViewModels;
using SolidWorks.Interop.sldworks;
using Speckle.ConnectorSolidWorks.UI;
using Speckle.Core.Logging;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
using Xarial.XCad.SolidWorks;

namespace Speckle.ConnectorSolidWorks;

public partial class SpeckleAddin
{
    #region Field
    private TaskpaneView? _taskpaneView;
    private ElementHost? _host;
    private SpeckleTaskPane? _pane;
    #endregion

    /// <summary>
    /// Create taskpane and show it.
    /// </summary>
    /// <remarks>
    /// <see cref="ISldWorks.CreateTaskpaneView3(object, string)"/> with 
    /// <see href="https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.ISldWorks~CreateTaskpaneView2.html"/>
    /// <see href="https://help.solidworks.com/2022/english/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.ISldWorks~CreateTaskpaneView3.html"/>
    /// </remarks>
    private void CreateTaskpane()
    {
        try
        {
            if (_pane == null)
            {
                BuildAvaloniaApp().Start(UserControlMain, null);
            }

            if (Application.IsVersionNewerOrEqual(Xarial.XCad.SolidWorks.Enums.SwVersion_e.Sw2017))
            {
                string[] iconGroup = new[] { 
                    "logo20x20.png", 
                    "logo32x32.png", 
                    "logo40x40.png", 
                    "logo64x64.png", 
                    "logo.png" }
                    .Select(n => Path.Combine(AddinDir, "Assets", "Images", n))
                    .ToArray();
                _taskpaneView = Application.Sw.CreateTaskpaneView3(iconGroup, "Speckle");
            }
            else
            {
                string icon = Path.Combine(AddinDir, "Assets", "Images", "logo16x18.png");
                _taskpaneView = Application.Sw.CreateTaskpaneView2(icon, "Speckle");
            }

            _host = new()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Child = _pane,
                TabStop = false,
            };

            if (!_taskpaneView.DisplayWindowFromHandlex64(_host.Handle.ToInt64()))
            {
                SpeckleLog.Logger.Warning("Failed to display Speckle Taskpane.");
            }
            _taskpaneView.ShowView();
        }
        catch (InvalidComObjectException) { }
        catch (Exception ex)
        {
            Application.ShowMessageBox(ex.Message);
        }
    }

    private void UserControlMain(Application app, string[] args)
    {
        var viewModel = new MainViewModel(new ConnectorBindingsSolidWorks(Application));
        _pane = new SpeckleTaskPane()
        {
            DataContext = viewModel
        };
        _pane.Init();
    }
}

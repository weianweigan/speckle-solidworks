using Avalonia;
using DesktopUI2.ViewModels;
using SolidWorks.Interop.sldworks;
using Speckle.ConnectorSolidWorks.UI;
using Speckle.Core.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;

namespace Speckle.ConnectorSolidWorks;

public partial class SpeckleAddin
{
    #region Field
    private TaskpaneView? _taskpaneView;
    private ElementHost? _host;
    private SpeckleTaskPane? _pane;
    #endregion

    private void CreateTaskpane()
    {
        try
        {
            if (_pane == null)
            {
                BuildAvaloniaApp().Start(UserControlMain, null);
            }

            string icon = Path.Combine(AddinDir, "Assets", "Images", "logo.png");
            _taskpaneView = Application.Sw.CreateTaskpaneView2(icon, "Speckle");

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
    }
}

using Avalonia;
using Avalonia.Win32.Embedding;
using DesktopUI2.ViewModels;
using DesktopUI2.Views;
using SolidWorks.Interop.sldworks;
using Speckle.ConnectorSolidWorks.UI;
using Speckle.Core.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Speckle.ConnectorSolidWorks;

public partial class SpeckleAddin
{
    #region Field
    private TaskpaneView _taskpaneView;
    private WinFormsAvaloniaControlHost _host;
    #endregion

    public static MainUserControl MainUserControl { get; private set; }

    private void CreateTaskpane()
    {
        try
        {
            if (MainUserControl == null)
            {
                BuildAvaloniaApp().Start(UserControlMain, null);
            }

            string icon = Path.Combine(AddinDir, "Assets", "Images", "logo.png");
            _taskpaneView = Application.Sw.CreateTaskpaneView2(icon, "Speckle");

            _host = new()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Content = MainUserControl,
                TabStop = false,
            };

            if (!_taskpaneView.DisplayWindowFromHandlex64(_host.Handle.ToInt64()))
            {
                SpeckleLog.Logger.Warning("Failed to display Speckle taskpane.");
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
        MainUserControl = new MainUserControl
        {
            DataContext = viewModel,
        };
    }
}

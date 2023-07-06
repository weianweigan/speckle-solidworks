using Avalonia;
using Avalonia.ReactiveUI;
using DesktopUI2.ViewModels;
using DesktopUI2.Views;
using Speckle.ConnectorSolidWorks.Properties;
using Speckle.ConnectorSolidWorks.UI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands;

namespace Speckle.ConnectorSolidWorks;

[ComVisible(true)]
[Title("Speckle")]
[Description("The world runs on 3D: Speckle enables you to deliver better designs, together.")]
[Icon(typeof(Resources), nameof(Properties.Resources.logo))]
public partial class SpeckleAddin : SwAddInEx
{
    const int GWL_HWNDPARENT = -8;
    private bool _useTaskpane = true;

    #region Properties
    public string AddinDir { get; } = Path.GetDirectoryName(typeof(SpeckleAddin).Assembly.Location);

    public static MainWindow? MainWindow { get; private set; }
    #endregion

    #region Win32 API
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr value);
    #endregion

    #region Public Methods
    public override void OnConnect()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        CommandManager.AddCommandGroup<SwCommands>().CommandClick += Addin_CommandClick;

        if (_useTaskpane)
            CreateTaskpane();
    }

    public override void OnDisconnect()
    {
        _taskpaneView?.DeleteView();
        _taskpaneView = null;
        _host?.Dispose();
        _host = null;
    }
    #endregion

    #region Private Methods
    private void Addin_CommandClick(SwCommands spec)
    {
        switch (spec)
        {
            case SwCommands.SolidWorksConnector:
                {
                    if (_useTaskpane)
                    {
                        _taskpaneView?.ShowView();
                    }
                    else
                    {
                        if (MainWindow == null)
                        {
                            BuildAvaloniaApp().Start(AppMain, null);
                        }
                        MainWindow?.Show();
                    }
                }
                break;
            default:
                break;
        }
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<DesktopUI2.App>()
        .UsePlatformDetect()
        .With(new SkiaOptions { MaxGpuResourceSizeBytes = 8096000 })
        .With(new Win32PlatformOptions { AllowEglInitialization = true, EnableMultitouch = false })
        .LogToTrace()
        .UseReactiveUI();

    private void AppMain(Application app ,string[] args)
    {
        var viewModel = new MainViewModel(new ConnectorBindingsSolidWorks(Application));
        MainWindow = new MainWindow
        {
            DataContext = viewModel,
            Topmost = true
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            IntPtr parentHwnd = Application.WindowHandle;
            IntPtr hwnd = MainWindow.PlatformImpl.Handle.Handle;
            SetWindowLongPtr(hwnd, GWL_HWNDPARENT, parentHwnd);
        }
    }

    private Assembly? CurrentDomain_AssemblyResolve(
        object sender, 
        ResolveEventArgs args)
    {
        var assemblyPath = string.Empty;
        var assemblyName = new AssemblyName(args.Name).Name + ".dll";

        try
        {
            assemblyPath = Path.Combine(AddinDir, assemblyName);

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            else
            {
                Debug.Print(assemblyPath);
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(
                string.Format(
                    "The location of the assembly, {0} could not be resolved for loading.",
                assemblyPath),
                ex);
        }
    }
    #endregion
}
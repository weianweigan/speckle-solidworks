using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using DesktopUI2.Views;
using Speckle.Addin.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.UI.Commands;
using DesktopUI2.ViewModels;

namespace Speckle.Addin
{
    [ComVisible(true)]
    [Title("Speckle")]
    [Description("The world runs on 3D: Speckle enables you to deliver better designs, together.")]
    [Icon(typeof(Resources), nameof(Properties.Resources.logo))]
    public class Addin : SwAddInEx
    {
        const int GWL_HWNDPARENT = -8;

        #region Properties
        public string AddinDir { get; } = Path.GetDirectoryName(typeof(Addin).Assembly.Location);

        public static Window MainWindow { get; private set; }
        #endregion

        #region Win32 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr value);
        #endregion

        #region Public Methods
        public override void OnConnect()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            CommandManager.AddCommandGroup<SwCommands>().CommandClick += Addin_CommandClick; ;
        }
        #endregion

        #region Private Methods
        private void Addin_CommandClick(SwCommands spec)
        {
            try
            {
                CreateOrFocusSpeckle();
            }
            catch (Exception ex)
            {
                Application.ShowMessageBox(ex.Message);
            }
        }

        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<DesktopUI2.App>()
              .UsePlatformDetect()
              .With(new SkiaOptions { MaxGpuResourceSizeBytes = 8096000 })
              .With(new Win32PlatformOptions { AllowEglInitialization = true, EnableMultitouch = false })
              .LogToTrace()
              .UseReactiveUI();

        public void CreateOrFocusSpeckle()
        {
            if (MainWindow == null)
            {
                BuildAvaloniaApp().Start(AppMain, null);
            }

            MainWindow.Show();
            MainWindow.Activate();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var parentHwnd = Application.WindowHandle;
                var hwnd = MainWindow.PlatformImpl.Handle.Handle;
                SetWindowLongPtr(hwnd, GWL_HWNDPARENT, parentHwnd);
            }
        }

        private void AppMain(Application app ,string[] args)
        {
            var viewModel = new MainViewModel();
            MainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            //Task.Run(() => app.Run(MainWindow));
            if (app == null)
            {
                MainWindow.Show();
            }
            else
            {
                app.Run(MainWindow);
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(
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
}
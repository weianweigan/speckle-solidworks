using DesktopUI2.Views;
using System;
using System.Windows.Controls;

namespace Speckle.ConnectorSolidWorks.UI;

/// <summary>
/// SpeckleTaskPane.xaml 的交互逻辑
/// </summary>
public partial class SpeckleTaskPane : Page
{
    private const UInt32 DLGC_WANTARROWS = 0x0001;
    private const UInt32 DLGC_HASSETSEL = 0x0008;
    private const UInt32 DLGC_WANTCHARS = 0x0080;
    private const UInt32 WM_GETDLGCODE = 0x0087;

    public SpeckleTaskPane()
    {
        InitializeComponent();
        AvaloniaHost.MessageHook += AvaloniaHost_MessageHook;
    }

    /// <summary>
    /// WPF was handling all the text input events and they where not being passed to the Avalonia control
    /// This ensures they are passed, see: https://github.com/AvaloniaUI/Avalonia/issues/8198#issuecomment-1168634451
    /// </summary>
    private IntPtr AvaloniaHost_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_GETDLGCODE) return IntPtr.Zero;
        handled = true;
        return new IntPtr(DLGC_WANTCHARS | DLGC_WANTARROWS | DLGC_HASSETSEL);
    }

    public void Init()
    {
        AvaloniaHost.Content = new MainUserControl();
    }
}

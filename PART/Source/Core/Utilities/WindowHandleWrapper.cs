using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Wraps a window
    /// Used by the installers to make the custom installers UI a child for the main installer (msi) UI
    /// </summary>
    public class WindowHandleWrapper : IWin32Window
    {
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public IntPtr Handle { get; private set; }

        public WindowHandleWrapper(IntPtr handle)
        {
            this.Handle = handle;
        }

        public WindowHandleWrapper(string installerWindowTitle)
            : this(GetWindowHandle(installerWindowTitle)) { }

        public static IntPtr GetWindowHandle(string parentWindowTitle)
        {
            var msiProcesses = Process.GetProcessesByName("msiexec");
            Process msiProcess = msiProcesses.FirstOrDefault((p) => p.MainWindowTitle.Equals(parentWindowTitle, StringComparison.OrdinalIgnoreCase) && p.MainWindowHandle != IntPtr.Zero);
            if (msiProcess != null)
            {
                return msiProcess.MainWindowHandle;
            }
            else
            {
                return FindWindow(null, parentWindowTitle);
            }
        }

    }
}

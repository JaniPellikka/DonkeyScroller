using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JaniPellikka.Windows.Forms.Internal
{
    /// <summary>
    /// Helper class for interop methods
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="hWnd">Pointer to a control Handle</param>
        /// <param name="wMsg">The message to send</param>
        /// <param name="wParam">wParam to send</param>
        /// <param name="lParam">lParam to send</param>
        /// <returns>Integer based on <paramref name="wMsg"/></returns>
        [DllImport("user32.dll")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Sets or unsets a controls ability to redraw itself
        /// </summary>
        public const int WM_SETREDRAW = 11;
        /// <summary>
        /// Left mouse button double click
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x0203;
        // ReSharper enable InconsistentNaming
    }
}
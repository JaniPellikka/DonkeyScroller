using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms.Internal
{
    /// <summary>
    ///     Internal utils
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        ///     Gets the Font Style for Char
        /// </summary>
        /// <param name="bold"></param>
        /// <param name="underline"></param>
        /// <param name="italic"></param>
        /// <param name="strikethrough"></param>
        /// <returns><see cref="FontStyle"/>  FontStyle based on the arguments passed to thos method</returns>
        public static FontStyle GetFontStyle(bool bold, bool underline, bool italic, bool strikethrough)
        {
            FontStyle fontStyle = FontStyle.Regular;


            if (bold)
                fontStyle |= FontStyle.Bold;
            if (underline)
                fontStyle |= FontStyle.Underline;
            if (italic)
                fontStyle |= FontStyle.Italic;
            if (strikethrough)
                fontStyle |= FontStyle.Strikeout;
            return fontStyle;
        }

        /// <summary>
        ///     Executes the specified delegate on the thread that owns the control's underlying window handle. If required, the delegate is Invoked on the controls thread.
        /// </summary>
        /// <param name="control">The control whose window handle the delegate should be invoked on.</param>
        /// <param name="method">A delegate method to be called in the control's thread context.</param>
        public static void InvokeIfRequired(Control control, Action method)
        {
            if (control.IsDisposed)
                return;
            try
            {
                if (control.InvokeRequired)
                    control.Invoke(method);
                else
                    method();
            }
            catch (InvalidAsynchronousStateException)
            {
                // HACK Do nothing??
            }
        }

        /// <summary>
        ///     Executes the specified delegate on the thread that owns the control's underlying window handle, returning a
        ///     value.
        /// </summary>
        /// <param name="control">The control whose window handle the delegate should be invoked on.</param>
        /// <param name="method">A delegate method to be called in the control's thread context and that returns a value.</param>
        /// <typeparam name="TResult">Result of the <paramref name="method"/></typeparam>
        /// <returns>The return value from the delegate being invoked.</returns>
        public static TResult InvokeIfRequired<TResult>(Control control, Func<TResult> method)
        {
            if (control.InvokeRequired)
                return (TResult)control.Invoke(method);
            return method();
        }

        /// <summary>
        ///     Checks whether or not a font is monospaced
        /// </summary>
        /// <param name="font">The font to check</param>
        /// <returns>True if the font is monospaced</returns>
        public static bool IsMonoSpaceFont(Font font)
        {
            return GetCharSize(font, 'M') == GetCharSize(font, '.');
        }

        /// <summary>
        ///     Gets the size of a char using specified Font
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="c">The char to get the size for</param>
        /// <returns>The size of the <paramref name="c"/> using <paramref name="font"/></returns>
        internal static SizeF GetCharSize(Font font, char c)
        {
            Size sz2 = TextRenderer.MeasureText("<" + c + ">", font);
            Size sz3 = TextRenderer.MeasureText("<>", font);

            return new SizeF(sz2.Width - sz3.Width + 1, /*sz2.Height*/font.Height);
        }

        /// <summary>
        ///     Disables window redrawing
        /// </summary>
        /// <param name="handle">Handle of the control</param>
        public static void LockWindowUpdate(IntPtr handle)
        {
            NativeMethods.SendMessage(handle, NativeMethods.WM_SETREDRAW, false, 0);
        }

        /// <summary>
        ///     Enables window redrawing
        /// </summary>
        /// <param name="handle">Handle of the control</param>
        public static void UnlockWindowUpdate(IntPtr handle)
        {
            NativeMethods.SendMessage(handle, NativeMethods.WM_SETREDRAW, true, 0);
        }

        /// <summary>
        ///     Checks if a mouse button is used by MouseEventArgs while taking in account that the user might have swapped the
        ///     button through controlpanel
        /// </summary>
        /// <param name="e">The event args</param>
        /// <param name="checkForButton">The button to check for</param>
        /// <returns>True if the <paramref name="checkForButton"/> is in <paramref name="e"/></returns>
        public static bool CheckMouseButton(MouseEventArgs e, MouseButtons checkForButton)
        {
            if (SystemInformation.MouseButtonsSwapped)
            {
                if (checkForButton == MouseButtons.Right)
                    return e.Button == MouseButtons.Left;
                if (checkForButton == MouseButtons.Left)
                    return e.Button == MouseButtons.Right;
            }
            return e.Button == checkForButton;
        }



    }
}
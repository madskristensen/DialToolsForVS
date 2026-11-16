
using System.ComponentModel.Composition;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

using Windows.UI.Input;

namespace DialControllerTools
{
    public abstract class BaseTextController : BaseController
    {
        private readonly IVsTextManager textManager;

#pragma warning disable IDE0044 // Add readonly modifier
        [Import]
        private IVsEditorAdaptersFactoryService editorAdapter;
#pragma warning restore IDE0044 // Add readonly modifier

        internal BaseTextController(RadialControllerMenuItem menuItem, IVsTextManager textManager) : base(menuItem)
        {
            this.textManager = textManager;
        }

        ///<summary>
        /// Gets the native view of the active text editor, or <see langword="null"/> when there is none.
        /// </summary>
        public IVsTextView GetCurrentNativeTextView()
        {
            // GetActiveView fails while no text editor is open, for example when only a preview is shown or nothing at all.
            // That is a missing view, which every caller handles, and not an error: a throw here would keep the Scroll item from
            // sending its keys and escape from CanHandleRotate of the Editor item, which the host does not guard.
            return ErrorHandler.Succeeded(textManager.GetActiveView(1, null, out IVsTextView activeView)) ? activeView : null;
        }

        ///<summary>
        /// Gets the TextView for the active document.
        /// </summary>
        public IWpfTextView GetCurrentTextView() => GetTextView(GetCurrentNativeTextView());

        public IWpfTextView GetTextView(IVsTextView nativeView)
        {
            if (nativeView == null)
                return null;

            return editorAdapter.GetWpfTextView(nativeView);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    public abstract class BaseTextController : BaseController
    {
        private readonly IDialControllerHost host;
        private readonly IVsTextManager textManager;

        internal BaseTextController(IDialControllerHost host, IVsTextManager textManager)
        {
            this.host = host;
            this.textManager = textManager;
        }

        public IVsTextView GetCurrentNativeTextView()
        {
            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));
            return activeView;
        }

        ///<summary>Gets the TextView for the active document.</summary>
        public IWpfTextView GetCurrentTextView() => GetTextView(GetCurrentNativeTextView());

        public IWpfTextView GetTextView(IVsTextView nativeView)
        {
            if (nativeView == null)
                return null;

            IVsEditorAdaptersFactoryService editorAdapter = host.CompositionService.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(nativeView);
        }
    }
}

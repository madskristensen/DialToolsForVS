using System.Windows;
using System.Windows.Media;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

using Windows.UI.Input;

namespace DialControllerTools
{
    internal class ZoomController : BaseTextController
    {
        private readonly Commands _commands;
        private readonly IVsMonitorSelection _monitorSelection;

        public override string Moniker => ZoomControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public ZoomController(RadialControllerMenuItem menuItem, DTE2 dte, IVsTextManager textManager, IVsMonitorSelection monitorSelection)
            : base(menuItem, textManager)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _commands = dte.Commands;
            _monitorSelection = monitorSelection;
        }

        public override bool OnClick()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DependencyObject webView = FindWebView();

            if (webView is not null)
            {
                WebViewZoom.Reset(webView);
                return true;
            }

            IWpfTextView view = GetCurrentTextView();

            if (view == null || view.ZoomLevel == 100)
                return false;

            view.ZoomLevel = 100;
            _commands.ExecuteCommand("View.ZoomOut");
            _commands.ExecuteCommand("View.ZoomIn");

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DependencyObject webView = FindWebView();

            if (webView is not null)
            {
                WebViewZoom.Zoom(webView, direction);
                return true;
            }

            if (direction == RotationDirection.Right)
            {
                return _commands.ExecuteCommand("View.ZoomIn");
            }
            else
            {
                return _commands.ExecuteCommand("View.ZoomOut");
            }
        }

        /// <summary>
        /// Finds the web view that the active window of Visual Studio shows, such as the standalone Markdown preview.
        /// <para>The commands of the text editor do nothing in such a window, so the web view is zoomed itself.</para>
        /// <para>The active window frame is asked, not the keyboard focus of WPF: the browser of a web view is a window of its own,
        /// so the keyboard focus can be in it while WPF reports another element or none.</para>
        /// </summary>
        /// <returns>The web view, or <see langword="null"/> when the active window shows none, and the text editor is zoomed.</returns>
        private DependencyObject FindWebView()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            int hr = _monitorSelection.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_WindowFrame, out object frameValue);

            if (ErrorHandler.Failed(hr) || frameValue is not IVsWindowFrame frame)
                return null;

            hr = frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object docView);

            if (ErrorHandler.Failed(hr))
                return null;

            // The text editor is a window pane as well, so it has to be told apart before the content of a window pane is searched.
            if (docView is IVsCodeWindow codeWindow)
                return FindWebViewNextToEditor(codeWindow);

            // A tool window, or a document without a text editor, as the standalone Markdown preview is.
            return docView is WindowPane { Content: Visual content } ? WebViewZoom.Find(content) : null;
        }

        /// <summary>
        /// Finds the web view of a preview that is shown next to the text of a code window, as the Markdown preview can be.
        /// <para>While the text has the focus the user means the text, so there is no web view then.</para>
        /// </summary>
        private DependencyObject FindWebViewNextToEditor(IVsCodeWindow codeWindow)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // A split editor has two views, and the text can be typed into either of them.
            codeWindow.GetPrimaryView(out IVsTextView primaryView);
            codeWindow.GetSecondaryView(out IVsTextView secondaryView);

            IWpfTextViewHost primaryHost = GetTextViewHost(primaryView);
            IWpfTextViewHost secondaryHost = GetTextViewHost(secondaryView);

            if (primaryHost?.TextView.HasAggregateFocus == true || secondaryHost?.TextView.HasAggregateFocus == true)
                return null;

            return WebViewZoom.Find(primaryHost?.HostControl) ?? WebViewZoom.Find(secondaryHost?.HostControl);
        }
    }
}

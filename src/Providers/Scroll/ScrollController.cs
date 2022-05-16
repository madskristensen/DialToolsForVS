using System.ComponentModel.Composition;
using System.Windows.Forms;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

using Windows.UI.Input;

namespace DialControllerTools
{
    internal class ScrollController : BaseTextController
    {
        private readonly DTE2 _dte;
#pragma warning disable IDE0044 // Add readonly modifier
        [Import]
        private ICompletionBroker _broker;
#pragma warning restore IDE0044 // Add readonly modifier

        public override string Moniker => ScrollControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;
        public override bool IsHapticFeedbackEnabled => false;

        public ScrollController(RadialControllerMenuItem menuItem, DTE2 dte, IVsTextManager textManager)
            : base(menuItem, textManager)
        {
            _dte = dte;
        }

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
        public override bool OnClick()
        {
            if (_dte.ActiveWindow.IsDocument())
            {
                IWpfTextView view = GetCurrentTextView();

                if (view != null && view.HasAggregateFocus && !_broker.IsCompletionActive(view))
                    SendKeys.Send("+{F10}");
                else
                    SendKeys.Send("{ENTER}");
            }
            else if (_dte.ActiveWindow.IsSolutionExplorer())
            {
                var selectedItems = _dte.ToolWindows.SolutionExplorer.SelectedItems as UIHierarchyItem[];

                if (selectedItems == null || selectedItems.Length != 1)
                    return false;

                if (selectedItems[0].UIHierarchyItems.Expanded)
                {
                    SendKeys.Send("{LEFT}");
                }
                else
                {
                    SendKeys.Send("{RIGHT}");
                }
            }
            else
            {
                SendKeys.Send("{ENTER}");
            }

            return true;
        }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

        public override bool OnRotate(RotationDirection direction)
        {
            bool handled = false;
            if (_dte.ActiveWindow.IsDocument())
            {
                IWpfTextView view = GetCurrentTextView();

                if (view != null && view.HasAggregateFocus)
                {
                    string cmd = direction == RotationDirection.Left ? "Edit.ScrollLineUp" : "Edit.ScrollLineDown";

                    for (int i = 0; i < DialPackage.Options.LinesToScroll; i++)
                    {
                        handled = _dte.Commands.ExecuteCommand(cmd);
                    }
                }
            }

            if (!handled)
            {
                string key = direction == RotationDirection.Left ? "{UP}" : "{DOWN}";
                SendKeys.Send(key);
            }

            return true;
        }
    }
}

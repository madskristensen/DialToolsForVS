using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    internal class ScrollController : BaseTextController
    {
        private readonly DTE2 _dte;
        private readonly ICompletionBroker _broker;

        public ScrollController(IDialControllerHost host, IVsTextManager textManager, ICompletionBroker broker)
            : base(host, textManager)
        {
            _dte = host.DTE;
            _broker = broker;
        }

        public override string Moniker => ScrollControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

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

        public override bool OnRotate(RotationDirection direction)
        {
            bool handled = false;
            if (_dte.ActiveWindow.IsDocument())
            {
                IWpfTextView view = GetCurrentTextView();

                if (view != null && view.HasAggregateFocus)
                {
                    string cmd = direction == RotationDirection.Left ? "Edit.ScrollLineUp" : "Edit.ScrollLineDown";
                    handled = _dte.Commands.ExecuteCommand(cmd);
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

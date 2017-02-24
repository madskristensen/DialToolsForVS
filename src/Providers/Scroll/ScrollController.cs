using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DialToolsForVS
{
    internal class ScrollController : BaseController
    {
        private ICompletionBroker _broker;

        public ScrollController(ICompletionBroker broker)
        {
            _broker = broker;
        }

        public override string Moniker => ScrollControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {
            if (VsHelpers.DTE.ActiveWindow.IsDocument())
            {
                IWpfTextView view = VsHelpers.GetCurentTextView();

                if (view != null && view.HasAggregateFocus && !_broker.IsCompletionActive(view))
                    SendKeys.Send("+{F10}");
                else
                    SendKeys.Send("{ENTER}");
            }
            else if (VsHelpers.DTE.ActiveWindow.IsSolutionExplorer())
            {
                var selectedItems = VsHelpers.DTE.ToolWindows.SolutionExplorer.SelectedItems as UIHierarchyItem[];

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
            if (VsHelpers.DTE.ActiveWindow.IsDocument())
            {
                IWpfTextView view = VsHelpers.GetCurentTextView();

                if (view != null && view.HasAggregateFocus)
                {
                    string cmd = direction == RotationDirection.Left ? "Edit.ScrollLineUp" : "Edit.ScrollLineDown";
                    handled = VsHelpers.ExecuteCommand(cmd);
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

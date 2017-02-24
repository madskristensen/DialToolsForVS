using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Forms;
using EnvDTE;

namespace DialToolsForVS
{
    internal class ScrollController : IDialController
    {
        public string Moniker => PredefinedMonikers.Scroll;
        public Specificity Specificity => (Specificity)int.MaxValue;
        public bool CanHandleClick => true;
        public bool CanHandleRotate => true;

        public void OnClick(DialEventArgs e)
        {
            if (VsHelpers.DTE.ActiveWindow.IsDocument())
            {
                IWpfTextView view = VsHelpers.GetCurentTextView();

                if (view != null && view.HasAggregateFocus)
                    SendKeys.Send("+{F10}");
                else
                    SendKeys.Send("{ENTER}");
            }
            else if (VsHelpers.DTE.ActiveWindow.IsSolutionExplorer())
            {
                var selectedItems = VsHelpers.DTE.ToolWindows.SolutionExplorer.SelectedItems as UIHierarchyItem[];

                if (selectedItems == null || selectedItems.Length != 1)
                    return;

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

            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (VsHelpers.DTE.ActiveWindow.IsDocument())
            {
                IWpfTextView view = VsHelpers.GetCurentTextView();

                if (view != null && view.HasAggregateFocus)
                {
                    string cmd = direction == RotationDirection.Left ? "Edit.ScrollLineUp" : "Edit.ScrollLineDown";
                    e.Handled = VsHelpers.ExecuteCommand(cmd);
                }
            }

            if (!e.Handled)
            {
                string key = direction == RotationDirection.Left ? "{UP}" : "{DOWN}";
                SendKeys.Send(key);
            }

            e.Handled = true;
        }
    }
}

using EnvDTE;
using System.Windows.Forms;

namespace DialToolsForVS
{
    internal class SolutionExplorerController : IDialController
    {
        public string Moniker => PredefinedMonikers.Scroll;
        public Specificity Specificity => Specificity.ToolWindow;

        public bool CanHandleClick
        {
            get
            {
                return VsHelpers.DTE.ActiveWindow.IsSolutionExplorer() && !VsHelpers.DTE.SelectedItems.MultiSelect;
            }
        }

        public bool CanHandleRotate => false;

        public void OnClick(DialEventArgs e)
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

            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
        }
    }
}

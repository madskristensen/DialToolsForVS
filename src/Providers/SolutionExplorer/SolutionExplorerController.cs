using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class SolutionExplorerController : IDialController
    {
        private DTE2 _dte = VsHelpers.DTE;

        public Specificity Specificity => Specificity.ToolWindow;

        public bool CanHandleClick
        {
            get
            {
                return _dte.ActiveWindow.IsSolutionExplorer() && !_dte.SelectedItems.MultiSelect;
            }
        }
        public bool CanHandleRotate
        {
            get
            {
                return _dte.ActiveWindow.IsSolutionExplorer();
            }
        }

        public bool OnClick(RadialControllerButtonClickedEventArgs args)
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

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                SendKeys.Send("{DOWN}");
            }
            else
            {
                SendKeys.Send("{UP}");
            }

            return true;
        }
    }
}

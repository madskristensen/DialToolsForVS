using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class SolutionExplorerController : IDialController
    {
        public Specificity Specificity => Specificity.ToolWindow;

        public bool CanHandleClick
        {
            get
            {
                return VsHelpers.DTE.ActiveWindow.IsSolutionExplorer() && !VsHelpers.DTE.SelectedItems.MultiSelect;
            }
        }

        public bool CanHandleRotate => false;

        public bool OnClick(RadialControllerButtonClickedEventArgs args)
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

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            return false;
        }
    }
}

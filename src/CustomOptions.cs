using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace DialToolsForVS
{
    [Guid("6C43E501-226F-4E73-949C-BF8808A94B48")]
    public class CustomOptions : DialogPage
    {
        private string clickAction = string.Empty;
        private string rightAction = string.Empty;
        private string leftAction = string.Empty;

        public string ClickAction { get => clickAction; set => clickAction = value; }
        public string RightAction { get => rightAction; set => rightAction = value; }
        public string LeftAction { get => leftAction; set => leftAction = value; }

        protected override IWin32Window Window
        {
            get
            {
                CustomOptionsControl page = new CustomOptionsControl();
                page.optionsPage = this;
                page.Initialize();
                return page;
            }
        }

    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.VisualStudio.Shell;

namespace DialControllerTools
{
    [ComVisible(true)]
    [Guid("6C43E501-226F-4E73-949C-BF8808A94B48")]
    public class CustomOptions : DialogPage
    {
        private string _clickAction = string.Empty;
        private string _rightAction = string.Empty;
        private string _leftAction = string.Empty;

        public string ClickAction { get => _clickAction; set => _clickAction = value; }

        public string RightAction { get => _rightAction; set => _rightAction = value; }

        public string LeftAction { get => _leftAction; set => _leftAction = value; }

        protected override IWin32Window Window
         => new CustomOptionsControl
         {
             CustomOptions = this
         };
    }
}

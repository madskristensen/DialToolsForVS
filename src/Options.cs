using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Collections.Generic;
using System;

namespace DialControllerTools
{
    public class Options : DialogPage
    {
        public event EventHandler OptionsApplied;

        [Category("Menu")]
        [DisplayName("Default menu")]
        [Description("Determines the default Dial menu item when opening Visual Studio.")]
        [DefaultValue(KnownProviders.Scroll)]
        [TypeConverter(typeof(EnumConverter))]
        public KnownProviders DefaultProvider { get; set; }

        [Category("Menu")]
        [DisplayName("Show Bookmarks menu")]
        [Description("Set to true to show the Bookmarks menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowBookmarksMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Customizable menu")]
        [Description("Set to true to show the Customizable menu")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowCustomizableMenu { get; set; } = false;

        [Category("Menu")]
        [DisplayName("Show Debug menu")]
        [Description("Set to true to show the Debug menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowDebugMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Editor menu")]
        [Description("Set to true to show the Editor menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowEditorMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Errors menu")]
        [Description("Set to true to show the Errors menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowErrorsMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Find menu")]
        [Description("Set to true to show the Find menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowFindMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Navigation menu")]
        [Description("Set to true to show the Navigation menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowNavigationMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Scroll menu")]
        [Description("Set to true to show the Scroll menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowScrollMenu { get; set; } = true;

        [Category("Menu")]
        [DisplayName("Show Zoom menu")]
        [Description("Set to true to show the Zoom menu")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowZoomMenu { get; set; } = true;

        [Category("Visual Studio Shell")]
        [DisplayName("Show Dial Log")]
        [Description("Set to true to show the Surface Dial log in the Output window")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanConverter))]
        public bool ShowLogInOutput { get; set; } = false;

        public Options()
        {
            InitializeMenuVisibility();
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            InitializeMenuVisibility();
            OptionsApplied?.Invoke(this, e);
        }

        private void InitializeMenuVisibility()
        {
            MenuVisibility = new Dictionary<string, bool>
            {
                {KnownProviders.Bookmarks.ToString(), ShowBookmarksMenu },
                {KnownProviders.Customizable.ToString(), ShowCustomizableMenu },
                {KnownProviders.Debug.ToString(), ShowDebugMenu },
                {KnownProviders.Editor.ToString(), ShowEditorMenu},
                {KnownProviders.Errors.ToString(), ShowErrorsMenu },
                {KnownProviders.Find.ToString(), ShowFindMenu },
                {KnownProviders.Navigation.ToString(), ShowNavigationMenu },
                {KnownProviders.Scroll.ToString(), ShowScrollMenu },
                {KnownProviders.Zoom.ToString(), ShowZoomMenu },
            };
        }

        internal Dictionary<string, bool> MenuVisibility { get; private set; }
    }
}

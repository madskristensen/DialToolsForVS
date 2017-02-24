using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace DialToolsForVS
{
    public class Options : DialogPage
    {
        [Category("General")]
        [DisplayName("Default menu")]
        [Description("Determines the default Dial menu item when opening Visual Studio.")]
        [DefaultValue(KnownProviders.Scroll)]
        [TypeConverter(typeof(EnumConverter))]
        public KnownProviders DefaultProvider { get; set; }
    }
}

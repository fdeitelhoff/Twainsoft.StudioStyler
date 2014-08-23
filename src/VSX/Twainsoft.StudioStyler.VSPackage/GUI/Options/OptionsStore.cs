using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Twainsoft.StudioStyler.VSPackage.GUI.Options
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public class OptionsStore : DialogPage
    {
        [Category("Twainsoft StudioStyler")]
        [DisplayName("Is the Scheme Preview In The Schemes Overview Visible")]
        [Description("Is the Scheme Preview In The Schemes Overview Visible")]
        public bool IsSchemePreviewVisible { get; set; }

        [Category("Twainsoft StudioStyler")]
        [DisplayName("Is the Scheme Preview In The History Overview Visible")]
        [Description("Is the Scheme Preview In The History Overview Visible")]
        public bool IsSchemeHistoryPreviewVisible { get; set; }

        [Category("Twainsoft StudioStyler")]
        [DisplayName("How Many Styles Are Visible Per Page")]
        [Description("How Many Styles Are Visible Per Page")]
        public int StylesPerPage { get; set; }

        public int MaxCacheAge { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                return new OptionsView(this);
            }
        }
    }
}
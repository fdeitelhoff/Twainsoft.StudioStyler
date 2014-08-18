using System;
using System.Globalization;
using System.Windows.Forms;

namespace Twainsoft.StudioStyler.VSPackage.GUI.Options
{
    public partial class OptionsView : UserControl
    {
        private OptionsStore OptionsStore { get; set; }

        private OptionsView()
        {
            InitializeComponent();
        }

        public OptionsView(OptionsStore optionsStore)
            : this()
        {
            OptionsStore = optionsStore;

            Initialize();
        }

        private void Initialize()
        {
            stylesPerPage.Text = OptionsStore.StylesPerPage.ToString(CultureInfo.InvariantCulture);
        }
        
        private void stylesPerPage_ValueChanged(object sender, EventArgs e)
        {
            OptionsStore.StylesPerPage = Convert.ToInt32(stylesPerPage.Value);
        }
    }
}

using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.VSPackage.Model;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class SchemesOverview
    {
        private SchemesOverviewModel Model { get; set; }

        public SchemesOverview()
        {
            InitializeComponent();

            Model = SchemesOverviewModel.Instance;
            DataContext = Model;
        }

        private void UpdateCache(object sender, RequestNavigateEventArgs e)
        {
            Model.RefreshCache();
        }

        private void Schemes_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Model.ActivateScheme();
        }
    }
}

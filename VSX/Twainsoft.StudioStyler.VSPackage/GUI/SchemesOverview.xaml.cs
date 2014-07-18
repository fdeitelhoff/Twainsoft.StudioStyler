using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.VSPackage.Model;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class SchemesOverview
    {
        private SchemesOverviewModel SchemesOverviewModel { get; set; }

        public SchemesOverview()
        {
            InitializeComponent();
        }

        public void SetModel(SchemesOverviewModel schemesOverviewModel)
        {
            SchemesOverviewModel = schemesOverviewModel;
            DataContext = SchemesOverviewModel;
        }

        private void UpdateCache(object sender, RequestNavigateEventArgs e)
        {
            SchemesOverviewModel.RefreshCache();
        }

        private void Schemes_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SchemesOverviewModel.ActivateScheme();
        }
    }
}

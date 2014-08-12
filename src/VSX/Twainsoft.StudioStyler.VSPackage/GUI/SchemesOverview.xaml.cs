using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.VSPackage.Model;
using Process = System.Diagnostics.Process;

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

        private void ViewScheme_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("http://studiostyl.es" + e.Uri);
        }

        private void CollapsePreview_OnClick(object sender, RoutedEventArgs e)
        {
            PreviewRow.Height = new GridLength(0);
        }

        private void StudioStylesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PreviewRow.Height = new GridLength(100);
        }
    }
}

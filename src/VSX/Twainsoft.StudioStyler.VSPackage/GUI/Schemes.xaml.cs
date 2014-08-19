using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.VSPackage.Model;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class Schemes
    {
        private SchemesModel Model { get; set; }

        public Schemes()
        {
            InitializeComponent();

            Model = SchemesModel.Instance;
            DataContext = Model;

            if (Model.OptionsStore != null)
            {
                PreviewRow.Height = !Model.OptionsStore.IsSchemePreviewVisible ? new GridLength(0) : new GridLength(100);
            }
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

            Model.OptionsStore.IsSchemePreviewVisible = false;
            Model.OptionsStore.SaveSettingsToStorage();
        }

        private void StudioStylesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PreviewRow.Height = new GridLength(100);

            Model.OptionsStore.IsSchemePreviewVisible = true;
            Model.OptionsStore.SaveSettingsToStorage();
        }
    }
}

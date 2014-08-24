using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.VSPackage.Model;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class SchemeView
    {
        private SchemeModel SchemesModel { get; set; }

        public SchemeView(SchemeModel schemesModel)
        {
            InitializeComponent();

            SchemesModel = schemesModel;
            DataContext = SchemesModel;

            // TODO: This OptionStore things can be made better!
            if (SchemesModel.OptionsStore != null)
            {
                PreviewRow.Height = !SchemesModel.OptionsStore.IsSchemePreviewVisible ? new GridLength(0) : new GridLength(100);
            }
        }

        private void UpdateCache(object sender, RequestNavigateEventArgs e)
        {
            SchemesModel.RefreshCache();
        }

        private void ViewScheme_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("http://studiostyl.es" + e.Uri);
        }

        private void CollapsePreview_OnClick(object sender, RoutedEventArgs e)
        {
            PreviewRow.Height = new GridLength(0);

            SchemesModel.OptionsStore.IsSchemePreviewVisible = false;
            SchemesModel.OptionsStore.SaveSettingsToStorage();
        }

        private void StudioStylesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PreviewRow.Height = new GridLength(100);

            SchemesModel.OptionsStore.IsSchemePreviewVisible = true;
            SchemesModel.OptionsStore.SaveSettingsToStorage();
        }
    }
}

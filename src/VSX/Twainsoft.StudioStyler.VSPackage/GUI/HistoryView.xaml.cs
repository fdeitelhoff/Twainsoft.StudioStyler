using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.VSPackage.Model;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class HistoryView
    {
        private HistoryModel HistoryModel { get; set; }

        public HistoryView(HistoryModel historyModel)
        {
            InitializeComponent();

            HistoryModel = historyModel;
            DataContext = HistoryModel;

            // TODO: This OptionsStore stuff can be made better!
            if (HistoryModel.OptionsStore != null)
            {
                PreviewRow.Height = !HistoryModel.OptionsStore.IsSchemeHistoryPreviewVisible ? new GridLength(0) : new GridLength(100);
            }
        }

        private void ViewScheme_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("http://studiostyl.es" + e.Uri);
        }

        private void CollapsePreview_OnClick(object sender, RoutedEventArgs e)
        {
            PreviewRow.Height = new GridLength(0);

            HistoryModel.OptionsStore.IsSchemeHistoryPreviewVisible = false;
            HistoryModel.OptionsStore.SaveSettingsToStorage();
        }

        private void StudioStylesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PreviewRow.Height = new GridLength(100);

            HistoryModel.OptionsStore.IsSchemeHistoryPreviewVisible = true;
            HistoryModel.OptionsStore.SaveSettingsToStorage();
        }

        private void SwitchBackOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            HistoryModel.SwitchBackStyle();
        }
    }
}

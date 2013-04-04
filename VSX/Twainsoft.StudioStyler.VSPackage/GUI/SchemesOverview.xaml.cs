using System.Windows;
using System.Windows.Input;
using Twainsoft.StudioStyler.Services.StudioStyles;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;
using Twainsoft.StudioStyler.Services.StudioStyles.Settings;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class SchemesOverview
    {
        private SchemesCollectionView SchemesCollectionView { get; set; }

        private StudioStylesService StudioStylesService { get; set; }
        private SettingsActivator SettingsActivator { get; set; }

        public SchemesOverview()
        {
            InitializeComponent();

            StudioStylesService = new StudioStylesService();
            StudioStylesService.SchemeSucessfullyDownloaded += StudioStylesServiceOnSchemeSucessfullyDownloaded;

            SettingsActivator = new SettingsActivator();
        }

        private void RefreshSchemes_Click(object sender, RoutedEventArgs e)
        {
            var schemes = StudioStylesService.All();

            SchemesCollectionView = new SchemesCollectionView(schemes, 15);
            DataContext = SchemesCollectionView;
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            SchemesCollectionView.MoveToPreviousPage();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            SchemesCollectionView.MoveToNextPage(); 
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            //if (!String.IsNullOrWhiteSpace(SearchString.Text))
            //{
            //    SchemesCollectionView.Filter = Filter;
            //}
        }

        //private bool Filter(object obj)
        //{
        //    var scheme = obj as Scheme;

        //    return scheme.Title.Contains(SearchString.Text.Trim());
        //}

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            //SchemesCollectionView.Filter = null;
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            SchemesCollectionView.MoveToFirstPage();
        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            SchemesCollectionView.MoveToLastPage();
        }

        private void Schemes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Download();
        }

        private void DownloadScheme_Click(object sender, RoutedEventArgs e)
        {
            Download();
        }

        private void Download()
        {
            var scheme = SchemesCollectionView.CurrentItem as Scheme;

            if (scheme != null)
            {
                StudioStylesService.Download(scheme.DownloadPath);
            }
        }

        private void StudioStylesServiceOnSchemeSucessfullyDownloaded(object sender, string file)
        {
            SettingsActivator.LoadScheme(file);
        }
    }
}

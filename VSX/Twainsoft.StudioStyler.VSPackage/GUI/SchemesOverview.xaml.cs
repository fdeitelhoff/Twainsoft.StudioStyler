using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using Twainsoft.StudioStyler.Services.StudioStyles;
using Twainsoft.StudioStyler.Services.StudioStyles.Cache;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;
using Twainsoft.StudioStyler.Services.StudioStyles.Settings;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public partial class SchemesOverview
    {
        private PagedCollectionView SchemesCollectionView { get; set; }

        private StudioStylesService StudioStylesService { get; set; }
        private SettingsActivator SettingsActivator { get; set; }

        private SchemeCache SchemeCache { get; set; }

        public SchemesOverview()
        {
            InitializeComponent();

            SchemeCache = SchemeCache.Instance;
            SchemeCache.SchemesLoaded += SchemesLoaded;
            CacheStatus.DataContext = SchemeCache;
            UpdateCacheProgress.DataContext = SchemeCache;

            StudioStylesService = new StudioStylesService();

            SettingsActivator = new SettingsActivator();

            SchemeCache.Check();
        }

        private void SchemesLoaded()
        {
            SchemesCollectionView = new PagedCollectionView(SchemeCache.Schemes) {PageSize = 15};
            Schemes.ItemsSource = SchemesCollectionView;
            MainMenu.DataContext = SchemesCollectionView;
            CurentItemRange.DataContext = SchemesCollectionView;

            SchemesCollectionView.Filter = null;
        }

        private async void Download()
        {
            var scheme = SchemesCollectionView.CurrentItem as Scheme;

            if (scheme != null)
            {
                var file = await StudioStylesService.DownloadAsync(scheme.DownloadPath);

                LoadScheme(file);
            }
        }

        private void LoadScheme(string file)
        {
            SettingsActivator.LoadScheme(file);
        }

        private async void RefreshCache()
        {
            await SchemeCache.Refresh();

            SchemesLoaded();
        }

        private void RefreshSchemes_Click(object sender, RoutedEventArgs e)
        {
            RefreshCache();
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
            if (!String.IsNullOrWhiteSpace(SearchString.Text))
            {
                SchemesCollectionView.Filter += Filter;
            }
            else
            {
                SchemesCollectionView.Filter = null;
            }
        }

        private bool Filter(object obj)
        {
            var scheme = obj as Scheme;

            if (scheme == null)
            {
                return false;
            }

            return scheme.Title.ToLower().Contains(SearchString.Text.ToLower().Trim());
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            SchemesCollectionView.Filter = null;
            SearchString.Text = null;
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

        private void UpdateCache(object sender, RequestNavigateEventArgs e)
        {
            RefreshCache();
        }
    }
}

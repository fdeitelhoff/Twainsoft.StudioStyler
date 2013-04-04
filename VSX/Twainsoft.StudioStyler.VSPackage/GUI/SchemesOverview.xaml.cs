using System;
using System.Windows;
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
        private SchemesCollectionView SchemesCollectionView { get; set; }

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
            SchemesCollectionView = new SchemesCollectionView(SchemeCache.Schemes, 15);
            Schemes.ItemsSource = SchemesCollectionView;
            MainMenu.DataContext = SchemesCollectionView;
            CurentItemRange.DataContext = SchemesCollectionView;
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
            var schemes = await SchemeCache.Refresh();

            SchemesCollectionView = new SchemesCollectionView(schemes, 15);
            Schemes.ItemsSource = SchemesCollectionView;
            MainMenu.DataContext = SchemesCollectionView;
            CurentItemRange.DataContext = SchemesCollectionView;
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
        //    if (!String.IsNullOrWhiteSpace(SearchString.Text))
        //    {
        //        SchemesCollectionView.Filter = Filter;
        //    }
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

        private void UpdateCache(object sender, RequestNavigateEventArgs e)
        {
            RefreshCache();
        }
    }
}

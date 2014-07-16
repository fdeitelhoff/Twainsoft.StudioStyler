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
        private PagedCollectionView PagedSchemesView { get; set; }

        private StudioStylesService StudioStylesService { get; set; }
        private SettingsActivator SettingsActivator { get; set; }

        private SchemeCache SchemeCache { get; set; }

        public SchemesOverview()
        {
            InitializeComponent();

            PagedSchemesView = PagedCollectionView.Instance;
            PagedSchemesView.PageChanged += InstanceOnPageChanged;
            PagedSchemesView.CurrentChanged += PagedSchemesViewOnCurrentChanged;

            SchemeCache = SchemeCache.Instance;
            Schemes.ItemsSource = PagedCollectionView.Instance;
            CacheStatus.DataContext = SchemeCache;
            UpdateCacheProgress.DataContext = SchemeCache;

            //PagedSchemesView = new PagedCollectionView();
            StudioStylesService = new StudioStylesService();
            SettingsActivator = new SettingsActivator();
        }

        private void PagedSchemesViewOnCurrentChanged(object sender, EventArgs eventArgs)
        {
            Console.WriteLine(PagedCollectionView.Instance.CurrentPage);
        }

        private void InstanceOnPageChanged(object sender, EventArgs eventArgs)
        {
            status.Text = string.Format("Showing items {0} - {1} (Page {2}/{3}", PagedSchemesView.FirstItemNumber,
                PagedSchemesView.LastItemNumber, PagedSchemesView.CurrentPage, PagedSchemesView.PageCount);
        }

        private async void Download()
        {
            //var scheme = PagedSchemesView.CurrentItem as Scheme;
            var scheme = Schemes.CurrentItem as Scheme;

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
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            PagedSchemesView.MoveToPreviousPage();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            PagedSchemesView.MoveToNextPage(); 
        }

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            PagedSchemesView.MoveToFirstPage();
        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            PagedSchemesView.MoveToLastPage();
        }

        private void DownloadScheme_Click(object sender, RoutedEventArgs e)
        {
            Download();
        }

        private void UpdateCache(object sender, RequestNavigateEventArgs e)
        {
            RefreshCache();
        }

        private void Schemes_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Download();
        }
    }
}

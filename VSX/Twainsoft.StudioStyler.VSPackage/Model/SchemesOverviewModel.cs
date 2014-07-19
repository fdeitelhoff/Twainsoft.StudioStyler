using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.Services.StudioStyles;
using Twainsoft.StudioStyler.Services.StudioStyles.Annotations;
using Twainsoft.StudioStyler.Services.StudioStyles.Cache;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;
using Twainsoft.StudioStyler.Services.StudioStyles.Settings;

namespace Twainsoft.StudioStyler.VSPackage.Model
{
    public sealed class SchemesOverviewModel : INotifyPropertyChanged
    {
        public SchemeCache SchemeCache { get; private set; }
        public PagedCollectionView PagedSchemesView { get; private set; }

        private string CurrentSearchString { get; set; }
        private List<string> SearchValues { get; set; }

        private StudioStylesService StudioStylesService { get; set; }
        private SettingsActivator SettingsActivator { get; set; }

        private static SchemesOverviewModel instance = null;

        public static SchemesOverviewModel Instance
        {
            get { return instance ?? (instance = new SchemesOverviewModel()); }
        }

        public int CurrentPage
        {
            get { return PagedSchemesView.CurrentPage; }
        }

        public int OverallPages
        {
            get { return PagedSchemesView.PageCount; }
        }

        public object FirstItemNumber
        {
            get { return PagedSchemesView.PageIndex * PagedSchemesView.PageSize + 1; }
        }

        public object LastItemNumber
        {
            get
            {
                return Math.Min(PagedSchemesView.PageIndex * PagedSchemesView.PageSize + PagedSchemesView.PageSize, PagedSchemesView.ItemCount);
            }
        }

        private SchemesOverviewModel()
        {
            SchemeCache = new SchemeCache();
            PagedSchemesView = new PagedCollectionView(SchemeCache.Schemes) { PageSize = 25};

            CurrentSearchString = "";
            SearchValues = new List<string>();

            StudioStylesService = new StudioStylesService();
            SettingsActivator = new SettingsActivator();
        }

        public async void RefreshCache()
        {
            await SchemeCache.Refresh();
        }

        public void CheckCache()
        {
            SchemeCache.Check();
        }

        public void Search(OleMenuCmdEventArgs eventArgs)
        {
            var input = eventArgs.InValue;

            var vOut = eventArgs.OutValue;

            if (vOut != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(CurrentSearchString, vOut);
            }

            else if (input != null)
            {
                CurrentSearchString = input.ToString().Trim().ToLower();

                if (!string.IsNullOrWhiteSpace(CurrentSearchString))
                {
                    if (!SearchValues.Contains(CurrentSearchString))
                    {
                        SearchValues.Add(CurrentSearchString);
                    }

                    PagedSchemesView.Filter += delegate(object obj)
                    {
                        var scheme = obj as Scheme;

                        return scheme != null 
                            && (scheme.Title.ToLower().Contains(CurrentSearchString)
                            //|| scheme.Author.ToLower().Contains(CurrentSearchString)
                            || scheme.Popularity.ToString(CultureInfo.InvariantCulture).Contains(CurrentSearchString)
                            || scheme.Rating.ToString(CultureInfo.InvariantCulture).Contains(CurrentSearchString));
                    };
                }
                else
                {
                    PagedSchemesView.Filter = null;
                }
            }
        }

        public void SearchTerm(OleMenuCmdEventArgs eventArgs)
        {
            var vOut = eventArgs.OutValue;

            if (vOut != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(SearchValues.ToArray(), vOut);
            }
        }

        public void ActivateScheme()
        {
            DownloadScheme();
        }

        private async void DownloadScheme()
        {
            var scheme = PagedSchemesView.CurrentItem as Scheme;

            if (scheme != null)
            {
                var file = await StudioStylesService.DownloadAsync(scheme.DownloadPath);

                SettingsActivator.LoadScheme(file);
            }
        }

        public void NextPage()
        {
            PagedSchemesView.MoveToNextPage();

            OnPropertyChanged("CurrentPage");
            OnPropertyChanged("OverallPages");
            OnPropertyChanged("FirstItemNumber");
            OnPropertyChanged("LastItemNumber");
        }

        public void FirstPage()
        {
            PagedSchemesView.MoveToFirstPage();

            OnPropertyChanged("CurrentPage");
            OnPropertyChanged("OverallPages");
            OnPropertyChanged("FirstItemNumber");
            OnPropertyChanged("LastItemNumber");
        }

        public void PreviousPage()
        {
            PagedSchemesView.MoveToPreviousPage();

            OnPropertyChanged("CurrentPage");
            OnPropertyChanged("OverallPages");
            OnPropertyChanged("FirstItemNumber");
            OnPropertyChanged("LastItemNumber");
        }

        public void LastPage()
        {
            PagedSchemesView.MoveToLastPage();

            OnPropertyChanged("CurrentPage");
            OnPropertyChanged("OverallPages");
            OnPropertyChanged("FirstItemNumber");
            OnPropertyChanged("LastItemNumber");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

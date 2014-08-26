using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Data;
using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.Services.StudioStyles.Annotations;
using Twainsoft.StudioStyler.Services.StudioStyles.Caches;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;
using Twainsoft.StudioStyler.Services.StudioStyles.Settings;
using Twainsoft.StudioStyler.Services.StudioStyles.Styles;
using Twainsoft.StudioStyler.VSPackage.GUI.Options;
using Twainsoft.StudioStyler.VSX;

namespace Twainsoft.StudioStyler.VSPackage.Model
{
    public sealed class HistoryModel : IModel, INotifyPropertyChanged
    {
        public PagedCollectionView PagedHistoryView { get; private set; }

        private string CurrentSearchString { get; set; }
        private List<string> SearchValues { get; set; }

        private StudioStylesService StudioStylesService { get; set; }
        private SettingsActivator SettingsActivator { get; set; }
        private SchemeHistory SchemesHistory { get; set; }

        public OptionsStore OptionsStore { get; private set; }

        public int CurrentPage
        {
            get { return PagedHistoryView.CurrentPage; }
        }

        public int OverallPages
        {
            get { return PagedHistoryView.PageCount; }
        }

        public int FirstItemNumber
        {
            get { return PagedHistoryView.PageIndex * PagedHistoryView.PageSize + 1; }
        }

        public int LastItemNumber
        {
            get
            {
                return Math.Min(PagedHistoryView.PageIndex * PagedHistoryView.PageSize + PagedHistoryView.PageSize, PagedHistoryView.ItemCount);
            }
        }

        public int OverallItemCount
        {
            get { return SchemesHistory.History.Count; }
        }

        public bool IsItemSelected
        {
            get { return PagedHistoryView.CurrentItem != null; }
        }

        public HistoryModel(SchemeHistory schemesHistory, OptionsStore optionsStore, 
            StudioStylesService studioStylesService, SettingsActivator settingsActivator)
        {
            SchemesHistory = schemesHistory;
            OptionsStore = optionsStore;
            StudioStylesService = studioStylesService;
            SettingsActivator = settingsActivator;

            var stylesPerPage = optionsStore.StylesPerPage;

            PagedHistoryView = new PagedCollectionView(SchemesHistory.History) { PageSize = stylesPerPage };
            PagedHistoryView.SortDescriptions.Add(new SortDescription("Activations", ListSortDirection.Descending));

            CurrentSearchString = "";
            SearchValues = new List<string>();
        }

        public void UpdateView()
        {
            PagedHistoryView.Refresh();
        }

        public void RefreshCache()
        {
            // TODO: Not necessary here. Refactor the Interface or create a abstract base class with virtual methods.
            //UpdateInfoBar();
        }

        public void CheckCache()
        {
            SchemesHistory.Check();
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

                    PagedHistoryView.Filter += delegate(object obj)
                    {
                        var scheme = obj as Scheme;

                        if (scheme == null)
                        {
                            return false;
                        }

                        var matchingTitle = false;
                        var matchingAuthor = false;

                        if (!string.IsNullOrWhiteSpace(scheme.Title))
                        {
                            matchingTitle = scheme.Title.ToLower().Contains(CurrentSearchString);
                        }

                        if (!string.IsNullOrWhiteSpace(scheme.Author))
                        {
                            matchingAuthor = scheme.Author.ToLower().Contains(CurrentSearchString);
                        }

                        return matchingTitle || matchingAuthor;
                    };
                }
                else
                {
                    PagedHistoryView.Filter = null;
                }
            }

            UpdateInfoBar();
        }

        public void SearchTerm(OleMenuCmdEventArgs eventArgs)
        {
            var vOut = eventArgs.OutValue;

            if (vOut != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(SearchValues.ToArray(), vOut);
            }
        }

        public void ClearSearch()
        {
            CurrentSearchString = null;
            PagedHistoryView.Filter = null;
        }

        public void ActivateScheme()
        {
            DownloadScheme();
        }

        private async void DownloadScheme()
        {
            var scheme = PagedHistoryView.CurrentItem as Scheme;

            if (scheme != null)
            {
                var file = await StudioStylesService.DownloadAsync(scheme.DownloadPath);

                if (SettingsActivator.LoadScheme(file))
                {
                    SchemesHistory.Add(scheme);

                    StatusBarHelper.Update(string.Format("The Style '{0}' was activated successfully!", scheme.Title));
                }
            }
        }

        public void NextPage()
        {
            PagedHistoryView.MoveToNextPage();

            UpdateInfoBar();
        }

        public void FirstPage()
        {
            PagedHistoryView.MoveToFirstPage();

            UpdateInfoBar();
        }

        public void PreviousPage()
        {
            PagedHistoryView.MoveToPreviousPage();

            UpdateInfoBar();
        }

        public void LastPage()
        {
            PagedHistoryView.MoveToLastPage();

            UpdateInfoBar();
        }

        public void SwitchBackStyle()
        { }

        private void UpdateInfoBar()
        {
            OnPropertyChanged("CurrentPage");
            OnPropertyChanged("OverallPages");
            OnPropertyChanged("FirstItemNumber");
            OnPropertyChanged("LastItemNumber");
            OnPropertyChanged("OverallItemCount");
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

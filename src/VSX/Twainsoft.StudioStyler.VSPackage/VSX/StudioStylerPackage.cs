﻿using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twainsoft.StudioStyler.VSPackage.GUI;
using Twainsoft.StudioStyler.VSPackage.GUI.Options;
using Twainsoft.StudioStyler.VSPackage.Model;

namespace Twainsoft.StudioStyler.VSPackage.VSX
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.4", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SchemesOverviewWindow))]
    [Guid(GuidList.guidTwainsoft_StudioStyler_VSPackagePkgString)]
    [ProvideOptionPage(typeof(OptionsStore), "Twainsoft StudioStyler", "General", 0, 0, true)]
    public sealed class StudioStylerPackage : Package
    {
        private IModel Model { get; set; }

        private SchemesView SchemesView { get; set; }
        private HistoryView HistoryView { get; set; }

        private ToolWindowPane Window { get; set; }
        private StudioStylesView StudioStylesView { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // The ToolWindow menu command.
                var toolWindowCommandId = new CommandID(GuidList.guidTwainsoft_StudioStyler_VSPackageCmdSet, CommandIds.ShowStudioStylesId);
                var menuToolWin = new MenuCommand(OnShowToolWindow, toolWindowCommandId);
                mcs.AddCommand(menuToolWin);

                // The search combobox command.
                var searchStringCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SearchStringComboId);
                var searchStringCommand = new OleMenuCommand(OnSearchString, searchStringCommandId);
                mcs.AddCommand(searchStringCommand);

                // The search combobox value list command.
                var searchStringValuesCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SearchStringValuesId);
                var searchSTringValuesCommand = new OleMenuCommand(OnSearchStringValues, searchStringValuesCommandId);
                mcs.AddCommand(searchSTringValuesCommand);

                // The clear search command.
                var clearSearchCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.ClearSearch);
                var clearSearchCommand = new OleMenuCommand(OnClearSearch, clearSearchCommandId);
                mcs.AddCommand(clearSearchCommand);

                // The refresh schemes cache command.
                var refreshSchemesCacheCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.RefreshSchemesCache);
                var refreshSchemesCacheCommand = new OleMenuCommand(OnRefreshSchemesCache, refreshSchemesCacheCommandId);
                mcs.AddCommand(refreshSchemesCacheCommand);

                // The activate scheme command.
                var activateSchemeCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.ActivateScheme);
                var activateSchemeCommand = new OleMenuCommand(OnActivateScheme, activateSchemeCommandId);
                activateSchemeCommand.BeforeQueryStatus += OnBeforeQueryStatusActivateScheme;
                mcs.AddCommand(activateSchemeCommand);

                // The history command.
                var historyCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SchemesHistory);
                var historyCommand = new OleMenuCommand(OnHistory, historyCommandId);
                historyCommand.BeforeQueryStatus += HistoryCommandOnBeforeQueryStatus;
                mcs.AddCommand(historyCommand);

                // The first page command.
                var firstPageCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.FirstPageNavigation);
                var firstPageCommand = new OleMenuCommand(OnFirstPage, firstPageCommandId);
                mcs.AddCommand(firstPageCommand);

                // The previous page command.
                var previousPageCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.PreviousPageNavigation);
                var previousPageCommand = new OleMenuCommand(OnPreviousPage, previousPageCommandId);
                mcs.AddCommand(previousPageCommand);

                // The next page command.
                var nextPageCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.NextPageNavigation);
                var nextPageCommand = new OleMenuCommand(OnNextPage, nextPageCommandId);
                mcs.AddCommand(nextPageCommand);

                // The last page command.
                var lastPageCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.LastPageNavigation);
                var lastPageCommand = new OleMenuCommand(OnLastPage, lastPageCommandId);
                mcs.AddCommand(lastPageCommand);
            }

            var optionsStore = GetDialogPage(typeof(OptionsStore)) as OptionsStore;

            HistoryModel.Instance.OptionsStore = optionsStore;
            SchemesModel.Instance.OptionsStore = optionsStore;

            HistoryModel.Instance.CheckCache();
            SchemesModel.Instance.CheckCache();

            Model = SchemesModel.Instance;

            // Maybe insert the model here?
            SchemesView = new SchemesView();
            HistoryView = new HistoryView();

            // Save those objects. We need them more than once!
            Window = FindToolWindow(typeof(SchemesOverviewWindow), 0, true);

            if (Window == null || Window.Frame == null)
            {
                throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            }

            var schemesOverview = Window as SchemesOverviewWindow;

            if (schemesOverview == null)
            {
                throw new InvalidOperationException("Cannot find the SchemesOverviewWindow!");
            }

            StudioStylesView = schemesOverview.Content as StudioStylesView;

            if (StudioStylesView == null)
            {
                throw new InvalidOperationException("Cannot find the StudioStylesView!");
            }

            StudioStylesView.Dock.Children.Add(SchemesView);
        }

        private void OnShowToolWindow(object sender, EventArgs e)
        {
            //var window = FindToolWindow(typeof(SchemesOverviewWindow), 0, true);

            //if ((null == window) || (null == window.Frame))
            //{
            //    throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            //}

            var windowFrame = (IVsWindowFrame)Window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void OnSearchStringValues(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                Model.SearchTerm(eventArgs);
            }
        }

        private void OnSearchString(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                Model.Search(eventArgs);
            }
        }

        private void OnClearSearch(object sender, EventArgs e)
        {
            Model.ClearSearch();
        }

        private void OnRefreshSchemesCache(object sender, EventArgs e)
        {
            Model.RefreshCache();
        }

        private void OnActivateScheme(object sender, EventArgs e)
        {
            Model.ActivateScheme();
        }

        private void OnBeforeQueryStatusActivateScheme(object sender, EventArgs e)
        {
            var myCommand = sender as OleMenuCommand;
            if (null != myCommand)
            {
                myCommand.Enabled = Model.IsItemSelected;
            }
        }

        private void OnHistory(object sender, EventArgs e)
        {
            //var window = FindToolWindow(typeof(SchemesOverviewWindow), 0, true);

            //if ((null == window) || (null == window.Frame))
            //{
            //    throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            //}

            //var schemesOverview = window as SchemesOverviewWindow;

            //if (schemesOverview == null)
            //{
            //    throw new InvalidOperationException("Cannot find the SchemesOverviewWindow!");
            //}

            //var studioStyles = schemesOverview.Content as StudioStylesView;

            //if (studioStyles == null)
            //{
            //    throw new InvalidOperationException("Cannot find the StudioStylesView!");
            //}

            if (Model is SchemesModel)
            {
                Model = HistoryModel.Instance;

                StudioStylesView.Dock.Children.Remove(SchemesView);
                StudioStylesView.Dock.Children.Add(HistoryView);
            }
            else
            {
                Model = SchemesModel.Instance;

                StudioStylesView.Dock.Children.Remove(HistoryView);
                StudioStylesView.Dock.Children.Add(SchemesView);
            }
        }

        private void HistoryCommandOnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var myCommand = sender as OleMenuCommand;
            if (null != myCommand)
            {
                if (Model is SchemesModel)
                {
                    myCommand.Text = "History";
                }
                else
                {
                    myCommand.Text = "Styles";
                }
            }
        }

        private void OnFirstPage(object sender, EventArgs e)
        {
            Model.FirstPage();
        }

        private void OnPreviousPage(object sender, EventArgs e)
        {
            Model.PreviousPage();
        }

        private void OnNextPage(object sender, EventArgs e)
        {
            Model.NextPage();
        }

        private void OnLastPage(object sender, EventArgs e)
        {
            Model.LastPage();
        }
    }
}

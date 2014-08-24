using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Twainsoft.StudioStyler.Services.StudioStyles.Caches;
using Twainsoft.StudioStyler.VSPackage.GUI;
using Twainsoft.StudioStyler.VSPackage.GUI.Options;
using Twainsoft.StudioStyler.VSPackage.GUI.ToolWindow;
using Twainsoft.StudioStyler.VSPackage.Model;
using Twainsoft.StudioStyler.VSX;

namespace Twainsoft.StudioStyler.VSPackage.VSX
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.4", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SchemeToolWindow))]
    [Guid(GuidList.guidTwainsoft_StudioStyler_VSPackagePkgString)]
    [ProvideOptionPage(typeof(OptionsStore), "Twainsoft StudioStyler", "General", 0, 0, true)]
    public sealed class StudioStylerPackage : Package
    {
        private IModel Model { get; set; }

        private SchemeView SchemeView { get; set; }
        private HistoryView HistoryView { get; set; }

        private SchemeCache SchemeCache { get; set; }
        private SchemeHistory SchemeHistory { get; set; }
        private SchemeModel SchemeModel { get; set; }
        private HistoryModel HistoryModel { get; set; }

        private ToolWindowPane Window { get; set; }
        private SchemeToolWindowView SchemeToolWindowView { get; set; }

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

            // The general options for this package.
            var optionsStore = GetDialogPage(typeof(OptionsStore)) as OptionsStore;

            // The cache holds all studio styles (called schemes) that are available.
            SchemeCache = new SchemeCache();
            SchemeHistory = new SchemeHistory(SchemeCache);

            // Instantiate the models. They are the main classes to interact with the ui and the data.
            SchemeModel = new SchemeModel(SchemeCache, SchemeHistory, optionsStore);
            HistoryModel = new HistoryModel(SchemeHistory, optionsStore);

            // Instantiate the views. This are the WPF controls that visualize the styles and the history.
            SchemeView = new SchemeView(SchemeModel);
            HistoryView = new HistoryView(HistoryModel);

            // Some Visual Studio visuals can fire some events so that the model is need very early.
            Model = SchemeModel;

            // Save those objects. We need them more than once!
            Window = FindToolWindow(typeof(SchemeToolWindow), 0, true);

            if (Window == null || Window.Frame == null)
            {
                throw new NotSupportedException(Resources.Resources.CanNotCreateWindow);
            }

            var schemesOverview = Window as SchemeToolWindow;

            if (schemesOverview == null)
            {
                throw new InvalidOperationException("Cannot find the SchemesOverviewWindow!");
            }

            SchemeToolWindowView = schemesOverview.Content as SchemeToolWindowView;

            if (SchemeToolWindowView == null)
            {
                throw new InvalidOperationException("Cannot find the StudioStylesView!");
            }

            SchemeToolWindowView.Dock.Children.Add(SchemeView);

            // Check both model caches (Styles and History) so that saved data is present.
            SchemeModel.CheckCache();
            HistoryModel.CheckCache();
        }

        private void OnShowToolWindow(object sender, EventArgs e)
        {
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
            // Refreshing can take some time, so let us ask first...
            if (VsMessageBox.ShowQuestionMessageBox("Refresh All Studio Styles?",
                "Do you really want to refresh all Studio Styles?",
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND) == VsMessageResult.Yes)
            {
                Model.RefreshCache();
            }
        }

        private void OnActivateScheme(object sender, EventArgs e)
        {
            Model.ActivateScheme();
        }

        private void OnHistory(object sender, EventArgs e)
        {
            // TODO: The update of the visuals (changed history activation count) doesnt work! We need some other methods!
            if (Model is SchemeModel)
            {
                Model = HistoryModel;

                SchemeToolWindowView.Dock.Children.Remove(SchemeView);
                SchemeToolWindowView.Dock.Children.Add(HistoryView);
            }
            else
            {
                Model = SchemeModel;

                SchemeToolWindowView.Dock.Children.Remove(HistoryView);
                SchemeToolWindowView.Dock.Children.Add(SchemeView);
            }
        }

        private void HistoryCommandOnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var myCommand = sender as OleMenuCommand;
            if (null != myCommand)
            {
                if (Model is SchemeModel)
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

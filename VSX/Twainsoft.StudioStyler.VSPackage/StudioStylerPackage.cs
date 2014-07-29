using System;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.VSPackage.GUI;
using Twainsoft.StudioStyler.VSPackage.Model;
using Twainsoft.StudioStyler.VSPackage.VSX;

namespace Twainsoft.StudioStyler.VSPackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SchemesOverviewWindow))]
    [Guid(GuidList.guidTwainsoft_StudioStyler_VSPackagePkgString)]
    public sealed class StudioStylerPackage : Package
    {
        private SchemesOverviewModel Model { get; set; }

        public StudioStylerPackage()
        {
            Model = SchemesOverviewModel.Instance;
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = FindToolWindow(typeof(SchemesOverviewWindow), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        //protected override void OnSaveOptions(string key, Stream stream)
        //{
        //    base.OnSaveOptions(key, stream);

        //    Console.WriteLine();
        //}

        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // The ToolWindow menu command.
                var toolWindowCommandId = new CommandID(GuidList.guidTwainsoft_StudioStyler_VSPackageCmdSet, CommandIds.ShowStudioStylesId);
                var menuToolWin = new MenuCommand(ShowToolWindow, toolWindowCommandId);
                mcs.AddCommand(menuToolWin);

                // The search combobox command.
                var searchStringCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SearchStringComboId);
                var searchStringCommand = new OleMenuCommand(OnSearchString, searchStringCommandId);
                mcs.AddCommand(searchStringCommand);

                // The search combobox value list command.
                var searchStringValuesCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SearchStringValuesId);
                var searchSTringValuesCommand = new OleMenuCommand(OnSearchStringValues, searchStringValuesCommandId);
                mcs.AddCommand(searchSTringValuesCommand);

                // The refresh schemes cache command.
                var refreshSchemesCacheCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.RefreshSchemesCache);
                var refreshSchemesCacheCommand = new OleMenuCommand(OnRefreshSchemesCache, refreshSchemesCacheCommandId);
                mcs.AddCommand(refreshSchemesCacheCommand);

                // The activate scheme command.
                var activateSchemeCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.ActivateScheme);
                var activateSchemeCommand = new OleMenuCommand(OnActivateScheme, activateSchemeCommandId);
                //activateSchemeCommand.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatusActivateScheme);
                mcs.AddCommand(activateSchemeCommand);

                // The history scheme command.
                var historyCommandId = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SchemesHistory);
                var historyPageCommand = new OleMenuCommand(OnSchemesHistory, historyCommandId);
                mcs.AddCommand(historyPageCommand);

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

            Model.CheckCache();
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

        private void OnRefreshSchemesCache(object sender, EventArgs e)
        {
            Model.RefreshCache();
        }

        private void OnActivateScheme(object sender, EventArgs e)
        {
            Model.ActivateScheme();
        }

        //private void OnBeforeQueryStatusActivateScheme(object sender, EventArgs e)
        //{
        //    var command = sender as OleMenuCommand;

        //    if (null != command)
        //    {
        //        command.Enabled = SchemesOverviewModel.IsSchemeSelected;
        //    }
        //}

        private void OnSchemesHistory(object sender, EventArgs e)
        {
            
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

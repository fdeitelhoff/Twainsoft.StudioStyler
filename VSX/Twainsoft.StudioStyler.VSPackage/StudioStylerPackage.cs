using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Data;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.Services.StudioStyles.Cache;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;
using Twainsoft.StudioStyler.VSPackage.GUI;
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
        private SchemeCache SchemeCache { get; set; }
        private PagedCollectionView PagedSchemesView { get; set; }
        private string CurrentSearchString { get; set; }
        private List<string> SearchValues { get; set; }

        public StudioStylerPackage()
        {
            SchemeCache = SchemeCache.Instance;
            PagedSchemesView = PagedCollectionView.NewInstance(SchemeCache.Schemes);
            PagedSchemesView.PageSize = 15;

            CurrentSearchString = "";
            SearchValues = new List<string>();
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
            }

            SchemeCache.Check();
        }

        private void OnSearchStringValues(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                var vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(SearchValues.ToArray(), vOut);
                }
            }
        }

        private void OnSearchString(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                var input = eventArgs.InValue;

                var vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(CurrentSearchString, vOut);
                }

                else if (input != null)
                {
                    CurrentSearchString = input.ToString();

                    if (!SearchValues.Contains(CurrentSearchString))
                    {
                        SearchValues.Add(CurrentSearchString);
                    }

                    //VsMessageBox.ShowInfoMessageBox("Combobox input", input.ToString());
                    if (!String.IsNullOrWhiteSpace(input.ToString().Trim()))
                    {
                        PagedSchemesView.Filter += delegate(object obj)
                        {
                            var scheme = obj as Scheme;

                            if (scheme == null)
                            {
                                return false;
                            }

                            return scheme.Title.ToLower().Contains(input.ToString().Trim());
                        };
                    }
                    else
                    {
                        PagedSchemesView.Filter = null;
                    }
                }
            }
        }

        private async void OnRefreshSchemesCache(object sender, EventArgs e)
        {
            await SchemeCache.Refresh();
        }
    }
}

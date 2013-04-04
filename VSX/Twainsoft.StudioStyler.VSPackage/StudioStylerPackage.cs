using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.VSPackage.GUI;

namespace Twainsoft.StudioStyler.VSPackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SchemesOverviewWindow))]
    [Guid(GuidList.guidTwainsoft_StudioStyler_VSPackagePkgString)]
    public sealed class StudioStylerPackage : Package
    {
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
            if ( null != mcs )
            {
                var toolWindowCommandId = new CommandID(GuidList.guidTwainsoft_StudioStyler_VSPackageCmdSet, (int)PkgCmdIDList.cmdidShowStudioStyles);
                var menuToolWin = new MenuCommand(ShowToolWindow, toolWindowCommandId);
                mcs.AddCommand( menuToolWin );
            }
        }
    }
}

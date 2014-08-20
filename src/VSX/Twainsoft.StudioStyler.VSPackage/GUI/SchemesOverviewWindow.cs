using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.VSPackage.VSX;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    [Guid("4d08b107-07d1-421d-8675-62653546cc32")]
    public class SchemesOverviewWindow : ToolWindowPane
    {
        public SchemesOverviewWindow() :
            base(null)
        {
            Caption = Resources.Resources.ToolWindowTitle;

            BitmapResourceID = 301;
            BitmapIndex = 1;

            ToolBar = new CommandID(GuidList.GuidSchemesToolbarCmdSet, CommandIds.SchemesToolbarId);

            base.Content = new StudioStyles();
        }
    }
}

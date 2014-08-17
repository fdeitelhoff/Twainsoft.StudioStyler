using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
    public static class StatusBar
    {
        public static void Update(string text)
        {
            var statusBar = Package.GetGlobalService(typeof(SVsStatusbar)) as IVsStatusbar;

            if (statusBar != null)
            {
                var frozen = 0;

                statusBar.IsFrozen(out frozen);

                if (frozen == 0)
                {
                    statusBar.SetText(text);
                }
            }
        }
    }
}

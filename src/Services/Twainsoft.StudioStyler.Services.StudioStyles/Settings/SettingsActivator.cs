using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Settings
{
    public class SettingsActivator
    {
        public bool LoadScheme(string file)
        {
            var profileDataManager = (IVsProfileDataManager)Package.GetGlobalService(typeof(SVsProfileDataManager));

            IVsProfileSettingsTree settingsTree;
            IVsProfileSettingsFileInfo settingsFileInfo;
            IVsProfileSettingsFileCollection settingsFileCollection;

            profileDataManager.GetSettingsFiles((uint)__VSPROFILELOCATIONS.PFL_Other, out settingsFileCollection);

            settingsFileCollection.AddBrowseFile(file, out settingsFileInfo);

            settingsFileInfo.GetSettingsForImport(out settingsTree);

            settingsTree.SetEnabled(1, 1);

            var success = false;

            IVsSettingsErrorInformation errorInformation;
            var returnCode = profileDataManager.ImportSettings(settingsTree, out errorInformation);

            var ex = Marshal.GetExceptionForHR(returnCode);

            if (ex != null)
            {
                // TODO: The errors needs to be used, not ignored.
                var pnErrors = 0;

                errorInformation.GetErrorCount(out pnErrors);
            }
            else
            {
                success = true;

                File.Delete(file);
            }

            return success;
        }
    }
}

using Microsoft.VisualStudio.Shell;
using Twainsoft.StudioStyler.VSPackage.GUI.Options;

namespace Twainsoft.StudioStyler.VSPackage.Model
{
    public interface IModel
    {
        OptionsStore OptionsStore { get; set; }
        bool IsItemSelected { get; }
        void CheckCache();
        void SearchTerm(OleMenuCmdEventArgs eventArgs);
        void Search(OleMenuCmdEventArgs eventArgs);
        void ClearSearch();
        void RefreshCache();
        void ActivateScheme();
        void FirstPage();
        void PreviousPage();
        void NextPage();
        void LastPage();
    }
}

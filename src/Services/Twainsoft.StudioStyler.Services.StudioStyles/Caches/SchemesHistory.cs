using System.Collections.Generic;
using System.Collections.ObjectModel;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Caches
{
    public class SchemesHistory
    {
        private Dictionary<Scheme, History> Schemes { get; set; }
        public ObservableCollection<History> History { get; private set; }

        public SchemesHistory()
        {
            Schemes = new Dictionary<Scheme, History>();
            History = new ObservableCollection<History>();
        }

        public void Add(Scheme scheme)
        {
            if (!Schemes.ContainsKey(scheme))
            {
                var history = new History(scheme);

                Schemes.Add(scheme, history);
                History.Add(history);
            }
            else
            {
                Schemes[scheme].Activations++;
            }
        }
    }
}

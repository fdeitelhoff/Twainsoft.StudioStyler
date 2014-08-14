using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Caches
{
    public class SchemesHistory
    {
        private Dictionary<Scheme, History> Schemes { get; set; }
        public ObservableCollection<History> History { get; private set; }

        private string HistoryDataPath { get; set; }
        private string HistoryCacheFile { get; set; }

        public SchemesHistory()
        {
            Schemes = new Dictionary<Scheme, History>();
            History = new ObservableCollection<History>();

            HistoryDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("Twainsoft", "StudioStyler"));

            HistoryCacheFile = "SchemesHistory.xml";
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

            SerializeSchemesHistory();
        }

        // In this check, we need to populate the schemes Dictionary too!!

        //public void Check()
        //{
        //    var schemesCachePath = Path.Combine(HistoryDataPath, HistoryCacheFile);

        //    try
        //    {
        //        if (File.Exists(schemesCachePath))
        //        {
        //            var lastWriteTime = File.GetLastWriteTime(schemesCachePath);
        //        }
        //        else
        //        {
        //        }
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        File.Delete(schemesCachePath);
        //    }
        //    catch (XmlException)
        //    {
        //        File.Delete(schemesCachePath);
        //    }
        //}

        private void SerializeSchemesHistory()
        {
            var histories = new Histories { AllHistories = History };

            var file = Path.Combine(HistoryDataPath, HistoryCacheFile);

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(History));
                xmlSerializer.Serialize(fileStream, histories);
            }
        }
    }
}

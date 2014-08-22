using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private static SchemesHistory instance;

        public static SchemesHistory Instance
        {
            get { return instance ?? (instance = new SchemesHistory()); }
        }

        private SchemesHistory()
        {
            Schemes = new Dictionary<Scheme, History>();
            History = new ObservableCollection<History>();

            HistoryDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("Twainsoft", "StudioStyler"));

            HistoryCacheFile = "SchemesHistory.xml";

            Check();
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

        public void Check()
        {
            var historyFile = Path.Combine(HistoryDataPath, HistoryCacheFile);

            try
            {
                if (File.Exists(historyFile))
                {
                    DeserializeSchemsHistory();
                }
            }
            catch (InvalidOperationException)
            {
                File.Delete(historyFile);
            }
            catch (XmlException)
            {
                File.Delete(historyFile);
            }
        }

        private void DeserializeSchemsHistory()
        {
            using (var streamReader = new StreamReader(Path.Combine(HistoryDataPath, HistoryCacheFile)))
            {
                var xmlSerializer = new XmlSerializer(typeof(Histories));
                var histories = xmlSerializer.Deserialize(streamReader) as Histories;

                if (histories != null)
                {
                    Schemes.Clear();
                    History.Clear();
                    foreach (var history in histories.AllHistories)
                    {
                        Schemes.Add(history.Scheme, history);
                        History.Add(history);
                    }
                }
            }
        }

        private void SerializeSchemesHistory()
        {
            var histories = new Histories { AllHistories = History };

            var historyFile = Path.Combine(HistoryDataPath, HistoryCacheFile);

            try
            {
                using (var fileStream = new FileStream(historyFile, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof (Histories));
                    xmlSerializer.Serialize(fileStream, histories);
                }
            }
            catch (InvalidOperationException)
            {
                File.Delete(historyFile);
            }
            catch (XmlException)
            {
                File.Delete(historyFile);
            }
        }
    }
}

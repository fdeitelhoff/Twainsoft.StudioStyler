using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    [XmlRoot("SchemesHistory")]
    public class Histories
    {
        [JsonProperty(PropertyName = "histories")]
        [XmlArray("History")]
        [XmlArrayItem("History", typeof(History))]
        public ObservableCollection<History> AllHistories { get; set; }
    }
}

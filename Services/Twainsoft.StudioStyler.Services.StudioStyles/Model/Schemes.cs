using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    [XmlRoot("SchemeCache")]
    public class Schemes
    {
        [JsonProperty(PropertyName = "schemes")]
        [XmlArray("Schemes")]
        [XmlArrayItem("Scheme", typeof(Scheme))]
        public ObservableCollection<Scheme> AllSchemes { get; set; }
    }
}

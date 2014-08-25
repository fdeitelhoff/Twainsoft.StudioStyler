using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    [XmlRoot("SchemeCache")]
    public class Schemes
    {
        [JsonProperty(PropertyName = "imagesFinished")]
        public bool ImagesFinished { get; set; }

        [JsonProperty(PropertyName = "lastRefresh")]
        public DateTime LastRefresh { get; set; }

        [JsonProperty(PropertyName = "schemes")]
        [XmlArray("Schemes")]
        [XmlArrayItem("Scheme", typeof(Scheme))]
        public ObservableCollection<Scheme> AllSchemes { get; set; }
    }
}

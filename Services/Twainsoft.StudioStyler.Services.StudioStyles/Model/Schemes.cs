using System.Collections.Generic;
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
        public List<Scheme> AllSchemes { get; set; }
    }
}

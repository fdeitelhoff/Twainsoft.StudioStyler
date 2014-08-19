using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    public class History
    {
        [JsonProperty(PropertyName = "scheme")]
        [XmlElement("Scheme")]
        public Scheme Scheme { get; set; }

        [JsonProperty(PropertyName = "activations")]
        [XmlElement("Activations")]
        public int Activations { get; set; }

        public History()
        { }

        public History(Scheme scheme)
        {
            Scheme = scheme;
            Activations = 1;
        }
    }
}

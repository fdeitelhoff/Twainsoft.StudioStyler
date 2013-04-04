using System;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    [Serializable]
    public class Scheme
    {
        [JsonProperty(PropertyName = "title")]
        [XmlElement("Title")]
        public string Title { get; set; }

        [XmlElement("Path")]
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [XmlElement("DownloadPath")]
        [JsonProperty(PropertyName = "downloadpath")]
        public string DownloadPath { get; set; }

        [XmlElement("Author")]
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [XmlElement("Submitted")]
        [JsonProperty(PropertyName = "submitted")]
        public DateTime Submitted { get; set; }

        [XmlElement("Popularity")]
        [JsonProperty(PropertyName = "popularity")]
        public double Popularity { get; set; }

        [XmlElement("Rating")]
        [JsonProperty(PropertyName = "rating")]
        public int Rating { get; set; }

        [XmlElement("Downloads")]
        [JsonProperty(PropertyName = "downloads")]
        public int Downloads { get; set; }

        [XmlElement("Preview")]
        [JsonIgnore]
        public BitmapSource Preview { get; set; }
    }
}

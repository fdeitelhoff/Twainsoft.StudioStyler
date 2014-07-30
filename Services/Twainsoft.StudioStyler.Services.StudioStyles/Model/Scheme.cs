using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Twainsoft.StudioStyler.Services.StudioStyles.Annotations;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    [Serializable]
    public sealed class Scheme : INotifyPropertyChanged
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

        [JsonIgnore]
        private BitmapSource preview;

        [JsonIgnore]
        public BitmapSource Preview {
            get
            {
                return preview;
            }
            set
            {
                preview = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [XmlElement("ImageDownloadTried")]
        [JsonProperty(PropertyName = "imageDownloadTried")]
        public bool ImageDownloadTried { get; set; }
    }
}

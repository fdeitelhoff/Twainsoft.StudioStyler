using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Twainsoft.StudioStyler.Services.StudioStyles.Annotations;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Cache
{
    public sealed class SchemeCache : INotifyPropertyChanged
    {
        private bool isCacheValid;
        private bool isCacheRefreshing;

        private readonly string schemesCacheFile;

        private StudioStyles StudioStyles { get; set; }

        public ObservableCollection<Scheme> Schemes { get; private set; }
        
        private string SchemesDataPath { get; set; }

        //public Action SchemesLoaded;

        public bool IsCacheValid
        {
            get { return isCacheValid; }
            set
            {
                if (value.Equals(isCacheValid)) return;
                isCacheValid = value;
                OnPropertyChanged();
            }
        }

        public bool IsCacheRefreshing
        {
            get { return isCacheRefreshing; }
            set
            {
                if (value.Equals(isCacheRefreshing)) return;
                isCacheRefreshing = value;
                OnPropertyChanged();
            }
        }

        public SchemeCache()
        {
            IsCacheValid = false;
            IsCacheRefreshing = false;

            Schemes = new ObservableCollection<Scheme>();

            StudioStyles = new StudioStyles();

            SchemesDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("Twainsoft", "StudioStyler"));

            if (!Directory.Exists(SchemesDataPath))
            {
                Directory.CreateDirectory(SchemesDataPath);
            }

            schemesCacheFile = "SchemesCache.xml";
        }

        public async Task Refresh()
        {
            IsCacheRefreshing = true;

            var schemes = await StudioStyles.AllAsync();

            //foreach (var scheme in Schemes)
            //{
            //    var png = await StudioStyles.Preview(scheme.Title);

            //    //var decoder = new PngBitmapDecoder(new MemoryStream(png), BitmapCreateOptions.PreservePixelFormat,
            //    //                                   BitmapCacheOption.OnLoad);
            //    //scheme.Preview = decoder.Frames[0];
            //    //CreateBitmapSourceFromBitmap(new Bitmap(Bitmap.FromStream(new MemoryStream(png))));

            //    var image = new BitmapImage();
            //    image.BeginInit();
            //    image.StreamSource = new MemoryStream(png);
            //    image.EndInit();

            //    scheme.Preview = image;
            //}

            SeserializeCachedSchemes();

            Schemes.Clear();
            foreach (var scheme in schemes)
            {
                Schemes.Add(scheme);
            }

            IsCacheValid = true;
            IsCacheRefreshing = false;
            
            //return Schemes;
        }

        public void Check()
        {
            try
            {
                if (File.Exists(Path.Combine(SchemesDataPath, schemesCacheFile)))
                {
                    var lastWriteTime = File.GetLastWriteTime(Path.Combine(SchemesDataPath, schemesCacheFile));

                    IsCacheValid = DateTime.Now.Subtract(lastWriteTime).Days <= 3;
                    IsCacheRefreshing = true;

                    DeserializeCachedSchemes();

                    IsCacheRefreshing = false;

                    //if (SchemesLoaded != null)
                    //{
                    //    SchemesLoaded();
                    //}
                }
                else
                {
                    IsCacheValid = false;
                }
            }
            catch (XmlException)
            {
                IsCacheValid = false;
                isCacheRefreshing = false;
            }
        }

        private void SeserializeCachedSchemes()
        {
            var schemes = new Schemes {AllSchemes = Schemes};

            var file = Path.Combine(SchemesDataPath, schemesCacheFile);

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof (Schemes));
                xmlSerializer.Serialize(fileStream, schemes);
            }
        }

        private void DeserializeCachedSchemes()
        {
            using (var streamReader = new StreamReader(Path.Combine(SchemesDataPath, schemesCacheFile)))
            {
                var xmlSerializer = new XmlSerializer(typeof (Schemes));
                var schemes = xmlSerializer.Deserialize(streamReader) as Schemes;

                if (schemes != null)
                {
                    Schemes.Clear();
                    foreach (var scheme in schemes.AllSchemes)
                    {
                        Schemes.Add(scheme);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

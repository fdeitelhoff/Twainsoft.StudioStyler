using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using Twainsoft.StudioStyler.Services.StudioStyles.Annotations;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;
using Twainsoft.StudioStyler.Services.StudioStyles.Styles;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Caches
{
    public sealed class SchemeCache : INotifyPropertyChanged
    {
        private bool isCacheValid;
        private bool isCacheRefreshing;
        private bool isImageCachRefreshing;
        private int currentSchemeNumber;

        private readonly string schemesCacheFile;

        // TODO: The StudioStyles class musst be renamed due to naming conflicts. StudioStylesService maybe?
        private StudioStylesService StudioStyles { get; set; }

        public ObservableCollection<Scheme> Schemes { get; private set; }
        
        private string SchemesDataPath { get; set; }
        private string SchemesPreviewPath { get; set; }

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

        public bool IsImageCacheRefreshing
        {
            get { return isImageCachRefreshing; }
            set
            {
                if (value.Equals(isImageCachRefreshing)) return;
                isImageCachRefreshing = value;
                OnPropertyChanged();
            }
        }

        public int CurrentSchemeNumber
        {
            get { return currentSchemeNumber; }
            set
            {
                if (value.Equals(currentSchemeNumber)) return;
                currentSchemeNumber = value;
                OnPropertyChanged();
            }
        }

        public SchemeCache()
        {
            IsCacheValid = false;
            IsCacheRefreshing = false;
            IsImageCacheRefreshing = false;
            CurrentSchemeNumber = 0;

            Schemes = new ObservableCollection<Scheme>();

            StudioStyles = new Styles.StudioStylesService();

            SchemesDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("Twainsoft", "StudioStyler"));

            if (!Directory.Exists(SchemesDataPath))
            {
                Directory.CreateDirectory(SchemesDataPath);
            }

            schemesCacheFile = "SchemesCache.xml";

            SchemesPreviewPath = Path.Combine(SchemesDataPath, "Previews");

            if (!Directory.Exists(SchemesPreviewPath))
            {
                Directory.CreateDirectory(SchemesPreviewPath);
            }
        }

        public async Task Refresh()
        {
            IsCacheRefreshing = true;

            var schemes = await StudioStyles.AllAsync();

            Schemes.Clear();
            foreach (var scheme in schemes)
            {
                Schemes.Add(scheme);
            }

            SerializeCachedSchemes(false);

            IsCacheRefreshing = false;
            IsImageCacheRefreshing = true;

            await CheckSchemes(false);

            SerializeCachedSchemes(true);

            IsCacheValid = true;
            IsImageCacheRefreshing = false;
        }

        private async Task CheckSchemes(bool imagesCompletelyLoaded)
        {
            CurrentSchemeNumber = 0;

            foreach (var scheme in Schemes)
            {
                try
                {
                    if (++CurrentSchemeNumber % 50 == 0)
                    {
                        SerializeCachedSchemes(false);
                    }

                    var file = Path.Combine(SchemesPreviewPath, TransformTitle(scheme.Title) + ".png");

                    var image = new BitmapImage();
                    image.BeginInit();

                    if (File.Exists(file))
                    {
                        image.StreamSource = new FileStream(file, FileMode.Open, FileAccess.Read);
                        image.EndInit();

                        scheme.Preview = image;

                        scheme.ImagePresent = true;
                        scheme.ImageDownloadTried = true;
                    }
                    else if (!imagesCompletelyLoaded && !scheme.ImageDownloadTried && !scheme.ImagePresent)
                    {
                        var png = await StudioStyles.Preview(scheme.Title);

                        // Hier noch die DecodePixelWith setzen. Braucht wohl nicht so viel Speicher.
                        image.StreamSource = new MemoryStream(png);
                        image.EndInit();

                        scheme.Preview = image;

                        using (var fileStream = new FileStream(file, FileMode.Create))
                        {
                            var encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(image));
                            encoder.Save(fileStream);
                        }

                        scheme.ImagePresent = true;
                        scheme.ImageDownloadTried = true;
                    }
                    else if (scheme.ImagePresent)
                    {
                        image.StreamSource = new FileStream(file, FileMode.Open, FileAccess.Read);
                        image.EndInit();

                        scheme.Preview = image;

                        scheme.ImagePresent = true;
                        scheme.ImageDownloadTried = true;
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e);

                    scheme.ImagePresent = false;
                    scheme.ImageDownloadTried = true;
                }
                catch (NotSupportedException e)
                {
                    Console.WriteLine(e);

                    scheme.ImagePresent = false;
                    scheme.ImageDownloadTried = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    scheme.ImagePresent = false;
                    scheme.ImageDownloadTried = true;
                }
            }
        }

        private string TransformTitle(string title)
        {
            return title.Replace('<', '-')
                .Replace('>', '-')
                .Replace('?', '-')
                .Replace('"', '-')
                .Replace(':', '-')
                .Replace('|', '-')
                .Replace('\\', '-')
                .Replace('/', '-')
                .Replace('*', '-');
        }

        public async Task Check()
        {
            var schemesCachePath = Path.Combine(SchemesDataPath, schemesCacheFile);

            try
            {
                if (File.Exists(schemesCachePath))
                {
                    var lastWriteTime = File.GetLastWriteTime(schemesCachePath);

                    IsCacheValid = DateTime.Now.Subtract(lastWriteTime).Days <= 5;
                    IsCacheRefreshing = true;

                    var imagesFinished = DeserializeCachedSchemes();

                    IsCacheRefreshing = false;
                    IsImageCacheRefreshing = true;

                    await CheckSchemes(imagesFinished);

                    SerializeCachedSchemes(true);

                    IsImageCacheRefreshing = false;
                }
                else
                {
                    IsCacheValid = false;
                    IsCacheRefreshing = false;
                }
            }
            catch (InvalidOperationException)
            {
                IsCacheValid = false;
                IsCacheRefreshing = false;

                File.Delete(schemesCachePath);
            }
            catch (XmlException)
            {
                IsCacheValid = false;
                IsCacheRefreshing = false;

                File.Delete(schemesCachePath);
            }
        }

        private void SerializeCachedSchemes(bool imagesFinished)
        {
            var schemes = new Schemes {AllSchemes = Schemes, ImagesFinished = imagesFinished};

            var file = Path.Combine(SchemesDataPath, schemesCacheFile);

            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof (Schemes));
                xmlSerializer.Serialize(fileStream, schemes);
            }
        }

        private bool DeserializeCachedSchemes()
        {
            using (var streamReader = new StreamReader(Path.Combine(SchemesDataPath, schemesCacheFile)))
            {
                var xmlSerializer = new XmlSerializer(typeof (Schemes));
                var schemes = xmlSerializer.Deserialize(streamReader) as Schemes;

                if (schemes == null)
                {
                    return false;
                }

                Schemes.Clear();
                foreach (var scheme in schemes.AllSchemes)
                {
                    Schemes.Add(scheme);
                }

                return schemes.ImagesFinished;
            }
        }

        // TODO: Are Schem titles are unique? Hopefully they are. Test it on the studiostyl.es homepage!
        public Scheme ByTitle(string title)
        {
            foreach (var scheme in Schemes)
            {
                if (scheme.Title == title)
                {
                    return scheme;
                }
            }

            throw new ArgumentOutOfRangeException("The Specific Style cannot be found!");
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

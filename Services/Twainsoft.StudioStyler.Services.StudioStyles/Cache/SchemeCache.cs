﻿using System;
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

namespace Twainsoft.StudioStyler.Services.StudioStyles.Cache
{
    public sealed class SchemeCache : INotifyPropertyChanged
    {
        private bool isCacheValid;
        private bool isCacheRefreshing;
        private bool isImageCachRefreshing;
        private int currentSchemeNumber;

        private readonly string schemesCacheFile;

        private StudioStyles StudioStyles { get; set; }

        public ObservableCollection<Scheme> Schemes { get; private set; }
        
        private string SchemesDataPath { get; set; }
        private string SchemesPreviewPath { get; set; }

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

            StudioStyles = new StudioStyles();

            // Das hier könnte etwas umgestellt werden. Allgemeiner Pfad und SchemeDataPath dann das konkrete Unterverzeichnis.
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

            SeserializeCachedSchemes(false);

            IsCacheRefreshing = false;
            IsImageCacheRefreshing = true;

            foreach (var scheme in schemes)
            {
                try
                {
                    var file = Path.Combine(SchemesPreviewPath, TransformTitle(scheme.Title) + ".png");

                    var image = new BitmapImage();
                    image.BeginInit();

                    if (!File.Exists(file))
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
                    }
                    else
                    {
                        image.StreamSource = new FileStream(file, FileMode.Open, FileAccess.Read);
                        image.EndInit();

                        scheme.Preview = image;
                    }

                    CurrentSchemeNumber = CurrentSchemeNumber + 1;
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e);
                }
                catch (NotSupportedException e)
                {
                    Console.WriteLine(e);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            SeserializeCachedSchemes(true);

            IsCacheValid = true;
            IsImageCacheRefreshing = false;
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

        public void Check()
        {
            var schemesCache = Path.Combine(SchemesDataPath, schemesCacheFile);

            try
            {
                if (File.Exists(Path.Combine(SchemesDataPath, schemesCacheFile)))
                {
                    var lastWriteTime = File.GetLastWriteTime(Path.Combine(SchemesDataPath, schemesCacheFile));

                    IsCacheValid = DateTime.Now.Subtract(lastWriteTime).Days <= 3;
                    IsCacheRefreshing = true;

                    DeserializeCachedSchemes();

                    foreach (var scheme in Schemes)
                    {
                        try
                        {
                            var file = Path.Combine(SchemesPreviewPath, TransformTitle(scheme.Title) + ".png");

                            if (File.Exists(file))
                            {
                                var image = new BitmapImage();
                                image.BeginInit();
                                image.StreamSource = new FileStream(file, FileMode.Open, FileAccess.Read);
                                image.EndInit();

                                scheme.Preview = image;
                            }

                            CurrentSchemeNumber = CurrentSchemeNumber + 1;
                        }
                        catch (InvalidOperationException e)
                        {
                            Console.WriteLine(e);
                        }
                        catch (NotSupportedException e)
                        {
                            Console.WriteLine(e);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

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
            catch (InvalidOperationException)
            {
                IsCacheValid = false;
                IsCacheRefreshing = false;

                File.Delete(schemesCache);
            }
            catch (XmlException)
            {
                IsCacheValid = false;
                IsCacheRefreshing = false;

                File.Delete(schemesCache);
            }
        }

        private void SeserializeCachedSchemes(bool imagesFinished)
        {
            var schemes = new Schemes {AllSchemes = Schemes, ImagesFinished = imagesFinished};

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

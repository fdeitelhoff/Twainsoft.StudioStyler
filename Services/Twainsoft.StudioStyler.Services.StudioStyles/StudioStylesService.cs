using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;

namespace Twainsoft.StudioStyler.Services.StudioStyles
{
    public class StudioStylesService
    {
        private string BaseUrl { get; set; }

        private IEnumerable<Scheme> Schemes { get; set; }

        public delegate void SchemeSucessfullyDownloadedEventHandler(object sender, string file);
        public event SchemeSucessfullyDownloadedEventHandler SchemeSucessfullyDownloaded;

        public StudioStylesService()
        {
            BaseUrl = "http://studiostyl.es/";
        }

        public IEnumerable<Scheme> All()
        {
            if (Schemes == null)
            {
                var client = new RestClient(BaseUrl);
                var request = new RestRequest("api/schemes", Method.GET);

                var response = client.Execute(request);

                Schemes = JsonConvert.DeserializeObject<Schemes>(response.Content).AllSchemes;

            }

            return Schemes;
        }

        public IList<Scheme> Range(int from, int until)
        {
            if (Schemes == null)
            {
                Schemes = All();
            }
            
            var schemes = new List<Scheme>();

            var enumerable = Schemes as IList<Scheme> ?? Schemes.ToList();

            for (var i = from - 1; i < until; i++)
            {
                schemes.Add(enumerable[i]);
            }

            return schemes;
        }

        public void Download(string path)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(path);

            var data = client.DownloadData(request);

            SaveData(data);
        }

        private void SaveData(byte[] data)
        {
            var file = Path.GetTempFileName();

            using (var fileStream = new FileStream(file, FileMode.OpenOrCreate))
            {
                fileStream.Write(data, 0, data.Length);
            }

            if (File.Exists(file))
            {
                OnSchemeDownloadedSucessfully(file);
            }
        }

        private void OnSchemeDownloadedSucessfully(string file)
        {
            if (SchemeSucessfullyDownloaded != null)
            {
                SchemeSucessfullyDownloaded(this, file);
            }
        }
    }
}

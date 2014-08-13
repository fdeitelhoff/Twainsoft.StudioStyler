using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Styles
{
    public class StudioStyles
    {
        private string BaseUrl { get; set; }

        public StudioStyles()
        {
            BaseUrl = "http://studiostyl.es/";
        }

        public async Task<ObservableCollection<Scheme>> AllAsync()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("api/schemes", Method.GET);

            var taskCompletionSource = new TaskCompletionSource<ObservableCollection<Scheme>>();

            client.ExecuteAsync(request, response => taskCompletionSource.SetResult(JsonConvert.DeserializeObject<Schemes>(response.Content).AllSchemes));

            return await taskCompletionSource.Task;
        }

        public async Task<byte[]> Preview(string title)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(string.Format("schemes/{0}/snippet.png", title.Replace(' ', '-')), Method.GET);

            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            client.ExecuteAsync(request, response => taskCompletionSource.SetResult(response.RawBytes));

            return await taskCompletionSource.Task;
        }

        public async Task<string> DownloadAsync(string path)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(path, Method.GET);

            var taskCompletionSource = new TaskCompletionSource<string>(); 
            client.ExecuteAsync(request, response => taskCompletionSource.SetResult(response.Content));

            return await SaveData(taskCompletionSource.Task.Result);
        }

        private async Task<string> SaveData(string content)
        {
            var file = Path.GetTempFileName();

            using (var streamWriter = new StreamWriter(file, false))
            {
                await streamWriter.WriteAsync(content);
            }

            var taskCompletionSource = new TaskCompletionSource<string>(); 
            taskCompletionSource.SetResult(file);

            return await taskCompletionSource.Task;
        }
    }
}

// Services/JsonContentService.cs
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class JsonContentService : IJsonContentService
    {
        private readonly string _rootPath;
        private readonly object _lock = new();

        public JsonContentService(IHostEnvironment env)
        {
            _rootPath = env.ContentRootPath;
        }

        // 1) Parametresiz overload: "content.json" kullanır
        public Task<Dictionary<string, string>> LoadAsync()
            => LoadAsync("content.json");

        public Task SaveAsync(Dictionary<string, string> data)
            => SaveAsync("content.json", data);

        public async Task<Dictionary<string, string>> LoadAsync(string fileName)
        {
            var path = Path.Combine(_rootPath, fileName);
            if (!File.Exists(path))
                return new Dictionary<string,string>();

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<Dictionary<string,string>>(json)
                   ?? new Dictionary<string,string>();
        }

        public Task SaveAsync(string fileName, Dictionary<string, string> data)
        {
            var path = Path.Combine(_rootPath, fileName);
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions {
                    WriteIndented = true
                });
                File.WriteAllText(path, json);
            }
            return Task.CompletedTask;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Patient_Enquiry_System_version_2.Services
{
    public class TelegramService
    {
        private readonly HttpClient _httpClient;

        public TelegramService()
        {
            _httpClient = new HttpClient();
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            var botToken = ConfigurationManager.AppSettings["TelegramBotToken"];

            var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            var payload = new
            {
                chat_id = chatId,
                text = message
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(url, content);
        }
        public async Task<string> DownloadFileAsync(string fileId, string saveFolderPath)
        {
            var botToken = ConfigurationManager.AppSettings["TelegramBotToken"];

            // Step 1 - get file path from Telegram
            var getFileUrl = $"https://api.telegram.org/bot{botToken}/getFile?file_id={fileId}";
            var response = await _httpClient.GetStringAsync(getFileUrl);
            var fileInfo = JsonConvert.DeserializeObject<dynamic>(response);
            string filePath = fileInfo.result.file_path;

            // Step 2 - download the actual file
            var downloadUrl = $"https://api.telegram.org/file/bot{botToken}/{filePath}";
            var fileBytes = await _httpClient.GetByteArrayAsync(downloadUrl);

            // Step 3 - save to server folder
            var fileName = $"{Guid.NewGuid()}_{System.IO.Path.GetFileName(filePath)}";
            var fullPath = System.IO.Path.Combine(saveFolderPath, fileName);
            System.IO.File.WriteAllBytes(fullPath, fileBytes);

            // return relative URL for storing in DB
            return $"/uploads/files/{fileName}";
        }
    }
}
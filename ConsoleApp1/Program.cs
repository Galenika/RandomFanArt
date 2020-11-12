using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> post = PostImage(ChooseFile());
            post.Wait();
            using (StreamWriter sw = new StreamWriter("response.json"))
            {
                sw.Write(post.Result);
            }
        }

        private static async Task<string> PostImage(FileInfo chosenFile)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent("Random fanart: " + chosenFile.Name.Substring(0, chosenFile.Name.Length - 4)), "content");
            form.Add(new ByteArrayContent(File.ReadAllBytes(chosenFile.FullName)), "file", chosenFile.Name);
            HttpResponseMessage response = await httpClient.PostAsync($"https://discordapp.com/api/webhooks/768447672758566919/{Environment.GetEnvironmentVariable("wolfbros_token", EnvironmentVariableTarget.User)}", form);
            httpClient.Dispose();
            try
            {
                return (await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                return $"{ex.Message}{Environment.NewLine}From: {ex.Source}";
            }
        }

        static FileInfo ChooseFile()
        {
            Random gen = new Random();
            string tumblrPath = @"D:\Google_Drive\WD_lis-media\le-louvre-de-lis2";
            string googlePath = @"D:\Google_Drive\WD_lis-media";
            string chosenPath;
            if (gen.Next(2) == 1)
                chosenPath = tumblrPath;
            else
                chosenPath = googlePath;
            DirectoryInfo directoryInfo = new DirectoryInfo(chosenPath);
            FileInfo[] files = directoryInfo.GetFiles();
            return files[gen.Next(files.Length)];
        }
    }
}

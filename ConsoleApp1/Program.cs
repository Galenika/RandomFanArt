using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace WebhookMadness
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {

                switch (args[0].ToLower())
                {
                    case "--fanart":
                        Task<string> postArt = PostImage(ChooseFile());
                        postArt.Wait();
                        using (StreamWriter sw = new StreamWriter("fanart_response.json"))
                        {
                            sw.Write(postArt.Result);
                        }
                        break;
                    case "--meme":
                        Task<string> postMeme = PostImage();
                        postMeme.Wait();
                        using (StreamWriter sw = new StreamWriter("meme_response.json"))
                        {
                            sw.Write(postMeme.Result);
                        }
                        break;

                }
            }
            else
            {
                Task<string> postArt = PostImage(ChooseFile());
                postArt.Wait();
                using (StreamWriter sw = new StreamWriter("fanart_response.json"))
                {
                    sw.Write(postArt.Result);
                }

                Task<string> postMeme = PostImage();
                postMeme.Wait();
                using (StreamWriter sw = new StreamWriter("meme_response.json"))
                {
                    sw.Write(postMeme.Result);
                }
            }
        }

        private static async Task<string> PostImage(FileInfo chosenFile)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent
            {
                { new StringContent("Random fanart: " + chosenFile.Name.Substring(0, chosenFile.Name.Length - 4)), "content" },
                { new ByteArrayContent(File.ReadAllBytes(chosenFile.FullName)), "file", chosenFile.Name }
            };
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

        private static async Task<string> PostImage()
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            string imgAddr;
            HttpResponseMessage finalPost = null;
            try
            {
                HttpResponseMessage response = (await httpClient.GetAsync(@"https://inspirobot.me/api?generate=true")).EnsureSuccessStatusCode();
                imgAddr = await response.Content.ReadAsStringAsync();
                byte[] memeData = await httpClient.GetByteArrayAsync(imgAddr);
                if (memeData != null)
                {
                    form.Add(new StringContent("InspiroBot says..."), "content");
                    form.Add(new ByteArrayContent(memeData), "file", "tempMeme.jpg"); //Filename is requred for the request to be valid.
                    finalPost = await httpClient.PostAsync($"https://discord.com/api/webhooks/778858705197203467/{Environment.GetEnvironmentVariable("dnd_token", EnvironmentVariableTarget.User)}", form);
                    httpClient.Dispose();
                }
                else
                {
                    finalPost = new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
                    finalPost.Content = new StringContent("Error loading image into byte array!");
                }
                
                return (await finalPost.EnsureSuccessStatusCode().Content.ReadAsStringAsync());
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

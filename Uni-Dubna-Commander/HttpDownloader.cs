using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Uni_Dubna_Commander
{
    public class HttpDownloader
    {
        public struct FileInfoJson
        {
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public string fileName { set; get; }

            // ReSharper disable once InconsistentNaming
            // ReSharper disable once UnusedMember.Global
            public string errorMessage { set; get; }
        }

        private readonly Login _login;

        private readonly HttpClient _client = new HttpClient();

        public HttpDownloader(Login login) => _login = login;

        public async Task<Dictionary<string, Item>> GetItemsInDirectory(Dir dir)
        {
            var ret = new Dictionary<string, Item>();

            var doc = new HtmlDocument();
            doc.LoadHtml(
                await (await _client.PostAsync(
                    $"https://www.uni-dubna.ru/LK/GetFtpFiles?address=ftp://10.210.50.199/{dir.FullName.Substring(dir.FullName.IndexOf('\\') + 1)}&login={_login.User}&password={_login.Password}",
                    null)).Content.ReadAsStringAsync());

            foreach (var file in doc.DocumentNode.SelectNodes("//tr[contains(@class,'ftpfile')]"))
            {
                var isFile = file.SelectSingleNode(".//td [contains(@class,'ftpfiletype')]").InnerText
                                 .IndexOf("Папка", StringComparison.Ordinal) == -1;
                var fileName = file.SelectSingleNode(".//td [contains(@class,'ftpfilename')]").InnerText.Trim();
                var size = file.SelectSingleNode(".//td [contains(@class,'ftpfilesize')]").InnerText;

                Item item;

                if (isFile)
                {
                    item = new File(dir.FullName + '\\' + fileName, Convert.ToInt64(size));
                }
                else
                {
                    item = new Dir(dir.FullName + '\\' + fileName);
                }

                ret[item.SplitPath.Last()] = item;
            }

            return ret;
        }

        public async Task<Stream> DownLoadFile(File dir)
        {
            var fileInfo = JsonConvert.DeserializeObject<FileInfoJson>(await (await _client.PostAsync(
                $"https://www.uni-dubna.ru/LK/DownloadFtpFile?address=ftp://10.210.50.199/{dir.FullName.Substring(0, dir.FullName.LastIndexOf('\\'))}&name={dir.SplitPath.Last()}&login={_login.User}&password={_login.Password}",
                null)).Content.ReadAsStringAsync());

            return new GZipStream(
                await (await _client.GetAsync($"https://www.uni-dubna.ru/LK/Download/?file={fileInfo.fileName}"))
                    .Content.ReadAsStreamAsync(), CompressionMode.Decompress);
        }
    }
}
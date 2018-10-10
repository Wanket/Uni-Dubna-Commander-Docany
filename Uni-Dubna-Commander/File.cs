using System.IO;
using System.Threading;

namespace Uni_Dubna_Commander
{
    public class File : Item
    {
        public File(string path, long size) : base(path) => FileInformation.Length = size;

        public Stream GetMemoryStream(Login login)
        {
            if (_data != null)
            {
                return new MemoryStream(_data, false);
            }

            if (_mutex.WaitOne())
            {
                var ms = new MemoryStream();
                new HttpDownloader(login).DownLoadFile(this).Result.CopyTo(ms);
                _data = ms.ToArray();
            }

            _mutex.Close();

            return new MemoryStream(_data, false);
        }

        private readonly Mutex _mutex = new Mutex();

        private byte[] _data;
    }
}
using System;
using System.Linq;
using System.Threading;

namespace Uni_Dubna_Commander
{
    public class FileSystem : Dir
    {
        private readonly HttpDownloader _downloader;

        private readonly Mutex _mutex = new Mutex();

        public FileSystem(Login login) : base("\\")
        {
            _downloader = new HttpDownloader(login);
            Childs = _downloader.GetItemsInDirectory(this).Result;
        }

        public Item Get(Item item)
        {
            Dir dir = this;
            for (var i = 1; i < item.SplitPath.Length - 1; ++i)
            {
                if (dir.Childs == null)
                {
                    UpdateChilds(dir);
                }

                if (dir.Childs.TryGetValue(item.SplitPath[i], out var findetItem) && findetItem is Dir findetDir)
                {
                    dir = findetDir;
                }
                else
                {
                    throw new ArgumentException("Item not found");
                }
            }

            return item.SplitPath.Last() == "" ? this : dir.Childs[item.SplitPath.Last()];
        }

        public void UpdateChilds(Dir dir)
        {
            if (_mutex.WaitOne())
            {
                dir.Childs = _downloader.GetItemsInDirectory(dir).Result;
            }

            _mutex.ReleaseMutex();
        }
    }
}
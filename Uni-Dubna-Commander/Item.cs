using System.IO;
using System.Linq;
using DokanNet;

namespace Uni_Dubna_Commander
{
    public class Item
    {
        public FileInformation FileInformation;

        public string FullName { get; }

        public string[] SplitPath { get; }
        
        public Item(string path)
        {
            FullName = path;
            SplitPath = path.Split('\\');
            FileInformation = new FileInformation
            {
                FileName = SplitPath.LastOrDefault(),
                Attributes = FileAttributes.ReadOnly
            };
        }
    }
}
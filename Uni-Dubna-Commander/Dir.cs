using System.Collections.Generic;
using System.IO;

namespace Uni_Dubna_Commander
{
    public class Dir : Item
    {
        public Dir(string path) : base(path) => FileInformation.Attributes |= FileAttributes.Directory;

        public Dictionary<string, Item> Childs;
    }
}
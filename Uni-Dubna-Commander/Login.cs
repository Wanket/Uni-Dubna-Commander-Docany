using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uni_Dubna_Commander
{
    public struct Login
    {
        public string User { get; }

        public string Password { get; }

        public Login(string user, string password)
        {
            User = user;
            Password = password;
        }

        public async Task<bool> CheckLogin() => (await new HttpDownloader(this).GetItemsInDirectory(new Dir("\\"))).ContainsKey("Personal");
    }
}
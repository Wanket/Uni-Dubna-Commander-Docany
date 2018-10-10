using System;
using DokanNet;

namespace Uni_Dubna_Commander
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Login: ");
            var userName = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            var login = new Login(userName, password);

            try
            {
                if (!login.CheckLogin().Result)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Wrong login or password");
                Console.Read();
                return;
            }

            var dokanOperation = new DokanOperations(login);
            dokanOperation.Mount("n:\\");
        }
    }
}
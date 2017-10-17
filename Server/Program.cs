using System;
using Microsoft.Owin.Hosting;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:5561"))
            {
                Console.WriteLine("Server is running");
                Console.WriteLine("Press any key to stop server...");
                Console.ReadLine();
            }
        }
    }
}

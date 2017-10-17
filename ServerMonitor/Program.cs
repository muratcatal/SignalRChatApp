using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace ServerMonitor
{
    class Program
    {
        private static IHubProxy _hub;
        private static HubConnection _connection;

        static void Main(string[] args)
        {

            var url = "http://localhost:5561";
            _connection = new HubConnection(url);

            _connection.Error += connection_Error; ;
            _connection.StateChanged += connection_StateChanged;
            _hub = _connection.CreateHubProxy("chat");
            _connection.Start().Wait();

            Console.WriteLine("You are connected with " + _connection.Transport.Name);

            Console.WriteLine("\nConnection is ready!");

            string key = "";

            do
            {
                PrintMenu();

                key = Console.ReadLine();
                switch (key)
                {
                    case "1":
                        AddBannedWord().Wait();
                        break;
                    case "2":
                        RemoveBannedWord().Wait();
                        break;
                    case "3":
                        ListBannedWords().Wait();
                        break;
                    case "4":
                        BanUser().Wait();
                        break;
                    case "5":
                        BanUserFromServer().Wait();
                        break;
                }
                Console.Clear();

            } while (key != "q");

        }

        private static void connection_Error(Exception obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error occurred on connection {obj.Message}");
        }

        private static void connection_StateChanged(StateChange obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (obj.OldState == ConnectionState.Connected && obj.NewState == ConnectionState.Reconnecting)
            {
                // after a connection problem occurred
                Console.WriteLine("Connection lost! Trying to reconnect");
            }
            else if (obj.OldState == ConnectionState.Reconnecting && obj.NewState == ConnectionState.Connected)
            {
                // after reconnection established
                Console.WriteLine("Reconnection established!");
                Console.ForegroundColor = ConsoleColor.White;
                PrintMenu();
            }
            else if (obj.OldState == ConnectionState.Connecting && obj.NewState == ConnectionState.Connected)
            {
                // first state
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static async Task ListBannedWords()
        {
            var bannedList = await _hub.Invoke<List<string>>("listBannedWords");
            Console.WriteLine(string.Join(", ", bannedList));
            Console.WriteLine("To return menu press any key...");
            Console.ReadLine();
        }

        private static async Task AddBannedWord()
        {
            Console.Clear();
            Console.Write("Enter a word to add banned list: ");
            var readLine = Console.ReadLine();
            if (readLine != null)
            {
                string word = readLine.Trim();
                var response = await _hub.Invoke<string>("addBannedWord", word);
                Console.WriteLine(response);
                Console.WriteLine("To return menu press any key...");
                Console.ReadLine();
            }
        }

        private static async Task RemoveBannedWord()
        {
            Console.Clear();
            Console.Write("Enter a word to remove from banned list: ");
            var readLine = Console.ReadLine();
            if (readLine != null)
            {
                string word = readLine.Trim();
                var response = await _hub.Invoke<string>("removeBannedWord", word);
                Console.WriteLine(response);

                Console.WriteLine("To return menu press any key...");
                Console.ReadLine();
            }
        }


        public static async Task BanUser()
        {
            try
            {
                Console.Clear();
                Console.Write("Enter user to ban: ");
                var name = Console.ReadLine();

                Console.Write("Enter channel: ");
                var channel = Console.ReadLine();

                if (name != null && channel != null)
                {
                    name = name.Trim();
                    channel = channel.Trim();

                    var response = await _hub.Invoke<string>("banUser", channel, name);
                    Console.WriteLine(response);

                    Console.WriteLine("To return menu press any key...");
                    Console.ReadLine();
                }
            }
            catch (HubException exception)
            {
                Console.WriteLine($"Error thrown from hub with message '{exception.Message}'");
                Console.WriteLine("To continue press any key...");
                Console.ReadLine();
            }
        }

        public static async Task BanUserFromServer()
        {
            Console.Clear();
            Console.Write("Enter user to ban: ");
            var name = Console.ReadLine();

            if (name != null)
            {
                name = name.Trim();
                var response = await _hub.Invoke<string>("banUserFromServer", name);
                Console.WriteLine(response);

                Console.WriteLine("To return menu press any key...");
                Console.ReadLine();
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("1. Add banned word");
            Console.WriteLine("2. Remove banned words");
            Console.WriteLine("3. List banned words");
            Console.WriteLine("4. Ban user from channel");
            Console.WriteLine("5. Ban user from server");

            Console.WriteLine("");
            Console.Write("Enter menu number:");
        }

    }
}

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GenshinLike.AP
{
    class Program
    {
        private static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 12348);

            try
            {
                Task.Run(() => m_ServerCommandMain(server));
                server.StartAndRun();
            }
            catch (Exception _ex)
            {
                server.Stop();
                Console.WriteLine(_ex.StackTrace);
                Console.WriteLine(_ex.Message);
            }
            finally
            {
                Console.WriteLine("Program Ends.");
                Console.WriteLine("PRESS ANY KEY TO EXIT.");
                Console.ReadKey(true);
            }
        }

        private static void m_ServerCommandMain(Server _server)
        {
            ConsoleKey key = 0;

            while (key != ConsoleKey.Q)
            {
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Q:
                        _server.Stop();
                        break;
                    default:
                        break;
                }
            }
        }

        public static void m_ServerTickMain(Server _server)
        {
            while (_server.IsStarted)
            {
                Console.WriteLine("client count: {0}", _server.ClientCount);
                Thread.Sleep(1000);
            }
        }
    }
}
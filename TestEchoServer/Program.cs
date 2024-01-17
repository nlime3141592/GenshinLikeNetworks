using System;

namespace GenshinLike.Net
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestEchoServer echoServer = new TestEchoServer();
            Console.WriteLine("Program Started.");
            echoServer.StartAsync(12347).Wait();
            Console.WriteLine("Program Ended.");
            Console.WriteLine("Press Any Key to Exit.");
            Console.ReadKey(true);
        }
    }
}

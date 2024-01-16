using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshinLike.Net
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestEchoServer echoServer = new TestEchoServer(5);
            Console.WriteLine("Program Started.");
            echoServer.StartAsync(12347).Wait();
            Console.WriteLine("Program Ended.");
            Console.WriteLine("Press Any Key to Exit.");
            Console.ReadKey(true);
        }
    }
}

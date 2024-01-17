using System;
using System.Threading.Tasks;

namespace GenshinLike.Net
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestEchoClient client = new TestEchoClient(5, "hello echo server.");
            client.Start("127.0.0.1", 12347).Wait();
            Console.WriteLine("Press Any Key to Exit.");
            Console.ReadKey(true);
        }
    }
}

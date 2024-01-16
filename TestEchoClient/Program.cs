using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenshinLike.Net
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 12347);
            NetworkStream stream = client.GetStream();

            try
            {
                int count = 10;

                Console.WriteLine("클라이언트를 실행합니다.");
                string sendMessage = "test message";
                byte[] sendBuffer = Encoding.ASCII.GetBytes(sendMessage);


                while (count > 0)
                {
                    stream.Write(sendBuffer, 0, sendBuffer.Length);

                    byte[] receiveBuffer = new byte[1024];
                    int receiveLength = stream.Read(receiveBuffer, 0, receiveBuffer.Length); // NOTE: 원격 호스트에서 로컬 호스트를 닫고 Read를 시도하면 Exception이 발생함.
                    string receiveMessage = Encoding.ASCII.GetString(receiveBuffer, 0, receiveLength);
                    Console.WriteLine($"Receive echo message from server. ({receiveMessage})");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }

            Console.WriteLine("Press Any Key to Exit.");
            Console.ReadKey(true);
        }
    }
}

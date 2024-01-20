using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GenshinLike.ConsoleClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 12348);
            BufferedStream bf = new BufferedStream(client.GetStream());
            bool started = true;

            bf.WriteByte(0);
            bf.Flush();
            started = (bf.ReadByte() == 0);

            Console.WriteLine("Start client.");

            while(started)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch(key)
                {
                    case ConsoleKey.D1:
                        m_Send0(bf);
                        break;
                    case ConsoleKey.D2:
                        m_Send1(bf);
                        break;
                    case ConsoleKey.D3:
                        m_Send100(bf);
                        break;
                    case ConsoleKey.D4:
                        m_Send1000(bf);
                        break;
                    case ConsoleKey.Q:
                        started = false;
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine("End client.");
        }

        private static void m_Send0(BufferedStream _bf)
        {
            byte[] bytes = BitConverter.GetBytes(0);

            _bf.Write(bytes, 0, 4);
            _bf.Flush();
        }

        private static void m_Send1(BufferedStream _bf)
        {
            byte[] bytes = BitConverter.GetBytes(1);

            _bf.Write(bytes, 0, 4);
            _bf.Flush();
        }

        private static void m_Send100(BufferedStream _bf)
        {
            Console.Write("메시지 입력: ");
            string message = Console.ReadLine();
            byte[] msgBytes = Encoding.ASCII.GetBytes(message);
            byte[] msgLength = BitConverter.GetBytes(msgBytes.Length);
            byte[] headerBytes = BitConverter.GetBytes(100);

            _bf.Write(headerBytes, 0, 4);
            _bf.Flush();
            _bf.Write(msgLength, 0, 4);
            _bf.Write(msgBytes, 0, msgBytes.Length);
            _bf.Flush();
        }

        private static void m_Send1000(BufferedStream _bf)
        {
            byte[] bytes = BitConverter.GetBytes(1000);

            _bf.Write(bytes, 0, 4);
            _bf.Flush();
        }
    }
}

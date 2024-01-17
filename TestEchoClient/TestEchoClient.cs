using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenshinLike.Net
{
    public class TestEchoClient
    {
        private int m_echoCount;
        private string m_echoMessage;

        private TcpClient m_client;
        private NetworkStream m_stream;

        public TestEchoClient(int _echoCount, string _message)
        {
            m_echoCount = _echoCount;
            m_echoMessage = _message;
        }

        public async Task Start(string _ipv4, int _portNumber)
        {
            m_client = new TcpClient(_ipv4, _portNumber);
            m_stream = m_client.GetStream();

            await Task.Run(m_ProcessAsync/*, object _state*/);
        }

        private async Task m_ProcessAsync(/*object _state*/)
        {
            try
            {
                int count = m_echoCount;
                byte[] sendBuffer = Encoding.ASCII.GetBytes(m_echoMessage);
                byte[] receiveBuffer = new byte[1024];

                while(count-- > 0)
                {
                    await m_stream.WriteAsync(sendBuffer, 0, sendBuffer.Length).ConfigureAwait(false);

                    int receiveLength = await m_stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length).ConfigureAwait(false);
                    string receiveMessage = Encoding.ASCII.GetString(receiveBuffer, 0, receiveLength);

                    Console.WriteLine($"Receive echo message from server. ({receiveMessage})");
                    Thread.Sleep(1000);
                }

                // await m_stream.WriteAsync(sendBuffer, 0, 0).ConfigureAwait(false);
                Console.WriteLine("Successfully receive services.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.WriteLine("Occur exception.");
            }
            finally
            {
                m_stream.Close();
                m_client.Close();
                Console.WriteLine("Exit server.");
            }
        }
    }
}

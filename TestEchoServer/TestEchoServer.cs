using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GenshinLike.Net
{
    public class TestEchoServer
    {
        private int m_maxEchoCount;

        private int m_leftEchoCount;
        private TcpListener m_listener;

        public TestEchoServer(int _maxEchoCount)
        {
            m_maxEchoCount = _maxEchoCount;
        }

        public async Task StartAsync(int _portNumber)
        {
            m_leftEchoCount = m_maxEchoCount;
            m_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _portNumber);
            m_listener.Start();

            // NOTE: m_ProcessAsync 함수 안에서 Exception Handling이 잘 이루어지면 while문이 멈춰버리는 현상은 없음.
            while (m_leftEchoCount > 0)
            {
                TcpClient acceptedClient = await m_listener.AcceptTcpClientAsync().ConfigureAwait(false);
                _ = Task.Factory.StartNew(m_ProcessAsync, acceptedClient);
            }
        }

        private async void m_ProcessAsync(object _object)
        {
            int maxBufferSize = 1024;
            byte[] receiveBuffer = new byte[maxBufferSize];

            TcpClient client = (TcpClient)_object;
            NetworkStream stream = client.GetStream();

            try
            {
                int receiveLength = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length).ConfigureAwait(false);

                if (receiveLength > 0)
                {
                    string receiveMessage = Encoding.ASCII.GetString(receiveBuffer, 0, receiveLength);
                    Console.WriteLine($"[{DateTime.Now}] {receiveMessage}");

                    await stream.WriteAsync(receiveBuffer, 0, receiveLength).ConfigureAwait(false);
                }
                else
                {
                    await stream.WriteAsync(receiveBuffer, 0, 0).ConfigureAwait(false);
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
        }
    }
}

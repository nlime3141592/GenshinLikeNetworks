using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GenshinLike.Net
{
    public class TestEchoServer
    {
        private TcpListener m_listener;

        public TestEchoServer()
        {
            
        }

        public async Task StartAsync(int _portNumber)
        {
            m_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _portNumber);
            m_listener.Start();

            // NOTE: m_ProcessAsync 함수 안에서 Exception Handling이 잘 이루어지면 이 함수에 예외가 전파되는 경우는 없음.
            Console.WriteLine("Wait client...");
            TcpClient acceptedClient = await m_listener.AcceptTcpClientAsync().ConfigureAwait(false);
            Console.WriteLine("Accept client!");

            await Task.Run(() => m_ProcessAsync(acceptedClient));
        }

        // NOTE: 원격 호스트를 핸들링합니다.
        private async Task m_ProcessAsync(object _object)
        {
            int maxBufferSize = 1024;
            byte[] receiveBuffer = new byte[maxBufferSize];

            TcpClient client = (TcpClient)_object;
            NetworkStream stream = client.GetStream();

            try
            {
                while (true)
                {
                    int receiveLength = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length).ConfigureAwait(false);

                    if (receiveLength <= 0)
                        break;

                    string receiveMessage = Encoding.ASCII.GetString(receiveBuffer, 0, receiveLength);
                    Console.WriteLine($"[{DateTime.Now}] {receiveMessage}");

                    await stream.WriteAsync(receiveBuffer, 0, receiveLength).ConfigureAwait(false);
                }

                Console.WriteLine("Successfully handle a client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.WriteLine("Occur exception.");
            }
            finally
            {
                stream.Close();
                client.Close();
                Console.WriteLine("Shutdown client.");
            }
        }
    }
}

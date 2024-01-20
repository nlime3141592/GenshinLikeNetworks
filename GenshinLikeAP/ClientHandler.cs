using GenshinLike.AP.AppServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace GenshinLike.AP
{
    public class ClientHandler
    {
        public const int c_BUFFER_SIZE = 4096;

        public bool IsStarted => m_started;
        public Queue<AppService> ServiceQueue => m_serviceQueue;

        private TcpClient m_tcpClient;
        private BufferedStream m_bufferedStream;
        private Thread m_networkThread;
        private bool m_started;
        private Queue<AppService> m_serviceQueue;
        private byte[] m_buffer;

        public ClientHandler(TcpClient _client)
        {
            m_tcpClient = _client;
            m_bufferedStream = new BufferedStream(_client.GetStream());
            m_networkThread = new Thread(new ThreadStart(m_NetworkThreadMain));
            m_serviceQueue = new Queue<AppService>(4);
            m_buffer = new byte[ClientHandler.c_BUFFER_SIZE];
        }

        public void Start()
        {
            m_started = true;
            m_networkThread.Start();
        }

        public void Stop()
        {
            m_started = false;
            m_bufferedStream.Close();
            m_tcpClient.Close();
        }

        private void m_NetworkThreadMain()
        {
            while (m_started)
            {
                try
                {
                    if (m_bufferedStream.Read(m_buffer, 0, 4) > 0)
                    {
                        int serviceHeader = BitConverter.ToInt32(m_buffer, 0);

                        lock (m_serviceQueue)
                        {
                            AppService service = m_SwitchServices(serviceHeader);

                            if(service != null)
                                m_serviceQueue.Enqueue(service);
                        }
                    }
                }
                catch (SocketException _socketException)
                {
                    this.Stop();
                    Console.WriteLine(_socketException.StackTrace);
                    Console.WriteLine(_socketException.Message);
                    throw _socketException;
                }
                catch (IOException _ioException)
                {
                    // TODO:
                    // 클라이언트가 접속 중일 때 서버가 닫히면
                    // 이 지점에서 IOException이 발생하는데, InnerException으로 SocketException을 가지고 있음.
                    // SocketException 10004가 발생하고, 이를 처리할 필요가 있음.
                    this.Stop();
                    Console.WriteLine(_ioException.StackTrace);
                    Console.WriteLine(_ioException.Message);
                    throw _ioException;
                }
                catch (Exception _ex)
                {
                    this.Stop();
                    Console.WriteLine(_ex.StackTrace);
                    Console.WriteLine(_ex.Message);
                    throw _ex;
                }
            }
        }

        private AppService m_SwitchServices(int _serviceHeader)
        {
            switch (_serviceHeader)
            {
                case 0:
                    Console.WriteLine("Service 00");
                    return null;
                case 1:
                    Console.WriteLine("Service 01");
                    return null;
                case 100:
                    return new EchoService(m_buffer, m_bufferedStream);
                default:
                    Console.WriteLine("잘못된 서비스 헤더를 수신했습니다. ({0})", _serviceHeader);
                    this.Stop();
                    return null;
            }
        }
    }
}
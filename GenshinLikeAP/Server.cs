using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GenshinLike.AP
{
    public class Server
    {
        public bool IsStarted => m_started;
        public bool IsDisposed => m_disposed;
        public int ClientCount => m_clientHandlers.Count;

        private TcpListener m_listener;
        private bool m_started;
        private bool m_disposed;

        private Thread m_acceptThread;
        private List<ClientHandler> m_clientHandlers;
        private ParallelOptions m_parallelOptions;

        public Server(IPAddress _addressSet, int _port)
        {
            m_listener = new TcpListener(_addressSet, _port);

            m_acceptThread = new Thread(new ThreadStart(m_AcceptThreadMain));
            m_clientHandlers = new List<ClientHandler>(10);
            m_parallelOptions = new ParallelOptions();

            m_parallelOptions.MaxDegreeOfParallelism = 1 + (Environment.ProcessorCount / 2);
        }

        public void StartAndRun()
        {
            m_started = true;

            Console.WriteLine("Opening Server ...");
            m_acceptThread.Start();
            Console.WriteLine("Server Opened.");

            // TEST: 서버 값 모니터링을 위한 테스트 코드입니다.
            Task.Run(() => Program.m_ServerTickMain(this));

            m_ServerThreadMain();

            Console.WriteLine("Closing Server ...");
            m_acceptThread.Join();
            Console.WriteLine("Server Closed.");
        }

        public void Stop()
        {
            if (m_started)
                m_listener.Stop();

            lock (m_clientHandlers)
            {
                while(m_clientHandlers.Count > 0)
                {
                    ClientHandler handler = m_clientHandlers[0];
                    handler.Stop();
                    m_clientHandlers.RemoveAt(0);
                }
            }

            m_started = false;
            m_disposed = true;
        }

        private void m_ServerThreadMain()
        {
            while (m_started)
            {
                lock (m_clientHandlers)
                {
                    for (int i = m_clientHandlers.Count - 1; i >= 0; --i)
                    {
                        if (!m_clientHandlers[i].IsStarted)
                            m_clientHandlers.RemoveAt(i);
                    }
                }

                Parallel.For(0, m_clientHandlers.Count, m_parallelOptions, (_index) =>
                {
                    ClientHandler handler = m_clientHandlers[_index];

                    lock(handler.ServiceQueue)
                    {
                        while(handler.ServiceQueue.Count > 0)
                        {
                            handler.ServiceQueue.Dequeue().Execute();
                        }
                    }
                });
            }
        }

        private void m_AcceptThreadMain()
        {
            byte[] acceptingBuffer = new byte[4];

            m_listener.Start();

            while (m_started)
            {
                try
                {
                    TcpClient client = m_listener.AcceptTcpClient();

                    if (client.GetStream().ReadByte() != 0)
                    {
                        client.GetStream().WriteByte(1);
                        client.GetStream().Flush();
                        client.Close();
                        Console.WriteLine("[WARNING] 잘못된 클라이언트 접속 시도가 있었음.");
                        continue;
                    }
                    else if (m_clientHandlers.Count >= m_clientHandlers.Capacity)
                    {
                        client.GetStream().WriteByte(2);
                        client.GetStream().Flush();
                        client.Close();
                    }
                    else
                    {
                        client.GetStream().WriteByte(0);
                        client.GetStream().Flush();

                        ClientHandler clientHandler = new ClientHandler(client);
                        clientHandler.Start();

                        lock (m_clientHandlers)
                        {
                            m_clientHandlers.Add(clientHandler);
                        }
                    }
                }
                catch (SocketException _socketException)
                {
                    // NOTE:
                    // 소켓 통신 시 발생하는 에러 코드 목록은 아래 링크를 참조하세요.
                    // WinSock2 오류 코드 문서: https://learn.microsoft.com/ko-kr/windows/win32/winsock/windows-sockets-error-codes-2
                    // .NET SocketError Enum 문서: https://learn.microsoft.com/ko-kr/dotnet/api/system.net.sockets.socketerror?view=net-8.0
                    switch ((SocketError)_socketException.ErrorCode)
                    {
                        case SocketError.Interrupted: // NOTE: TcpListener의 수신 차단에 따라 Accept Thread가 강제로 종료되었습니다.
                            break;
                        default:
                            Console.WriteLine(_socketException.StackTrace);
                            Console.WriteLine(_socketException.Message);
                            throw _socketException;
                    }
                }
                catch (ObjectDisposedException _objectDisposedException)
                {
                    // NOTE: 닫힌 TcpClient의 Socket을 Accept 시도했습니다.
                    Console.WriteLine(_objectDisposedException.StackTrace);
                    Console.WriteLine(_objectDisposedException.Message);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex.StackTrace);
                    Console.WriteLine(_ex.Message);

                    // #if DEBUG
                    throw _ex;
                    // #endif
                }
            }
        }
    }
}
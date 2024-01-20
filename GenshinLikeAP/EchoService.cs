using System;
using System.IO;
using System.Text;

namespace GenshinLike.AP.AppServices
{
    public sealed class EchoService : AppService
    {
        private string m_message;

        public EchoService(byte[] _buffer, BufferedStream _stream)
        {
            _stream.Read(_buffer, 0, 4);
            int messageLength = BitConverter.ToInt32(_buffer, 0);

            if (_stream.Read(_buffer, 4, messageLength) == messageLength)
            {
                m_message = Encoding.ASCII.GetString(_buffer, 4, messageLength);

                _stream.Write(_buffer, 0, messageLength + 4);
                _stream.Flush();
            }
            else
            {
                // TODO: You should handle error while receiving datas from client.
            }
        }

        public override void Execute()
        {
            this.Say(m_message);
        }
    }
}

using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public class StreamController
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamController(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public ChatMessage ReceiveMessage()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            var str = streamEncoding.GetString(inBuffer);

            var message = JsonConvert.DeserializeObject<ChatMessage>(str);

            return message;
        }

        public void SendMessage(ChatMessage message)
        {
            var str = JsonConvert.SerializeObject(message);

            byte[] outBuffer = streamEncoding.GetBytes(str);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();
        }

        public List<ChatMessage> GetMessagesStory(ChatMessage message)
        {
            var store = JsonConvert.DeserializeObject<List<ChatMessage>>(message.Content);

            return store;
        }
    }
}

using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    public class ClientHandler
    {
        public string Id { get; private set; }
        public string ClientName { get; private set; }

        private MessageStore _store;

        private Thread _messagesFromUserThread;
        private Thread _messagesToUserThread;

        private NamedPipeServerStream _messagesFromUserPipe;
        private NamedPipeServerStream _messagesToUserPipe;

        public ClientHandler(string id, string clientName, MessageStore store)
        {
            Id = id;
            ClientName = clientName;

            _store = store;
            _store.NewMessage += Store_NewMessage;
        }

        private void Store_NewMessage(string message)
        {
            Console.WriteLine(message);
            StreamString ss = new StreamString(_messagesToUserPipe);

            var infoMessageToClient = new ChatMessage()
            {
                Content = message
            };

            //Console.WriteLine("Sending id to client");

            var infoMessageToClientstr = JsonConvert.SerializeObject(infoMessageToClient);

            ss.WriteString(infoMessageToClientstr);
        }

        public void Start()
        {
            _messagesFromUserThread = new Thread(MessagesFromUserThreadFunc);
            _messagesFromUserThread.Start();

            _messagesToUserThread = new Thread(MessagesToUserThreadFunc);
            _messagesToUserThread.Start();
        }

        public void MessagesFromUserThreadFunc()
        {
            Console.WriteLine("Creating messagesFromUserPipe pipe.");
            _messagesFromUserPipe = new NamedPipeServerStream(PipeNameHelper.GetMessagesFromUserPipeName(Id), PipeDirection.In);

            // Wait for a client to connect
            Console.WriteLine("Waiting For Connection to messagesFromUserPipe");
            _messagesFromUserPipe.WaitForConnection();
            Console.WriteLine("_messagesFromUserPipe Connected");

            while (true)
            {
                StreamString ss = new StreamString(_messagesFromUserPipe);

                Console.WriteLine("Reading message");
                //ss.WriteString("I am the one true server!");
                var chatMessageStr = ss.ReadString();
                var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(chatMessageStr);

                //Console.WriteLine($"{ClientName}: {chatMessage.Content}");

                _store.AddNewMessage($"{ClientName}: {chatMessage.Content}");
            }
        }

        public void MessagesToUserThreadFunc()
        {
            Console.WriteLine("Creating messagesToUserPipe pipe.");
            _messagesToUserPipe = new NamedPipeServerStream(PipeNameHelper.GetMessagesToUserPipeName(Id), PipeDirection.Out);

            // Wait for a client to connect
            Console.WriteLine("Waiting For Connection to messagesToUserPipe");
            _messagesToUserPipe.WaitForConnection();
            Console.WriteLine("_messagesToUserPipe Connected");
        }
    }
}

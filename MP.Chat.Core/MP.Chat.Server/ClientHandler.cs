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
               
        private Thread messagesFromUserThread;
        private Thread messagesToUserThread;

        public ClientHandler(string id, string clientName)
        {
            Id = id;
            ClientName = clientName;
        }

        public void Start()
        {
            messagesFromUserThread = new Thread(MessagesFromUserThreadFunc);
            messagesFromUserThread.Start();

            messagesToUserThread = new Thread(MessagesToUserThreadFunc);
            messagesToUserThread.Start();
        }

        public void MessagesFromUserThreadFunc()
        {
            Console.WriteLine("Creating messagesFromUserPipe pipe.");
            NamedPipeServerStream messagesFromUserPipe = new NamedPipeServerStream(PipeNameHelper.GetMessagesFromUserPipeName(Id), PipeDirection.In);

            // Wait for a client to connect
            Console.WriteLine("Waiting For Connection to messagesFromUserPipe");
            messagesFromUserPipe.WaitForConnection();


            while (true)
            {
                StreamString ss = new StreamString(messagesFromUserPipe);

                Console.WriteLine("Reading message");
                //ss.WriteString("I am the one true server!");
                var chatMessageStr = ss.ReadString();
                var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(chatMessageStr);

                Console.WriteLine($"{ClientName}: {chatMessage.Content}");


            }
        }

        public void MessagesToUserThreadFunc()
        {
            Console.WriteLine("Creating messagesToUserPipe pipe.");
            NamedPipeServerStream messagesToUserPipe = new NamedPipeServerStream(PipeNameHelper.GetMessagesToUserPipeName(Id), PipeDirection.Out);

            // Wait for a client to connect
            Console.WriteLine("Waiting For Connection to messagesToUserPipe");
            messagesToUserPipe.WaitForConnection();


           
        }
    }
}

using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP.Chat.Client
{
    public class Client
    {
        public string Name { get; private set; }

        public string _id;

        public Client()
        {
            Name = RandomHelper.GetRandomName();
        }

        public void ConnectToServer()
        {
            var pipeClient = new NamedPipeClientStream(Constant.ServerName, Constant.ServerListeningPipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();

            StreamString ss = new StreamString(pipeClient);
            // Validate the server's signature string
            //if (ss.ReadString() == "I am the one true server!")
            //{
            //The client security token is sent with the first write.
            // Send the name of the file whose contents are returned
            // by the server.

            var messege = new ChatMessage()
            {
                Command = ChatCommand.RegisterUser,
                Content = Name
            };

            var messegeStr = JsonConvert.SerializeObject(messege);

            ss.WriteString(messegeStr);

            var responseStr = ss.ReadString();
            var response = JsonConvert.DeserializeObject<ChatMessage>(responseStr);

            pipeClient.Close();

            _id = response.Content;

            var thread1 = new Thread(StartChatting);
            thread1.Start();


            // Print the file to the screen.
            //Console.Write(ss.ReadString());
            //}
            //else
            //{
            // Console.WriteLine("Server could not be verified.");
            //}
            Thread.Sleep(4000000);

            // Give the client process some time to display results before exiting.

        }



        public void StartChatting()
        {
            var pipeClient = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesFromUserPipeName(_id), PipeDirection.Out, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();

            StreamString ss = new StreamString(pipeClient);

            

            while (true)
            {
                var messege = new ChatMessage()
                {
                    Command = ChatCommand.Message,
                    Content = RandomHelper.GetRandomStoryes()
                };
                var messegeStr = JsonConvert.SerializeObject(messege);

                ss.WriteString(messegeStr);

                Thread.Sleep(1000);
            }
        }

        public void StopChatting()
        {

        }
    }
}

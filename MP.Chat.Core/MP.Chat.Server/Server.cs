using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    public class Server
    {


        private List<ClientHandler> ClientHandlers;

        public Server()
        {
            ClientHandlers = new List<ClientHandler>();
        }

        public void StartListening()
        {
            Console.WriteLine("Creating listening pipe.");
            NamedPipeServerStream pipeServer = new NamedPipeServerStream(Constant.ServerListeningPipeName, PipeDirection.InOut, 1);

            while (true)
            {
                // Wait for a client to connect
                Console.WriteLine("Waiting For Connection.");
                pipeServer.WaitForConnection();

                Console.WriteLine("Client connected");

                try
                {
                    StreamString ss = new StreamString(pipeServer);

                    Console.WriteLine("Reading command");
                    //ss.WriteString("I am the one true server!");
                    var chatMessageStr = ss.ReadString();
                    var chatMessage =  JsonConvert.DeserializeObject<ChatMessage>(chatMessageStr);

                    if(chatMessage.Command == ChatCommand.RegisterUser)
                    {
                        Console.WriteLine("RegisterUser command");
                        var rnd = new Random();
                        var id = rnd.Next(0,1000).ToString();

                        Console.WriteLine($"Id '{id}'");

                        var clientHandler = new ClientHandler(id, chatMessage.Content);
                        clientHandler.Start();

                        var infoMessageToClient = new ChatMessage()
                        {
                            Command = ChatCommand.RegisterApproved,
                            Content = id
                        };

                        Console.WriteLine("Sending id to client");

                        var infoMessageToClientstr = JsonConvert.SerializeObject(infoMessageToClient);
                        ss.WriteString(infoMessageToClientstr);
                        pipeServer.Disconnect();
                        Console.WriteLine("Disconnected");
                    }

                    // Read in the contents of the file while impersonating the client.
                    //ReadFileToStream fileReader = new ReadFileToStream(ss, filename);

                    //// Display the name of the user we are impersonating.
                    //Console.WriteLine("Reading file: {0} on thread[{1}] as user: {2}.",
                    //    filename, threadId, pipeServer.GetImpersonationUserName());
                    //pipeServer.RunAsClient(fileReader.Start);
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
               // Console.WriteLine("Closing...");
                //pipeServer.Close();
                
            }            
        }

        public void ClientThreadFunc()
        {

        }
    }
}

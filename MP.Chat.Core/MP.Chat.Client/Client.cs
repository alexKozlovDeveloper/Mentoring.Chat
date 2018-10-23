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

        private Thread _sendMessageThread;
        private Thread _receiveMessageThread;

        private Logger _logger;

        private ConsoleViewer _consoleViewer;

        public Client(Logger logger)
        {
            _logger = logger;

            _consoleViewer = new ConsoleViewer(logger);

            Name = RandomHelper.GetRandomName();
            _logger.Info($"Name '{Name}' generated");
        }

        public void ConnectToServer()
        {
            _logger.Info("Creating pipe for init user on server...");
            var pipeClient = new NamedPipeClientStream(Constant.ServerName, Constant.ServerListeningPipeName, 
                PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to server...");
            pipeClient.Connect();

            StreamController streamController = new StreamController(pipeClient);

            var messege = new ChatMessage()
            {
                Command = ChatCommand.RegisterUser,
                Name = Name
            };

            _logger.Info("Sending registering message");
            streamController.SendMessage(messege);

            var response = streamController.ReceiveMessage();

            if(response.Command == ChatCommand.RegisterApproved)
            {
                _logger.Info("Register approved");
            }

            pipeClient.Close();

            _id = response.Content;
            _logger.Info($"Client Id '{_id}'");

            _logger.Info($"Creating threads for Send/Receive funcs...");
            _logger.Info($"Starting SendMessageFunc");
            _sendMessageThread = new Thread(SendMessageFunc);
            _sendMessageThread.Start();

            _logger.Info($"Starting ReceiveMessageFunc");
            _receiveMessageThread = new Thread(ReceiveMessageFunc);
            _receiveMessageThread.Start();

        }

        public void SendMessageFunc()
        {
            _logger.Info($"SendMessageFunc start.");
            var pipeClient = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesFromUserPipeName(_id), 
                PipeDirection.Out, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to GetMessagesFromUserPipeName...");
            pipeClient.Connect();
            _logger.Info("Connected");
            
            StreamController streamController = new StreamController(pipeClient);

            while (true)
            {
                var messege = new ChatMessage()
                {
                    Command = ChatCommand.Message,
                    Content = RandomHelper.GetRandomStoryes(),
                    Name = Name,
                    Id = _id
                };

                _logger.Info("Sending random story to server");

                streamController.SendMessage(messege);

                _logger.Info("Sleeping...");
                Thread.Sleep(100000);
                //Thread.Sleep(RandomHelper.GetRandomSleepTime());
            }

            _logger.Info($"SendMessageFunc end.");
        }

        public void ReceiveMessageFunc()
        {
            _logger.Info($"ReceiveMessageFunc start.");

            var pipeClient = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesToUserPipeName(_id), 
                PipeDirection.In, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to server...");
            pipeClient.Connect();
            _logger.Info("Connected");

            StreamController streamController = new StreamController(pipeClient);

            while (true)
            {
                _logger.Info("Waiting message from server...");
                var messege = streamController.ReceiveMessage();

                _logger.Info("Message received...");

                _logger.Info(messege.Content);
            }

            _logger.Info($"ReceiveMessageFunc end.");
        }

        public void StopChatting()
        {

        }
    }
}

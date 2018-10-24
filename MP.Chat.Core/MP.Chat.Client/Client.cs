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
        public bool IsAlive { get; private set; }
        public string Name { get; private set; }
        public string Id { get; private set; }

        private Thread _receiveMessageThread;

        private NamedPipeClientStream _messagesFromServer;
        private NamedPipeClientStream _messagesToServer;

        private Logger _logger;

        public delegate void NewMessageDelegate(ChatMessage message);
        public event NewMessageDelegate NewMessage;

        public Client(Logger logger, string name)
        {
            _logger = logger;

            Name = name;     
        }

        public void Login()
        {
            IsAlive = true;

            _logger.Info("Try to login...");
            var response = SendCommandMessage(ChatCommand.Login);

            _logger.Info("Login success");
            Id = response.Content;
            _logger.Info($"Client Id '{Id}'");

            var history = GetMessagesHistory();

            foreach (var item in history)
            {
                NewMessage(item);
            }
        }

        public void Start()
        {
            _logger.Info($"Init messagesToServer pipe");
            _messagesToServer = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesFromUserPipeName(Id),
                PipeDirection.Out, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to GetMessagesFromUserPipeName...");
            _messagesToServer.Connect();
            _logger.Info("Connected");


            _logger.Info($"Init messagesFromServer pipe");
            _messagesFromServer = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesToUserPipeName(Id),
                PipeDirection.In, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to server...");
            _messagesFromServer.Connect();
            _logger.Info("Connected");


            _logger.Info($"Starting ReceiveMessage thread");
            _receiveMessageThread = new Thread(ReceiveMessageFunc);
            _receiveMessageThread.Start();
        }

        public void SendMessage(string message)
        {
            var chatMessage = new ChatMessage()
            {
                Command = ChatCommand.AddMessage,
                Content = message,
                Id = Id,
                Name = Name                
            };

            StreamController streamController = new StreamController(_messagesToServer);

            _logger.Info($"Sending message '{chatMessage}' story to server");
            streamController.SendMessage(chatMessage);
        }

        private NamedPipeClientStream ConnectToServer()
        {
            _logger.Info("Creating pipe...");
            var pipe = new NamedPipeClientStream(Constant.ServerName, Constant.ServerListeningPipeName,
                PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to server...");

            while (IsAlive)
            {
                try
                {
                    pipe.Connect(500);
                    break;
                }
                catch (TimeoutException)
                {
                    _logger.Info($"Server is offline");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }

            _logger.Info("Connected");

            return pipe;
        }

        private List<ChatMessage> GetMessagesHistory()
        {
            var response = SendCommandMessage(ChatCommand.GetMessagesHistory);

            var result = JsonConvert.DeserializeObject<List<ChatMessage>>(response.Content);

            return result;
        }

        private ChatMessage SendCommandMessage(ChatCommand command)
        {
            var pipe = ConnectToServer();

            StreamController streamController = new StreamController(pipe);

            var messege = new ChatMessage()
            {
                Command = command,
                Name = Name
            };

            _logger.Info($"Sending command '{command}' to server...");
            streamController.SendMessage(messege);

            var response = streamController.ReceiveMessage();
            _logger.Info($"Responce received");

            pipe.Dispose();

            return response;
        }

        private void Sleep(int time)
        {
            _logger.Info($"Sleeping '{time}'...");

            for (int i = 0; i < time; i++)
            {
                if (IsAlive == false)
                {
                    break;
                }

                Thread.Sleep(100);
            }
        }

        private void ReceiveMessageFunc()
        {
            _logger.Info($"ReceiveMessageFunc start.");

            StreamController streamController = new StreamController(_messagesFromServer);

            while (IsAlive)
            {
                try
                {
                    _logger.Info("Waiting message from server...");
                    var messege = streamController.ReceiveMessage();

                    _logger.Info("Message received...");
                    NewMessage(messege);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }

            _logger.Info($"ReceiveMessageFunc end.");
        }

        public void Stop()
        {
            _logger.Info($"Starting shut down...");

            IsAlive = false;

            try
            {
                if(_messagesFromServer != null)
                {
                    _logger.Info($"messagesFromServer disposing...");
                    _messagesFromServer.Dispose();
                }

                if(_messagesToServer != null)
                {
                    _logger.Info($"messagesToServer disposing...");
                    _messagesToServer.Dispose();
                }

                if(_receiveMessageThread.ThreadState == ThreadState.Running)
                {
                    //_receiveMessageThread.Join();
                    _receiveMessageThread.Abort();
                }                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}

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
        public bool IsAlive { get; set; }

        public string Name { get; private set; }
        public string Id { get; private set; }

        private Thread _sendMessageThread;
        private Thread _receiveMessageThread;

        private Thread _loginThread;

        private Logger _logger;

        private ConsoleViewer _consoleViewer;

        private NamedPipeClientStream _messagesFromServer;
        private NamedPipeClientStream _messagesToServer;

        public Client(Logger logger)
        {
            _logger = logger;

            _consoleViewer = new ConsoleViewer(logger);

            Name = RandomHelper.GetRandomName();
            _logger.Info($"Name '{Name}' generated");           
        }

        public bool Login()
        {
            //IsAlive = true;

            //_loginThread = new Thread(Login);
            //_loginThread.Start();

            LoginUser();

            var history = GetMessagesHistory();

            foreach (var item in history)
            {
                _consoleViewer.WriteMessageToConsole(item);
            }

            return true;
        }

        private void Start()
        {
            CreateThreads();
        }

        private void CreateThreads()
        {
            _logger.Info($"Creating threads for Send/Receive funcs...");
            _logger.Info($"Starting SendMessageFunc");
            _sendMessageThread = new Thread(SendMessageFunc);
            _sendMessageThread.Start();

            _logger.Info($"Starting ReceiveMessageFunc");
            _receiveMessageThread = new Thread(ReceiveMessageFunc);
            _receiveMessageThread.Start();
        }

        public void SendMessage()
        {

        }

        private void LoginUser()
        {
            //_logger.Info("Creating pipe...");
            //var pipe = new NamedPipeClientStream(Constant.ServerName, Constant.ServerListeningPipeName,
            //    PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            //_logger.Info("Connecting to server...");
            //pipe.Connect();

            var pipe = ConnectToServer();

            StreamController streamController = new StreamController(pipe);

            var messege = new ChatMessage()
            {
                Command = ChatCommand.Login,
                Name = Name
            };

            _logger.Info("Try to login...");
            streamController.SendMessage(messege);

            var response = streamController.ReceiveMessage();

            _logger.Info("Login success");
            Id = response.Content;
            _logger.Info($"Client Id '{Id}'");

            pipe.Dispose();
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
                }
                catch (TimeoutException ex)
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
            //_logger.Info("Creating pipe...");
            //var pipe = new NamedPipeClientStream(Constant.ServerName, Constant.ServerListeningPipeName,
            //    PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            //_logger.Info("Connecting to server...");
            //pipe.Connect();

            var pipe = ConnectToServer();

            StreamController streamController = new StreamController(pipe);

            var messege = new ChatMessage()
            {
                Command = ChatCommand.GetMessagesHistory,
                Name = Name
            };

            _logger.Info("Sending request for old messages...");
            streamController.SendMessage(messege);

            var response = streamController.ReceiveMessage();

            var result = streamController.GetMessagesStory(response);

            pipe.Dispose();

            return result;
        }

        public void SendMessageFunc()
        {
            _logger.Info($"SendMessageFunc start.");
            _messagesToServer = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesFromUserPipeName(Id),
                PipeDirection.Out, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to GetMessagesFromUserPipeName...");
            _messagesToServer.Connect();
            _logger.Info("Connected");

            StreamController streamController = new StreamController(_messagesToServer);

            while (IsAlive)
            {
                var messege = new ChatMessage()
                {
                    Command = ChatCommand.AddMessage,
                    Content = RandomHelper.GetRandomStoryes(),
                    Name = Name,
                    Id = Id
                };

                try
                {
                    _logger.Info("Sending random story to server");

                    streamController.SendMessage(messege);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                Sleep(RandomHelper.GetRandomSleepTime());
            }

            _logger.Info($"SendMessageFunc end.");
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

        public void ReceiveMessageFunc()
        {
            _logger.Info($"ReceiveMessageFunc start.");

            _messagesFromServer = new NamedPipeClientStream(Constant.ServerName, PipeNameHelper.GetMessagesToUserPipeName(Id),
                PipeDirection.In, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            _logger.Info("Connecting to server...");
            _messagesFromServer.Connect();
            _logger.Info("Connected");

            StreamController streamController = new StreamController(_messagesFromServer);

            while (IsAlive)
            {
                try
                {
                    _logger.Info("Waiting message from server...");
                    var messege = streamController.ReceiveMessage();

                    _logger.Info("Message received...");

                    _consoleViewer.WriteMessageToConsole(messege);
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}

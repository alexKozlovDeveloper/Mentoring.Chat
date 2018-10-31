using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP.Chat.Client
{
    public class BotCreator
    {
        private Thread _botCreatorThread;
        private bool _isThreadActive;

        private Logger _logger;

        private ConsoleViewer _consoleViewer;

        public BotCreator(Logger logger)
        {
            _logger = logger;

            _consoleViewer = new ConsoleViewer(logger);
        }

        public void StartBot()
        {
            _botCreatorThread = new Thread(BotCreatorThreadFunc);

            _isThreadActive = true;

            _botCreatorThread.Start();
        }

        private void BotCreatorThreadFunc()
        {
            while (_isThreadActive)
            {
                try
                {
                    Console.Clear();

                    var name = RandomHelper.GetRandomName();

                    _logger.Info($"Creating new client '{name}'...");
                    var client = new Client(_logger, name);

                    client.NewMessage += Client_NewMessage;

                    _logger.Info("Starting client...");
                    client.Login();
                    client.Start();

                    client.SendMessage(RandomHelper.GetRandomGreetingMessages());
                    Sleep(RandomHelper.GetRandomSleepTime());

                    var count = RandomHelper.Random.Next(1, 2);

                    for (int i = 0; i < count; i++)
                    {
                        Sleep(RandomHelper.GetRandomSleepTime());
                        client.SendMessage(RandomHelper.GetRandomStoryes());
                    }

                    Sleep(RandomHelper.GetRandomSleepTime());
                    client.SendMessage(RandomHelper.GetRandomFinishMessages());

                    Sleep(RandomHelper.GetRandomSleepTime());
                    _logger.Info("Stopping client...");
                    client.Stop();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        private void Client_NewMessage(ChatMessage message)
        {
            _consoleViewer.WriteMessageToConsole(message);
        }

        private void Sleep(int time)
        {
            _logger.Info($"Sleeping '{time}'...");

            for (int i = 0; i < time; i++)
            {
                if (_isThreadActive == false)
                {
                    break;
                }

                Thread.Sleep(100);
            }
        }

        public void StopBot()
        {
            _isThreadActive = false;
        }
    }
}

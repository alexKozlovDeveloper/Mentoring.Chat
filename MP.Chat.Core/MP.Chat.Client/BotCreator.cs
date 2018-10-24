using MP.Chat.Core;
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

        public BotCreator(Logger logger)
        {
            _logger = logger;
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
                _logger.Info("Creating new client...");
                var client = new Client(_logger);

                _logger.Info("Starting client...");
                client.Login();

                Sleep(RandomHelper.GetRandomSleepTime());

                _logger.Info("Stopping client...");
                client.Stop();
            }
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

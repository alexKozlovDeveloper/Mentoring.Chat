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

        public BotCreator()
        {

        }

        public void StartBotChatting()
        {
            _botCreatorThread = new Thread(BotCreatorThreadFunc);

            _isThreadActive = true;

            _botCreatorThread.Start();
        }

        private void BotCreatorThreadFunc()
        {
            while (_isThreadActive)
            {
                var client = new Client();

                client.ConnectToServer();

            }       
        }

        public void StopBotChatting()
        {
            _isThreadActive = false;
        }
    }
}

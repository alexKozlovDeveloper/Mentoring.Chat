using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger(@"log.txt", false);
            logger.Info("Start.");

            logger.Info("Creating bot...");
            var botCreator = new BotCreator(logger);

            logger.Info("Start Bot Chatting...");
            botCreator.StartBot();

            while (true)
            {
                logger.Info("Press 'Esc' for exit...");
                var key = Console.ReadKey();

                //if (key.Key == ConsoleKey.Escape)
                if (key.Key == ConsoleKey.A)
                {
                    botCreator.StopBot();
                    break;
                }
            }

            logger.Info("End.");
            Console.ReadKey();
        }
    }
}

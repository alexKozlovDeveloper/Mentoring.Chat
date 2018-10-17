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
            var d = JsonConvert.SerializeObject(new ChatMessage
            {
                Command = ChatCommand.RegisterUser,
                Content = "ololo"
            });
            var c = JsonConvert.DeserializeObject<ChatMessage>(d);

            Console.WriteLine("Star.");

            var botCreator = new BotCreator();

            Console.WriteLine("Start Bot Chatting...");
            botCreator.StartBotChatting();

            while (true)
            {
                Console.WriteLine("Press 'Esc' for exit...");
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    botCreator.StopBotChatting();
                    break;
                }
            }

            Console.WriteLine("End.");
        }
    }
}

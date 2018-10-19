using MP.Chat.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger(@"log.txt", true);
            logger.Info("Start.");

            try
            {            
                var server = new Server(logger);
                server.StartListening();
            }
            catch (Exception ex) 
            {
                logger.Error(ex);
            }

            logger.Info("End.");
            Console.ReadKey();
        }
    }
}

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
            try
            {
                Console.WriteLine("Start.");
                //var server = new PipeServer();
                var server = new Server();
                server.StartListening();
                Console.WriteLine("End.");

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);

            }

           
            Console.ReadKey();
        }
    }
}

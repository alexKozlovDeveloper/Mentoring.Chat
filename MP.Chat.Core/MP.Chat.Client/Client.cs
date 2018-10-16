using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Client
{
    public class Client
    {
        public string Name { get; private set; }

        public Client()
        {
            Name = RandomHelper.GetRandomName();
        }

        public void ConnectToServer()
        {

        }

        public void StartChatting()
        {

        }
       
    }
}

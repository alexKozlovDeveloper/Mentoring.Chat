using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core.Protocol
{
    public class ChatMessage
    {
        public ChatCommand Command { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }

        public ChatMessage()
        {
            Date = DateTime.Now.ToString("hh:mm:ss");
        }

        public ChatMessage(string name, string message)
        {
            Name = name;
            Content = message;
            Date = DateTime.Now.ToString("hh:mm:ss");
        }

        public override string ToString()
        {
            return $"{Date} [{Name}]: {Content}";
        }
    }
}

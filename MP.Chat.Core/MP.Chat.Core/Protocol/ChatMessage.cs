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

        public string Content { get; set; }
    }
}

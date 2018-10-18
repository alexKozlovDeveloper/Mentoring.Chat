﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    public class MessageStore
    {
        public List<string> Messages { get; private set; }

        public delegate void NewMessageDelegate(string message);
        public event NewMessageDelegate NewMessage;


        public MessageStore()
        {
            Messages = new List<string>();
        }

        public void AddNewMessage(string message)
        {
            Messages.Add(message);

            NewMessage(message);
        }

    }
}

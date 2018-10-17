using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public static class Constant
    {
        public static class FilePaths
        {
            public const string Names = @"Resources\Names.txt";
            public const string FinishMessages = @"Resources\FinishMessages.txt";
            public const string GreetingMessages = @"Resources\GreetingMessages.txt";
            public const string Storyes = @"Resources\Storyes.txt";
        }

        public const string ServerListeningPipeName = "ServerPipe";
        public const string PipeForClentsNameBase = "ClientPipe";
        public const string ServerName = "."; // this PC
    }
}
    
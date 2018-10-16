using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Client
{
    public static class ResourcesHelper
    {
        public static List<string> Names
        {
            get
            {
                var lines = File.ReadAllLines(@"Resources\Names.txt");

                return lines.ToList();
            }
        }

        public static List<string> FinishMessages
        {
            get
            {
                var lines = File.ReadAllLines(@"Resources\FinishMessages.txt");

                return lines.ToList();
            }
        }

        public static List<string> GreetingMessages
        {
            get
            {
                var lines = File.ReadAllLines(@"Resources\GreetingMessages.txt");

                return lines.ToList();
            }
        }

        public static List<string> Storyes
        {
            get
            {
                var lines = File.ReadAllLines(@"Resources\Storyes.txt");

                return lines.ToList();
            }
        }
    }
}

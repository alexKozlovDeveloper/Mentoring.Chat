using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public static class ResourcesHelper
    {
        public static List<string> Names
        {
            get
            {
                return GetLinesFromFile(Constant.FilePaths.Names);
            }
        }

        public static List<string> FinishMessages
        {
            get
            {
                return GetLinesFromFile(Constant.FilePaths.FinishMessages);
            }
        }

        public static List<string> GreetingMessages
        {
            get
            {
                return GetLinesFromFile(Constant.FilePaths.GreetingMessages);
            }
        }

        public static List<string> Storyes
        {
            get
            {
                return GetLinesFromFile(Constant.FilePaths.Storyes);
            }
        }

        private static List<string> GetLinesFromFile(string filePath)
        {
            IEnumerable<string> lines = File.ReadAllLines(filePath);

            lines = lines.Where(a => string.IsNullOrEmpty(a) != true);
            lines = lines.Distinct();

            return lines.ToList();
        }
    }
}

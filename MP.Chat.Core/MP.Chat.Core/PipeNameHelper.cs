using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public static class PipeNameHelper
    {
        public static string GetMessagesToUserPipeName(string id)
        {
            return $"{Constant.PipeForClentsNameBase}_{id}_ToUser";
        }

        public static string GetMessagesFromUserPipeName(string id)
        {
            return $"{Constant.PipeForClentsNameBase}_{id}_FromUser";
        }
    }
}

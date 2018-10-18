using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public static class RandomHelper
    {
        private static Random _random;

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }

                return _random;
            }
        }
               
        public static string GetRandomName()
        {
            return GetRandomString(ResourcesHelper.Names);
        }

        public static string GetRandomGreetingMessages()
        {
            return GetRandomString(ResourcesHelper.GreetingMessages);
        }

        public static string GetRandomFinishMessages()
        {
            return GetRandomString(ResourcesHelper.FinishMessages);
        }

        public static string GetRandomStoryes()
        {
            return GetRandomString(ResourcesHelper.Storyes);
        }

        private static string GetRandomString(List<string> strs)
        {
            var index = RandomHelper.Random.Next(0, strs.Count - 1);

            return strs[index];
        }
    }
}

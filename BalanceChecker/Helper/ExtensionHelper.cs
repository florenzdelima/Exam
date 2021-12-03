using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceChecker.Helper
{
    public class ExtensionHelper
    {
        private static Random random = new Random();

        //Create Random string
        public static string GenerateRandom(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

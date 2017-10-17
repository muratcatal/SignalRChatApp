using System.Collections.Generic;
using System.Linq;
using Server.Interfaces;

namespace Server.Business
{
    public class WordProcesser : IWordProcesser
    {
        public bool CheckBannedWord(List<string> bannedWords, string message)
        {
            return bannedWords.Any(w => message.Contains(w));
        }
    }
}

using System.Collections.Generic;

namespace Server.Interfaces
{
    public interface IWordProcesser
    {
        bool CheckBannedWord(List<string> bannedWords, string message);
    }
}

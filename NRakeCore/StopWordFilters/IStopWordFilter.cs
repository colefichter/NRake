using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRakeCore.StopWordFilters
{
    public interface IStopWordFilter
    {
        bool IsStopWord(string word);
        bool IsPunctuation(string word);
    }
}

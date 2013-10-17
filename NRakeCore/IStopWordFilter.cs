using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRakeCore
{
    public interface IStopWordFilter
    {
        bool IsStopWord(string word);
    }
}

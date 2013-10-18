using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRakeCore
{
    public sealed class WordScore
    {
        public int Degree { get; private set; }
        public int Frequency { get; private set; }

        public double Ratio
        {
            get
            {
                if (Frequency == 0)
                {
                    return 0;
                }

                return (double)Degree / Frequency;
            }
        }

        private WordScore() { }
        public WordScore(int degree, int frequency)
        {
            this.Degree = degree;
            this.Frequency = frequency;
        }
    }
}

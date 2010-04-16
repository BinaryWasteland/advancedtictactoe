using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtremeT3Library
{
    [Serializable]
    public class XtremeWho
    {
        public enum Who { USER = 0, USER2 = 1, NOBODY };
        public enum Symbol { USER = 'X', USER2 = 'O' }; // ASCII value of character used by each player
    }
}

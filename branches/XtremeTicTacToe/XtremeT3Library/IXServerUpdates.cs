using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtremeT3Library
{
    public interface IXServerUpdates
    {
        void UpdateBoardCallback(char[] board);
        void UpdatePlayersCallback(string[] id);
        void GameOverCallback();
    }
}

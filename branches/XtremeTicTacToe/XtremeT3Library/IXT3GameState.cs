using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtremeT3Library
{
    public interface IXT3GameState
    {
        Guid RegisterCallback(IXServerUpdates callback, string name);
        void UnregisterCallback(Guid id, string name);
        void userSelection(int cell, Guid id);
        bool getGameOver();
        void GameDone();
        void Reset();
        XtremeWho.Who getWinner();
    }
}

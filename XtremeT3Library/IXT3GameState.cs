using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtremeT3Library
{
    public interface IXT3GameState
    {
        Guid RegisterCallback(IXServerUpdates callback);
        void UnregisterCallback(Guid id);
        void userSelection(int cell, Guid id);
        bool getGameOver();
        XtremeWho.Who getWinner();
    }
}

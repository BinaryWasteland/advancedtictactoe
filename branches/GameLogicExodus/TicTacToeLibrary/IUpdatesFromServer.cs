using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeLibrary
{
    public interface IUpdatesFromServer
    {
        void UpdateLocationCallback(int pos, char val);
    }
}

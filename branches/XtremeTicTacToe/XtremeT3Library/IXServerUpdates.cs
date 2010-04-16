using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtremeT3Library
{
    public interface IXServerUpdates
    {
        void UpdatePlayerCallback();
        void UpdatePlayersCallback(Guid[] id);
    }
}

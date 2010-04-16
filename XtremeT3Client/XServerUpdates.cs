using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtremeT3Library;

namespace XtremeT3Client
{
    public class XServerUpdates : MarshalByRefObject, IXServerUpdates
    {
        // Private member variables
        private XtremeT3Board XT3B;

        // Constructor
        public XServerUpdates( XtremeT3Board Xt3b )
        {
            XT3B = Xt3b;
        }

        public void UpdatePlayerCallback() 
        {
            //XT3B.UpdatePlayer(); 
        }

        public void UpdatePlayersCallback(Guid[] id) 
        {
            XT3B.UpdatePlayers(id); 
        }
    }
}

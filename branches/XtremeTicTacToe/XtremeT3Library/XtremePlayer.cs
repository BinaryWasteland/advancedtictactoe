using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtremeT3Library
{
    [Serializable]
    public class XtremePlayer
    {
        private Guid xPlayerID;

        //Constructor
        public XtremePlayer(Guid XPid)
        {
            xPlayerID = XPid;
        }

        // Public Properties
        public Guid XPlayerID 
        { 
            get 
            { 
                return xPlayerID; 
            } 
        }
    }
}

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
        private string name = "";

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

        public string XPlayerName
        {
            get { return name; }
            set { name = value; }
        }
    }
}

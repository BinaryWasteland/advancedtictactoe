using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeLibrary
{
    [Serializable]
    public class Locations
    {
        public enum LocationID
        {
            TopLeft, TopMid, TopRight, 
            MidLeft, Middle, MidRight,
            BotLeft, BotMid, BotRight
        };

        // Member variables used to identify a specific location
        private LocationID loc;

        //Constructor
        public Locations(LocationID id)
        {
            loc = id;
        }

        //Accessor methods
        public LocationID Loc
        {
            get { return loc; }
        }
    }
}

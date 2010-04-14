using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeLibrary
{
    [Serializable]
    public class Locations
    {
        private Dictionary<Guid, IUpdatesFromServer> clientCallbacks
            = new Dictionary<Guid, IUpdatesFromServer>();

        

        //  ----------------- Public methods -----------------

        // Each client calls this method after activating the 
        // Shoe object so that the Shoe will have the client's
        // callback information
        public Guid RegisterCallback(IUpdatesFromServer callback)
        {
            Guid token = Guid.NewGuid();
            clientCallbacks.Add(token, callback);
            Console.WriteLine("Registering client " + token);
            return token;
        }

        public void UnregisterCallback(Guid token)
        {
            Console.WriteLine("Unregistering client " + token);
            clientCallbacks.Remove(token);
        }

        // ----------------- Helper methods -----------------

        // This method is called whenever the Shoe object's
        // state changes and the client's need to reflect 
        // the new information in their user interfaces.
        private void Fire_UpdateShoeInfo()
        {
            foreach (IUpdatesFromServer callback in clientCallbacks.Values)
                callback.UpdateLocationCallback( char['A'] /* Value will go here */);
        }



        //public enum LocationID
        //{
        //    TopLeft, TopMid, TopRight, 
        //    MidLeft, Middle, MidRight,
        //    BotLeft, BotMid, BotRight
        //};

        //// Member variables used to identify a specific location
        //private LocationID loc;

        ////Constructor
        //public Locations(LocationID id)
        //{
        //    loc = id;
        //}

        ////Accessor methods
        //public LocationID Loc
        //{
        //    get { return loc; }
        //}
    }
}

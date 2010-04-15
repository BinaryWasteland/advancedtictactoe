using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeLibrary
{
    //[Serializable]
    public class Locations : MarshalByRefObject
    {
        // Member variables used to identify a specific location
        private char[] loc;
        private Dictionary<Guid, IUpdatesFromServer> clientCallbacks = new Dictionary<Guid, IUpdatesFromServer>();

        // Constructor
        public Locations() { }

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

        private void UpdateGridInfo()
        {
            int[] blank = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            int[] x = null;
            int[] o = null;
            foreach (IUpdatesFromServer callback in clientCallbacks.Values)
                callback.UpdateLocationCallback(blank, x, o);
        }

        //Accessor methods
        public char[] Grid
        {
            get { return loc; }
            set { loc = value; }
        }
    }
}

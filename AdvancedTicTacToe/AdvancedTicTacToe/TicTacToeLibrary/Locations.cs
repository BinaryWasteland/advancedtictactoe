using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeLibrary
{
    public class Locations : MarshalByRefObject
    {
        // Member variables used to identify a specific location
        private Dictionary<Guid, IUpdatesFromServer> clientCallbacks = new Dictionary<Guid, IUpdatesFromServer>();
        private TicTacToeState gameState = new TicTacToeState();
        private char[] grid;

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

        public void UpdateGridInfo(char[] gridloc)
        {
            grid = gridloc;
            foreach (IUpdatesFromServer callback in clientCallbacks.Values)
                callback.UpdateLocationCallback(grid);
        }

        //Accessor methods
        public char[] Grid
        {
            get
            {
                return grid;
            }
        }
    }
}

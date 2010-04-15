﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeLibrary
{
    //[Serializable]
    public class Locations : MarshalByRefObject
    {
        // Member variables used to identify a specific location
        private char[] grid = null;
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

        //Accessor methods
        public char[] Grid
        {
            get 
            { 
                return grid;
            }
        }

        public void setLoc(char value, int cell)
        {
            grid[cell] = value;
            foreach (IUpdatesFromServer callback in clientCallbacks.Values)
                callback.UpdateLocationCallback(cell, value);
        }
    }
}

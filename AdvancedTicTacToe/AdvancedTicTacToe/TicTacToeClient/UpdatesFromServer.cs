using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TicTacToeLibrary; // IUpdatesFromServer interface

namespace TicTacToeClient
{
    public class UpdatesFromServer : MarshalByRefObject, IUpdatesFromServer
    {
        private ClientForm client;

        public UpdatesFromServer(ClientForm f)
        {
            // Store a ref to the clients user interface
            client = f;
        }

        public void UpdateLocationCallback(char[] grid)
        {

        }
    }
}

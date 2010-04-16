using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Contexts;
using XtremeT3Library;

namespace XtremeT3Server
{
    [Synchronization]
    public class XT3GameState : MarshalByRefObject, IXT3GameState
    {
        private Dictionary<Guid, IXServerUpdates> clientCallbacks = new Dictionary<Guid, IXServerUpdates>();
        private RandomList<XtremePlayer> players = new RandomList<XtremePlayer>();

        public char[] Position = new char[9]; // Xs and Os in the positions they were played 
        public bool UserMovesFirst = false; // Determines which player moves first
        public int MoveNumber = 0;  // Keeps track of which move number is being played 
        public enum Symbol { USER = 'X', USER2 = 'O' }; // ASCII value of character used by each player
        public enum Who { USER = 0, USER2 = 1, NOBODY }; // Programmatic identifier used for each player
        public Who Winner = Who.USER2;
        public bool GameOver = false;

        public Guid RegisterCallback(IXServerUpdates callback) 
        { 
            if (players.Count == 2) 
            { 
                return Guid.Empty; 
            } 
            Guid id = Guid.NewGuid(); 
            clientCallbacks.Add(id, callback); 
            Console.WriteLine("Registering client " + id); 

            addPlayer(id); 
            Fire_UpdatePlayers(); 

            //whoGoesFirst(); 

            return id; 
        } 

        public void UnregisterCallback(Guid id) 
        { 
            Console.WriteLine("Unregistering client " + id); 
            clientCallbacks.Remove(id); 
            Console.WriteLine(players.Find(p => p.XPlayerID == id) + " has left."); 
            removePlayer(id); 
            Fire_UpdatePlayersList(); 
            Fire_UpdatePlayers(); 
        }

        private void addPlayer(Guid id)
        {
            XtremePlayer newPlayer = new XtremePlayer(id);

            if (players.Count == 0)
            {
                Console.WriteLine("Starting Game State");
            }
            players.Add(newPlayer);
            Console.WriteLine(id + " has joined.");

            Fire_UpdatePlayersList();
        }

        private void Fire_UpdatePlayersList()
        {
            try
            {
                List<Guid> pl = new List<Guid>();
                foreach (XtremePlayer p in players)
                {
                    pl.Add(p.XPlayerID);
                }

                //for all other players update their gui 
                foreach (IXServerUpdates callback in clientCallbacks.Values)
                {
                    callback.UpdatePlayersCallback(pl.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void Fire_UpdatePlayers()
        {
            foreach (XtremePlayer player in players)
            {
                XtremePlayer play;

                if(players.Count > 1)
                {
                    play = players.Find(p1 => p1.XPlayerID != p1.XPlayerID);
                    clientCallbacks[player.XPlayerID].UpdatePlayerCallback();
                }
            }
        }

        private void removePlayer(Guid plyr)
        {
            players.RemoveAll(p => p.XPlayerID == plyr);
        }
    }
}

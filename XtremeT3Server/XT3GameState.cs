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

        public int[] Sequence = new int[9];
        public char[] Position = new char[9]; // Xs and Os in the positions they were played 
        public bool UserMovesFirst = false; // Determines which player moves first
        public int MoveNumber = 0;  // Keeps track of which move number is being played 
        public XtremeWho.Who Winner = XtremeWho.Who.USER2;
        public bool GameOver = false;

        public XT3GameState() 
        {
            for (int i = 0; i < Position.Length; i++)
            {
                Position[i] = ' ';
            }
        }

        public bool getGameOver() { return GameOver; }
        public XtremeWho.Who getWinner() { return Winner; }

        public void userSelection(int cell, Guid id)
        {
                if (players.Count > 1)
                {
                    if (Position[cell] != ' ')
                        throw new Exception("Cell is already occupied.");

                    if (players[0].XPlayerID == id)
                    {
                        if (MoveNumber % 2 == 0)
                        {
                            Position[cell] = (char)(XtremeWho.Symbol.USER);
                            Sequence[MoveNumber++] = cell;

                            // Reset gameState's over attribute
                            updateGameOver();
                            Fire_UpdatePlayersBoards();
                        }
                    }
                    else
                    {
                        if (MoveNumber % 2 != 0)
                        {
                            Position[cell] = (char)(XtremeWho.Symbol.USER2);
                            Sequence[MoveNumber++] = cell;

                            // Reset gameState's over attribute
                            updateGameOver();
                            Fire_UpdatePlayersBoards();
                        }
                    }
                }
        }

        public void updateGameOver()
        {
            if (!GameOver)
            {
                // Assume game has been won
                GameOver = true;

                // Test for a win

                // Rows
                if (!testForWin(0, 1, 2))
                    if (!testForWin(3, 4, 5))
                        if (!testForWin(6, 7, 8))

                            // Columns
                            if (!testForWin(0, 3, 6))
                                if (!testForWin(1, 4, 7))
                                    if (!testForWin(2, 5, 8))

                                        // Diagonals
                                        if (!testForWin(0, 4, 8))
                                            if (!testForWin(2, 4, 6))
                                                if (MoveNumber < 9)
                                                    GameOver = false;
            }
        }

        private bool testForWin(int p1, int p2, int p3)
        {
            // Assume no win
            bool won = false;

            if (Position[p1] == Position[p2] && Position[p1] == Position[p3] && Position[p1] != ' ')
            {
                won = true;
                if (Position[p1] == (char)(XtremeWho.Symbol.USER))
                    Winner = XtremeWho.Who.USER;
                else
                    Winner = XtremeWho.Who.USER2;                
           }

            return won;
        }

        public void Reset()
        {
            for (int i = 0; i < 9; i++)
            {
                Position[i] = ' ';
                Sequence[i] = -1;
            }
            MoveNumber = 0;
            GameOver = false;
        }

        public Guid RegisterCallback(IXServerUpdates callback, string name) 
        { 
            if (players.Count == 2)
            { 
                return Guid.Empty; 
            } 
            Guid id = Guid.NewGuid(); 
            clientCallbacks.Add(id, callback); 
            Console.WriteLine("Registering client " + name); 

            addPlayer(id, name);
            Fire_UpdatePlayersBoards(); 

            return id; 
        } 

        public void UnregisterCallback(Guid id, string name) 
        { 
            Console.WriteLine("Unregistering client " + name); 
            clientCallbacks.Remove(id); 
            Console.WriteLine(name + " has left."); 
            removePlayer(id); 
            Fire_UpdatePlayersList();
            Fire_UpdatePlayersBoards(); 
        }

        private void addPlayer(Guid id, string name)
        {
            XtremePlayer newPlayer = new XtremePlayer(id);
            newPlayer.XPlayerName = name;
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
                List<string> pl = new List<string>();
                foreach (XtremePlayer p in players)
                {
                    pl.Add(p.XPlayerName);
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

        private void Fire_UpdatePlayersBoards()
        {
            foreach (XtremePlayer player in players)
            {
                if(players.Count > 1)
                {
                    clientCallbacks[player.XPlayerID].UpdateBoardCallback(Position);
                }
            }
            if (GameOver)
            {
                foreach (IXServerUpdates callback in clientCallbacks.Values)
                {
                    callback.GameOverCallback();
                }
            }
        }

        private void removePlayer(Guid plyr)
        {
            players.RemoveAll(p => p.XPlayerID == plyr);
        }
    }
}

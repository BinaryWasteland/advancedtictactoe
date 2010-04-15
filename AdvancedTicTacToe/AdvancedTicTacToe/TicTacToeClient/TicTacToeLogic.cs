/*
 *  Module:         T3Logic.cs 
 *  Author:         T. Haworth
 *  Date:           January 15, 2009
 *  Description:    Tic-Tac-Toe game logic.
 */

using System;
using System.Collections.Generic;

namespace TicTacToeClient
{
    public class TicTacToeLogic : IDisposable
    {
        // ----------- Private Member Variables -----------

        private Random random = new Random();
        private TicTacToeState gameState = new TicTacToeState();
        private TicTacToeData games = new TicTacToeData();

        // ---------------- Public Methods ----------------

        // Initialize game state to start a new interactive game
        // and process the first one or two moves
        public TicTacToeState NextGame()
        {
            resetArrays();

            if (gameState.Winner < 0)
                // Draw: Toggle who starts
                gameState.UserMovesFirst = !gameState.UserMovesFirst;
            else
                // Win: Loser starts
                gameState.UserMovesFirst = (gameState.Winner != TicTacToeState.Who.USER);

            gameState.MoveNumber = 0;
            gameState.GameOver = false;
            gameState.Winner = TicTacToeState.Who.NOBODY;

            if (!gameState.UserMovesFirst)
                addProgMoveToGrid();

            return gameState;
        }

        // Process the next user move in the current interactive game 
        public TicTacToeState NextMove(int cell)
        {
            addUserMoveToGrid(cell);

            if (!gameState.GameOver)
                addProgMoveToGrid();

            return gameState;
        }

        // Retrieve and return a game state from the data layer for a 
        // game with the specified game id
        public TicTacToeState ReplayGame(int id)
        {
            // Refresh games dataset to incorporate all game IDs
            games.Dispose();
            games = new TicTacToeData();

            // Initialize some game state attributes
            resetArrays();
            gameState.GameOver = false;
            gameState.Id = id;
            gameState.Winner = TicTacToeState.Who.NOBODY;

            // Retrieve moves information from database
            games.RetrieveGame(ref gameState);

            // Update game state with identity of the winner
            updateGameOver();

            return gameState;
        }

        // --------------- Helper Methods -----------------

        private void resetArrays()
        {
            for (int i = 0; i < gameState.Position.Length; ++i)
            {
                gameState.Sequence[i] = -1;
                gameState.Position[i] = ' ';
            }
        }

        private void addUserMoveToGrid(int cell)
        {
            if (gameState.Position[cell] != ' ')
                throw new ArgumentException("Cell is already occupied.");
            //gameState.Position[cell] = gameState.Symbols[(int)(T3State.Who.USER)];
            gameState.Position[cell] = (char)(TicTacToeState.Symbol.USER);
            gameState.Sequence[gameState.MoveNumber++] = cell;

            // Reset gameState's over attribute
            updateGameOver();
        }

        private void addProgMoveToGrid()
        {
            int cell;

            // Get a random move
            List<int> availableCells = new List<int>();
            for (int i = 0; i < gameState.Position.Length; ++i)
                if (gameState.Position[i] == ' ')
                    availableCells.Add(i);
            cell = availableCells[random.Next(availableCells.Count)];

            // Make move
            gameState.Position[cell] = (char)(TicTacToeState.Symbol.USER2);
            gameState.Sequence[gameState.MoveNumber++] = cell;

            // Reset gameState's over attribute
            updateGameOver();
        }

        public void updateGameOver()
        {
            if (!gameState.GameOver)
            {
                // Assume game has been won
                gameState.GameOver = true;

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
                                                if (gameState.MoveNumber < 9)
                                                    gameState.GameOver = false;
                                                else
                                                    // It's a draw; save the game
                                                    games.SaveGame(gameState);
            }
        }

        private bool testForWin(int p1, int p2, int p3)
        {
            // Assume no win
            bool won = false;

            if (gameState.Position[p1] == gameState.Position[p2]
                && gameState.Position[p1] == gameState.Position[p3]
                && gameState.Position[p1] != ' ')
            {
                won = true;
                //if (gameState.Position[p1] == gameState.Symbols[(int)(T3State.Who.USER)])
                if (gameState.Position[p1] == (char)(TicTacToeState.Symbol.USER))
                    gameState.Winner = TicTacToeState.Who.USER;
                else
                    gameState.Winner = TicTacToeState.Who.USER2;

                // Save the game
                games.SaveGame(gameState);
            }

            return won;
        }

        // ------------- Finalization Code ----------------

        #region IDisposable Members

        public void Dispose()
        {
            games.Dispose();
        }

        #endregion
    }
}
/*
 *  Module:         T3Data.cs 
 *  Author:         T. Haworth
 *  Date:           January 15, 2009
 *  Description:    Manages Tic-Tac-Toe data layer for managing a history of past games.
 */

using System;
using System.Collections.Generic;

using System.Data;          // Generic ADO.NET namespace
using System.Data.OleDb;    // OleDb (Access) namespace

namespace TicTacToeClient
{

    public class TicTacToeData : IDisposable
    {
        // ----------- Private Member Variables -----------

        private DataSet dsHistory;
        private OleDbConnection con;
        private OleDbDataAdapter adGames;

        // ------------------ Constructor -----------------

        public TicTacToeData()
        {
            setupGames();
        }

        // ---------------- Public Methods ----------------

        // Save a new complete game to the games history dataset
        public void SaveGame(TicTacToeState gameState)
        {
            try
            {
                // Add new game to the dataset
                DataRow newGame = dsHistory.Tables["Games"].NewRow();
                newGame["UserMovesFirst"] = gameState.UserMovesFirst;
                for (int i = 0; i < gameState.Sequence.Length; ++i)
                    newGame["Move" + i] = gameState.Sequence[i];
                dsHistory.Tables["Games"].Rows.Add(newGame);
            }
            catch (ConstraintException)
            {
                // If the catch block is used it means the game is not unique!
            }
        }

        // Retrieve and return by-reference a game state from 
        // the games history dataset 
        public bool RetrieveGame(ref TicTacToeState gameState)
        {
            bool found = false;

            // Look for a game that matches the given id
            string filter = "ID = " + gameState.Id;
            DataRow[] games = dsHistory.Tables["Games"].Select(filter);

            // Populate game state using the first game record (should be one at most)
            if (games.Length > 0)
            {
                found = true;
                gameState.UserMovesFirst = (bool)games[0]["UserMovesFirst"];
                int pos;
                char sym;
                for (int m = 0; m < 9; m++)
                {
                    // Add entry to Sequence array
                    pos = (short)games[0]["Move" + m];
                    gameState.Sequence[m] = pos;

                    // Add entry to Position array
                    if ((gameState.UserMovesFirst && m % 2 == 0) || (!gameState.UserMovesFirst && m % 2 == 1))
                        sym = (char)(TicTacToeState.Symbol.USER);
                    else
                        sym = (char)(TicTacToeState.Symbol.USER2);
                    if (pos > -1)
                        gameState.Position[pos] = sym;
                }
            }
            else
                throw new DataException("Nonexistent game id.");

            return found;
        }

        // --------------- Helper Methods -----------------

        private void setupGames()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=TicTacToe.mdb");

            // Create data adpater
            adGames = new OleDbDataAdapter();
            adGames.SelectCommand = new OleDbCommand("SELECT * FROM Games", con);
            adGames.InsertCommand = new OleDbCommand("INSERT INTO Games (UserMovesFirst,"
                + " Move0, Move1, Move2, Move3, Move4, Move5, Move6, Move7, Move8) "
                + "VALUES(@UserMovesFirst, @Move0, @Move1, @Move2, @Move3, @Move4, @Move5, @Move6, @Move7, @Move8)", con);

            // Create parameter variables for the insert command
            adGames.InsertCommand.Parameters.Add("@UserMovesFirst", OleDbType.Boolean, 1, "UserMovesFirst");
            adGames.InsertCommand.Parameters["@UserMovesFirst"].SourceVersion = DataRowVersion.Current;
            for (int i = 0; i < 9; ++i)
            {
                adGames.InsertCommand.Parameters.Add("@Move" + i, OleDbType.SmallInt, 0, "Move" + i);
                adGames.InsertCommand.Parameters["@Move" + i].SourceVersion = DataRowVersion.Current;
            }

            // Fill the data set
            dsHistory = new DataSet();
            con.Open();
            adGames.Fill(dsHistory, "Games");
            con.Close();

            // Add a primary key that will prevent duplicate entries in the dataset
            DataColumn[] keys = new DataColumn[9];
            keys[0] = dsHistory.Tables["Games"].Columns["UserMovesFirst"];
            for (int col = 0; col < 8; ++col)
                keys[col + 1] = dsHistory.Tables["Games"].Columns["Move" + col];
            dsHistory.Tables["Games"].PrimaryKey = keys;
        }

        // ------------- Finalization Code ----------------

        #region IDisposable Members

        public void Dispose()
        {
            if (dsHistory.HasChanges())
                adGames.Update(dsHistory, "Games");
        }

        #endregion
    }
}
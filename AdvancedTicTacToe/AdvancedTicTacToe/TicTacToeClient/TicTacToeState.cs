/*
 *  Module:         T3State.cs 
 *  Author:         T. Haworth
 *  Date:           January 15, 2009
 *  Description:    Tic-Tac-Toe game state. Used to pass information between gui, 
 *                  logic and data layers.
 */

using System;

namespace TicTacToeClient
{
    public class TicTacToeState
    {
        public int[] Sequence = new int[9]; // A sequence of which cells played by cell index
        public char[] Position = new char[9]; // Xs and Os in the positions they were played 
        public bool UserMovesFirst = false; // Determines which player moves first
        public int MoveNumber = 0;  // Keeps track of which move number is being played 
        public enum Symbol { USER = 'X', USER2 = 'O' }; // ASCII value of character used by each player
        public enum Who { USER = 0, USER2 = 1, NOBODY }; // Programmatic identifier used for each player
        public Who Winner = Who.USER2;
        public bool GameOver = false;
        public int Id = 0; // ID field used to identify a unique game in the database
    }
}
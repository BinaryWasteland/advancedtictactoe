/*
 *  Module:         T3gui.cs 
 *  Author:         T. Haworth
 *  Date:           January 14, 2009
 *  Description:    Graphical user interface for interactive Tic-Tac-Toe game.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.Remoting;  // RemotingConfiguration class
using System.Runtime.Remoting.Channels; // ChannelServices class
using TicTacToeLibrary;

namespace TicTacToeClient
{
    public partial class ClientForm : Form
    {
        // Member variables & constants
        private Locations loc = new Locations();
        private Guid callbackId;
        private const int CELL_MARGIN = 16;     // Used for aligning grid cells
        private const int CELL_WIDTH = 80;      // Used for aligning grid cells
        private TicTacToeLogic t3 = new TicTacToeLogic();     // Game logic layer
        private bool playMode = false;          // Used to disable cells' click event during 'replays'
        private Label[] cells = new Label[9];   // Tic-Tac-Toe cell controls

        // C'tor method
        public ClientForm()
        {
            InitializeComponent();

            try
            {
                // Load the remoting config file
                RemotingConfiguration.Configure("TicTacToeClient.exe.config", false);

                // Activate a TicTacToeLibrary.Locations object
                loc = (Locations)Activator.GetObject(typeof(Locations), "http://localhost:10001/locations.soap");

                // Register this client instance for server callbacks
                callbackId = loc.RegisterCallback(new UpdatesFromServer(this));

                // Display client's GUID in form caption
                this.Text += " [" + callbackId.ToString() + "]";

                // Setup label controls for grid cells
                createCells();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tic-Tac-Toe Error");
            }
        }

        private void createCells()
        {
            int x, y; // horizontal & vertical positions
            int xIdx, yIdx; // horizontal & vertical indexes
            for (int i = 0; i < cells.Length; ++i)
            {
                // Get horizontal and vertical offsets
                xIdx = getXOffset(i);
                yIdx = getYOffset(i);

                // Get positional coordinates for each cell
                x = CELL_MARGIN + xIdx * CELL_WIDTH;
                y = CELL_MARGIN + yIdx * CELL_WIDTH;

                // Initialize label that represents this card
                cells[i] = new Label();
                cells[i].BackColor = System.Drawing.SystemColors.ControlLightLight;
                cells[i].Location = new System.Drawing.Point(x, y);
                cells[i].TabIndex = i;  // Use to identify cell index
                cells[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                cells[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                cells[i].Size = new System.Drawing.Size(CELL_WIDTH, CELL_WIDTH);
                cells[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                cells[i].Text = "";
                cells[i].Click += new System.EventHandler(cell_Click);
                Controls.Add(cells[i]);
            }
        }

        private int getXOffset(int arrayIdx)
        {
            return arrayIdx % 3;
        }

        private int getYOffset(int arrayIdx)
        {
            return arrayIdx / 3;
        }

        private void cell_Click(object sender, System.EventArgs e)
        {
            if (playMode)
            {
                Label cell = (Label)sender;

                // Label's TabIndex property indicates position in grid
                int cellIdx = ((Label)sender).TabIndex;

                // Process the requested move
                makeUserMove(cellIdx);
            }
        }

        // Pass requested move to TictacToe logic layer and display move
        private void makeUserMove(int cell)
        {
            if (!btnNew.Enabled)
            {
                try
                {
                    TicTacToeState gameState = t3.NextMove(cell);
                    refreshGrid(gameState.Position);
                    if (gameState.GameOver)
                    {
                        playMode = false;
                        reportWinner(gameState);
                        btnNew.Enabled = btnReplay.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Tic-Tac-Toe Error");
                }
            }
        }

        // Indirectly called by the server to update the UI
        private delegate void FormUpdateDelegate(char[] grid);

        public void refreshGrid(char[] grid)
        {
            try
            {
                for (int i = 0; i < cells.Length; ++i)
                {

                    if (cells[i].InvokeRequired)
                    {
                        // This will happen if current thread isn't the UI's own thread
                        cells[i].BeginInvoke(new FormUpdateDelegate(refreshGrid),
                            grid);
                    }
                    else
                    {
                        cells[i].Text = grid[i].ToString();
                        //// This will happen if the current thread IS the UI's thread
                        //txtShoeCount.Text = numCardsInShoe.ToString();
                        //numDecks.Value = numDecksInShoe;

                        //// Only refresh the "hand" if the shoe is "refreshed"
                        //if (numCardsInShoe == 52 * numDecksInShoe)
                        //{
                        //    lstCards.Items.Clear();
                        //    txtHandCount.Text = "0";
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Tic-Tac-Toe Error");
            }

            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            playMode = true;
            TicTacToeState gameState = t3.NextGame();
            refreshGrid(gameState.Position);
            btnNew.Enabled = btnReplay.Enabled = false;
        }

        private void reportWinner(TicTacToeState gameState)
        {
            if (gameState.Winner == TicTacToeState.Who.USER)
                MessageBox.Show("Game Over! " + (char)TicTacToeState.Symbol.USER + " is the winner.");
            else if (gameState.Winner == TicTacToeState.Who.USER2)
                MessageBox.Show("Game Over! " + (char)TicTacToeState.Symbol.USER2 + " is the winner.");
            else
                MessageBox.Show("Game Over! It's a draw!");
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            // Disable buttons
            btnNew.Enabled = btnReplay.Enabled = false;
            // Clear previous game for UI
            foreach (Label cell in cells)
                cell.Text = "";

            // Retrieve requested game
            try
            {
                TicTacToeState gameState = t3.ReplayGame(int.Parse(txtId.Text));
                foreach (int p in gameState.Sequence)
                    if (p > -1)
                    {
                        // Pause 1 sec.
                        System.Threading.Thread.Sleep(1000);

                        // Display next move
                        cells[p].Text = gameState.Position[p].ToString();
                        this.Refresh();
                    }
                reportWinner(gameState);
            }
            catch (DataException ex)
            {
                MessageBox.Show(ex.Message, "Tic-Tac-Toe Error");
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid game ID. Enter a positive integer.", "Tic-Tac-Toe Error");
            }

            // Enable buttons
            btnNew.Enabled = btnReplay.Enabled = true;
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {

        }


    }
}

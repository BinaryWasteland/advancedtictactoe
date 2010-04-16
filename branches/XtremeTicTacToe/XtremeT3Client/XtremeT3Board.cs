using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using XtremeT3Library;

namespace XtremeT3Client
{
    public partial class XtremeT3Board : Form
    {
        private Guid callbackId;
        private const int CELL_MARGIN = 16;     // Used for aligning grid cells 
        private const int CELL_WIDTH = 80;      // Used for aligning grid cells 
        private Label[] cells = new Label[9];   // Tic-Tac-Toe cell controls
        private IXT3GameState gameState;

        public XtremeT3Board()
        {
            InitializeComponent();

            try
            {
                // Load the remoting config file 
                RemotingConfiguration.Configure("XtremeT3Client.exe.config", false);

                // Activate a TicTacToeLibrary.Locations object 
                gameState = (IXT3GameState)Activator.GetObject(typeof(IXT3GameState), "http://localhost:10001/xt3gamestate.soap");

                // Register this client instance for server callbacks 
                callbackId = gameState.RegisterCallback(new XServerUpdates(this));
                if (callbackId.Equals(Guid.Empty))
                {
                    throw new Exception("There are already two players. You cannot be added at this time.");
                }

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
                xIdx = i % 3;
                yIdx = i / 3;

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

        private void cell_Click(object sender, System.EventArgs e)
        {
            try
            {
                Label cell = (Label)sender;

                // Label's TabIndex property indicates position in grid 
                int cellIdx = ((Label)sender).TabIndex;

                // Process the requested move 
                gameState.userSelection(cellIdx, callbackId);

                /*if (gameState.getGameOver())
                {
                    reportWinner();
                }*/
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Tic-Tac-Toe Error");
            }
        }

        private delegate void FormUpdatePlayers(Guid[] plIDs);
        public void UpdatePlayers(Guid[] plIDs)
        {
            if (plIDs != null && plIDs.Length != 0)
            {
                try
                {
                    if (lblOpposition.InvokeRequired)
                        lblOpposition.BeginInvoke(new FormUpdatePlayers(UpdatePlayers), new Object[] { plIDs });
                    else
                    {
                        foreach (Guid n in plIDs)
                        {
                            if (plIDs.ToString() != n.ToString())
                            {
                                lblOpposition.Text = n.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private delegate void FormUpdateBoard(char[] board);
        public void UpdateBoard(char[] board)
        {
            try
            {
                if (cells[0].InvokeRequired)
                {
                    // This will happen if current thread isn't the UI's own thread
                    cells[0].BeginInvoke(new FormUpdateBoard(UpdateBoard), board);
                }
                else
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        cells[i].Text = board[i].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Tic-Tac-Toe Error");
            }
        }

        public void ReportWinner()
        {
            if (gameState.getWinner() == XtremeWho.Who.USER)
                MessageBox.Show("Game Over! " + (char)XtremeWho.Symbol.USER + " is the winner.");
            else if (gameState.getWinner() == XtremeWho.Who.USER2)
                MessageBox.Show("Game Over! " + (char)XtremeWho.Symbol.USER2 + " is the winner.");
            else
                MessageBox.Show("Game Over! It's a draw!");
        }
    }
}

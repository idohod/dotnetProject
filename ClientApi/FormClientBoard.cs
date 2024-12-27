using Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApi
{
    public partial class FormClientBoard : Form
    {
        private PieceModels.HalfChessGame game;
        private Bitmap userDrawingBitmap;
        private Graphics userDrawingGraphics;
        private bool isDrawing;
        private Point lastMousePos;
        private int timeLeft;
        private int animationStep;

        // For a simple "select and move"
        private PieceModels.Cell selectedCell;
        private const int CELL_SIZE = 60;

        public FormClientBoard()
        {
            InitializeComponent();
        }

        private void FormClientBoard_Load(object sender, EventArgs e)
        {
            // Initialize a new HalfChessGame
            game = new PieceModels.HalfChessGame();

            // Populate time combo
            comboBoxTime.Items.AddRange(new object[] { 10, 20, 30, 60 });
            comboBoxTime.SelectedIndex = 1;

            // Prepare scribble Bitmap
            userDrawingBitmap = new Bitmap(panelBoard.Width, panelBoard.Height);
            userDrawingGraphics = Graphics.FromImage(userDrawingBitmap);
            userDrawingGraphics.Clear(Color.Transparent);

            // Default time
            timeLeft = 20;
            lblTimeLeft.Text = "Time left: " + timeLeft;

            // Force repaint
            panelBoard.Invalidate();
        }

        private void panelBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    Rectangle rect = new Rectangle(c * CELL_SIZE, r * CELL_SIZE, CELL_SIZE, CELL_SIZE);
                    Color cellColor = ((r + c) % 2 == 0) ? Color.Beige : Color.Brown;

                    if (selectedCell != null && selectedCell.Row == r && selectedCell.Col == c)
                    {
                        cellColor = Color.LightBlue;
                    }

                    using (var brush = new SolidBrush(cellColor))
                    {
                        g.FillRectangle(brush, rect);
                    }
                    using (var pen = new Pen(Color.Black))
                    {
                        g.DrawRectangle(pen, rect);
                    }

                    var boardCell = game.GameBoard.GetCell(r, c);
                    if (boardCell != null && boardCell.OccupiedBy != null)
                    {
                        var piece = boardCell.OccupiedBy;
                        string symbol = PieceSymbol(piece);

                        // Use a darker color for white pieces to improve visibility
                        Color textColor = piece.IsWhite ? Color.Silver : Color.Black;

                        using (var textBrush = new SolidBrush(textColor))
                        {
                            g.DrawString(symbol, this.Font, textBrush, rect.X + 20, rect.Y + 20);
                        }
                    }
                }
            }

            g.DrawImage(userDrawingBitmap, Point.Empty);
        }


        // Minimal: return a single-letter or short string for each piece type
        private string PieceSymbol(PieceModels.Piece piece)
        {
            if (piece == null) return "";
            switch (piece.Type)
            {
                case PieceModels.PieceType.Pawn: return "P";
                case PieceModels.PieceType.Rook: return "R";
                case PieceModels.PieceType.Knight: return "N";
                case PieceModels.PieceType.Bishop: return "B";
                case PieceModels.PieceType.King: return "K";
                default: return "?";
            }
        }

        // Handle the user's attempt to click & move a piece
        private void panelBoard_MouseDown(object sender, MouseEventArgs e)
        {
            // Convert click coordinates to board row/col
            int col = e.X / CELL_SIZE;
            int row = e.Y / CELL_SIZE;
            if (row < 0 || row > 7 || col < 0 || col > 3) return;

            PieceModels.Cell clickedCell = game.GameBoard.GetCell(row, col);

            // If we have no cell selected yet, try to select a piece
            if (selectedCell == null)
            {
                if (clickedCell.OccupiedBy != null)
                {
                    // Check if piece color matches the current turn
                    if (clickedCell.OccupiedBy.IsWhite == game.IsWhiteTurn)
                    {
                        selectedCell = clickedCell;
                    }
                }
            }
            else
            {
                // We already have a selected cell, so let's attempt a move
                bool success = game.MovePiece(selectedCell.Row, selectedCell.Col, row, col);
                selectedCell = null;
            }

            panelBoard.Invalidate();
        }

        private void panelBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    userDrawingGraphics.DrawLine(pen, lastMousePos, e.Location);
                }
                lastMousePos = e.Location;
                panelBoard.Invalidate();
            }
        }

        private void panelBoard_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void btnClearDrawings_Click(object sender, EventArgs e)
        {
            userDrawingGraphics.Clear(Color.Transparent);
            panelBoard.Invalidate();
        }

        private void TurnTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            lblTimeLeft.Text = "Time left: " + timeLeft;
            if (timeLeft <= 0)
            {
                turnTimer.Stop();
                MessageBox.Show("Time's up!");
            }
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;
            animationStep %= 6;
            panelBoard.Invalidate();
        }

        private void panelBoard_MouseDown(object sender, MouseEventArgs e, bool isDrawMode)
        {
            // This line is just to avoid collisions if you used the Designer to create this event:
            // Typically you'll unify your MouseDown logic in one method. 
        }

        private void panelBoard_MouseMove(object sender, MouseEventArgs e, bool isDrawMode)
        {
            // Same note as above
        }
    }
}

using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
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

        private PieceModels.Cell selectedCell;
        private const int CELL_SIZE = 60;

        private bool isTimeSelected;
        private Timer checkFlashTimer;
        private Rectangle kingCheckSquare;
        private bool isFlashingRed;
        private bool isGameOver = false;
        private DateTime gameStartTime;
        //private Game CurrentGame;
        private int CurruntPlayerId;
        private const string PATH = "https://localhost:7272/";
        private static HttpClient client = new HttpClient();




        public FormClientBoard(Player player)


        {

            CurruntPlayerId = player.Id;
            MessageBox.Show($"Welcome {player.Name.Trim()}!\nYour details:\nId: {player.Id}\nPhone: {player.Phone}\nCountry: {player.Country}\nNumber of Games: {player.NumOfGames}",
                            "WELCOME",
                            MessageBoxButtons.OK);
            InitializeComponent();

            isTimeSelected = false;
            turnTimer = new Timer { Interval = 1000 };
            turnTimer.Tick += TurnTimer_Tick;
        }

        private void FormClientBoard_Load(object sender, EventArgs e)
        {
            client.BaseAddress = new Uri(PATH);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));


            gameStartTime = DateTime.UtcNow;
           // CurrentGame = new Game(CurruntPlayerId, gameStartTime);

            game = new PieceModels.HalfChessGame();
            game.MoveMade += OnMoveMade;

            comboBoxTime.Items.AddRange(new object[] { 10, 20, 30, 60 });
            comboBoxTime.SelectedIndex = 1;

            this.DoubleBuffered = true;

            userDrawingBitmap = new Bitmap(panelBoard.Width, panelBoard.Height);
            userDrawingGraphics = Graphics.FromImage(userDrawingBitmap);
            userDrawingGraphics.Clear(Color.Transparent);

            timeLeft = 20;
            lblTimeLeft.Text = "Time left: " + timeLeft;

            panelBoard.Invalidate();
        }
        private void animationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;
            animationStep %= 6;
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
                    if (rect == kingCheckSquare && isFlashingRed)
                    {
                        cellColor = Color.Red;
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
                    if (boardCell?.OccupiedBy != null)
                    {
                        var piece = boardCell.OccupiedBy;
                        string symbol = PieceSymbol(piece);

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

        private void panelBoard_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isDrawing = true;
                lastMousePos = e.Location;
            }

            int col = e.X / CELL_SIZE;
            int row = e.Y / CELL_SIZE;
            if (row < 0 || row > 7 || col < 0 || col > 3) return;

            PieceModels.Cell clickedCell = game.GameBoard.GetCell(row, col);

            if (selectedCell == null)
            {
                if (clickedCell?.OccupiedBy != null && clickedCell.OccupiedBy.IsWhite == game.IsWhiteTurn)
                {
                    selectedCell = clickedCell;
                }
            }
            else
            {
                bool success = game.MovePiece(selectedCell.Row, selectedCell.Col, row, col);
                ResetTurnTimer();
                comboBoxTime.Enabled = false;

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
            if (isGameOver) return; // Prevent actions after the game is over

            timeLeft--;
            lblTimeLeft.Text = "Time left: " + timeLeft;

            if (timeLeft <= 0)
            {
                turnTimer.Stop();
                MessageBox.Show("Time's up!");
                EndGame("Time's up!");
            }
        }

        private void comboBoxTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isTimeSelected)
            {
                timeLeft = Convert.ToInt32(comboBoxTime.SelectedItem);
                lblTimeLeft.Text = "Time left: " + timeLeft;
                turnTimer.Start();
                comboBoxTime.Enabled = true;
                isTimeSelected = true;
            }
            else
            {
                comboBoxTime.Enabled = false;
                ResetTurnTimer();
            }
        }

        private void ResetTurnTimer()
        {
            turnTimer.Stop();
            timeLeft = Convert.ToInt32(comboBoxTime.SelectedItem);
            lblTimeLeft.Text = "Time left: " + timeLeft;
            turnTimer.Start();
        }

        private void OnMoveMade()
        {

            UpdateGameState();
        }

        public void UpdateGameState()
        {
            
            if (game.IsCheckmate(true))
            {
                MessageBox.Show("Checkmate! Black wins!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EndGame("");
                return;
            }

            if (game.IsCheckmate(false))
            {
                MessageBox.Show("Checkmate! White wins!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ResetGame();
                return;
            }

            // Flash the king's square if in check
            bool isWhiteKingInCheck = game.IsKingInCheck(true);
            bool isBlackKingInCheck = game.IsKingInCheck(false);

            if (isWhiteKingInCheck || isBlackKingInCheck)
            {
                bool isWhiteKing = isWhiteKingInCheck;
                var kingCell = game.FindKingCell(isWhiteKing);

                if (kingCell != null)
                {
                    StartCheckAnimation(kingCell.Row, kingCell.Col);
                }
            }
            else
            {
                StopCheckAnimation();
            }
        }
        private  void EndGame(string result)
        {
            if (isGameOver) return;
            isGameOver = true;
           
                DateTime gameEndTime = DateTime.UtcNow;
                int duration = (int)(gameEndTime - gameStartTime).TotalSeconds;
            Application.Exit();
              
        }

       


        private void LogGameResult(Game game)
        {
            // Example method to log game data, adjust as necessary
            Console.WriteLine(game.ToString());
        }
        static async Task<Uri> CreateGameAsync(Game game)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(PATH + "api/TblGames", game);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
        private void StartCheckAnimation(int row, int col)
        {
            kingCheckSquare = new Rectangle(col * CELL_SIZE, row * CELL_SIZE, CELL_SIZE, CELL_SIZE);
            isFlashingRed = true;

            if (checkFlashTimer == null)
            {
                checkFlashTimer = new Timer { Interval = 500 };
                checkFlashTimer.Tick += CheckFlashTimer_Tick;
            }

            checkFlashTimer.Start();
        }

        private void StopCheckAnimation()
        {
            if (checkFlashTimer != null)
            {
                checkFlashTimer.Stop();
                checkFlashTimer.Dispose();
                checkFlashTimer = null;
            }

            kingCheckSquare = Rectangle.Empty;
            panelBoard.Invalidate();
        }

        private void CheckFlashTimer_Tick(object sender, EventArgs e)
        {
            isFlashingRed = !isFlashingRed;
            panelBoard.Invalidate(kingCheckSquare);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            game.MoveMade -= OnMoveMade;
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}
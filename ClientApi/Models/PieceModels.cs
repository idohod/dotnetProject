using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class PieceModels
    {
        public enum PieceType
        {
            None,
            Pawn,
            King,
            Rook,
            Knight,
            Bishop
        }

        public class Piece
        {
            public PieceType Type { get; set; }
            public bool IsWhite { get; set; }

            public Piece(PieceType type, bool isWhite)
            {
                Type = type;
                IsWhite = isWhite;
            }
        }

        public class Cell
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public Piece OccupiedBy { get; set; }

            public Cell(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }

        public class Board
        {
            private const int ROWS = 8;   // 8 rows
            private const int COLS = 4;   // 4 columns (Half Chess)
            private readonly Cell[,] cells;

            public Board()
            {
                cells = new Cell[ROWS, COLS];
                for (int r = 0; r < ROWS; r++)
                {
                    for (int c = 0; c < COLS; c++)
                    {
                        cells[r, c] = new Cell(r, c);
                    }
                }
            }

            public Cell GetCell(int row, int col)
            {
                if (row < 0 || row >= ROWS || col < 0 || col >= COLS)
                    return null;
                return cells[row, col];
            }

            public void PlacePiece(int row, int col, Piece piece)
            {
                var cell = GetCell(row, col);
                if (cell != null)
                {
                    cell.OccupiedBy = piece;
                }
            }
        }

        public class HalfChessGame
        {
            public Board GameBoard { get; private set; }
            public bool IsWhiteTurn { get; private set; }

            public HalfChessGame()
            {
                GameBoard = new Board();
                IsWhiteTurn = true;
                SetupInitialPosition();
            }

            private void SetupInitialPosition()
            {
                // Black (top)
                GameBoard.PlacePiece(0, 0, new Piece(PieceType.Rook, false));
                GameBoard.PlacePiece(0, 1, new Piece(PieceType.Knight, false));
                GameBoard.PlacePiece(0, 2, new Piece(PieceType.Bishop, false));
                GameBoard.PlacePiece(0, 3, new Piece(PieceType.King, false));

                for (int col = 0; col < 4; col++)
                {
                    GameBoard.PlacePiece(1, col, new Piece(PieceType.Pawn, false));
                }

                // White (bottom)
                GameBoard.PlacePiece(7, 0, new Piece(PieceType.Rook, true));
                GameBoard.PlacePiece(7, 1, new Piece(PieceType.Knight, true));
                GameBoard.PlacePiece(7, 2, new Piece(PieceType.Bishop, true));
                GameBoard.PlacePiece(7, 3, new Piece(PieceType.King, true));

                for (int col = 0; col < 4; col++)
                {
                    GameBoard.PlacePiece(6, col, new Piece(PieceType.Pawn, true));
                }
            }

            public bool MovePiece(int fromRow, int fromCol, int toRow, int toCol)
            {
                var fromCell = GameBoard.GetCell(fromRow, fromCol);
                var toCell = GameBoard.GetCell(toRow, toCol);
                if (fromCell == null || toCell == null) return false;
                if (fromCell.OccupiedBy == null) return false;

                // Must move your own color
                if (fromCell.OccupiedBy.IsWhite != IsWhiteTurn) return false;

                // Check piece-specific rules
                if (!IsLegalMove(fromCell.OccupiedBy, fromRow, fromCol, toRow, toCol))
                    return false;

                // Capture or move (overwrite)
                toCell.OccupiedBy = fromCell.OccupiedBy;
                fromCell.OccupiedBy = null;

                // Switch turns
                IsWhiteTurn = !IsWhiteTurn;
                return true;
            }

            /// <summary>
            /// Checks if moving 'piece' from (fromRow, fromCol) to (toRow, toCol) 
            /// is valid based on piece type. Ignores checks, castling, en passant, etc.
            /// </summary>
            private bool IsLegalMove(Piece piece, int fromRow, int fromCol, int toRow, int toCol)
            {
                // Can't "move" to the same square
                if (fromRow == toRow && fromCol == toCol) return false;

                int rowDiff = toRow - fromRow;
                int colDiff = toCol - fromCol;

                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        return IsLegalPawnMove(piece, fromRow, fromCol, toRow, toCol);

                    case PieceType.King:
                        // King moves 1 square in any direction
                        if (Math.Abs(rowDiff) <= 1 && Math.Abs(colDiff) <= 1)
                            return true;
                        return false;

                    case PieceType.Rook:
                        // Rook moves along row or column
                        if (rowDiff == 0 || colDiff == 0)
                        {
                            // Check if path is blocked
                            return !IsPathBlocked(fromRow, fromCol, toRow, toCol);
                        }
                        return false;

                    case PieceType.Bishop:
                        // Bishop moves diagonally
                        if (Math.Abs(rowDiff) == Math.Abs(colDiff))
                        {
                            return !IsPathBlocked(fromRow, fromCol, toRow, toCol);
                        }
                        return false;

                    case PieceType.Knight:
                        // Knight moves in "L" shape
                        if ((Math.Abs(rowDiff) == 2 && Math.Abs(colDiff) == 1)
                         || (Math.Abs(rowDiff) == 1 && Math.Abs(colDiff) == 2))
                            return true;
                        return false;

                    default:
                        return false;
                }
            }

            /// <summary>
            /// Checks if a Pawn move is legal in "standard" chess sense:
            /// no sideways, no en passant, no promotion. 
            /// White pawns move up (-1 row), black pawns move down (+1 row).
            /// </summary>
            private bool IsLegalPawnMove(Piece pawn, int fromRow, int fromCol, int toRow, int toCol)
            {
                int direction = pawn.IsWhite ? -1 : 1;

                // Single-step forward
                if (toCol == fromCol && toRow == fromRow + direction)
                {
                    var targetCell = GameBoard.GetCell(toRow, toCol);
                    if (targetCell?.OccupiedBy == null)
                        return true;
                    return false;
                }

                // Two-step from start row
                // White pawns start at row=6, black at row=1 (8 rows total)
                int startRow = pawn.IsWhite ? 6 : 1;
                if (fromRow == startRow && toCol == fromCol && toRow == fromRow + 2 * direction)
                {
                    var cellOne = GameBoard.GetCell(fromRow + direction, fromCol);
                    var cellTwo = GameBoard.GetCell(toRow, toCol);
                    if (cellOne?.OccupiedBy == null && cellTwo?.OccupiedBy == null)
                        return true;
                    return false;
                }

                // Diagonal capture
                if (Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + direction)
                {
                    var targetCell = GameBoard.GetCell(toRow, toCol);
                    // Must contain opponent piece
                    if (targetCell?.OccupiedBy != null
                        && targetCell.OccupiedBy.IsWhite != pawn.IsWhite)
                    {
                        return true;
                    }
                    return false;
                }

                // If your Half Chess has sideways moves for pawns, add here.

                return false;
            }

            /// <summary>
            /// Checks if any piece is blocking the path (except the final cell)
            /// between (fromRow, fromCol) and (toRow, toCol).
            /// Used for Rook and Bishop.
            /// </summary>
            private bool IsPathBlocked(int fromRow, int fromCol, int toRow, int toCol)
            {
                int rowDiff = toRow - fromRow;
                int colDiff = toCol - fromCol;

                int rowStep = (rowDiff == 0) ? 0 : (rowDiff > 0 ? 1 : -1);
                int colStep = (colDiff == 0) ? 0 : (colDiff > 0 ? 1 : -1);

                int steps = Math.Max(Math.Abs(rowDiff), Math.Abs(colDiff));

                int curRow = fromRow + rowStep;
                int curCol = fromCol + colStep;

                // Check intermediate squares (not including destination)
                for (int i = 1; i < steps; i++)
                {
                    var cell = GameBoard.GetCell(curRow, curCol);
                    if (cell == null) return false;
                    if (cell.OccupiedBy != null)
                        return true; // blocked

                    curRow += rowStep;
                    curCol += colStep;
                }

                return false; // not blocked
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApi
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
            private const int ROWS = 8;
            private const int COLS = 4;
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
            public event Action MoveMade;
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

                // Prevent moving onto a square occupied by the same player's piece
                if (toCell.OccupiedBy != null && toCell.OccupiedBy.IsWhite == IsWhiteTurn) return false;

                // Check piece-specific rules
                if (!IsLegalMove(fromCell.OccupiedBy, fromRow, fromCol, toRow, toCol)) return false;

                // Capture or move (overwrite)
                toCell.OccupiedBy = fromCell.OccupiedBy;
                fromCell.OccupiedBy = null;

                // Check if the king has any available moves
                if (IsKingInCheck(IsWhiteTurn) && !HasAvailableMoves(IsWhiteTurn))
                {
                    // Revert the move
                    toCell.OccupiedBy = null;
                    fromCell.OccupiedBy = toCell.OccupiedBy;
                    return false;
                }

                // Switch turns
                IsWhiteTurn = !IsWhiteTurn;

                MoveMade?.Invoke();

                return true;
            }

            private bool HasAvailableMoves(bool isWhite)
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        var cell = GameBoard.GetCell(r, c);
                        if (cell?.OccupiedBy != null && cell.OccupiedBy.IsWhite == isWhite)
                        {
                            foreach (var move in GetLegalMoves(r, c))
                            {
                                if (SimulateMoveAndCheckSafety(r, c, move.Item1, move.Item2, isWhite))
                                    return true;
                            }
                        }
                    }
                }
                return false; // No legal moves available
            }
            public bool IsCheckmate(bool isWhiteKing)
            {

                // Step 1: Check if the king is in check
                if (!IsKingInCheck(isWhiteKing))
                {
                    return false;
                }

                // Step 2: Iterate through all pieces of the current player
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        var cell = GameBoard.GetCell(r, c);
                        if (cell?.OccupiedBy != null && cell.OccupiedBy.IsWhite == isWhiteKing)
                        {
                            // Step 3: Check if any legal move gets the king out of check
                            foreach (var move in GetLegalMoves(r, c))
                            {
                                if (SimulateMoveAndCheckSafety(r, c, move.Item1, move.Item2, isWhiteKing))
                                {
                                    return false; // A valid move exists
                                }
                            }
                        }
                    }
                }

                // Step 4: No valid moves exist, king is in checkmate
                MessageBox.Show("No valid moves available. Checkmate!");
                return true;
            }


            public bool IsStalemate(bool isWhiteKing)
            {
                if (IsKingInCheck(isWhiteKing)) return false;

                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        var cell = GameBoard.GetCell(r, c);
                        if (cell?.OccupiedBy != null && cell.OccupiedBy.IsWhite == isWhiteKing)
                        {
                            foreach (var move in GetLegalMoves(r, c))
                            {
                                if (SimulateMoveAndCheckSafety(r, c, move.Item1, move.Item2, isWhiteKing))
                                    return false;
                            }
                        }
                    }
                }

                return true; // No legal moves available, king is not in check
            }

            public Cell FindKingCell(bool isWhiteKing)
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        var cell = GameBoard.GetCell(r, c);
                        if (cell?.OccupiedBy != null &&
                            cell.OccupiedBy.Type == PieceType.King &&
                            cell.OccupiedBy.IsWhite == isWhiteKing)
                        {
                            return cell;
                        }
                    }
                }
                return null;
            }

            private IEnumerable<(int, int)> GetLegalMoves(int row, int col)
            {
                var piece = GameBoard.GetCell(row, col)?.OccupiedBy;
                if (piece == null) yield break;

                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        if (IsLegalMove(piece, row, col, r, c))
                        {
                            MessageBox.Show($"Legal move for piece at ({row}, {col}): ({r}, {c})");
                            yield return (r, c);
                        }
                    }
                }
            }

            private bool SimulateMoveAndCheckSafety(int fromRow, int fromCol, int toRow, int toCol, bool isWhiteKing)
            {
                var fromCell = GameBoard.GetCell(fromRow, fromCol);
                var toCell = GameBoard.GetCell(toRow, toCol);
                var originalPiece = toCell.OccupiedBy;

                // Simulate move
                toCell.OccupiedBy = fromCell.OccupiedBy;
                fromCell.OccupiedBy = null;

                // Check if the king is safe
                bool isSafe = !IsKingInCheck(isWhiteKing);

                // Revert move
                fromCell.OccupiedBy = toCell.OccupiedBy;
                toCell.OccupiedBy = originalPiece;

                return isSafe;
            }


            /// <summary>
            /// Checks if moving 'piece' from (fromRow, fromCol) to (toRow, toCol) 
            /// is valid based on piece type. Ignores checks, castling, en passant, etc.
            /// </summary>
            private bool IsLegalMove(Piece piece, int fromRow, int fromCol, int toRow, int toCol)
            {
                if (fromRow == toRow && fromCol == toCol) return false; // Cannot move to the same square

                int rowDiff = toRow - fromRow;
                int colDiff = toCol - fromCol;

                switch (piece.Type)
                {
                    case PieceType.Pawn:
                        return IsLegalPawnMove(piece, fromRow, fromCol, toRow, toCol);

                    case PieceType.King:
                        // King moves one square in any direction
                        return Math.Abs(rowDiff) <= 1 && Math.Abs(colDiff) <= 1;

                    case PieceType.Rook:
                        // Rook moves in straight lines, horizontally or vertically
                        return (rowDiff == 0 || colDiff == 0) && !IsPathBlocked(fromRow, fromCol, toRow, toCol);

                    case PieceType.Bishop:
                        // Bishop moves diagonally
                        return Math.Abs(rowDiff) == Math.Abs(colDiff) && !IsPathBlocked(fromRow, fromCol, toRow, toCol);

                    case PieceType.Knight:
                        // Knight moves in "L" shapes
                        return (Math.Abs(rowDiff) == 2 && Math.Abs(colDiff) == 1) ||
                               (Math.Abs(rowDiff) == 1 && Math.Abs(colDiff) == 2);

                    default:
                        return false;
                }
            }
            private bool IsLegalPawnMove(Piece pawn, int fromRow, int fromCol, int toRow, int toCol)
            {
                int direction = pawn.IsWhite ? -1 : 1;

                // Standard forward move
                if (toCol == fromCol && toRow == fromRow + direction)
                {
                    return GameBoard.GetCell(toRow, toCol)?.OccupiedBy == null;
                }

                // Double forward move from starting position
                int startRow = pawn.IsWhite ? 6 : 1;
                if (fromRow == startRow && toCol == fromCol && toRow == fromRow + 2 * direction)
                {
                    return GameBoard.GetCell(fromRow + direction, fromCol)?.OccupiedBy == null &&
                           GameBoard.GetCell(toRow, toCol)?.OccupiedBy == null;
                }

                // Diagonal capture
                if (Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + direction)
                {
                    return GameBoard.GetCell(toRow, toCol)?.OccupiedBy?.IsWhite != pawn.IsWhite;
                }

                // Sideways movement (strategic improvement)
                if (toRow == fromRow && Math.Abs(toCol - fromCol) == 1)
                {
                    return GameBoard.GetCell(toRow, toCol)?.OccupiedBy == null;
                }

                return false;
            }

            private bool IsPathBlocked(int fromRow, int fromCol, int toRow, int toCol)
            {
                int rowDiff = toRow - fromRow;
                int colDiff = toCol - fromCol;
                int rowStep = rowDiff == 0 ? 0 : (rowDiff > 0 ? 1 : -1);
                int colStep = colDiff == 0 ? 0 : (colDiff > 0 ? 1 : -1);
                int steps = Math.Max(Math.Abs(rowDiff), Math.Abs(colDiff));

                int curRow = fromRow + rowStep;
                int curCol = fromCol + colStep;

                for (int i = 1; i < steps; i++)
                {
                    if (GameBoard.GetCell(curRow, curCol)?.OccupiedBy != null) return true;
                    curRow += rowStep;
                    curCol += colStep;
                }

                return false;
            }

            public bool IsKingInCheck(bool isWhiteKing)
            {
                var kingCell = FindKingCell(isWhiteKing);
                if (kingCell == null) return false;

                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        var cell = GameBoard.GetCell(r, c);
                        if (cell?.OccupiedBy != null && cell.OccupiedBy.IsWhite != isWhiteKing)
                        {
                            if (IsLegalMove(cell.OccupiedBy, r, c, kingCell.Row, kingCell.Col))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

           
        }
    }
}



    
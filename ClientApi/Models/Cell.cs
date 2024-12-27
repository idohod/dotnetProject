using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApi
{
    public class Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public PieceModels.Piece OccupiedBy { get; set; } 

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            OccupiedBy = null;
        }
    }

}

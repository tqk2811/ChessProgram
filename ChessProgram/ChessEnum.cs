using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProgram
{
    public enum ChessEnum
    {
        Empty = 0,
        Pawn = 1,
        Bishop = 2,
        Knight = 3,
        Castle = 4,
        Queen = 5,
        King = 6
    }

    public enum ChessColor
    {
        White = 1,
        Black = -1,
        Die = 0
    }

    public class MapChess
    {
        public ChessEnum chessType = ChessEnum.Empty;
        public ChessColor Color = ChessColor.Die;
        public int IndexControl = -1;
    }

    public class PosChess
    {
        public int X;
        public int Y;
        public bool Eat = false;
    }
}

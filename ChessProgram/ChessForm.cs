using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessProgram
{
    public partial class ChessForm : Form
    {
        public static string FolderImg = Directory.GetCurrentDirectory() + "\\ChessImage\\";
        ChessEnum[] array_ChessEnum = { ChessEnum.Castle, ChessEnum.Knight, ChessEnum.Bishop, ChessEnum.King, ChessEnum.Queen, ChessEnum.Bishop, ChessEnum.Knight, ChessEnum.Castle };
        MapChess[,] Map_Chess = new MapChess[8, 8];
        ChessColor round = ChessColor.Black;

        public static Color default_color = Color.BurlyWood;
        public static Color canmove_color = Color.Fuchsia;
        int edge = 50;
        bool isMove = false;
        int index_control = -1;
        Point temp_p;
        PictureBox temp_pic = new PictureBox();
        List<PosChess> listcanmove = new List<PosChess>();
        List<PictureBox> listchess = new List<PictureBox>();
        List<PictureBox> listBackground = new List<PictureBox>();

        public ChessForm()
        {
            InitializeComponent();
        }
        private void ChessForm_Load(object sender, EventArgs e)
        {
            temp_pic.Visible = false;
            Console.WriteLine((5 / 2).ToString());
            CreateMapChessPos();
            CreateBackground();
        }
        private void CreateMapChessPos()
        {
            Map_Chess = new MapChess[8, 8];
            ChessColor color;
            for (int y = 0; y < 8; y++)
            {
                if (y == 0 | y == 1) color = ChessColor.Black;
                else color = ChessColor.White;
                for (int x = 0; x < 8; x++)
                {
                    Map_Chess[x, y] = new MapChess();
                    if (y == 2 || y == 3 || y == 4 || y == 5) continue;

                    if (y == 0 | y == 7)
                    {
                        PictureBox pic = LoadPic(color, array_ChessEnum[x], new Point(x * edge, y * edge));
                        this.Controls.Add(pic);
                        listchess.Add(pic);
                        pic.BringToFront();
                        Map_Chess[x, y].chessType = array_ChessEnum[x];
                        Map_Chess[x, y].Color = color;
                        Map_Chess[x, y].IndexControl = this.Controls.Count - 1;
                    }
                    else
                    {
                        PictureBox pic = LoadPic(color, ChessEnum.Pawn, new Point(x * edge, y * edge));
                        this.Controls.Add(pic);
                        listchess.Add(pic);
                        pic.BringToFront();
                        Map_Chess[x, y].chessType = ChessEnum.Pawn;
                        Map_Chess[x, y].Color = color;
                        Map_Chess[x, y].IndexControl = this.Controls.Count - 1;
                    }
                }
            }
        }
        private void CreateBackground()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(50, 50);
                    pic.BackColor = (x + y) % 2 == 0 ? Color.Gray : Color.DarkGray;
                    pic.BorderStyle = BorderStyle.FixedSingle;
                    pic.Location = new Point(x * edge, y * edge);
                    pic.SendToBack();
                    listBackground.Add(pic);
                    this.Controls.Add(pic);
                }
            }
        }

        private PictureBox LoadPic(ChessColor color, ChessEnum chess, Point point,bool CreateEvent = true)
        {
            PictureBox pic = new PictureBox();
            pic.Size = new Size(50, 50);
            pic.Image = Image.FromFile(FolderImg + color.ToString() + "\\" + chess.ToString() + ".png");
            pic.BackColor = default_color;
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.Location = point;
            if (CreateEvent)
            {
                pic.MouseMove += Pic_MouseMove;
                pic.MouseUp += Pic_MouseUp;
                pic.MouseDown += Pic_MouseDown;
            }
            return pic;
        }

        private void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            isMove = true;
            temp_p = e.Location;
            index_control = this.Controls.IndexOf((PictureBox)sender);
            PosChess pos_chess = GetPosMapsChess(((PictureBox)sender).Location);
            if (round != Map_Chess[pos_chess.X, pos_chess.Y].Color) { isMove = false; return; }
            ((PictureBox)sender).Visible = false;
            temp_pic = LoadPic(Map_Chess[pos_chess.X, pos_chess.Y].Color, Map_Chess[pos_chess.X, pos_chess.Y].chessType, ((PictureBox)sender).Location,false);
            this.Controls.Add(temp_pic);
            temp_pic.BringToFront();
            listcanmove = new List<PosChess>();
            listcanmove = ChessMove.ListCanMove(pos_chess, Map_Chess);
            foreach (PosChess pos in listcanmove)
            {
                foreach (PictureBox pic in GetPictureBoxFromPosChess(pos))
                {
                    pic.BackColor = canmove_color;
                }
            }
        }

        private void Pic_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMove) isMove = false;
            else return;
            int _x = 0, _y = 0;
            Point pos = new Point();
            ChessColor lose = ChessColor.Die;
            if (CheckDropTrueSquare(ref _x, ref _y) && CheckDropTruePosAllow(_x, _y, ref pos))
            {
                //if eat
                foreach (PictureBox pic in listchess)
                {
                    if (pic.Location == pos)
                    {
                        pic.Visible = false;
                        pic.Dispose();
                        this.Controls.Remove(pic);
                        listchess.Remove(pic);
                        break;
                    }
                }
                PosChess pos_map = GetPosMapsChess(((PictureBox)sender).Location);
                MapChess map = Map_Chess[pos_map.X, pos_map.Y];
                ((PictureBox)sender).Location = pos;
                PosChess pos_map_ = GetPosMapsChess(((PictureBox)sender).Location);
                //check king
                if (Map_Chess[pos_map_.X, pos_map_.Y].chessType == ChessEnum.King)
                {
                    lose = Map_Chess[pos_map_.X, pos_map_.Y].Color;
                }
                //check pawn
                if (Map_Chess[pos_map.X, pos_map.Y].chessType == ChessEnum.Pawn)
                {
                    if (pos_map_.Y == 0 | pos_map_.Y == 7)
                    {
                        PawnForm nform = new PawnForm(Map_Chess[pos_map.X, pos_map.Y].Color);
                        nform.ShowDialog();
                        map.chessType = nform.chess;
                        ((PictureBox)sender).Image = Image.FromFile(FolderImg + Map_Chess[pos_map.X, pos_map.Y].Color.ToString() + "\\" + nform.chess.ToString() + ".png");
                        ((PictureBox)sender).BringToFront();
                    }
                }
                //overwrite new pos
                Map_Chess[pos_map_.X, pos_map_.Y].chessType = map.chessType;
                Map_Chess[pos_map_.X, pos_map_.Y].Color = map.Color;
                //clear old pos maps
                Map_Chess[pos_map.X, pos_map.Y].chessType = ChessEnum.Empty;
                Map_Chess[pos_map.X, pos_map.Y].Color = ChessColor.Die;
                //change round
                round = round == ChessColor.Black ? ChessColor.White : ChessColor.Black;
            }
            else { }
            //clean
            ((PictureBox)sender).Visible = true;
            this.Controls.Remove(temp_pic);
            temp_pic.Dispose();
            temp_pic = null;
            foreach (PictureBox pic in listchess)
            {
                if (pic.BackColor == canmove_color) pic.BackColor = default_color;
            }
            foreach (PictureBox pic in listBackground)
            {
                if (pic.BackColor == canmove_color) pic.BackColor = ((pic.Location.X / 50) + (pic.Location.Y / 50)) % 2 == 0 ? Color.Gray : Color.DarkGray;
            }
            if(lose != ChessColor.Die)
            {
                MessageBox.Show(lose.ToString() + " lose.");
                MakeNewRound();
                return;
            }
            GC.Collect();
        }
        
        private void Pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMove)
            {
                temp_pic.Location = GetPixelLocationPic();
            }
        }

        private Point GetPixelLocationPic()
        {
            Point p = this.PointToClient(Cursor.Position);
            p.X -= temp_p.X + 1;
            p.Y -= temp_p.Y + 1;
            return p;
        }

        private PosChess GetPosMapsChess(Point p)
        {
            return new PosChess() { X = p.X / edge, Y = p.Y / edge };
        }

        private bool CheckDropTrueSquare(ref int _x_, ref int _y_, int px = 15)
        {
            Point p = temp_pic.Location;
            int _x = p.X % edge;
            int _y = p.Y % edge;
            if (_x >= edge - px) _x_ += 1;
            if (_y >= edge - px) _y_ += 1;
            if ((_x <= px | _x >= edge - px) & (_y <= px | _y >= edge - px)) return true;
            return false;
        }

        private bool CheckDropTruePosAllow(int _x,int _y,ref Point pos)
        {
            pos = new Point(((temp_pic.Location.X / 50) + _x) * 50, ((temp_pic.Location.Y / 50) + _y) * 50);
            foreach(PosChess pc in listcanmove)
            {
                if (pc.X == pos.X/50 & pc.Y == pos.Y/50) return true;
            }
            return false;
        }

        private List<PictureBox> GetPictureBoxFromPosChess(PosChess pos)
        {
            List<PictureBox> list = new List<PictureBox>();
            foreach(PictureBox pic in listchess)
            {
                if (pic.Location == new Point(pos.X * edge, pos.Y * edge)) list.Add(pic);
            }
            foreach(PictureBox pic in listBackground)
            {
                if (pic.Location == new Point(pos.X * edge, pos.Y * edge)) list.Add(pic);
            }
            return list;
        }

        private void MakeNewRound()
        {
            for(int i =0; i < listchess.Count;i++)
            {
                this.Controls.Remove(listchess[0]);
                listchess.RemoveAt(0);
                i--;
            }
            CreateMapChessPos();
        }
    }
}

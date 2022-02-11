using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessProgram
{
    public partial class PawnForm : Form
    {
        public PawnForm(ChessColor color)
        {
            InitializeComponent();
            this.color = color;
        }
        public ChessEnum chess = ChessEnum.Pawn;
        ChessColor color = ChessColor.Die;
        private void PawnForm_Load(object sender, EventArgs e)
        {
            Point p = Cursor.Position;
            p.X = p.X - 100;
            p.Y = p.Y - 40;
            this.Location = p;
            for (int i = 0; i < 4; i++)
            {
                PictureBox pic = LoadPic(this.color, (ChessEnum)(i + 2), new Point(i * 50, 14));
                this.Controls.Add(pic);
            }
        }

        private PictureBox LoadPic(ChessColor color, ChessEnum chess, Point point, bool CreateEvent = true)
        {
            PictureBox pic = new PictureBox();
            pic.Size = new Size(50, 50);
            pic.Image = Image.FromFile(ChessForm.FolderImg + color.ToString() + "\\" + chess.ToString() + ".png");
            pic.BackColor = ChessForm.default_color;
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.Location = point;
            if (CreateEvent)
            {
                pic.Click += Pic_Click;
            }
            return pic;
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            chess = (ChessEnum)((((PictureBox)sender).Location.X / 50) + 2);
            this.Close();
        }
    }
}

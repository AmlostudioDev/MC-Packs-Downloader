using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftBedrockPackDownloader.CustomControls
{
    public static partial class Structures
    {
        
    }
    public class FormBorderTop : Panel
    {
        private Font font = new Font("Arrial", 14f, FontStyle.Bold);
        private Label Title = new Label();
        private Label lbl_Close = new Label();

        public FormBorderTop(string sTitle)
        {
            
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            
            this.BackColor = Color.DimGray;
            this.Location = new Point(0,0);
            this.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;

            Title.AutoSize = true;
            Title.Text = sTitle;
            Title.Font = font;
            Title.BackColor = this.BackColor;
            Title.ForeColor = Color.Black;
            Title.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
            this.Controls.Add(Title);

            lbl_Close.AutoSize = true;
            lbl_Close.Text = "X";
            lbl_Close.Font = font;
            lbl_Close.BackColor = this.BackColor;
            lbl_Close.ForeColor = Color.Black;
            lbl_Close.MouseEnter += MinecraftBedrockPackDownloader.Events.MouseEnter;
            lbl_Close.MouseLeave += MinecraftBedrockPackDownloader.Events.MouseLeave;
            lbl_Close.Click += (s, e) => { Function.CloseApp(); };
            this.Controls.Add(lbl_Close);

            DoUpdate();
        }
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    Rectangle rect = ClientRectangle;
        //    Graphics g = e.Graphics;
        //    //rect.Inflate(-3, -3);

        //    using (Font f = font)
        //    {
        //        g.DrawString(Title, f, new SolidBrush(Color.Black), TitleLocation);
        //    }
        //}
        public void DoUpdate()
        {
            this.Size = new Size(Var.App.Width, 40);
            Size tmp = this.CreateGraphics().MeasureString(Title.Text, font).ToSize();
            Title.Location = new Point((this.Size.Width - tmp.Width) / 2, (this.Size.Height - tmp.Height) / 2);
            tmp = this.CreateGraphics().MeasureString(lbl_Close.Text, font).ToSize();
            lbl_Close.Location = new Point(this.Size.Width - tmp.Width - 10, (this.Size.Height - tmp.Height) / 2);
            Invalidate();
        }
    };
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace MinecraftBedrockPackDownloader
{
    internal class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new App());
        }
    }
    public class App : Form
    {
        public App()
        {
            Var.App = this;
            if (Directory.Exists(Var.GamePath) && Directory.Exists(Var.GameUserDataPath) && Directory.Exists(Var.DownloadedMapPath))
            {

                this.BackColor = Color.Black;
                this.Size = new Size(800, 600);
                this.Text = "MC Packs: Bedrock Edition";
                this.FormBorderStyle = FormBorderStyle.None;
                Var.pnl_BorderTop = new CustomControls.FormBorderTop("MC Packs: Bedrock Edition");
                this.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                this.Controls.Add(Var.pnl_BorderTop);
                

                Var.AppUI = new Panel();
                Var.AppUI.AutoScroll = true;
                Var.AppUI.Location = new Point(0,Var.pnl_BorderTop.Location.Y + Var.pnl_BorderTop.Height + 1);
                Var.AppUI.Size = this.Size;
                Var.AppUI.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                this.Controls.Add(Var.AppUI);

                #region App Constructor

                bool NoMapsFoundCreated = false;

                Timer timer = new Timer();
                timer.Interval = 100;
                timer.Tick += (s, e) =>
                {
                    if (Var.isOnMainUI)
                    {
                        int MapCount = Var.MapPaths.Count();
                        if (MapCount == 0) { NoMapsFoundCreated = true; }
                        if (MapCount > 0 && Var.Maps.Count() < MapCount)
                        {
                            Var.AppUI.Controls.Clear();
                            Var.AppUI.Controls.AddRange(Function.GetMaps(Var.MapPaths));
                            Function.ResizeAppControls();
                            NoMapsFoundCreated = false;
                            
                        }
                        else
                        {
                            if (NoMapsFoundCreated == true)
                            {
                                Var.AppUI.Controls.Clear();
                                Label lbl_NoMapsFound = new Label();
                                lbl_NoMapsFound.AutoSize = true;
                                lbl_NoMapsFound.Text = "Maps Not Found : Download A Map From Minecraft Store First";
                                lbl_NoMapsFound.ForeColor = Color.White;
                                lbl_NoMapsFound.Font = new Font("Arrial", 16f, FontStyle.Bold);
                                Size lbl_NoMapsFound_Size = Var.AppUI.CreateGraphics().MeasureString(lbl_NoMapsFound.Text, lbl_NoMapsFound.Font).ToSize();
                                lbl_NoMapsFound.Location = new Point((Var.AppUI.Width - lbl_NoMapsFound_Size.Width) / 2, 10);
                                Var.AppUI.Controls.Add(lbl_NoMapsFound);
                            }
                        }
                    }
                };
                timer.Start();
                #endregion
            }
            else {
                MessageBox.Show("Error : You don't have Minecraft Bedrock Edition", "MC Packs Downloader", MessageBoxButtons.OK);
                Environment.Exit(-1);
            }
        }        
    }
}

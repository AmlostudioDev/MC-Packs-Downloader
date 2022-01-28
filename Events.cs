using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftBedrockPackDownloader
{
    public class Events
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public static void MoveForm(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                 
                SendMessage(Var.App.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #region Download Events/Worker Events  //100% Work
        public struct iDownload
        {
            public Var._Map Map;
            public BackgroundWorker Worker;
        }

        public static List<iDownload> DownloadsQueue = new List<iDownload>();
        public static void DownloadClick(object sender, EventArgs e, Var.PackType iPackType, Var._Map Map)
        {
            //int index = Var.Maps.IndexOf(Var.Maps.Where(x => x.Name == Map.Name).Select(x => x).ToList()[0]);
            switch (iPackType)
            {
                case Var.PackType.ResourcePack: Map.RessourcesPack.PackState = Var.PackState.InQueu; break;
                case Var.PackType.BehaviorPack: Map.BehaviorPack.PackState = Var.PackState.InQueu; break;
            }

            Label lbl = (Label)sender;
            CustomControls.DownloaderBar DownloadBar = iPackType == Var.PackType.ResourcePack ? Map.RessourcesPack.DownloadBar : Map.BehaviorPack.DownloadBar;
            DownloadBar.Size = new Size(300, 30);
            DownloadBar.Location = lbl.Location;
            DownloadBar.Minimum = 0;
            DownloadBar.Maximum = 100;
            if (iPackType == Var.PackType.ResourcePack) { Map.RessourcesPack.DownloadBar = DownloadBar; }
            else { Map.BehaviorPack.DownloadBar = DownloadBar; }
            lbl.Hide();
            Var.AppUI.Controls.Add(DownloadBar);
            
            
            iDownload DownloadInstance = new iDownload();
            DownloadInstance.Map = Map;
            DownloadInstance.Worker = new BackgroundWorker();
            DownloadInstance.Worker.WorkerReportsProgress = true;
            DownloadInstance.Worker.WorkerSupportsCancellation = true;
            DownloadInstance.Worker.DoWork += (s, ee) => DownloadWorker_DoWork(s, ee, iPackType, Map);
            DownloadInstance.Worker.ProgressChanged += (s, ee) => DownloadWorker_ProgressChanged(s, ee, iPackType == Var.PackType.ResourcePack? Map.RessourcesPack.DownloadBar : Map.BehaviorPack.DownloadBar);
            DownloadInstance.Worker.RunWorkerCompleted += (s, ee) => DownloadWorker_RunWorkerCompleted(s, ee, DownloadInstance, iPackType, lbl);
            DownloadInstance.Worker.RunWorkerAsync();
            DownloadsQueue.Add(DownloadInstance);       

            //Var.AppUI.Cursor = Cursors.Default;
            lbl.MouseEnter -= MouseEnter;
            lbl.MouseLeave -= MouseLeave;
            lbl.Click -= (s, ee) => DownloadClick(s, ee, iPackType, Map);
        }
        public static void DownloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e, iDownload Queu, Var.PackType iPackType, Label lbl)
        {
            lbl.Text = "In Your Game Folder";
            lbl.ForeColor = Color.Green;
            CustomControls.DownloaderBar iDownloadBar = null;
            switch(iPackType)
            {
                case Var.PackType.ResourcePack: iDownloadBar = Queu.Map.RessourcesPack.DownloadBar; Queu.Map.RessourcesPack.PackState = Var.PackState.InCollection; break;
                case Var.PackType.BehaviorPack: iDownloadBar = Queu.Map.BehaviorPack.DownloadBar; Queu.Map.BehaviorPack.PackState = Var.PackState.InCollection; break;
            }

            Var.AppUI.Controls.Remove(iDownloadBar);
            lbl.Show();
            DownloadsQueue.Remove(Queu);

        }
        public static void DownloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e, CustomControls.DownloaderBar Bar)
        {
            Bar.Value = e.ProgressPercentage;
        }
        public static void DownloadWorker_DoWork(object sender, DoWorkEventArgs e, Var.PackType iPackType, Var._Map map)
        {
            Function.DownloadPack(iPackType, map, (BackgroundWorker)sender);
        }
        #endregion
        public static void MouseLeave(object sender, EventArgs e)
        {
            Var.App.Cursor = Cursors.Default;
        }
        public static void MouseEnter(object sender, EventArgs e)
        {
            Var.App.Cursor = Cursors.Hand;
        }
    }
}

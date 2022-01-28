using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftBedrockPackDownloader
{
    public class Function
    {
        public static void CloseApp()
        {
            int queuesCount = 0;
            foreach (Events.iDownload iDownload in Events.DownloadsQueue)
            {
                queuesCount++;
            }
            if (queuesCount > 0)
            {
                DialogResult result = MessageBox.Show($"You Have {queuesCount} Downloads Queues Are You Sure You Wan't Exit?", "MC Packs Downloader", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    foreach (Events.iDownload iDownload in Events.DownloadsQueue)
                    {
                        iDownload.Worker.CancelAsync();
                        string path = "";
                        if(iDownload.Map.RessourcesPack.PackState == Var.PackState.InQueu)
                        { 

                            path = $@"{Var.GameUserDataPath}\resources_packs\{iDownload.Map.Name.Replace(" ", "")}";
                            if (Directory.Exists(path)){ Directory.Delete(path); MessageBox.Show("Deleted"); } 
                        }
                        if (iDownload.Map.BehaviorPack.PackState == Var.PackState.InQueu)
                        {
                            path = $@"{Var.GameUserDataPath}\behavior_packs\{iDownload.Map.Name.Replace(" ", "")}";
                            if (Directory.Exists(path)) { Directory.Delete(path); }
                        }

                    }
                    Application.Exit();
                }
            }
            else { Application.Exit(); }
        }
        public static void ResizeAppControls()
        {
            //Default Size of App
            int DefaultHeight = Var.pnl_BorderTop.Size.Height + (10 * 3)/*Margin*/ + (200*2)/*ImageSize*/;
            Var.App.Size = new Size(Var.App.Size.Width, DefaultHeight);
            
            Label lbl_HighWidth = Var.AppUI.Controls.OfType<Label>().ToList().Where(x => x.Size.Width == Var.AppUI.Controls.OfType<Label>().ToList().Max(i => /*i.Location.X + */ i.Size.Width )).Select(x => x).ToList()[0];
            
            Size size = Var.AppUI.CreateGraphics().MeasureString(lbl_HighWidth.Text, lbl_HighWidth.Font).ToSize();
            Var.AppUI.Size = new Size(lbl_HighWidth.Location.X + size.Width + 8, Var.AppUI.Size.Height);
            Var.App.Size = new Size(Var.AppUI.Size.Width,Var.App.Size.Height);

            int dif = Var.AppUI.Size.Height - Var.App.Size.Height + Var.pnl_BorderTop.Size.Height + 10;
            Var.AppUI.Size = new Size(Var.AppUI.Size.Width+20, Var.AppUI.Size.Height-dif); //Make Scrollbar out of view

            Var.pnl_BorderTop.DoUpdate();
        }
        public static bool IsInCollection(Var.PackType PackType, string Name)
        {
            Name = Name.Replace(" ", "");
            string Pack = "";
            switch (PackType)
            {
                case Var.PackType.ResourcePack: Pack = "resource_packs"; break;
                case Var.PackType.BehaviorPack: Pack = "behavior_packs"; break;
                //case Var.PackType.BehaviorPack: Pack = "behavior_packs"; break;
            }

            if (Directory.Exists($@"{Var.GameUserDataPath}\{Pack}\{Name}")) { return true; }
            else { return false; }
        }
        public static string ReadLine(string Text, string WordToFind, string WordEnd = "")
        {
            int StartIndex = Text.IndexOf(WordToFind) + WordToFind.Length;
            int Length = 0;

            if (WordEnd == "")
            {
                Length = Text.IndexOf("\r\n", StartIndex);
                if (Length < 0) { Length = Text.IndexOf("\n", StartIndex); }
                if (Length < 0) { Length = Text.Length; }
                return Text.Substring(StartIndex, Length - StartIndex);
            }
            else
            {
                Length = Text.IndexOf(WordEnd, StartIndex) - StartIndex;
                return Text.Substring(StartIndex, Length);
            }
        }
        public static string Parse_Name(string Path)
        {
            RichTextBox TextBlock = new RichTextBox();
            TextBlock.Multiline = true;
            StreamReader reader;

            if (Directory.Exists($@"{Path}\texts"))
            {
                reader = new StreamReader($@"{Path}\texts\languages.json");
                TextBlock.Text = reader.ReadToEnd(); reader.Close();
                reader = new StreamReader($@"{Path}\texts\{ReadLine(TextBlock.Text, "\"", "\"")}.lang");
                TextBlock.Text = reader.ReadToEnd(); reader.Close();
                return ReadLine(TextBlock.Text, "pack.name=");
            }
            else
            {
                reader = new StreamReader($@"{Path}\levelname.txt");
                TextBlock.Text = reader.ReadToEnd(); reader.Close();
                TextBlock.AppendText("\r\n");
                return ReadLine(TextBlock.Text, "");
            }
        }
        public static Image Parse_Image(string Path, int width, int height)
        {
            //pack_icon
            //world_icon
            var FilesPath = Directory.GetFiles(Path);

            // Loop through each items(filenames) and  check the filename(without extension)
            // is matching with our method parameter value
            foreach (var file in FilesPath)
            {
                if (file.Contains("pack_icon.") || file.Contains("world_icon."))
                {
                    return ResizeImage(file, width, height);
                }
            }
            return null;

        }
        public static Image ResizeImage(string Path, int width, int height)
        {
            Image image = Image.FromFile(Path);
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
        public static Control[] GetMaps(string[] MapPaths)
        {
            List<Control> Controls = new List<Control>();
            Point iLocation = new Point(10, 10);

            foreach (string MapPath in MapPaths)
            {
                try
                {
                    Var._Map iMap = new Var._Map();
                    iMap.Path = MapPath;
                    iMap.Name = Parse_Name(MapPath);
                    iMap.Image = Parse_Image(MapPath, 300, 200);
                    iMap.RessourcesPack = Directory.Exists($@"{MapPath}\resource_packs\rp0") ? new Var._RessourcesPack { Name = Parse_Name($@"{MapPath}\resource_packs\rp0"), Image = Parse_Image($@"{MapPath}\resource_packs\rp0", 64 * 2, 64 * 2), PackState = Var.PackState.Default } : new Var._RessourcesPack();
                    iMap.BehaviorPack = Directory.Exists($@"{MapPath}\behavior_packs\bp0") ? new Var._BehaviorPack { Name = Parse_Name($@"{MapPath}\behavior_packs\bp0"), Image = Parse_Image($@"{MapPath}\behavior_packs\bp0", 64 * 2, 64 * 2), PackState = Var.PackState.Default } : new Var._BehaviorPack();
                    iMap.RessourcesPack.DownloadBar = new CustomControls.DownloaderBar(CustomControls.Structures.ProgressBarDisplayText.Percentage, Color.Black);
                    iMap.BehaviorPack.DownloadBar = new CustomControls.DownloaderBar(CustomControls.Structures.ProgressBarDisplayText.Percentage, Color.Black);
                    if (Var.Maps.Where(x => x.Path == MapPath).Select(x => x).Count() == 0) { Var.Maps.Add(iMap); }

                    PictureBox MapImage = new PictureBox();
                    MapImage.Image = iMap.Image;
                    MapImage.Size = MapImage.Image.Size;
                    MapImage.Location = iLocation;
                    MapImage.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                    Controls.Add(MapImage);
                    
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Text = iMap.Name;
                    label.ForeColor = Color.White;
                    label.Location = new Point(iLocation.X + MapImage.Width + 8, iLocation.Y + 2);
                    label.Font = new Font("Arial", 20f, FontStyle.Bold);
                    label.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                    Controls.Add(label);

                    Label label2 = new Label();
                    label2.AutoSize = true;

                    label2.Text = "Click Here To View Contents";
                    label2.MouseEnter += Events.MouseEnter;
                    label2.MouseLeave += Events.MouseLeave;
                    label2.Click += (s, e) => { Var.AppUI.Controls.Clear(); Var.AppUI.Controls.AddRange(GetContents(iMap)); ResizeAppControls(); };

                    label2.ForeColor = Color.Red;
                    label2.Location = new Point(label.Location.X, label.Location.Y + (int)Var.AppUI.CreateGraphics().MeasureString(label.Text, label.Font).Height + 10);
                    label2.Font = new Font("Arial", 14f, FontStyle.Bold);
                    Controls.Add(label2);


                    iLocation = new Point(iLocation.X, iLocation.Y + MapImage.Size.Height + 10);
                }
                catch (Exception error) { }
            }
            Var.isOnMainUI = true;
            return Controls.ToArray();
        }
        public static Control[] GetContents(Var._Map iMap)
        {
            List<Control> Controls = new List<Control>();
            Point iLocation = new Point(10, 10);

            for (int index = 0; index < 2; index++)
            {
                if (index == 0 && iMap.RessourcesPack.Name != null || index == 1 && iMap.BehaviorPack.Name != null)
                {
                    Label CurrentItem = new Label();
                    CurrentItem.AutoSize = true;
                    CurrentItem.Text = index == 0 ? "[Resources Pack]" : "[Behavior Pack]";
                    CurrentItem.ForeColor = Color.White;
                    CurrentItem.Location = iLocation;
                    CurrentItem.Font = new Font("Arial", 14f, FontStyle.Bold);
                    CurrentItem.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                    Controls.Add(CurrentItem);

                    if (index == 0)
                    {
                        Label lblReturn = new Label();
                        lblReturn.AutoSize = true;
                        lblReturn.Text = "Click Here To Return Back";
                        lblReturn.MouseEnter += Events.MouseEnter;
                        lblReturn.MouseLeave += Events.MouseLeave;
                        lblReturn.Click += (s, e) => { Var.AppUI.Controls.Clear(); Var.AppUI.Controls.AddRange(GetMaps(Var.MapPaths)); ResizeAppControls(); };
                        lblReturn.ForeColor = Color.Red;
                        lblReturn.Location = new Point(CurrentItem.Location.X + (int)Var.AppUI.CreateGraphics().MeasureString(CurrentItem.Text, CurrentItem.Font).Width + 10, CurrentItem.Location.Y);
                        lblReturn.Font = new Font("Arial", 14f, FontStyle.Bold);
                        Controls.Add(lblReturn);
                    }

                    iLocation = new Point(10, CurrentItem.Location.Y + (int)Var.AppUI.CreateGraphics().MeasureString(CurrentItem.Text, CurrentItem.Font).Height + 10);

                    PictureBox itemImage = new PictureBox();
                    itemImage.Image = index == 0 ? iMap.RessourcesPack.Image : iMap.BehaviorPack.Image;
                    itemImage.Size = itemImage.Image.Size;
                    itemImage.Location = iLocation;
                    itemImage.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                    Controls.Add(itemImage);

                    Label label = new Label();
                    label.AutoSize = true;
                    label.Text = index == 0 ? iMap.RessourcesPack.Name : iMap.BehaviorPack.Name;
                    label.ForeColor = Color.White;
                    label.Location = new Point(itemImage.Location.X + itemImage.Width + 8, itemImage.Location.Y + 2);
                    label.Font = new Font("Arial", 20f, FontStyle.Bold);
                    label.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                    Controls.Add(label);

                    Var.PackType iPackType = index == 0 ? Var.PackType.ResourcePack : Var.PackType.BehaviorPack;   //$@"{map.Path}\resource_packs" : $@"{map.Path}\behavior_packs";//\rp0, \bp0
                    string FolderName = iMap.Name.Replace(" ", "");

                    Label label2 = new Label();
                    label2.AutoSize = true;
                    label2.Location = new Point(label.Location.X, label.Location.Y + (int)Var.AppUI.CreateGraphics().MeasureString(label.Text, label.Font).Height + 10);
                    label2.Font = new Font("Arial", 14f, FontStyle.Bold);

                    Var.PackState PackState = index==0 ? iMap.RessourcesPack.PackState : iMap.BehaviorPack.PackState;
                    if (!IsInCollection(iPackType, iMap.Name) && PackState != Var.PackState.InQueu)
                    {
                        label2.Text = "Click Here To Download";
                        if (PackState != Var.PackState.InQueu)
                        {
                            label2.MouseEnter += Events.MouseEnter;
                            label2.MouseLeave += Events.MouseLeave;
                            label2.Click += (s, e) => Events.DownloadClick(s, e, iPackType, iMap);
                        }
                        label2.ForeColor = Color.Red;
                    }
                    else if (PackState == Var.PackState.InQueu)
                    {
                        if (index == 0) { Controls.Add(iMap.RessourcesPack.DownloadBar); }
                        else { Controls.Add(iMap.BehaviorPack.DownloadBar); }
                        label2.Hide();
                    }
                    else
                    {
                        label2.Text = "Your owned it";
                        label2.ForeColor = Color.Green;
                        label2.MouseDown += MinecraftBedrockPackDownloader.Events.MoveForm;
                    }
                    Controls.Add(label2);
                    


                    iLocation = new Point(iLocation.X, iLocation.Y + itemImage.Size.Height + 10);
                }
            }
            Var.isOnMainUI = false;
            return Controls.ToArray();
        }
        public static void DownloadPack(Var.PackType PackType, Var._Map Map, BackgroundWorker worker)
        {
            string Pack = "";
            string MapName = Map.Name.Replace(" ", "");
            string type = "";
            switch (PackType)
            {
                case Var.PackType.ResourcePack: Pack = "resource_packs"; type = "rp0"; Map.RessourcesPack.PackState = Var.PackState.InQueu; break;
                case Var.PackType.BehaviorPack: Pack = "behavior_packs"; type = "bp0"; Map.BehaviorPack.PackState = Var.PackState.InQueu; break;
            }

            string sourcePath = $@"{Map.Path}\{Pack}\{type}";
            string targetPath = $@"{Var.GameUserDataPath}\{Pack}\{MapName}";
            if (!Directory.Exists(targetPath)) { Directory.CreateDirectory(targetPath); }


            string[] DirsPath = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            string[] FilesPath = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int Max = DirsPath.Count() + FilesPath.Count();
            int Actual = 0;
            int percentage = 0;

            //Create all directories paths
            foreach (string dirPath in DirsPath)
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                Actual++;
                percentage = (Actual * 100) / Max;
                if (worker != null) { worker.ReportProgress(percentage); }
            }
            //Copy all the files & Replaces any files with the same name           
            foreach (string newPath in FilesPath)
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                Actual++;
                percentage = (Actual * 100) / Max;
                if (worker != null) { worker.ReportProgress(percentage); }
            }

            switch (PackType)
            {
                case Var.PackType.ResourcePack: Map.RessourcesPack.PackState = Var.PackState.InCollection; break;
                case Var.PackType.BehaviorPack: Map.BehaviorPack.PackState = Var.PackState.InCollection; break;
            }
        }
    }  
}

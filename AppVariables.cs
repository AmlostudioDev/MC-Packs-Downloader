using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftBedrockPackDownloader
{
    public class Var
    {
        public static App App { get; set; }

        public static string GamePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe";
        public static string DownloadedMapPath { get { return $@"{GamePath}\LocalState\premium_cache\world_templates\"; } }
        public static string GameUserDataPath { get { return $@"{GamePath}\LocalState\games\com.mojang"; } }

        public static bool isOnMainUI = true;
        public static Panel AppUI { get; set; }
        public static CustomControls.FormBorderTop pnl_BorderTop { get; set; }

        public static string[] MapPaths { get { return Directory.GetDirectories(DownloadedMapPath); } }
        public static List<_Map> Maps = new List<_Map>();

        //Structures
        public class _Map
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public Image Image { get; set; }
            public _RessourcesPack RessourcesPack { get; set; }
            public _BehaviorPack BehaviorPack { get; set; }
        }
        public class _RessourcesPack
        {
            public string Name { get; set; }
            public Image Image { get; set; }
            public PackState PackState { get; set; }
            public CustomControls.DownloaderBar DownloadBar;
        }
        public class _BehaviorPack
        {
            public string Name { get; set; }
            public Image Image { get; set; }
            public PackState PackState { get; set; }
            public CustomControls.DownloaderBar DownloadBar;
        }


        public enum PackType : int
        {
            ResourcePack = 1,
            BehaviorPack = 2,
        }
        public enum PackState : int
        {
            Default = 1,
            InCollection = 2,
            InQueu = 3
        }

    }
}

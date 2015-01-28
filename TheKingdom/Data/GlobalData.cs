using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TheKingdom
{
    public static class GlobalData
    {
        public static bool GameInProgress = false;
        public static int Screen_Width = 1920;
        public static int Screen_Height = 1080;
        public static int Fullscreen = 0;
        public static int MusicVolume = 100;
        public static int SoundVolume = 100;
        public static bool Closing = false;
        
        public static List<BaseBuilding> CityBuildings = new List<BaseBuilding>();

        public static void LoadSettings()
        {
            if (!File.Exists("kingdom.ini")) File.Create("kingdom.ini");

            foreach (string line in File.ReadAllLines("kingdom.ini"))
            {
                switch (line.Split(':')[0])
                {
                    case "Screen_Width":
                        Screen_Width = Convert.ToInt32(line.Split(':')[1]);
                        break;
                    case "Screen_Height":
                        Screen_Height = Convert.ToInt32(line.Split(':')[1]);
                        break;
                    case "Fullscreen":
                        Fullscreen = Convert.ToInt32(line.Split(':')[1]);
                        break;
                    case "Music_Volume":
                        MusicVolume = Convert.ToInt32(line.Split(':')[1]);                        
                        break;
                    case "Game_Volume":
                        SoundVolume = Convert.ToInt32(line.Split(':')[1]);
                        break;
                }
            }
        }

        public static void SaveSettings()
        {
            File.WriteAllText("kingdom.ini", "Screen_Width:" + Screen_Width + Environment.NewLine);
            File.AppendAllText("kingdom.ini", "Screen_Height:" + Screen_Height + Environment.NewLine);
            File.AppendAllText("kingdom.ini", "Fullscreen:" + Fullscreen + Environment.NewLine);
            File.AppendAllText("kingdom.ini", "Music_Volume:" + MusicVolume + Environment.NewLine);
            File.AppendAllText("kingdom.ini", "Sound_Volume:" + SoundVolume + Environment.NewLine);
        }
    }    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;
using SFML.Web;

namespace TheKingdom
{        
    public class SceneManager
    {
        RenderWindow mainWindow;
        static TabTypes.MainMenuTab MainMenu;
        static TabTypes.SettingsTab Settings;
        static TabTypes.CityTab CityView;
        public static int state = 0;

        public SceneManager(RenderWindow win)
        {            
            mainWindow = win;            
        }

        public void Start()
        {
            MainMenu = new TabTypes.MainMenuTab(0, "file:///UI/mainmenu.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);            
            Settings = new TabTypes.SettingsTab(0, "file:///UI/settings.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);
            CityView = new TabTypes.CityTab(0, "file:///UI/CityView.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);
            BrowserManager.UI = MainMenu;            
        }

        /// <summary>
        /// id 0 = Main Menu, 1 = New, 2 = load, 3 = settings
        /// 4 = City, 5 = Book, 6 = Council, 7 = Map
        /// </summary>
        /// <param name="id"></param>
        public static void ChangeScene(int id)
        {
            switch (id)
            {
                case 0:
                    BrowserManager.UI = MainMenu;
                    break;
                case 1:                    
                    break;
                case 2:
                    break;
                case 3:
                    BrowserManager.UI = Settings;
                    break;
                case 4:
                    BrowserManager.UI = CityView;
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;                
            }
            state = id;
        }

        public void Draw()
        {
            foreach (BaseTab t in BrowserManager.Tabs.ToList())
            {
                try
                {
                    mainWindow.Draw(t.Draw(mainWindow));                    
                }

                catch { }
            }

            if (BrowserManager.UI != null)
            {
                try { mainWindow.Draw(BrowserManager.UI.Draw(mainWindow)); }
                catch { }
            }
        }
    }
}

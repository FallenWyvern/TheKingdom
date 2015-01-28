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
        static TabTypes.LoadTab LoadingTab;
        static TabTypes.SaveTab SavingTab;
        public static int state = -1;

        public SceneManager(RenderWindow win)
        {            
            mainWindow = win;            
        }

        public static void Start()
        {
            BrowserManager.awesomiumContext.Send(state =>
            {
                MainMenu = new TabTypes.MainMenuTab(0, "file:///UI/mainmenu.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);
                Settings = new TabTypes.SettingsTab(0, "file:///UI/settings.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);
                CityView = new TabTypes.CityTab(0, "file:///UI/CityView.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);
                LoadingTab = new TabTypes.LoadTab(0, "file:///UI/loadscreen.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);
                SavingTab = new TabTypes.SaveTab(0, "file:///UI/savescreen.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);                
            }, null);
        }

        /// <summary>
        /// id 0 = Main Menu, 1 = New, 2 = load, 3 = save, 4 = settings
        /// 5 = City, 6 = Book, 7 = Council, 8 = Map
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
                    BrowserManager.UI = LoadingTab;                    
                    break;
                case 3:
                    BrowserManager.UI = SavingTab;
                    break;
                case 4:
                    BrowserManager.UI = Settings;                    
                    break;
                case 5:
                    BrowserManager.UI = CityView;
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
            if (state == -1)
            {                
                using (Texture t = new Texture("splash.png"))
                {
                    using (Sprite s = new Sprite(t))
                    {
                        s.Scale = new Vector2f(t.Size.X / GlobalData.Screen_Width,  t.Size.Y / GlobalData.Screen_Height);
                        mainWindow.Draw(s);                        
                    }
                }
            }
            else
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
}

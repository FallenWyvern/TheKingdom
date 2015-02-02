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
        public static TabTypes.UITab UI;
        public static int GameState = -1;

        public SceneManager(RenderWindow win)
        {            
            mainWindow = win;            
        }

        public static void Start()
        {
            BrowserManager.awesomiumContext.Send(state =>
            {
                UI = new TabTypes.UITab (0, "file:///UI/MainMenu.html", GlobalData.Screen_Width, GlobalData.Screen_Height, 0, 0);                
            }, null);            
        }
        public static void ChangeScene(int sceneID)
        {
            Console.WriteLine("Scene change: " + sceneID);
            GameState = sceneID;
            switch (sceneID)
            {
                case 0:                
                    UI.MyTab.Source = new Uri("file:///UI/MainMenu.html");
                    break;
                case 1:
                    UI.MyTab.Source = new Uri("file:///UI/settings.html");
                    break;
                case 2:
                    UI.MyTab.Source = new Uri("file:///UI/savescreen.html");
                    break;
                case 3:
                    UI.MyTab.Source = new Uri("file:///UI/loadscreen.html");
                    break;
                case 4:
                    UI.MyTab.Source = new Uri("file:///UI/CityView.html");
                    break;
                case 5:
                    break;
            }            
        }
      
        public void Draw()
        {
            if (GameState == -1)
            {
                using (Texture t = new Texture("splash.png"))
                {
                    using (Sprite s = new Sprite(t))
                    {
                        s.Scale = new Vector2f(t.Size.X / GlobalData.Screen_Width, t.Size.Y / GlobalData.Screen_Height);
                        mainWindow.Draw(s);
                    }
                }
            }
            else
            {
                // Render Scene, Tabs then UI.
                
                foreach (BrowserTab t in BrowserManager.Tabs.ToList())
                {
                    try
                    {
                        if (GameState > 4)
                        {
                            if (!t.Active) t.Active = true;
                            mainWindow.Draw(t.Draw(mainWindow));
                        }
                        else
                        {
                            if (t.Active) t.Active = false;
                        }
                    }

                    catch { }
                }

                if (UI != null)
                {
                    try { mainWindow.Draw(UI.Draw(mainWindow)); }
                    catch { }
                }
            }
        }
    }
}

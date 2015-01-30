using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Awesomium.Core;
using SFML.Web;

namespace TheKingdom
{
    public class TabTypes
    {
        // The tabs below are used for testing purposes. Static tab is a tab that doesn't really change
        // and a responsive tab does.
        public class StaticTab : BrowserTab
        {
            public StaticTab(int id, string URL, int w, int h, int x, int y)
                : base(id, URL, w, h, x, y)
            {                
            }

            public override void Callback(string message)
            {
                switch (message)
                {
                    case "close":
                        break;
                    default:
                        break;
                }
            }
        }

        public class UITab : BrowserTab
        {
            public UITab(int id, string URL, int w, int h, int x, int y)
                : base(id, URL, w, h, x, y)
            {
                this.MyTab.LoadingFrameComplete += MyTab_LoadingFrameComplete;
            }

            void MyTab_LoadingFrameComplete(object sender, FrameEventArgs e)
            {
                SceneManager.GameState = 0;
            }

            public override void Callback(string message)
            {
                switch (message)
                {
                    case "back":
                        MyTab.Source = new Uri("file:///UI/MainMenu.html");
                        break;
                    case "settings":
                        MyTab.Source = new Uri("file:///UI/settings.html");
                        break;
                    case "savegame":
                        MyTab.Source = new Uri("file:///UI/savescreen.html");
                        break;
                    case "loadgame":
                        MyTab.Source = new Uri("file:///UI/loadscreen.html");
                        break;
                    case "continue":                        
                        break;
                    case "close":
                        GlobalData.Closing = true;
                        break;
                    default:
                        break;
                }
            }
        }        
    }
}

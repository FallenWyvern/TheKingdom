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
                if (SceneManager.GameState == -1) SceneManager.GameState = 0;
                if (SceneManager.GameState == 1) 
                {
                    CallJS("setData", new JSValue[] { GlobalData.Fullscreen, GlobalData.MusicVolume, GlobalData.SoundVolume, GlobalData.Screen_Width + "x" + GlobalData.Screen_Height });
                };
            }

            public override void Callback(string message)
            {
                Console.WriteLine(SceneManager.GameState + " " + message);
                switch (SceneManager.GameState)
                {
                    case 0:
                        switch (message)
                        {
                            case "back":
                                SceneManager.ChangeScene(0);
                                break;
                            case "settings":
                                SceneManager.ChangeScene(1);
                                break;
                            case "savegame":
                                SceneManager.ChangeScene(2);
                                break;
                            case "loadgame":
                                SceneManager.ChangeScene(3);
                                break;
                            case "newgame":
                            case "continue":
                                SceneManager.ChangeScene(4);
                                break;
                            case "close":
                                GlobalData.Closing = true;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 1:
                        switch (message)
                        {
                            case "back":
                                SceneManager.ChangeScene(0);
                                break;
                            default:                        
                                string[] parameters = message.Split('&');
                                GlobalData.Screen_Width = Convert.ToInt32(parameters[0].Split('=')[1].Split('x')[0]);
                                GlobalData.Screen_Height = Convert.ToInt32(parameters[0].Split('=')[1].Split('x')[1]);
                                GlobalData.Fullscreen = Convert.ToInt32(parameters[1].Split('=')[1]);
                                GlobalData.MusicVolume = Convert.ToInt32(parameters[2].Split('=')[1]);
                                GlobalData.SoundVolume = Convert.ToInt32(parameters[3].Split('=')[1]);
                                GlobalData.SaveSettings();
                                SceneManager.ChangeScene(0);
                                break;
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    default:
                        break;
                }
            }

            public void CallJS(string methodName, JSValue[] args)
            {
                JSObject window = MyTab.ExecuteJavascriptWithResult("window");

                if (window == null)
                    return;

                using (window)
                {
                    window.InvokeAsync(methodName, args);
                }
            }
        }        
    }
}

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
    class TabTypes
    {
        // The tabs below are used for testing purposes. Static tab is a tab that doesn't really change
        // and a responsive tab does.
        public class StaticTab : BaseTab
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
                        BrowserManager.DestroyTab(this.ID);
                        break;
                    case "response":
                        string jquery = "<script src=\"jquery-2.1.3.min.js\"></script>";
                        string js = "<script type=\"text/javascript\">function setDivText(s){$(\"#msgDiv\").text(s);} $(document).ready(function () {setDivText(\"This is the start up text.\");});</script>";
                        string html = jquery + "<html><head>" + jquery + Environment.NewLine + js + "</head><body bgcolor=\"#1111ff\"><p style='color:#fff'>Replacement Text</p><div id=\"msgDiv\"></div><button type=\"button\" onclick=\"window.alert('close');\">Close</button></body></html>";
                        System.IO.File.WriteAllText("./UI/dynamic" + BrowserManager.Tabs.Count + ".html", html);
                        ResponseTab tab = new ResponseTab(BrowserManager.Tabs.Count, "file:///UI/dynamic" + BrowserManager.Tabs.Count + ".html", 200, 200, 50, 50);
                        tab.Clickable = true;
                        tab.DragEnabled = true;
                        BrowserManager.NewTab(tab);
                        break;
                    case "create":
                        string chtml = "<html><head></head><body bgcolor=\"#ff1111\"><p style='color:#fff'> %TIME% </p><button type=\"button\" onclick=\"window.alert('close');\">Close</button></body></html>";
                        StaticTab ctab = new StaticTab(BrowserManager.Tabs.Count, "data:text/html, " + chtml.Replace("%TIME%", DateTime.Now.ToString()), 200, 200, 50, 50);
                        ctab.Clickable = true;
                        ctab.DragEnabled = true;
                        BrowserManager.NewTab(ctab);
                        break;
                    default:
                        break;
                }
            }
        }

        public class ResponseTab : BaseTab
        {
            System.Timers.Timer t = new System.Timers.Timer();
            public ResponseTab(int id, string URL, int w, int h, int x, int y)
                : base(id, URL, w, h, x, y)
            {
                t.Interval = 500;
                t.Elapsed += (sender, e) =>
                {                    
                    MyTab.BeginInvoke((ThreadStart)delegate()
                    {
                        ExecuteJavascript(this, new EventArgs());
                    });
                };

                t.Disposed += (sender, e) =>
                {
                    System.IO.File.Delete("./UI/dynamic" + ID + ".html");
                };

                MyTab.LoadingFrameComplete += (sender, e) =>
                {                    
                    t.Start();
                };
            }

            private void ExecuteJavascript(object sender, EventArgs eventArgs)
            {                
                WebCore.QueueWork(MyTab, () =>
                {
                    JSObject window = MyTab.ExecuteJavascriptWithResult("window");

                    if (window == null)
                        return;

                    using (window)
                    {
                        window.InvokeAsync("setDivText", DateTime.Now.ToString());
                    }
                });
            }

            public override void Callback(string message)
            {
                switch (message)
                {
                    case "close":
                        t.Stop();
                        t.Dispose();
                        BrowserManager.DestroyTab(this.ID);
                        break;
                    default:
                        break;
                }
            }
        }

        public class MainMenuTab : BaseTab
        {
            public MainMenuTab(int id, string URL, int w, int h, int x, int y)
                : base(id, URL, w, h, x, y)
            {
                
            }

            public override void Callback(string message)
            {
                switch (message)
                {
                    case "newgame":
                        GlobalData.GameInProgress = true;
                        MyTab.BeginInvoke((ThreadStart)delegate()
                        {
                            ExecuteJavascript(this, new EventArgs());
                        });                        
                        SceneManager.ChangeScene(4);                        
                        break;
                    case "loadgame":
                        break;
                    case "settings":
                        SceneManager.ChangeScene(3);
                        break;
                    case "close":
                        TheKingdom.GlobalData.Closing = true;
                        break;
                    default:
                        break;
                }
            }

            private void ExecuteJavascript(object sender, EventArgs eventArgs)
            {
                WebCore.QueueWork(MyTab, () =>
                {
                    JSObject window = MyTab.ExecuteJavascriptWithResult("window");

                    if (window == null)
                        return;

                    using (window)
                    {
                        string newOrContinue = "";                        
                        newOrContinue = GlobalData.GameInProgress ? "Continue" : "New";
                        window.InvokeAsync("setNewButton", newOrContinue);
                    }
                });
            }
        }

        public class SettingsTab : BaseTab
        {
            public SettingsTab (int id, string URL, int w, int h, int x, int y)
                : base(id, URL, w, h, x, y)
            {                
                MyTab.LoadingFrameComplete += (sender, e) =>
                {                    
                    MyTab.BeginInvoke((ThreadStart)delegate()
                    {
                        ExecuteJavascript(this, new EventArgs());
                    });
                };
            }

            private void ExecuteJavascript(object sender, EventArgs eventArgs)
            {                
                JSObject window = MyTab.ExecuteJavascriptWithResult("window");

                if (window == null)          
                    return;                

                using (window)
                {                    
                    window.InvokeAsync("setDivText", GlobalData.Screen_Width + "x" + GlobalData.Screen_Height);
                }
            }

            public override void Callback(string message)
            {
                switch (message)
                {
                    case "resolution":

                        break;
                    case "back":
                        SceneManager.ChangeScene(0);
                        break;
                    default:
                        break;
                }
            }
        }

        public class CityTab : BaseTab
        {
            System.Timers.Timer t = new System.Timers.Timer();
            public CityTab(int id, string URL, int w, int h, int x, int y)
                : base(id, URL, w, h, x, y)
            {                            
                t.Interval = 500;
                t.Elapsed += (sender, e) =>
                {
                    if (SceneManager.state < 4) return;
                    MyTab.BeginInvoke((ThreadStart)delegate()
                    {
                        ExecuteJavascript(this, new EventArgs());
                    });
                };
                
                MyTab.LoadingFrameComplete += (sender, e) =>
                {
                    Buildings.Tavern myTavern = new Buildings.Tavern();
                    GlobalData.CityBuildings.Add(myTavern);                    
                    BuildingUpgrades.TavernUpgrade Upgrade = new BuildingUpgrades.TavernUpgrade(myTavern, UpgradeTypes.TavernFoodQuality);
                    t.Start();                    
                };
            }

            private void ExecuteJavascript(object sender, EventArgs eventArgs)
            {
                WebCore.QueueWork(MyTab, () =>
                {
                    JSObject window = MyTab.ExecuteJavascriptWithResult("window");

                    if (window == null)
                        return;

                    using (window)
                    {                        
                        window.InvokeAsync("setHours", "Hours: " + TimeKeeper.hours + " (" + TimeKeeper.phase + ")");
                        window.InvokeAsync("setDays", "Day: " + TimeKeeper.days);
                        window.InvokeAsync("setWeeks", "Week: " + TimeKeeper.weeks);
                        window.InvokeAsync("setMonths", "Month: " + TimeKeeper.months);
                        window.InvokeAsync("setYears", "Year: " + TimeKeeper.years);                        
                    }
                });
            }

            public override void Callback(string message)
            {
                switch (message)
                {           
                    case "menu":
                        SceneManager.ChangeScene(0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;
using Awesomium.Core;
using System.Threading.Tasks;
using System.Threading;

/*
    The SFML Browser Manager is a class that manages your Awesomium tabs while the base tab class allows you to create
    context specific tabs. Ideally, if you simply want a web page, you use base tab. If you want to do something specific
    like watch for context sensitive messages or javascript callbacks, you create a new tab and inherit from BaseTab.
    by overriding "Callback(string Message)" you can get any standard window.alert("MESSAGE") from javascript.
    and by using MyTab.BeginInvoke((ThreadStart)delegate() {}); you can send information into the webpage.   
*/

namespace SFML.Web
{
    /// <summary>
    /// Browser Manager handles all of the threading and messaging between SFML and Awesomium.
    /// </summary>
    public static class BrowserManager
    {
        public static List<BaseTab> Tabs = new List<BaseTab>();   // List of all tabs.
        public static int CurrentTab = 0;                               // Current tabe we're on.        
        public static SynchronizationContext awesomiumContext = null;   // Used to synchronize multiple tabs.
        private static bool Running = false;                            // Check to determine if we're in use.
        public static event EventHandler<EventArgs> CheckContext;       // Subscribe to this event to be notified once the engine is ready.                
        public static BaseTab UI = null;                                // User Interface layer.
        public static bool DraggingUI = false;                          // Are we dragging?
        
        /// <summary>
        /// Starts the Browser Manager on another thread. Without this, no web services are available.
        /// </summary>
        public static void StartBrowserManagerService()
        {
            if (!BrowserManager.Running)
            {
                new System.Threading.Thread(() =>
                {
                    Awesomium.Core.WebCore.Started += (s, e) =>
                    {
                        BrowserManager.awesomiumContext = System.Threading.SynchronizationContext.Current;                        
                        Console.WriteLine("Starting Synchronization Context for Browser");
                        OnContextReady();                        
                    };
                    BrowserManager.Start();
                }).Start();
            }
        }

        /// <summary>
        /// Event called when context is ready.
        /// </summary>
        /// <returns></returns>
        private static bool OnContextReady()
        {            
            if (CheckContext != null)
            {
                CheckContext(null, null);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ends the Browser Manager. Use this when closing your application.
        /// </summary>
        public static void EndBrowserManagerService()
        {
            if (BrowserManager.Running)                         // Stops the browser manager.
            {
                BrowserManager.Close();
            }

            BrowserManager.awesomiumContext = null;             // Release the sync context.            
        }

        /// <summary>
        /// Used internally to start the service.
        /// </summary>
        private static void Start()
        {
            Console.WriteLine("Starting WebCore Engine");
            Running = true;
            WebCore.Run();
        }
        
        /// <summary>
        /// Creates a tab at X/Y location, a w/h size and with an INT ID and a string URL
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="url"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void NewTab(int ID, string url, int w, int h, int x, int y, bool clickable = true, bool keyevent = false)
        {
            while (awesomiumContext == null)
            {
                Console.WriteLine("Context sleeping, waiting for context");                
                return;
            }

            foreach (BaseTab b in Tabs.ToList())
            {
                if (b.ID == ID)
                {
                    Console.WriteLine("Tab already exists on this ID:" + ID);
                    return;
                }
            }

            awesomiumContext.Post(state =>
            {
                Console.WriteLine("Creating tab for " + url);
                BaseTab t = new BaseTab(ID, url, w, h, x, y);                
                t.Clickable = clickable;
                t.KeyEvents = keyevent;
                Tabs.Add(t);
            }, null);
        }

        /// <summary>
        /// New Tab via giving a tab.
        /// </summary>
        /// <param name="tab"></param>
        public static void NewTab(BaseTab tab)
        {
            while (awesomiumContext == null)
            {
                Console.WriteLine("Context sleeping, waiting for context");
                return;
            }
            
            foreach (BaseTab b in Tabs.ToList())
            {
                if (b.ID == tab.ID)
                {
                    Console.WriteLine("Tab already exists on this ID:" + tab.ID);
                    return;
                }
            }
            Tabs.Add(tab);
        }

        /// <summary>
        /// Cleans all tabs. Use this if you want to destroy every existing tab.
        /// </summary>
        public static void Clean()
        {
            Console.WriteLine("Cleaning tabs... Count: " + Tabs.Count);
            while (Tabs.Count > 0)
            {
                for (int i = 0; i < Tabs.Count; i++)
                {
                    awesomiumContext.Send(state =>
                    {
                        Tabs[i].Dispose();
                    }, null);

                    Tabs.Remove(Tabs[i]);
                }
            }
            BrowserManager.EndBrowserManagerService();
            Console.WriteLine("Tabs should be clean. Count: " + Tabs.Count);
        }

        /// <summary>
        /// Returns a specific tab.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static BaseTab FindTabByID(int ID)
        {
            foreach (BaseTab t in Tabs)
            {
                if (t.ID == ID)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a tab that's been clicked
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static BaseTab FindTabByClick(int x, int y)
        {            
            foreach (BaseTab t in Tabs.ToList())
            {
                if (t.MouseOver(x, y))
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Clicks at a specific tab at a specific location.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void MouseMove(int x, int y)
        {
            if (UI != null && !DraggingUI)
            {
                WebCore.QueueWork(UI.MyTab, () => { UI.MyTab.FocusView(); UI.MyTab.InjectMouseMove(x, y); });                                
            }

            if (Tabs.Count == 0) return;

            if (Mouse.IsButtonPressed(Mouse.Button.Right)) { return; }
            
            DraggingUI = false;
            foreach (BaseTab t in Tabs.ToList())
            {
                if (t.MouseOver(x, y) && t.Clickable && !t.closing)
                {
                    WebCore.QueueWork(t.MyTab, () => { t.MyTab.FocusView(); t.MyTab.InjectMouseMove((int)(x - t.View.Position.X), (int)(y - t.View.Position.Y)); });                                    
                    DraggingUI = true;
                }
            }
        }

        /// <summary>
        /// Release mouse on tabs.
        /// </summary>
        public static void MouseUp()
        {
            DraggingUI = false;
            if (UI != null)                
            {                
                awesomiumContext.Send(state =>
                {
                    try { UI.MyTab.InjectMouseUp(MouseButton.Left); }
                    catch { Console.WriteLine("UI Cannot be moved"); }
                }, null);
            }

            if (Tabs.Count == 0) return;

            foreach (BaseTab t in Tabs.ToList())
            {
                if (t.Clickable)
                {
                    awesomiumContext.Send(state =>
                    {
                        t.MyTab.InjectMouseUp(MouseButton.Left);
                    }, null);
                }
                if (t.mouseDrag)
                {
                    t.mouseDrag = false;                    
                }                
            }            
        }

        /// <summary>
        /// Inject MouseDown at x/y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void MouseDown(int x, int y, SFML.Window.Mouse.Button b)
        {
            if (UI != null && UI.MouseOver(x, y))
            {
                awesomiumContext.Send(state =>
                {
                    try { UI.MyTab.InjectMouseDown(MouseButton.Left); }
                    catch { Console.WriteLine("UI Cannot be moved"); }
                }, null);
            }

            if (Tabs.Count == 0) return;

            List<BaseTab> tablist = Tabs.ToList();
            tablist.Reverse();

            foreach (BaseTab t in tablist)
            {
                if (t.MouseOver(x, y) && t.Clickable && b == Mouse.Button.Left)
                {                    
                    CurrentTab = t.ID;
                    Tabs.Remove(t);
                    Tabs.Add(t);

                    awesomiumContext.Send(state =>
                    {
                        t.MyTab.InjectMouseDown(MouseButton.Left);                        
                    }, null);                    
                    return;
                }                              
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if (Tabs.Count == 0) return;
                foreach (BaseTab t in tablist)
                {
                    if (t.MouseOver(x, y))
                    {
                        Tabs.Remove(t);
                        Tabs.Add(t);
                        t.mouseDrag = true;
                        t.dragOffsetX = (uint)(x - t.View.Position.X);
                        t.dragOffsetY = (uint)(y - t.View.Position.Y);
                        return;
                    }
                }                
            }
        }

        /// <summary>
        /// Allows keys to be passed to the view that the mouse is over.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void InjectKey(Keyboard.Key k, int x, int y)
        {            
            if (Tabs.Count == 0) return;
            foreach (BaseTab t in Tabs)
            {                
                if (t.MouseOver(x, y) && t.KeyEvents)
                {                    
                    Console.WriteLine("Sending " + k.ToString() + " to " + t.ID);
                    WebCore.QueueWork(t.MyTab, () =>
                    {
                        if (k != Keyboard.Key.F5)
                        {
                            WebKeyboardEvent keyboardEvent = new WebKeyboardEvent();
                            keyboardEvent.Text = k.ToString();
                            keyboardEvent.Type = WebKeyboardEventType.Char;
                            t.MyTab.InjectKeyboardEvent(keyboardEvent);                            
                        }
                        else
                        {
                            Console.WriteLine("Refreshing tab...");
                            t.MyTab.Source = new Uri(t.url);
                        }
                    });
                    return;
                }
            }
        }

        /// <summary>
        /// Used to destroy a tab.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool DestroyTab(int ID)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].ID == ID)
                {
                    awesomiumContext.Send(state =>
                    {
                        Tabs[i].closing = true;
                        Tabs[i].Dispose();
                    }, null);

                    Tabs.Remove(Tabs[i]);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Shuts down the WebManager.
        /// </summary>
        private static void Close()
        {
            Console.WriteLine("Shutting Down WebCore Engine");
            Running = false;
            Clean();
            WebCore.Shutdown();
        }
    }

    /// <summary>
    /// Base tab is the class to inherit new types of tabs from.
    /// </summary>
    public class BaseTab : IDisposable
    {
        public int ID = 0;              // ID of this tab.
        public string url;              // Current URL
        public bool closing = false;    // Used to close tab.
        public WebView MyTab;           // The Awesomeium WebView
        public Sprite View;             // The SFML Sprite that is returned for drawing.
        public bool Clickable = true;   // Is this view clickable.
        public bool KeyEvents = true;  // Is this view typeable.
        public bool mouseDrag = false;  // Is this view being dragged.        
        public bool DragEnabled = false;    // Can this view be dragged.
        public bool Active = true;
        public bool opened = false;         // Is the tab open yet. Not being open causes white blank tabs.        
        public uint dragOffsetX = 0;    // Where was the mouse when dragging began.
        public uint dragOffsetY = 0;    // ^

        private Texture BrowserTex;     // Texture that copies the Bitmap Surface
        private BitmapSurface s;        // Bitmap surface for the Webview.
        private byte[] webBytes;        // byte array of image data buffer.

        private int browserwidth;       // Width of browesr.
        private int browserheight;      // Height of browser.        

        /// <summary>
        /// Instantiates a new tab.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="URL"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public BaseTab(int id, string URL, int w, int h, int x, int y)
        {
            ID = id;
            url = URL;            

            browserwidth = w;
            browserheight = h;

            s = new BitmapSurface(w, h);
            webBytes = new byte[w * h * 4];

            BrowserTex = new Texture((uint)browserwidth, (uint)browserheight);
            View = new Sprite(BrowserTex);
            View.Position = new Vector2f((uint)x, (uint)y);            

            WebSession session = WebCore.CreateWebSession(new WebPreferences()
            {
                WebSecurity = false,
                FileAccessFromFileURL = true,
                UniversalAccessFromFileURL = true,
                LocalStorage = true,
            });

            MyTab = WebCore.CreateWebView(browserwidth, browserheight, session, WebViewType.Offscreen);            
            MyTab.IsTransparent = true;
                        
            MyTab.Source = new Uri(URL);
            
            MyTab.Surface = s;

            MyTab.ShowJavascriptDialog += (sender, e) =>
            {
                Console.WriteLine("JS: " + e.Message + " : " + id + " " + this.GetType().ToString());
                Callback(e.Message);                
            }; 

            s.Updated += (sender, e) =>
            {
                if (!Active) return;
                UpdateTexture();
            };            
        }

        public virtual void Callback(string message)
        {
            // This can be called by anything which inherits from
            // basetab. By using override, this will be called
            // whenever javascript is called.
        }

        /// <summary>
        /// Updates texture.
        /// </summary>
        public void UpdateTexture()
        {            
            unsafe
            {
                fixed (Byte* byteptr = webBytes)
                {
                    s.CopyTo((IntPtr)byteptr, s.RowSpan, 4, true, false);
                    BrowserTex.Update(webBytes);
                }
            }            
        }
                
        /// <summary>
        /// Returns the surface for drawing.
        /// </summary>
        /// <returns></returns>
        public Sprite Draw(RenderWindow win)
        {
            if (!opened) { System.Threading.Thread.Sleep(100); opened = true; }
            if (!Active) return null;
            if (!closing)
            {                
                if (mouseDrag && DragEnabled)
                {                                  
                    Move((uint)Mouse.GetPosition(win).X - dragOffsetX, (uint)Mouse.GetPosition(win).Y - dragOffsetY, win.Size.X, win.Size.Y);                    
                }
                
                return View;                
            }
            else
            {                
                return null;
            }
        }

        /// <summary>
        /// Check if point is over this tab.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool MouseOver(int x, int y)
        {
            if (closing) return false;

            if (x > View.Position.X && x < View.Position.X + BrowserTex.Size.X)
            {
                if (y > View.Position.Y && y < View.Position.Y + BrowserTex.Size.Y)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Move the Sprite around the screen.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(uint x, uint y, uint max_x = 1920, uint max_y = 1080)
        {
            if (x < 0) x = 0;
            if (x > max_x) x = 0;
            if (y < 0) y = 0;
            if (y > max_y) y = 0;            
            View.Position = new Vector2f(x, y);              
        }

        /// <summary>
        /// Used to dispose of resources.
        /// </summary>
        public void Dispose()
        {
            Console.Write("Destroying Browser");
            s.Updated += (sender, e) => { Console.WriteLine("Destroying browser: " + url); };
            MyTab.Stop();
            MyTab.Dispose();
            Console.Write(".");
            closing = true;
            Console.Write(".");
            BrowserTex.Dispose();
            s.Dispose();
            View.Dispose();
            Console.Write(". ");
            Console.WriteLine("Browser Destroyed");
        }
    }
}

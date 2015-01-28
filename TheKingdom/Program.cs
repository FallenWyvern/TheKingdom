using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.Web;

namespace TheKingdom
{
    class Program
    {
        static RenderWindow Win;
        static SceneManager Scenes;
        static SoundManager SoundEngine = new SoundManager();

        static void Main(string[] args)
        {
            GlobalData.LoadSettings();
         
            Win = new RenderWindow(new VideoMode((uint)GlobalData.Screen_Width, (uint)GlobalData.Screen_Height), "The Kingdom", Styles.Titlebar);
            Win.Resized += (sender, e) =>
            {
                GlobalData.Screen_Width = (int)e.Width;
                GlobalData.Screen_Height = (int)e.Height;
                Win.SetView(new View(new FloatRect(0f, 0f, e.Width, e.Height)));                
            };

            BrowserManager.StartBrowserManagerService();
            BrowserManager.CheckContext += BrowserManager_CheckContext;                

            Win.Closed += (Sender, e) => Win.Close();
            
            Scenes = new SceneManager(Win);
            TimeKeeper.Start();
            SoundEngine.Play();
 
            while (Win.IsOpen() && !GlobalData.Closing)
            {
                Win.DispatchEvents();
                Win.Clear();
                Scenes.Draw();
                TimeKeeper.Update(); 
                Win.Display();                
            }

            SoundEngine.Stop();            
            Environment.Exit(0);  
        }

        static void BrowserManager_CheckContext(object sender, EventArgs e)
        {            
            Win.MouseMoved += (s, se) => BrowserManager.MouseMove(se.X, se.Y);
            Win.MouseButtonPressed += (s, se) => BrowserManager.MouseDown(se.X, se.Y, se.Button);
            Win.MouseButtonReleased += (s, se) => BrowserManager.MouseUp();
            Win.KeyPressed += (s, se) => BrowserManager.InjectKey(se.Code, Mouse.GetPosition(Win).X, Mouse.GetPosition(Win).Y);

            Scenes.Start();

            TabTypes.StaticTab t = new TabTypes.StaticTab(0, "file:///UI/test.html", 300, 200, 400, 400);
            t.Clickable = true;
            t.DragEnabled = true;
            t.KeyEvents = true;
            BrowserManager.NewTab(t);            
        }
    }
}

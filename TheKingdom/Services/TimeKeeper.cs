using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKingdom
{
    public static class TimeKeeper
    {
        public enum Phase { None, Dawn, Morning, Noon, Afternoon, Dusk, Night };

        public static Phase phase = Phase.Dawn;
        public static int hours = 6;
        public static int days = 1;
        public static int weeks = 1;
        public static int months = 1;
        public static int years = 1;
        
        public static System.Timers.Timer t = new System.Timers.Timer();

        static public void Start()
        {                                                            
            t.Interval = 3000;
            t.Elapsed += (sender, e) =>
            {
                hours++;

                if (hours > 5 && hours < 7)
                {
                    phase = Phase.Dawn;
                }

                if (hours > 7 && hours < 11)
                {
                    phase = Phase.Morning;
                }
                
                if (hours > 11 && hours < 13)
                {
                    phase = Phase.Noon;
                }

                if (hours > 13 && hours < 17)
                {
                    phase = Phase.Afternoon;
                }

                if (hours > 17 && hours < 20)
                {
                    phase = Phase.Dusk;
                }

                if (hours > 21 || hours < 5)
                {
                    phase = Phase.Night;
                }

                if (hours % 25 == 0)
                {
                    hours = 1;
                    days++;
                }

                if (days % 11 == 0)
                {
                    days = 1;
                    weeks++;
                }

                if (weeks % 5 == 0)
                {
                    weeks = 1;
                    months++;
                }

                if (months % 11 == 0)
                {
                    months = 1;
                    years++;
                }
            };            
        }        

        public static void Update()
        {
            if (SceneManager.state < 5)
            {
                if (t.Enabled) t.Stop();
                return;
            }

            if (SceneManager.state > 4)
            {
                foreach (BaseBuilding b in GlobalData.CityBuildings)
                {
                    b.Update();
                }

                if (!t.Enabled) t.Start();
            }
        }        
    }

    public class Check
    {
        TimeKeeper.Phase p;
        int H;
        int D;
        int W;
        int M;
        int Y;

        public Check(TimeKeeper.Phase phase = TimeKeeper.Phase.None, int Hours = 0, int Days = 0, int Weeks = 0, int Months = 0, int Years = 0)
        {
            p = phase;
            H = Hours;
            D = Days;
            W = Weeks;
            M = Months;
            Y = Years;
        }

        public bool Trigger()
        {
            if (TimeKeeper.phase == p || p == TimeKeeper.Phase.None)
            {
                if (TimeKeeper.hours == H || H == 0)
                {
                    if (TimeKeeper.days == D || D == 0)
                    {
                        if (TimeKeeper.weeks == W || W == 0)
                        {
                            if (TimeKeeper.years == Y || Y == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}

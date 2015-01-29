using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKingdom
{    
        public class BaseEvent
        {            
            public Check EventTimer;
            public bool Reset = false;
            public int BaseChance = 0;
            public int Chance = 0;
            Random r = new Random();
            public bool Done = false;

            public BaseEvent(Check eventTime, int chance, bool reset, string ResetValue = "0;0;0;0;0")
            {
                Reset = reset;
                EventTimer = eventTime;
                BaseChance = chance;
                Chance = chance;
            }

            public void Check()
            {
                if (Done) return;

                PreCheck();                

                if (EventTimer.Trigger())
                {
                    if (r.Next(0, 100) <= Chance)
                    {
                        OnSuccess();
                    }
                    else
                    {
                        OnFail();
                    }
                }
                
                ResetTimer();
            }

            public virtual void PreCheck()
            {
                // Do anything here you need to before checking for success.
            }

            public virtual void OnSuccess()
            {

            }

            public virtual void OnFail()
            {

            }

            public void ResetTimer()
            {
                if (!Reset) { Done = true; }
            }
        }

        public class GainResidentEvent : BaseEvent
        {
            public GainResidentEvent(Check eventTime, int chance, bool reset, string ResetValue = "0;0;0;0;0") : base (eventTime, chance, reset, ResetValue = "0;0;0;0;0")
            {                
            }

            public override void OnSuccess()
            {
                Console.WriteLine("Add Citizens");
            }

            public override void PreCheck()
            {
                Chance = BaseChance;

                foreach (BaseBuilding b in GlobalData.CityBuildings)
                {
                    if (b.ThisType == BuildingType.Tavern)
                    {
                        Chance += 2;
                        foreach (BaseUpgrades upgrade in b.ThisUpgrades)
                        {
                            if (upgrade.MyType == UpgradeTypes.TavernEntertainmentQuality)
                            {
                                Chance += 1;
                            }
                        }
                    }
                }

                //Console.WriteLine("Chance to gain Resident: %" + Chance);
            }
        }

        public class GainTravellerEvent : BaseEvent
        {                     
            public GainTravellerEvent(Check eventTime, int chance, bool reset, string ResetValue = "0;0;0;0;0")
                : base(eventTime, chance, reset, ResetValue = "0;0;0;0;0")
            {
            }

            public override void OnSuccess()
            {
                Console.WriteLine("Add Travellers");
                List<Buildings.Tavern> TavernList = new List<Buildings.Tavern>();
                foreach (Buildings.Tavern b in GlobalData.CityBuildings)
                {
                    TavernList.Add(b);
                }

                Random r = new Random();
                TavernList[r.Next(0, TavernList.Count)].AddPatrons();
            }

            public override void PreCheck()
            {
                Chance = BaseChance;                

                foreach (BaseBuilding b in GlobalData.CityBuildings)
                {
                    if (b.ThisType == BuildingType.Tavern)
                    {
                        Chance += 2;
                        Chance += (b.ThisUpgrades.Count * 2);
                    }
                }

                Console.WriteLine("Tavern chance for new Patrons : %" + Chance);
            }
        }

        public class LoseTravellerEvent : BaseEvent
        {
            public LoseTravellerEvent(Check eventTime, int chance, bool reset, string ResetValue = "0;0;0;0;0")
                : base(eventTime, chance, reset, ResetValue = "0;0;0;0;0")
            {
            }

            public override void OnSuccess()
            {
                Console.WriteLine("Add Travellers");
                List<Buildings.Tavern> TavernList = new List<Buildings.Tavern>();
                foreach (Buildings.Tavern b in GlobalData.CityBuildings)
                {
                    TavernList.Add(b);
                }

                Random r = new Random();
                Buildings.Tavern temp = TavernList[r.Next(0, TavernList.Count)];
                int patronsLost = r.Next(1, temp.Patrons);
                temp.Patrons -= patronsLost;
                GlobalData.Visitors -= patronsLost;
            }

            public override void PreCheck()
            {                
            }
        }
}

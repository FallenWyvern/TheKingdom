using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    So this is how to do buildings:
    Create a building that is inherited from BaseBuilding.
    Each frame, each building is checked by running Building.Update();
    For performance reasons, skip the update method when possible.
    
    When adding an upgrade, create the specific upgrade you want and have
    it target the building you want. The Upgrade has a timer that will go off
    when the construction is complete, adds itself to the target and runs the
    method 'OnAdd'. Each upgrade is suited to a building, and runs through
    a switch case of the various *types* of upgrades. If it's appropriate, it
    does it's thing.
*/

namespace TheKingdom
{
    public enum BuildingType { Tavern }
    public enum UpgradeTypes { TavernCapacityUpgrade, TavernFoodQuality, TavernEntertainmentQuality }

    // Base class buildings are built from.
    public class BaseBuilding
    {    
            public int quality = 100;
            public BuildingType ThisType;
            public List<BaseUpgrades> ThisUpgrades = new List<BaseUpgrades>();

            public virtual void Update()
            {
                // Add code inside an overridden version of this
                // in each building to figure out what they do on update.
            }
    }

    // Base class upgrades are built from.
    public class BaseUpgrades
    {
        public Check TimeToBuild = null;
        public UpgradeTypes MyType;
        public bool Finished = false;
        
        public void Start(Check triggertime, BaseBuilding target)
        {            
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 500;
            t.Elapsed += (sender, e) =>
            {
                if (triggertime.Trigger())
                {
                    t.Stop();
                    t.Dispose();
                    target.ThisUpgrades.Add(this);
                    OnAdd();
                }
            };
            t.Start();
        }

        public virtual void OnAdd()
        {

        }
    }

    // All building upgrades. The base upgrade class has a timer
    // but the timer value is filled in here. 
    public class BuildingUpgrades 
    {
        public class TavernUpgrade : BaseUpgrades
        {
            Buildings.Tavern TargetTavern;

            public TavernUpgrade(Buildings.Tavern target, UpgradeTypes upgrade, Check OverrideTimeToBuild = null)
            {
                TargetTavern = target;
                MyType = upgrade;

                if (OverrideTimeToBuild == null)
                {
                    TimeToBuild = new Check(TimeKeeper.Phase.Morning);
                }
                else
                {
                    TimeToBuild = OverrideTimeToBuild;
                }
                
                Start(TimeToBuild, target);
            }

            public override void OnAdd()
            {
                switch (MyType)
                {
                    case UpgradeTypes.TavernCapacityUpgrade:
                        Console.WriteLine("Upgrading Tavern Capacity");
                        break;
                    case UpgradeTypes.TavernEntertainmentQuality:
                        Console.WriteLine("Upgrading Tavern Entertainment");
                        break;
                    case UpgradeTypes.TavernFoodQuality:
                        Console.WriteLine("Upgrading Food Quality");
                        break;
                }                
            }
        }        
    }

    // All buildings. 
    public class Buildings
    {
        public class Tavern : BaseBuilding
        {
            public int Capacity = 10;
            public int Patrons = 0;
            public int Entertainment = 0;
            public List<BaseEvent> TavernEvents = new List<BaseEvent>();

            public Tavern()
            {
                ThisType = BuildingType.Tavern;
            }

            public override void Update()
            {                

            }            

            public void AddPatrons()
            {
                if (Capacity == Patrons) return;

                Random r = new Random();
                double low = Capacity * 0.1;
                double high = (Capacity * 0.1) * 4;
                
                if (high > Capacity - Patrons) high = Capacity - Patrons;
                if (low > high) low = high - 1;

                int adding = r.Next((int)low, (int)high);
                Patrons += adding;
                Console.WriteLine(high + " " + low + " " + adding);
                Console.WriteLine("Patrons: " + Patrons + " of " + Capacity);
                Console.WriteLine("Adding " + adding + " to tavern.");

                GlobalData.Visitors += adding;
            }            
        }        
    }
}

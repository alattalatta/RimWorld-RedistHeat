using System.Text;
using RimWorld;
using Verse;

namespace RedistHeat
{
    public class Building_ExhaustPort : Building_DuctSwitchable
    {
        public IntVec3 VecNorth { get; private set; }
        public IntVec3 VecSouth { get; private set; }
        public IntVec3 VecEast { get; private set; }
        public IntVec3 VecWest { get; private set; }

        public bool isAvailable;

        private Building_IndustrialCooler neighCooler;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            VecNorth = Position + IntVec3.North.RotatedBy( Rotation );
            VecSouth = Position + IntVec3.South.RotatedBy( Rotation );
            VecEast = Position + IntVec3.East.RotatedBy(Rotation);
            VecWest = Position + IntVec3.West.RotatedBy(Rotation);
        }

        public override void Tick()
        {
            if (!this.IsHashIntervalTick( 60 ))
            {
                return;
            }

            neighCooler = AdjacentCooler();
            if (compPowerTrader.PowerOn && neighCooler != null && !VecNorth.Impassable(this.Map))
            {
                isAvailable = true;
                compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
            }
            else
            {
                isAvailable = false;
                compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption*0.1f;
            }
        }

        public override string GetInspectString()
        {
            var str = new StringBuilder();
            str.AppendLine( base.GetInspectString() );
            str.Append( ResourceBank.StringState + ": " );

            str.Append( isAvailable ? ResourceBank.StringWorking : ResourceBank.StringNotWorking );

            return str.ToString();
        }

        public void PushHeat( float amount )
        {
            if (Net)
            {
                compAir.SetNetTemperatureDirect(amount);
            }
            else
            {
                if (VecNorth.UsesOutdoorTemperature(this.Map))
                {
                    return;
                }
                GenTemperature.PushHeat(VecNorth, this.Map, amount);
            }
        }

        public AirNet GetNet()
        {
            return compAir.connectedNet;
        }

        private Building_IndustrialCooler AdjacentCooler()
        {
            var cooler = Map.thingGrid.ThingAt< Building_IndustrialCooler >( VecSouth );
            if (cooler == null)
            {
                cooler = Map.thingGrid.ThingAt<Building_IndustrialCooler>(VecEast);
                if (cooler == null)
                {
                    cooler = Map.thingGrid.ThingAt<Building_IndustrialCooler>(VecWest);
                }
            }
            return cooler;
        }
    }
}
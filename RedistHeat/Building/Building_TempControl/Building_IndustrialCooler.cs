using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_IndustrialCooler : Building_DuctSwitchable
    {
        private IntVec3 vecSouth, vecSouthEast;
        private Room roomSouth;
        private List< Building_ExhaustPort > activeExhausts;

        private float Energy => compTempControl.Props.energyPerSecond;

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            vecSouth = Position + IntVec3.South.RotatedBy( Rotation );
            vecSouthEast = vecSouth + IntVec3.East.RotatedBy( Rotation );
        }

        public override void Tick()
        {
            if (!this.IsHashIntervalTick( 60 ))
            {
                return;
            }
            if (!Validate())
            {
                WorkingState = false;
                return;
            }

            WorkingState = true;
            ControlTemperature();
        }

        protected override bool Validate()
        {
            if (compAir.connectedNet == null)
            {
                return false;
            }

            if (vecSouth.Impassable(this.Map) || vecSouthEast.Impassable(this.Map))
            {
                return false;
            }

            roomSouth = vecSouth.GetRoom(this.Map);
            if (roomSouth == null)
            {
                return false;
            }

            activeExhausts = GetActiveExhausts();

            if (!compPowerTrader.PowerOn || activeExhausts.Count == 0)
            {
                return false;
            }

            return compTempControl.targetTemperature - 1 < roomSouth.Temperature - 3;
        }

        private void ControlTemperature()
        {
            //Average of exhaust ports' room temperature
            var tempHotAvg = activeExhausts.Sum(s => s.Net ? s.GetNet().NetTemperature : s.VecNorth.GetTemperature(this.Map)) / activeExhausts.Count;

            //Cooler's temperature
            var tempCold = Net ? compAir.connectedNet.NetTemperature : roomSouth.Temperature;
            var tempDiff = tempHotAvg - tempCold;
            var nodeCount = Net ? compAir.connectedNet.nodes.Count : roomSouth.CellCount;

            if (tempHotAvg - tempDiff > 40.0)
            {
                tempDiff = tempHotAvg - 40f;
            }

            var effectiveness = 1.0 - tempDiff * (1.0 / 130.0);
            if (effectiveness < 0.0)
            {
                effectiveness = 0.0f;
            }

            var energyLimit = (float)(Energy * activeExhausts.Count * effectiveness * 4.16666650772095);

            var coldAir = ControlTemperatureTempChange(nodeCount, tempCold, energyLimit,
                                                                       compTempControl.targetTemperature);
            WorkingState = !Mathf.Approximately(coldAir, 0.0f);
            if (!WorkingState)
            {
                return;
            }

            if (Net)
            {
                compAir.SetNetTemperatureDirect(coldAir);
            }
            else
            {
                roomSouth.Temperature += coldAir;
            }

            var hotAir = (float) (-energyLimit*1.25/activeExhausts.Count);

            if (Mathf.Approximately( hotAir, 0.0f ))
            {
                return;
            }

            foreach (var current in activeExhausts)
            {
                current.PushHeat( hotAir );
            }
        }

        private List< Building_ExhaustPort > GetActiveExhausts()
        {
            var origin = GenAdj.CellsAdjacentCardinal( this )
                               .Select( s => Find.VisibleMap.thingGrid.ThingAt< Building_ExhaustPort >( s ) )
                               .Where( thingAt => thingAt != null )
                               .ToList();

            return origin.Where( s => s.isAvailable ).ToList();
        }

        public override string GetInspectString()
        {
            var str = new StringBuilder();
            str.Append( base.GetInspectString() )
               .Append( ResourceBank.StringWorkingDucts )
               .Append( ": " );

            if (activeExhausts != null)
            {
                str.Append( activeExhausts.Count );
            }
            else
            {
                str.Append( "0" );
            }
            return str.ToString();
        }

        private static float ControlTemperatureTempChange(int count, float temperature, float energyLimit, float targetTemperature)
        {
            var b = energyLimit / count;
            var a = targetTemperature - temperature;
            float num;
            if (energyLimit > 0f)
            {
                num = Mathf.Min(a, b);
                num = Mathf.Max(num, 0f);
            }
            else
            {
                num = Mathf.Max(a, b);
                num = Mathf.Min(num, 0f);
            }
            return num;
        }
    }
}
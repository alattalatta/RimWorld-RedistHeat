using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_IndustrialCooler : Building_TempControl
    {
        private IntVec3 vecSouth, vecSouthEast;
        private Room roomSouth;
        private List< Building_ExhaustPort > activeExhausts;

        private float Energy => compTempControl.Props.energyPerSecond;

        private bool isWorking;

        private bool WorkingState
        {
            set
            {
                isWorking = value;

                if (isWorking)
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
                }
                else
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption*
                                                  compTempControl.Props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }

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

        private bool Validate()
        {
            if (vecSouth.Impassable(Map) || vecSouthEast.Impassable(Map))
            {
                return false;
            }

            roomSouth = vecSouth.GetRoom(Map);
            if (roomSouth == null)
            {
                return false;
            }

            activeExhausts = GetActiveExhausts();

            if (!compPowerTrader.PowerOn || activeExhausts.Count == 0)
            {
                return false;
            }

            return compTempControl.targetTemperature < roomSouth.Temperature;
        }

        private void ControlTemperature()
        {
            //Average of exhaust ports' room temperature
            var tempHotAvg = activeExhausts.Sum( s => s.VecNorth.GetTemperature(Map) )/activeExhausts.Count;

            //Cooler's temperature
            var tempCold = roomSouth.Temperature;
            var tempDiff = tempHotAvg - tempCold;

            if (tempHotAvg - tempDiff > 40.0)
            {
                tempDiff = tempHotAvg - 40f;
            }

            var num2 = 1.0 - tempDiff*(1.0/130.0);
            if (num2 < 0.0)
            {
                num2 = 0.0f;
            }

            var energyLimit = (float) (Energy*activeExhausts.Count*num2*4.16666650772095);
            var coldAir = GenTemperature.ControlTemperatureTempChange( vecSouth, Map, energyLimit,
                                                                       compTempControl.targetTemperature );
            isWorking = !Mathf.Approximately( coldAir, 0.0f );
            if (!isWorking)
            {
                return;
            }
            roomSouth.Temperature += coldAir;

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
    }
}
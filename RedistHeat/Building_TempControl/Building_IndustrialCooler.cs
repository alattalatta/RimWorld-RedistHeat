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

        private float Energy => compTempControl.props.energyPerSecond;

        private bool isWorking;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            vecSouth = Position + IntVec3.South.RotatedBy( Rotation );
            vecSouthEast = vecSouth + IntVec3.East.RotatedBy( Rotation );
        }

        public override void TickRare()
        {
            if ( roomSouth == null )
            {
                roomSouth = vecSouth.GetRoom();
            }
            if ( roomSouth == null )
            {
                isWorking = false;
                return;
            }
            
            activeExhausts = GetActiveExhausts();

            if ( AdjacentExhausts().Count == 0 )
            {
                isWorking = false;
                return;
            }

            if ( !compPowerTrader.PowerOn || vecSouth.Impassable() || vecSouthEast.Impassable() || activeExhausts.Count == 0 )
            {
                isWorking = false;
            }
            else
            {
                isWorking = true;
            }

            if ( isWorking )
            {
                ControlTemperature();
                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption;
            }
            else
            {
                compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption*
                                              compTempControl.props.lowPowerConsumptionFactor;
            }

            compTempControl.operatingAtHighPower = isWorking;
        }

        public override string GetInspectString()
        {
            var str = new StringBuilder();
            str.Append( base.GetInspectString() )
               .Append( ResourceBank.StringWorkingDucts )
               .Append( ": ");

            if ( activeExhausts != null )
            {
                str.Append( activeExhausts.Count );
            }
            else
            {
                str.Append( "0" );
            }
            return str.ToString();
        }

        private void ControlTemperature()
        {
            //Average of exhaust ports' room temperature
            float tempHotSum = 0;
            foreach ( var current in activeExhausts )
            {
                tempHotSum += current.VecNorth.GetTemperature();
            }
            var tempHotAvg = tempHotSum/activeExhausts.Count;

            //Cooler's temperature
            var tempCold = roomSouth.Temperature;
            var tempDiff = tempHotAvg - tempCold;

            if ( tempHotAvg - tempDiff > 40.0 )
            {
                tempDiff = tempHotAvg - 40f;
            }

            var num2 = 1.0 - tempDiff*(1.0/130.0);
            if ( num2 < 0.0 )
            {
                num2 = 0.0f;
            }

            var energyLimit = (float) (Energy*activeExhausts.Count*num2*4.16666650772095);
            var coldAir = GenTemperature.ControlTemperatureTempChange( vecSouth, energyLimit,
                compTempControl.targetTemperature );
            isWorking = !Mathf.Approximately( coldAir, 0.0f );
            if ( !isWorking )
            {
                return;
            }
            roomSouth.Temperature += coldAir;

            var hotAir = (float) (-energyLimit*1.25/activeExhausts.Count);

            if ( Mathf.Approximately( hotAir, 0.0f ) )
            {
                return;
            }

            foreach ( var current in activeExhausts )
            {
                GenTemperature.PushHeat( current.VecNorth, hotAir );
            }
        }

        private List< Building_ExhaustPort > AdjacentExhausts()
        {
            var list = new List< Building_ExhaustPort >();
            foreach ( var c in GenAdj.CellsAdjacentCardinal( this ) )
            {
                var finder = Find.ThingGrid.ThingAt< Building_ExhaustPort >( c );
                if ( finder != null )
                {
                    list.Add( finder );
                }
            }
            return list;
        }

        private List< Building_ExhaustPort > GetActiveExhausts()
        {
            var list = new List< Building_ExhaustPort >();

            foreach ( var current in AdjacentExhausts().Where( s => s.isAvailable ) )
            {
                list.Add( current );
            }

            return list;
        }
    }
}
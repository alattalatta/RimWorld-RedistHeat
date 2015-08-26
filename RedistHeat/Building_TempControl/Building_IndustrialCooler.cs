using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_IndustrialCooler : Building_TempControl
    {
        private IntVec3 vecSouth, vecSouthEast;
        private List< Building_ExhaustPort > neighDucts;

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
            if ( AdjacentDucts().Count == 0 )
            {
                isWorking = false;
                return;
            }

            neighDucts = GetAvailableDucts( AdjacentDucts() );

            if ( !compPowerTrader.PowerOn || vecSouth.Impassable() || vecSouthEast.Impassable() || neighDucts.Count == 0 )
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
            str.Append( base.GetInspectString() );
            if ( neighDucts != null )
            {
                str.Append( ResourceBank.StringWorkingDucts + ": " + neighDucts.Count );
            }
            else
            {
                str.Append( ResourceBank.StringWorkingDucts + ": 0" );
            }
            return str.ToString();
        }

        private void ControlTemperature()
        {
            var room = vecSouth.GetRoom();
            if ( room == null )
            {
                Log.Warning( "Tried to get northRoom of null." );
                return;
            }

            //Average of exhaust ports' room temperature
            float tempHotSum = 0;
            foreach ( var finder in neighDucts )
            {
                tempHotSum += finder.VecNorth.GetTemperature();
            }
            var tempHotAvg = tempHotSum/neighDucts.Count;

            //Cooler's temperature
            var tempCold = room.Temperature;
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

            var energyLimit = (float) (Energy*neighDucts.Count*num2*4.16666650772095);
            var coldAir = GenTemperature.ControlTemperatureTempChange( vecSouth, energyLimit,
                compTempControl.targetTemperature );
            isWorking = !Mathf.Approximately( coldAir, 0.0f );
            if ( !isWorking )
            {
                return;
            }
            room.Temperature += coldAir;

            var hotAir = (float) (-energyLimit*1.25/neighDucts.Count);

            if ( Mathf.Approximately( hotAir, 0.0f ) )
            {
                return;
            }

            foreach ( var finder in neighDucts )
            {
                GenTemperature.PushHeat( finder.VecNorth, hotAir );
            }
        }

        private List< Building_ExhaustPort > AdjacentDucts()
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

        private static List< Building_ExhaustPort > GetAvailableDucts( IEnumerable< Building_ExhaustPort > list )
        {
            var list2 = new List< Building_ExhaustPort >();
            foreach ( var finder in list )
            {
                if ( finder.isAvailable )
                {
                    list2.Add( finder );
                }
            }
            return list2;
        }
    }
}
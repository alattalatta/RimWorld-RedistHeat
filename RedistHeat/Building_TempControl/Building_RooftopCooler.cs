using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_RoofTopCooler : Building_TempControl
    {
        private Room room;
        private bool isWorking;
        private bool WorkingState
        {
            set
            {
                isWorking = value;

                if ( isWorking )
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption;
                }
                else
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.props.basePowerConsumption *
                                                  compTempControl.props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }

        private float Energy => compTempControl.props.energyPerSecond;

        public override void TickRare()
        {
            if ( Position.Impassable() )
            {
                WorkingState = false;
                return;
            }
            if ( room == null )
            {
                room = Position.GetRoom();
                if ( room == null )
                {
                    WorkingState = false;
                    return;
                }
            }

            isWorking = compPowerTrader.PowerOn;

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

        private void ControlTemperature()
        {
            //Average of exhaust ports' room temperature
            var outdoorTemp = GenTemperature.OutdoorTemp;

            //Cooler's temperature
            var roomTemp = room.Temperature;
            var tempDiff = outdoorTemp - roomTemp;

            if ( outdoorTemp - tempDiff > 40.0 )
            {
                tempDiff = outdoorTemp - 40f;
            }

            var num2 = 1.0 - tempDiff*(1.0/130.0);
            if ( num2 < 0.0 )
            {
                num2 = 0.0f;
            }

            var energyLimit = (float) (Energy*num2*4.16666650772095);
            var coldAir = GenTemperature.ControlTemperatureTempChange( Position, energyLimit,
                                                                       compTempControl.targetTemperature );
            isWorking = !Mathf.Approximately( coldAir, 0.0f );
            room.Temperature += coldAir;
        }
    }
}
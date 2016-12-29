using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_MediumHeater : Building_TempControl, IWallAttachable
    {
        public Thing glower;

        private IntVec3 vecNorth;
        private Room roomNorth;
        private CompMyGlower compGlower;
        private bool isWorking;
        private bool wasLit;

        private bool WorkingState
        {
            get { return isWorking; }
            set
            {
                isWorking = value;

                if (compPowerTrader == null || compTempControl == null)
                {
                    return;
                }
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
            vecNorth = Position + IntVec3.North.RotatedBy( Rotation );

            glower = GenSpawn.Spawn( ThingDef.Named( "RedistHeat_HeaterGlower" ), vecNorth, map );
            ((Building_HeaterGlower) glower).Reinit( this );
            compGlower = glower.TryGetComp< CompMyGlower >();
            //compGlower.Lit = false;
        }

        public override void Destroy( DestroyMode mode = DestroyMode.Vanish )
        {
            glower.Destroy();
            base.Destroy( mode );
        }

        public override void Tick()
        {
            base.Tick();
            if (compPowerTrader.PowerOn && !wasLit)
            {
                compGlower.UpdateLit(true);
                wasLit = true;
            }
            else if (!compPowerTrader.PowerOn && wasLit)
            {
                compGlower.UpdateLit(false);
                wasLit = false;
            }

            if (!this.IsHashIntervalTick( 60 ))
            {
                return;
            }
            if (!Validate())
            {
                WorkingState = false;
                return;
            }

            ControlTemperature();
        }

        protected virtual bool Validate()
        {
            if (vecNorth.Impassable(Map))
            {
                return false;
            }

            roomNorth = vecNorth.GetRoom(Map);
            if (roomNorth == null)
            {
                return false;
            }

            return (compPowerTrader == null || compPowerTrader.PowerOn);
        }

        private void ControlTemperature()
        {
            var temperature = roomNorth.Temperature;
            float energyMod;
            if (temperature < 20f)
            {
                energyMod = 1f;
            }
            else
            {
                energyMod = temperature > 120f
                    ? 0f
                    : Mathf.InverseLerp( 120f, 20f, temperature );
            }
            var energyLimit = compTempControl.Props.energyPerSecond*energyMod*4.16666651f;
            var hotAir = GenTemperature.ControlTemperatureTempChange( vecNorth, Map, energyLimit,
                                                                      compTempControl.targetTemperature );

            var hotIsHot = !Mathf.Approximately( hotAir, 0f );
            if (hotIsHot)
            {
                roomNorth.Temperature += hotAir;
                WorkingState = true;
            }
            else
            {
                WorkingState = false;
            }
        }
    }
}
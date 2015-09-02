using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Building_ActiveVent : Building_Vent
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        protected override bool Validate()
        {
            if ( compPowerTrader == null )
                return true;

            return (base.Validate() && compPowerTrader.PowerOn &&
                    ValidateTemp( roomNorth.Temperature, roomSouth.Temperature ));
        }

        private bool ValidateTemp( float controlled, float other )
        {
            return ((controlled < compTempControl.targetTemperature && controlled < other) ||
                    (controlled > compTempControl.targetTemperature && controlled > other));
        }

        private void Revert()
        {
            Rotation = new Rot4( Rotation.AsInt + 2 );
            vecNorth = Position + IntVec3.North.RotatedBy( Rotation );
            vecSouth = Position + IntVec3.South.RotatedBy( Rotation );
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            var com = new Command_Action
            {
                defaultLabel = "RedistHeat_RevertVentLabl",
                defaultDesc = "RedistHeat_RevertVentDesc",
                action = Revert,
                icon = Texture2D.blackTexture
            };

            foreach ( var current in base.GetGizmos() )
                yield return current;

            yield return com;
        }
    }
}
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_RooftopCooler : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot )
        {
            var cellsRadial = GenRadial.RadialCellsAround( center, 3, true );
            GenDraw.DrawFieldEdges(cellsRadial.ToList(), Color.white);
        }

        public override AcceptanceReport AllowsPlacing( BuildableDef checkingDef, IntVec3 loc, Rot4 rot )
        {
            var cellsRadial = GenRadial.RadialCellsAround( loc, 3, true );

            foreach ( var current in cellsRadial )
            {
                if ( !current.InBounds() )
                    return false;

                if ( Find.ThingGrid.ThingAt< Building_RoofTopCooler >( current ) != null )
                    return ResourceBank.NotNearWithOther;
            }

            var roof = loc.GetRoof();
            if ( roof == null || loc.GetRoof().isNatural )
                return ResourceBank.NeedConstructedRoof;

            return true;
        }
    }
}

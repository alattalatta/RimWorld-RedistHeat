using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RedistHeat
{
    class PlaceWorker_WallObject : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 center, Rot4 rot, Thing thingToIgnore = null)
        {
            IntVec3 c = center;
            Building wall = c.GetEdifice(base.Map);

            if (wall == null)
            {
                return ResourceBank.NeedsWall;
            }

            if ((wall.def == null) || (wall.def.graphicData == null))
            {
                return ResourceBank.NeedsWall;
            }

            return (wall.def.graphicData.linkFlags & LinkFlags.Wall) != 0 ? AcceptanceReport.WasAccepted : ResourceBank.NeedsWall;
        }
    }
}

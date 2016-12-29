using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RedistHeat
{
    class CompWallObject : ThingComp
    {

        public CompWallObjectProperties Props
        {
            get
            {
                return (CompWallObjectProperties)this.props;
            }
        }

        public override void CompTick()
        {
            //#if DEBUG
            //            Log.Message("Ticking...");
            //#endif
            base.CompTick();
            if (!parent.IsHashIntervalTick(Props.checkTickInterval))
            {
                return;
            }
            Validate();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            Validate();
        }

        public void DestroyParent()
        {
            parent.Destroy(DestroyMode.Deconstruct);
        }


        public void Validate()
        {
//#if DEBUG
//            Log.Message("Checking...");
//#endif
            IntVec3 c = parent.Position;
            Building wall = c.GetEdifice(parent.Map);
            if ((wall == null) || ((wall.def.graphicData.linkFlags & LinkFlags.Wall) == 0))
            {
                DestroyParent();
                return;
            }
        }
    }
}

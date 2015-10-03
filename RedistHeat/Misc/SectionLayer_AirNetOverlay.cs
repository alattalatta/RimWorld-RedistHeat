using RedistHeat;
using Verse;

namespace RimWorld
{
    public class SectionLayer_AirNetOverlay : SectionLayer_Things
    {
        public SectionLayer_AirNetOverlay( Section section ) : base( section )
        {
            requireAddToMapMesh = false;
            relevantChangeTypes = MapMeshFlag.PowerGrid;
        }

        public override void DrawLayer()
        {
            if (OverlayDrawHandler_AirNet.ShouldDrawAirNetOverlay)
            {
                base.DrawLayer();
            }
        }

        protected override void TakePrintFrom( Thing t )
        {
            var ductBuilding = t as Building_DuctBase;
            ductBuilding?.PrintForAirGrid( this );
        }
    }
}
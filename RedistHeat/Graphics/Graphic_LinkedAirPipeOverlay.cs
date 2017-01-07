using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Grahpic_LinkedAirPipeOverlay : Graphic_Linked
    {
        public Grahpic_LinkedAirPipeOverlay( Graphic subGraphic )
            : base( subGraphic )
        {
        }

        public override bool ShouldLinkWith( IntVec3 c, Thing parent )
        {
            var compAir = parent.TryGetComp< CompAir >();
            if (compAir == null)
            {
                return false;
            }

            var lowerFlag = AirNetGrid.NetAt( c, parent.Map, NetLayer.Lower ) != null && compAir.IsLayerOf( NetLayer.Lower );
            var upperFlag = AirNetGrid.NetAt( c, parent.Map, NetLayer.Upper ) != null && compAir.IsLayerOf( NetLayer.Upper );
            return c.InBounds(parent.Map) && (lowerFlag || upperFlag);
        }

        public override void Print( SectionLayer layer, Thing parent )
        {
            var occupiedRect = parent.OccupiedRect();
            foreach (var current in occupiedRect)
            {
                var center = current.ToVector3ShiftedWithAltitude( AltitudeLayer.WorldDataOverlay );
                Printer_Plane.PrintPlane( layer, center, new Vector2( 1f, 1f ), LinkedDrawMatFrom( parent, current ),
                                          0f );
            }
        }
    }
}
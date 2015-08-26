using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Graphic_LinkedAirPipe : Graphic_Linked
    {
        public Graphic_LinkedAirPipe( Graphic subGraphic )
            : base( subGraphic )
        {
        }

        public override bool ShouldLinkWith( IntVec3 c, Thing parent )
        {
            var compAir = parent.TryGetComp< CompAir >();
            if ( compAir == null )
                return false;

            var lowerFlag = AirNetGrid.NetAt( c, NetLayer.Lower ) != null && compAir.IsLayerOf( NetLayer.Lower );
            var upperFlag = AirNetGrid.NetAt( c, NetLayer.Upper ) != null && compAir.IsLayerOf( NetLayer.Upper );

            return c.InBounds() && (lowerFlag || upperFlag);
        }

        public override void Print( SectionLayer layer, Thing parent )
        {
            base.Print( layer, parent );
            var compAir = parent.TryGetComp< CompAir >();
            if ( compAir == null )
                return;
            
            for ( var i = 0; i < 4; i++ )
            {
                var neighCell = parent.Position + GenAdj.CardinalDirections[i];
                if ( !neighCell.InBounds() )
                {
                    continue;
                }

                Material mat;
                if ( compAir.IsLayerOf( NetLayer.Lower ) )
                {
                    var lowerTransmitter = neighCell.GetAirTransmitter( NetLayer.Lower );
                    if ( lowerTransmitter != null && !lowerTransmitter.def.graphicData.Linked )
                    {
                        mat = LinkedDrawMatFrom(parent, neighCell);
                        Printer_Plane.PrintPlane(layer, neighCell.ToVector3ShiftedWithAltitude(parent.def.Altitude),
                            Vector2.one, mat, 0f);
                    }
                }

                if ( compAir.IsLayerOf( NetLayer.Upper ) )
                {
                    var upperTransmitter = neighCell.GetAirTransmitter( NetLayer.Upper );
                    if ( upperTransmitter != null && !upperTransmitter.def.graphicData.Linked )
                    {
                        mat = LinkedDrawMatFrom(parent, neighCell);
                        Printer_Plane.PrintPlane(layer, neighCell.ToVector3ShiftedWithAltitude(parent.def.Altitude),
                            Vector2.one, mat, 0f);
                    }
                }
            }
        }
    }
}
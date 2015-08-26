using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public static class AirOverlayMat
    {
        private static readonly Shader TransmitterShader;
        public static readonly Graphic []LinkedOverlayGraphic;

        static AirOverlayMat()
        {
            LinkedOverlayGraphic = new Graphic[Common.NetLayerCount()];

            TransmitterShader = ShaderDatabase.MetaOverlay;

            var graphicLower = GraphicDatabase.Get< Graphic_Single >( "Things/Specia/AirTransmitterOverlayUpper", TransmitterShader );
            var graphicUpper = GraphicDatabase.Get< Graphic_Single >( "Things/Special/AirTransmitterOverlayLower", TransmitterShader );
            LinkedOverlayGraphic[(int) NetLayer.Lower] = new Grahpic_LinkedAirPipeOverlay( graphicLower );
            LinkedOverlayGraphic[(int) NetLayer.Upper] = new Grahpic_LinkedAirPipeOverlay( graphicUpper );
            graphicLower.MatSingle.renderQueue = 3800;
            graphicUpper.MatSingle.renderQueue = 4000;
        }

        public static IEnumerable<Graphic> GetLayeredOverlayGraphics( CompAir compAir )
        {
            var graphics = new List< Graphic >();
            if ( compAir.IsLayerOf( NetLayer.Lower ) )
                graphics.Add( LinkedOverlayGraphic[(int) NetLayer.Lower] );
            if ( compAir.IsLayerOf( NetLayer.Upper ) )
                graphics.Add( LinkedOverlayGraphic[(int) NetLayer.Upper] );

            return graphics;
        }
    }
}
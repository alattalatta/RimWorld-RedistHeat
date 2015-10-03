using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public static class AirOverlayMat
    {
        private static readonly Shader TransmitterShader;
        public static readonly Graphic[] LinkedOverlayGraphic;

        static AirOverlayMat()
        {
            LinkedOverlayGraphic = new Graphic[Common.NetLayerCount()];

            TransmitterShader = ShaderDatabase.MetaOverlay;

            var graphicLower = GraphicDatabase.Get< Graphic_Single >( "Things/Special/AirPipeOverlayLower",
                                                                      TransmitterShader );
            var graphicUpper = GraphicDatabase.Get< Graphic_Single >( "Things/Special/AirPipeOverlayUpper",
                                                                      TransmitterShader );
            LinkedOverlayGraphic[(int) NetLayer.Lower] = new Grahpic_LinkedAirPipeOverlay( graphicLower );
            LinkedOverlayGraphic[(int) NetLayer.Upper] = new Grahpic_LinkedAirPipeOverlay( graphicUpper );
            graphicLower.MatSingle.renderQueue = 3800;
            graphicUpper.MatSingle.renderQueue = 4000;
        }

        public static Graphic GetLayeredOverlayGraphic( CompAir compAir )
        {
            if (compAir.IsLayerOf( NetLayer.Lower ))
            {
                return LinkedOverlayGraphic[(int) NetLayer.Lower];
            }
            if (compAir.IsLayerOf( NetLayer.Upper ))
            {
                return LinkedOverlayGraphic[(int) NetLayer.Upper];
            }
            throw new ArgumentOutOfRangeException( nameof( compAir ), "LT-RH: compAir has no valid net layer!" );
        }
    }
}
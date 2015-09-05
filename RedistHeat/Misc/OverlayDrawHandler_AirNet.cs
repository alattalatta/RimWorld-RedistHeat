using UnityEngine;

namespace RedistHeat
{
    public static class OverlayDrawHandler_AirNet
    {
        private static int lastPowerGridDrawFrame;

        public static bool ShouldDrawAirNetOverlay => lastPowerGridDrawFrame + 1 >= Time.frameCount;
        
        public static void DrawAitNetOverlayThisFrame()
        {
            lastPowerGridDrawFrame = Time.frameCount;
        }
    }
}

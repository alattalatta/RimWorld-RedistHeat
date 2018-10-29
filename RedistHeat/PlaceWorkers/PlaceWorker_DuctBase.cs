using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class PlaceWorker_DuctBase : PlaceWorker
    {
        public override void DrawGhost( ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            OverlayDrawHandler_AirNet.DrawAitNetOverlayThisFrame();
        }
    }
}
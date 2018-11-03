﻿using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Designator_DeconstructReversed : Designator
    {
        public override int DraggableDimensions => 2;

        public Designator_DeconstructReversed()
        {
            defaultLabel = ResourceBank.DeconstructReversed;
            defaultDesc = ResourceBank.DeconstructReversedDesc;
            icon = ContentFinder< Texture2D >.Get( "UI/Designators/Deconstruct" );
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.Designate_Deconstruct;
            hotKey = KeyBindingDefOf.Designator_Deconstruct;
        }

        public override AcceptanceReport CanDesignateCell( IntVec3 c )
        {
            if (!c.InBounds(this.Map))
            {
                return false;
            }
            if (!DebugSettings.godMode && c.Fogged(this.Map))
            {
                return false;
            }
            return TopDeconstructibleInCell( c ) != null;
        }

        public override void DesignateSingleCell( IntVec3 loc )
        {
            DesignateThing( TopDeconstructibleInCell( loc ) );
        }

        private Thing TopDeconstructibleInCell( IntVec3 loc )
        {
            return
                (from t in Find.CurrentMap.thingGrid.ThingsAt( loc ) orderby t.def.altitudeLayer ascending select t).FirstOrDefault
                    ( current => CanDesignateThing( current ).Accepted );
        }

        public override void DesignateThing( Thing t )
        {
            if (t.Faction != Faction.OfPlayer)
            {
                t.SetFaction( Faction.OfPlayer );
            }
            var innerIfMinified = t.GetInnerIfMinified();
            if (DebugSettings.godMode || Mathf.Approximately( innerIfMinified.GetStatValue( StatDefOf.WorkToBuild ), 0 ) ||
                t.def.IsFrame)
            {
                t.Destroy( DestroyMode.Deconstruct );
            }
            else
            {
                Find.CurrentMap.designationManager.AddDesignation( new Designation( t, DesignationDefOf.Deconstruct ) );
            }
        }

        public override AcceptanceReport CanDesignateThing( Thing t )
        {
            var building = t.GetInnerIfMinified() as Building;
            if (building?.def.category != ThingCategory.Building)
            {
                return false;
            }
            if (!DebugSettings.godMode)
            {
                if (!building.def.building.IsDeconstructible)
                {
                    return false;
                }
                if (building.Faction != Faction.OfPlayer)
                {
                    if (building.Faction != null)
                    {
                        return false;
                    }
                    if (!building.ClaimableBy( Faction.OfPlayer ))
                    {
                        return false;
                    }
                }
            }
            if (Find.CurrentMap.designationManager.DesignationOn( t, DesignationDefOf.Deconstruct ) != null)
            {
                return false;
            }
            return Find.CurrentMap.designationManager.DesignationOn( t, DesignationDefOf.Uninstall ) == null;
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }
    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;

// ReSharper disable InvertIf

namespace RedistHeat
{
    public class BuildingDuctIntersection : Building
    {
        private readonly IntVec3[] vec = new IntVec3[4];

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            for ( var i = 0; i < 4; i++ )
            {
                vec[i] = Position + GenAdj.CardinalDirections[i];
            }
            Common.WipeExistingPipe( Position );
        }

        public override void DeSpawn()
        {
            base.DeSpawn();
            var cardinals = new CompAir[4];
            for ( var i = 0; i < 4; i++ )
            {
                cardinals[i] = AirNetGrid.AirNodeAt( vec[i] ) as CompAir;
            }
            //Split: North and South
            if ( cardinals[0] != null && cardinals[2] != null )
            {
                if ( cardinals[0].connectedNet == cardinals[2].connectedNet )
                {
                    SplitConnection( cardinals[0] );
                }
            }
            //Split: East and West
            if ( cardinals[1] != null && cardinals[3] != null )
            {
                if ( cardinals[1].connectedNet == cardinals[3].connectedNet )
                {
                    SplitConnection( cardinals[1] );
                }
            }
        }

        public override void TickRare()
        {
            base.TickRare();
            MakeConnection();
        }

        public override IEnumerable< Gizmo > GetGizmos()
        {
            foreach ( var g in base.GetGizmos() )
            {
                yield return g;
            }

            var l = new Command_Action
            {
                defaultLabel = StaticSet.StringUIRefreshIDLabel,
                defaultDesc = StaticSet.StringUIRefreshIDDesc,
                hotKey = KeyBindingDefOf.CommandTogglePower,
                icon = StaticSet.UIRefreshID,
                action = () => MakeConnection()
            };
            yield return l;
        }

        private void MakeConnection()
        {
            var cardinals = new CompAir[4];
            for ( var i = 0; i < 4; i++ )
            {
                cardinals[i] = AirNetGrid.AirNodeAt( vec[i] ) as CompAir;
            }
            //Start: North and South
            if ( cardinals[0] != null && cardinals[2] != null )
            {
                if ( cardinals[0].connectedNet != cardinals[2].connectedNet )
                {
                    MergeNet( cardinals[2], cardinals[0] );
                }
            }
            //Start: East and West
            if ( cardinals[1] != null && cardinals[3] != null )
            {
                if ( cardinals[1].connectedNet != cardinals[3].connectedNet )
                {
                    MergeNet( cardinals[3], cardinals[1] );
                }
            }
        }

        //When despawning, split
        private static void SplitConnection( CompAir compAir )
        {
            compAir.connectedNet.SplitNetAt( compAir );
            compAir.TryConnectTo();
            if ( compAir.connectedNet == null )
            {
                //Make a new net
                compAir.connectedNet = new AirNet( new List< CompAir >
                {
                    compAir
                } );
            }
            else
            {
                compAir.connectedNet.MergeIntoNet( new AirNet() );
            }
            AirNetGrid.Register( compAir );
        }

        private static void MergeNet( CompAir compAirSource, CompAir compAirTarget )
        {
            compAirSource.connectedNet.MergeIntoNet( compAirTarget.connectedNet );
        }
    }
}
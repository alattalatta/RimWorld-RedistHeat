using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class AirNet
    {
        public readonly int debugId;
        private static int debugIdNext;
        private readonly CompAir root;
        
        public readonly List< CompAir > nodes = new List< CompAir >();
        public int pushers;
        public int pullers;
        private int countdown;

        private float netTemperature;

        public float NetTemperature
        {
            get => netTemperature;
            set => netTemperature = Mathf.Clamp( value, -270, 2000 ); 
        }

        public NetLayer Layer { get; }
        public int LayerInt => (int) Layer;

        public AirNet( IEnumerable< CompAir > newNodes, NetLayer layer, CompAir root, Map map )
        {
            var intakeRoomTemp = -500f;
            
            netTemperature = -500f;
            Layer = layer;
            pushers = 0;
            pullers = 0;
            
            this.root = root;
            debugId = debugIdNext++;

            foreach (var current in newNodes)
            {
                RegisterNode( current );
                current.connectedNet = this;

                if (netTemperature >= Common.AbsoluteZero) continue;
                switch (current)
                {
                    case CompAirTrader airTrader:
                        if (airTrader.Props.units > 0)
                        {
                            AddPusherOrPuller(airTrader);
                            var room = airTrader.parent.GetRoom();
                            if (room != null)
                            {
                                intakeRoomTemp = room.Temperature;
                            }
                        }

                        if (airTrader.netTemp >= Common.AbsoluteZero)
                        {
                            netTemperature = airTrader.netTemp;
                        }
                        break;    
                }
            }

            if (netTemperature < Common.AbsoluteZero)
            {
                netTemperature = intakeRoomTemp < Common.AbsoluteZero
                    ? intakeRoomTemp
                    : map.mapTemperature.OutdoorTemp;
            }
        }

        private void AddPusherOrPuller(CompAirTrader airTrader)
        {
            if (airTrader.Props.units > 0)
            {
                pullers += airTrader.Props.units;
            } 
            else if (airTrader.Props.units < 0)
            {
                pushers += airTrader.Props.units;
            }
        }
        
        public void AirNetTick()
        {
            // TODO use hugs custom ticker instead
            // https://github.com/UnlimitedHugs/RimworldHugsLib/wiki/Custom-Tick-Scheduling
            if(countdown >= 60)
            {
                countdown = 0;
                pullers = 0;
                pushers = 0;
                
                foreach (var current in nodes.Select(s => s as CompAirTrader).Where(s => s != null))
                {
                    switch (current.parent)
                    {
                        case Building_DuctComp ductComp:
                            if (!ductComp.isLocked) AddPusherOrPuller(current);
                            break;
                        default:
                            AddPusherOrPuller(current);
                            break;
                    }
                }
#if DEBUG
                Log.Message("RedistHeat: the net has " + pullers + " pullers and " + pushers + "pushers");
#endif
            }
            countdown++;
        }

        public void RegisterNode( CompAir node )
        {
            if (nodes.Contains( node ))
            {
                Log.Error( "RedistHeat: AirNet adding node " + node + " which it already has." );
            }
            else
            {
                nodes.Add( node );
            }
        }

        public void DeregisterNode( CompAir node )
        {
            nodes.Remove( node );
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append( "AirNet " )
                  .Append( debugId )
                  .Append( " (nodes count: " )
                  .Append( nodes.Count )
                  .Append( ", layer " )
                  .Append( Layer )
                  .Append( ", root " )
                  .Append( root.parent.Position )
                  .Append( ", temp " )
                  .Append( netTemperature );
            return result.ToString();
        }
    }
}


using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RedistHeat
{
    public class NetSaver : IExposable
    {
        public ThingWithComps rootBuilding;
        public float netTemperature;
        public bool done = false;

        public void ExposeData()
        {
            Scribe_Values.LookValue( ref netTemperature, "netTemperature", -270 );
            Scribe_References.LookReference( ref rootBuilding, "rootBuilding" );
        }
    }

    //TODO rename to AirNetManager
    public class AirNetTicker : MapComponent
    {
        private bool doneInit;
        private bool doneLoad;

        private List< NetSaver > savers = new List< NetSaver >();

        public override void MapComponentUpdate()
        {
            if ( !doneInit )
            {
                Initialize();
            }
#if DEBUG
            //Log.Message( "Updating." );
#endif
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();

            if ( !doneLoad )
            {
                RestoreTemperature();
            }
        }

        public override void MapComponentTick()
        {
            if ( !doneInit )
            {
                Initialize();
            }

            AirNetManager.AirNetsTick();
        }

        public override void ExposeData()
        {
            foreach ( var channel in AirNetManager.allNets )
            {
                foreach ( var current in channel )
                {
                    savers.Add( new NetSaver()
                    {
                        netTemperature = current.NetTemperature,
                        rootBuilding = current.root.parent
                    } );
                }
            }

            Scribe_Collections.LookList( ref savers, "savers", LookMode.Deep );
        }

        private void Initialize()
        {
            var watch = new Stopwatch();
            watch.Start();

            AirNetManager.Reload();
            var elapsed = watch.ElapsedMilliseconds;

            if ( Prefs.LogVerbose )
            {
                Log.Message( "LT-RH: " + elapsed + "ms: Reloaded NetManager." );
            }

            doneInit = true;
            watch.Reset();
        }

        private void RestoreTemperature()
        {
            var watch = new Stopwatch();
            watch.Start();

            foreach ( var channel in AirNetManager.allNets )
            {
                foreach ( var current in channel )
                {
#if DEBUG
                    Log.Message( "LT-RH: LOOK: Looking for save with root " + current.root.parent );
#endif
                    foreach ( var save in savers )
                    {
#if DEBUG
                        Log.Message( "LT-RH: SAVE: Looking at save with root " + save.rootBuilding + ", temperature " +
                                     save.netTemperature );
#endif
                        if ( save.done )
                        {
                            continue;
                        }

                        if ( current.nodes.Exists( s => s.parent.GetHashCode() == save.rootBuilding.GetHashCode() ) )
                        {
#if DEBUG
                            Log.Message( "Found matching one. Restoring." );
#endif
                            current.NetTemperature = save.netTemperature;
                            save.done = true;
                            break;
                        }
                    }
                }
            }

            var elapsed = watch.ElapsedMilliseconds;
            if ( Prefs.LogVerbose )
            {
                Log.Message("LT-RH: " + elapsed + "ms: Restored saved data.");
            }

            doneLoad = true;
            watch.Reset();
        }
    }
}

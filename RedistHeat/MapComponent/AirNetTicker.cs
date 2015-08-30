

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RedistHeat
{
    public class NetSaver : IExposable
    {
        public ThingWithComps rootBuilding;
        public float netTemperature;
        public bool done;


        public void                         ExposeData()
        {
            Scribe_Values.LookValue( ref netTemperature, "netTemperature", -270 );
            Scribe_References.LookReference( ref rootBuilding, "rootBuilding" );
        }
    }
    
    public class AirNetTicker : MapComponent
    {
        private bool doneInit;
        private bool doneLoad;

        private List< NetSaver > savers = new List< NetSaver >();


        public override void                MapComponentUpdate()
        {
            if ( !doneInit )
            {
                Initialize();
            }
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();

            if ( doneInit && !doneLoad )
            {
                RestoreTemperature();
            }
        }


        public override void                MapComponentTick()
        {
            if ( !doneInit )
            {
                Initialize();
            }

            AirNetManager.AirNetsTick();
        }


        public override void                ExposeData()
        {
            if ( Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving )
                return;
            
            savers.Clear();

            foreach ( var current in AirNetManager.allNets.SelectMany( channel => channel ) )
            {
                savers.Add( new NetSaver()
                {
                    netTemperature = current.NetTemperature,
                    rootBuilding = current.root.parent
                } );
            }

            Scribe_Collections.LookList( ref savers, "savers", LookMode.Deep );
        }


        private void                        Initialize()
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


        private void                        RestoreTemperature()
        {
            if ( savers == null )
            {
#if DEBUG
                Log.Message( "LT-RH: save list is null. ");
#endif
                doneLoad = true;
                return;
            }

            var watch = new Stopwatch();
            watch.Start();


            for ( var i = 0; i < Common.NetLayerCount(); i++ )
            {
                var channel = AirNetManager.allNets[i];

                foreach ( var current in channel )
                {
                    foreach ( var save in savers.Where( s => !s.done ) )
                    {
                        try
                        {
                            if ( !current.nodes.Exists( s => s.parent.GetHashCode() == save.rootBuilding.GetHashCode() ) )
                            {
                                continue;
                            }

                            current.NetTemperature = save.netTemperature;
                            save.done = true;
                            break;
                        }
                        catch ( Exception e )
                        {
                            Log.ErrorOnce( "LT-RH: Exception occured while loading.\n" + e, 13395831 );
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

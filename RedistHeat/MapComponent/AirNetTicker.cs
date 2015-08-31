using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    /*
    public class NetSaver : IExposable
    {
        public ThingWithComps rootBuilding;
        public float netTemperature;
        public bool done;


        public void ExposeData()
        {
            // TODO Change save label to netTemp and root
            Scribe_Values.LookValue(ref netTemperature, "netTemperature", -270);
            Scribe_References.LookReference(ref rootBuilding, "rootBuilding");
        }
    }*/

    public class AirNetTicker : MapComponent
    {
        private bool doneInit;
        //private bool doneLoad;

        //private List<NetSaver> savers = new List<NetSaver>();
        
        public override void MapComponentUpdate()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();
            
            /*
            if (!doneLoad)
            {
                RestoreTemperature();
            }*/
        }


        public override void MapComponentTick()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsTick();
        }


        /*
    public override void ExposeData()
    {
        if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
            return;

        Log.Warning("Exposing.");
        savers.Clear();

        foreach (var current in AirNetManager.allNets.SelectMany(s => s))
        {
            savers.Add(new NetSaver()
            {
                netTemperature = current.NetTemperature,
                rootBuilding = current.root.parent
            });
        }

        //TODO Change save label to saves
        Scribe_Collections.LookList(ref savers, "savers", LookMode.Deep);
        
    }
    */

        private void Initialize()
        {
            AirNetManager.Reload();
            Log.Message( "LT-RH: Initialized RedistHeat." );
            doneInit = true;
        }

        /*
        private void RestoreTemperature()
        {
            Log.Warning("Restoring.");
            if ( savers == null )
            {
#if DEBUG
                Log.Message("LT-RH: Save list is null!");
#endif
                doneLoad = true;
                return;
            }
            
            foreach ( var current in AirNetManager.allNets.SelectMany( s => s ) )
            {
                foreach ( var save in savers.Where( s => !s.done ) )
                {
                    try
                    {
                        if (!current.nodes.Exists( s => s.parent.GetHashCode() == save.rootBuilding.GetHashCode() ) )
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
#if DEBUG
            Log.Message( "LT-RH: Restored saved data." );
#endif
            doneLoad = true;
        }
        */
    }
}

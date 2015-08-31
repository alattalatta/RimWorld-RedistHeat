using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace RedistHeat
{
    public class NetSaver : IExposable
    {
        public ThingWithComps root;
        public float netTemp;
        public bool done;


        public void ExposeData()
        {
            // TODO Change save label to netTemp and root
            Scribe_Values.LookValue(ref netTemp, "netTemperature", -270);
            Scribe_References.LookReference(ref root, "rootBuilding");
        }
    }

    public class AirNetTicker : MapComponent
    {
        private bool doneInit;
        private bool doneLoad;

        private List<NetSaver> saves = new List<NetSaver>();
        
        public override void MapComponentUpdate()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsUpdate();
            AirNetManager.UpdateMapDrawer();

            if (!doneLoad)
            {
                RestoreTemperature();
            }
        }


        public override void MapComponentTick()
        {
            if (!doneInit)
            {
                Initialize();
            }
            AirNetManager.AirNetsTick();
        }


        public override void ExposeData()
        {
            if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.Saving)
                return;

            saves.Clear();

            foreach (var current in AirNetManager.allNets.SelectMany(s => s))
            {
                saves.Add(new NetSaver()
                {
                    netTemp = current.NetTemperature,
                    root = current.root.parent
                });
            }

            //TODO Change save label to saves
            Scribe_Collections.LookList(ref saves, "savers", LookMode.Deep);
        }


        private void Initialize()
        {
            AirNetManager.Reload();
            Log.Message( "LT-RH: Initialized RedistHeat." );
            doneInit = true;
        }


        private void RestoreTemperature()
        {
            if ( saves == null )
            {
                Log.Warning( "LT-RH: Save list is null!" );
                doneLoad = true;
                return;
            }
            
            foreach ( var current in AirNetManager.allNets.SelectMany( s => s ) )
            {
                foreach ( var save in saves.Where( s => !s.done ) )
                {
                    try
                    {
                        if (!current.nodes.Exists( s => s.parent.GetHashCode() == save.root.GetHashCode() ) )
                        {
                            continue;
                        }

                        current.NetTemperature = save.netTemp;
                        save.done = true;
                        break;
                    }
                    catch ( Exception e )
                    {
                        Log.ErrorOnce( "LT-RH: Exception occured while loading.\n" + e, 13395831 );
                    }
                }
            }

            doneLoad = true;
        }
    }
}

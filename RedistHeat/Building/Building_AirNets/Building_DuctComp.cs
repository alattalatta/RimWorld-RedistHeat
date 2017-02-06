using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

namespace RedistHeat
{
    public class Building_DuctComp : Building_DuctBase, IWallAttachable
    {
        protected const float EqualizationRate = 0.204f; // RareTick @ 0.85f;

        protected CompAirTrader compAir;
        protected Room room;
        protected virtual IntVec3 RoomVec => Position + IntVec3.North.RotatedBy( Rotation );

        public bool isLocked;
        public bool shouldChange = true;
        private bool isWorking;
        private int ticksElapsed = 0;
        private List<Room> adjacentRooms = new List<Room>();
        private static BoolGrid fieldGrid;

        protected bool WorkingState
        {
            get { return isWorking; }
            set
            {
                isWorking = value;

                if (compPowerTrader == null || compTempControl == null)
                {
                    return;
                }
                if (isWorking)
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
                }
                else
                {
                    compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption*
                                                  compTempControl.Props.lowPowerConsumptionFactor;
                }

                compTempControl.operatingAtHighPower = isWorking;
            }
        }


        public override string LabelNoCount => base.LabelNoCount + " (" + compAir.currentLayer.ToString().ToLower() + ")";

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            compAir = GetComp< CompAirTrader >();

            Common.WipeExistingPipe( Position );
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue( ref isLocked, "isLocked", false );
        }

        public override void Tick()
        {
            base.Tick();
            ticksElapsed++;
            if (!this.IsHashIntervalTick( 60 ))
            {
                return;
            }

            if (!Validate())
            {
                WorkingState = false;
                return;
            }

            if(ticksElapsed >= 1000)
            {
                ticksElapsed = 0;
                FindAdjacentRooms();
            }

            WorkingState = true;
            Equalize();
        }

        /// <summary>
        /// Base validater. Checks if vecNorth is passable, room is there, is building locked, and power is on.
        /// </summary>
        protected virtual bool Validate()
        {
            if (compAir.connectedNet == null)
            {
                return false;
            }

            if (RoomVec.Impassable(this.Map))
            {
                return false;
            }

            room = RoomVec.GetRoom(this.Map);

            if (room == null)
            {
                return false;
            }

            if(isLocked)
            {
                if(shouldChange)
                {
                    shouldChange = !shouldChange;
                    int units = compAir.Props.units;
                    if (units > 0)
                    {
                        compAir.connectedNet.pushers -= units;
                    }
                    else
                    {
                        compAir.connectedNet.pullers -= -units;
                    }
                }
                return false;
            }

            return (compPowerTrader == null || compPowerTrader.PowerOn);
        }

        protected virtual void Equalize()
        {
            float pushers = compAir.connectedNet.pushers;
            float pullers = compAir.connectedNet.pullers;
            int units = compAir.Props.units;
            float force = 0f;

            if (units > 0)
            {
                float temp = room.Temperature;
                float count = compAir.connectedNet.nodes.Count;

                if (pushers > pullers)
                {
                    force = (pullers / pushers) * units;
#if DEBUG
                    Log.Message("RedistHeat: Intake force: " +force+" with pullers: "+pullers+" and pushers: "+pushers);
#endif
                }
                else
                {
                    force = units;
                }

                if(force >= count)
                {
                    compAir.connectedNet.NetTemperature = temp;
                }
                else
                {
                    float diff = count - force;
                    float result = ((compAir.connectedNet.NetTemperature * diff) + (temp * force)) / count;
#if DEBUG
                                Log.Message("RedistHeat: Intake net result temp: " +result+" with diff: "+diff+" force: "+force+" temp: "+temp+" count: "+count);
#endif
                    compAir.connectedNet.NetTemperature = result;
                }
                if(!room.UsesOutdoorTemperature)
                {
                    float avgTemp = 0;
                    int cells = 0;
                    if (adjacentRooms.Count > 0)
                    {
                        foreach (var current in adjacentRooms)
                        {
                            avgTemp += current.Temperature;
                            cells += current.CellCount;
                        }
                        if (cells >= force)
                        {
                            avgTemp /= adjacentRooms.Count;
                        }
                        else
                        {
                            avgTemp += this.Map.mapTemperature.OutdoorTemp;
                            avgTemp /= (adjacentRooms.Count+1);
                        }
                    }
                    else
                    {
                        avgTemp = room.Temperature;
                    }
                    if (force >= room.CellCount)
                    {
                        room.Temperature = avgTemp;
                    }
                    else
                    {
                        float diff = room.CellCount - force;
                        float result = ((diff * room.Temperature) + (avgTemp * force)) / room.CellCount;
                        room.Temperature = result;
                    }
#if DEBUG
                    Log.Message("RedistHeat: Intake room result temp: " + result);
#endif
                }
            }
            else
            {
                float temp = compAir.connectedNet.NetTemperature;
                float count = room.CellCount;

                if (pushers < pullers)
                {
                    force = (pushers  / pullers) * -units;
                }
                else
                {
                    force = -units;
                }
                if (force >= count)
                {
                    room.Temperature = temp;
                }
                else
                {
#if DEBUG
                    Log.Message("RedistHeat: Outlet force: " + force + " with pullers: " + pullers + " and pushers: " + pushers);
#endif
                    float diff = count - force;
                    float result = ((diff * room.Temperature) + (temp * force)) / count;
#if DEBUG
                    Log.Message("RedistHeat: Outlet result temp: " + result + " with diff: " + diff + " force: " + force + " temp: " + temp + " count: " + count);
#endif
                    room.Temperature = result;
                }
            }
            /*float pointTemp;
            if (room.UsesOutdoorTemperature)
            {
                pointTemp = room.Temperature;
            }
            else
            {
                pointTemp = (room.Temperature*room.CellCount +
                             compAir.connectedNet.NetTemperature*compAir.connectedNet.nodes.Count*5)
                            /(room.CellCount + compAir.connectedNet.nodes.Count*5);
            }

            if (compTempControl != null)
            {
                // Trying to remove temperature spiking
                if (compTempControl.targetTemperature < room.Temperature)
                {
                    pointTemp = Mathf.Max( pointTemp, compTempControl.targetTemperature ) - 1;
                }
                else
                {
                    pointTemp = Mathf.Min( pointTemp, compTempControl.targetTemperature ) + 1;
                }
            }
//#if DEBUG
//            Log.Message("RedistHeat: DuctComp ----- Device: " + this + ", pointTemp: " + pointTemp);
//#endif
            compAir.EqualizeWithNet( pointTemp, EqualizationRate );
            if (!room.UsesOutdoorTemperature)
            {
                compAir.EqualizeWithRoom( room, pointTemp, EqualizationRate );
            }
                
             */
        }

        private void FindAdjacentRooms()
        {
            adjacentRooms.Clear();
            if(room.UsesOutdoorTemperature)
            {
                return;
            }
            Map visibleMap = Find.VisibleMap;
            List<IntVec3> cells = new List<IntVec3>();

            foreach (IntVec3 current in room.Cells)
            {
                cells.Add(current);
            }

            if (fieldGrid == null)
            {
                fieldGrid = new BoolGrid(visibleMap);
            }
            else
            {
                fieldGrid.ClearAndResizeTo(visibleMap);
            }

            int x = visibleMap.Size.x;
            int z = visibleMap.Size.z;
            int count = cells.Count;
            for (int i = 0; i < count; i++)
            {
                if (cells[i].InBounds(visibleMap))
                {
                    fieldGrid[cells[i].x, cells[i].z] = true;
                }
            }
            for (int j = 0; j < count; j++)
            {
                IntVec3 c = cells[j];
                if (c.InBounds(visibleMap))
                {
                    if(c.z < z - 1 && !fieldGrid[c.x, c.z + 1])
                    {
                        var door = Find.VisibleMap.thingGrid.ThingsAt(new IntVec3(c.x,c.y,c.z+1)).ToList().Find(s => s.def.defName == "Door" );
                        if(door != null)
                        {
                            adjacentRooms.Add(RoomQuery.RoomAt(new IntVec3(c.x, c.y, c.z + 2), Map));
                        }
                    }
                    if(c.x < x - 1 && !fieldGrid[c.x + 1, c.z])
                    {
                        var door = Find.VisibleMap.thingGrid.ThingsAt(new IntVec3(c.x + 1, c.y, c.z)).ToList().Find(s => s.def.defName == "Door");
                        if (door != null)
                        {
                            adjacentRooms.Add(RoomQuery.RoomAt(new IntVec3(c.x + 2, c.y, c.z), Map));
                        }
                    }
                    if (c.z > 0 && !fieldGrid[c.x, c.z - 1])
                    {
                        var door = Find.VisibleMap.thingGrid.ThingsAt(new IntVec3(c.x, c.y, c.z - 1)).ToList().Find(s => s.def.defName == "Door");
                        if (door != null)
                        {
                            adjacentRooms.Add(RoomQuery.RoomAt(new IntVec3(c.x, c.y, c.z - 2), Map));
                        }
                    }
                    if (c.z > 0 && !fieldGrid[c.x - 1, c.z])
                    {
                        var door = Find.VisibleMap.thingGrid.ThingsAt(new IntVec3(c.x - 1, c.y, c.z)).ToList().Find(s => s.def.defName == "Door");
                        if (door != null)
                        {
                            adjacentRooms.Add(RoomQuery.RoomAt(new IntVec3(c.x - 2, c.y, c.z), Map));
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (isLocked)
            {
                this.Map.overlayDrawer.DrawOverlay( this, OverlayTypes.ForbiddenBig );
            }
        }

        public override IEnumerable< Gizmo > GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            var l = new Command_Toggle
            {
                defaultLabel = ResourceBank.StringToggleAirflowLabel,
                defaultDesc = ResourceBank.StringToggleAirflowDesc,
                hotKey = KeyBindingDefOf.CommandItemForbid,
                icon = ResourceBank.UILock,
                groupKey = 912515,
                isActive = () => !isLocked,
                toggleAction = () =>
                {
                    isLocked = !isLocked;
                    shouldChange = false;
                    int units = compAir.Props.units;
                    if(isLocked)
                    {
                        if (units > 0)
                        {
                            compAir.connectedNet.pushers -= units;
                        }
                        else
                        {
                            compAir.connectedNet.pullers -= -units;
                        }
                    }
                    else
                    {
                        if (units > 0)
                        {
                            compAir.connectedNet.pushers += units;
                        }
                        else
                        {
                            compAir.connectedNet.pullers += -units;
                        }
                    }
                }
            };
            yield return l;
        }
    }
}
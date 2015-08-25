using Verse;

namespace RedistHeat
{
    public class CompAirBase : ThingComp
    {
        public IntVec3 Position { get; private set; }

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();
            Position = parent.Position;
            AirNetGrid.Register( this );
        }

        public override void PostDeSpawn()
        {
            base.PostDeSpawn();
            AirNetGrid.Deregister( this );
        }
    }
}
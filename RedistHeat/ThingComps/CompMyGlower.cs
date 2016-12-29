using Verse;

namespace RedistHeat
{
    class CompMyGlower : CompGlower
    {
        private bool glowOnInt;
        
        public void UpdateLit(bool lit)
        {
            bool shouldBeLitNow = lit;
            if (this.glowOnInt == shouldBeLitNow)
            {
                return;
            }
            this.glowOnInt = shouldBeLitNow;
            if (!this.glowOnInt)
            {
                Find.VisibleMap.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                Find.VisibleMap.glowGrid.DeRegisterGlower(this);
            }
            else
            {
                Find.VisibleMap.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                Find.VisibleMap.glowGrid.RegisterGlower(this);
            }
        }

        public override void PostSpawnSetup()
        {
            this.UpdateLit(false);
        }

        public override void PostExposeData()
        {
            Scribe_Values.LookValue<bool>(ref this.glowOnInt, "glowOn", false, false);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            this.UpdateLit(false);
        }
    }
}

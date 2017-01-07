using Verse;

namespace RedistHeat
{
    class CompMyGlower : CompGlower
    {
        private bool glowOnInt;
        
        public void UpdateLit(bool lit, Map map)
        {
            bool shouldBeLitNow = lit;
            if (this.glowOnInt == shouldBeLitNow)
            {
                return;
            }
            this.glowOnInt = shouldBeLitNow;
            if (!this.glowOnInt)
            {
                map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                map.glowGrid.DeRegisterGlower(this);
            }
            else
            {
                map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                map.glowGrid.RegisterGlower(this);
            }
        }

        public override void PostSpawnSetup()
        {
            this.UpdateLit(false, this.parent.Map);
        }

        public override void PostExposeData()
        {
            Scribe_Values.LookValue<bool>(ref this.glowOnInt, "glowOn", false, false);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            this.UpdateLit(false,map);
        }
    }
}

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
                Find.MapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                Find.GlowGrid.DeRegisterGlower(this);
            }
            else
            {
                Find.MapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                Find.GlowGrid.RegisterGlower(this);
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

        public override void PostDeSpawn()
        {
            base.PostDeSpawn();
            this.UpdateLit(false);
        }
    }
}

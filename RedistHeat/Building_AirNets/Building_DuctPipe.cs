using Verse;

namespace RedistHeat
{
    public class Building_DuctPipe : Building
    {
        private Graphic_LinkedAirPipe graphicLinked;

        public override Graphic Graphic
        {
            get
            {
                if ( graphicLinked?.MatSingle != null )
                {
                    return graphicLinked;
                }

                GetGraphic();
                if ( graphicLinked?.MatSingle != null )
                {
                    return graphicLinked;
                }

                return def.graphic;
            }
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            GetGraphic();
        }

        private void GetGraphic()
        {
            if ( graphicLinked?.MatSingle != null )
            {
                return;
            }

            var graphicSingle = GraphicDatabase.Get< Graphic_Single >( def.graphicData.texPath );
            graphicLinked = new Graphic_LinkedAirPipe( graphicSingle );
        }
    }
}
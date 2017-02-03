using Verse;

namespace RedistHeat
{
    public class Building_DuctPipe : Building_DuctBase
    {
        private Graphic_LinkedAirPipe graphicLinked;

        public override Graphic Graphic
        {
            get
            {
                if (graphicLinked?.MatSingle != null)
                {
                    return graphicLinked;
                }

                GetGraphic();
                if (graphicLinked?.MatSingle != null)
                {
                    return graphicLinked;
                }

                return def.graphic;
            }
        }

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            GetGraphic();
        }

        private void GetGraphic()
        {
            if (graphicLinked?.MatSingle != null)
            {
                return;
            }
                       
            //Things/Building/Linked/DuctPipeLower
            //var graphicSingle = GraphicDatabase.Get< Graphic_Single >( def.graphicData.texPath );
            //graphicLinked = new Graphic_LinkedAirPipe( graphicSingle );
            if (def.graphicData.texPath == "Things/Building/Linked/DuctPipeUpper")
            {
                graphicLinked = new Graphic_LinkedAirPipe( ResourceBank.graphicSingleUpper);
            }
            if (def.graphicData.texPath == "Things/Building/Linked/DuctPipeLower")
            {
                graphicLinked = new Graphic_LinkedAirPipe( ResourceBank.graphicSingleLower);
            }
        }
    }
}
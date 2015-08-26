using Verse;

namespace RedistHeat
{
    public class Building_DuctPipe : Building
    {
        private NetLayer layer;
        private CompAirTransmitter compAir;
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
            compAir = GetComp< CompAirTransmitter >();
            var compProps = compAir.props as CompAirTransmitterProperties;
            if ( compProps == null )
            {
                layer = NetLayer.Lower;
                Log.Error( "LT-RH: compProps is null!" );
            }
            else
            {
                layer = compProps.layer;
            }
        }

        /*
        public override IEnumerable< Gizmo > GetGizmos()
        {
            foreach ( var g in base.GetGizmos() )
            {
                yield return g;
            }

            var l = new Command_Action
            {
                defaultLabel = ResourceBank.StringUIRefreshIDLabel,
                defaultDesc = ResourceBank.StringUIRefreshIDDesc,
                hotKey = KeyBindingDefOf.CommandTogglePower,
                icon = ResourceBank.UIRefreshID,
                action = () => compAir.TryConnectTo()
            };
            yield return l;
        }*/

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
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class Graphic_MultiDXT5 : Graphic
    {
        private Material[] mats = new Material[3];

        public override Material MatSingle => mats[2];

        public override Material MatFront => mats[2];

        public override Material MatSide => mats[1];

        public override Material MatBack => mats[0];

        public override bool ShouldDrawRotated => MatSide == MatBack;

        public override void Init( GraphicRequest req )
        {
            path = req.path;
            color = req.color;
            colorTwo = req.colorTwo;
            drawSize = req.drawSize;
            var array = new Texture2D[3];
            array[0] = DXTLoader.LoadTextureDXT( ResourceBank.modName, req.path + "_back", TextureFormat.DXT5 );
            if (array[0] == null)
            {
                Log.Error( "Failed to find any texture while constructing " + ToString() );
                return;
            }
            array[1] = DXTLoader.LoadTextureDXT( ResourceBank.modName, req.path + "_side", TextureFormat.DXT5 );
            if (array[1] == null)
            {
                array[1] = array[0];
            }
            array[2] = DXTLoader.LoadTextureDXT( ResourceBank.modName, req.path + "_front", TextureFormat.DXT5 );
            if (array[2] == null)
            {
                array[2] = array[0];
            }
            /*
			var array2 = new Texture2D[3];
			if (req.shader.SupportsMaskTex())
			{
				array2[0] = DXTLoader.LoadTextureDXT(req.path + "_backm");
				if (array2[0] != null)
				{
					array2[1] = DXTLoader.LoadTextureDXT(req.path + "_sidem");
					if (array2[1] == null)
					{
						array2[1] = array2[0];
					}
					array2[2] = DXTLoader.LoadTextureDXT(req.path + "_frontm");
					if (array2[2] == null)
					{
						array2[2] = array2[0];
					}
				}
			}*/
            for (var i = 0; i < 3; i++)
            {
                var req2 = default(MaterialRequest);
                req2.mainTex = array[i];
                req2.shader = req.shader;
                req2.color = color;
                req2.colorTwo = colorTwo;
                mats[i] = MaterialPool.MatFrom( req2 );
            }
        }

        public override Graphic GetColoredVersion( Shader newShader, Color newColor, Color newColorTwo )
        {
            var graphic = GraphicDatabase.Get< Graphic_MultiDXT5 >( path, newShader, drawSize, newColor, newColorTwo );
            graphic.data = data;
            return graphic;
        }

        public override string ToString()
        {
            return string.Concat( "Multi(initPath=", path, ", color=", color, ", colorTwo=", colorTwo, ")" );
        }

        public override int GetHashCode()
        {
            var num = path.GetHashCode()*7553;
            num ^= color.GetHashCode()*921;
            return num ^ colorTwo.GetHashCode()*511;
        }
    }
}
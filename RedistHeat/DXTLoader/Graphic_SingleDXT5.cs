using System;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public class Graphic_SingleDXT5 : Graphic
	{
		protected Material mat;

		public override Material MatSingle => mat;

		public override Material MatFront => mat;

		public override Material MatSide => mat;

		public override Material MatBack => mat;

		public override bool ShouldDrawRotated => data == null || data.drawRotated;

		public override void Init(GraphicRequest req)
		{
			path = req.path;
			color = req.color;
			colorTwo = req.colorTwo;
			drawSize = req.drawSize;
			var req2 = default(MaterialRequest);
			req2.mainTex = DXTLoader.LoadTextureDXT( ResourceBank.modName, req.path, TextureFormat.DXT5 );
			req2.shader = req.shader;
			req2.color = color;
			req2.colorTwo = colorTwo;
			/*
			if (req.shader.SupportsMaskTex())
			{
				req2.maskTex = DXTLoader.LoadTextureDXT(req.path + "_m", TextureFormat.DXT5);
			}*/
			mat = MaterialPool.MatFrom(req2);
		}

		public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
		{
			var graphic = GraphicDatabase.Get<Graphic_SingleDXT5>(path, newShader, drawSize, newColor, newColorTwo);
			graphic.data = data;
			return graphic;
		}

		public override Material MatAt(Rot4 rot, Thing thing = null)
		{
			return mat;
		}

		public override string ToString()
		{
			return string.Concat( "Single(path=", path, ", color=", color, ", colorTwo=", colorTwo, ")" );
		}
	}
}

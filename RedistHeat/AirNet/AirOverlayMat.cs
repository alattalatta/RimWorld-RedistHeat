using UnityEngine;
using Verse;
namespace RedistHeat
{
	public static class AirOverlayMat
	{
		private static readonly Shader TransmitterShader;
		public static readonly Graphic LinkedOverlayGraphic;
		static AirOverlayMat()
		{
			AirOverlayMat.TransmitterShader = ShaderDatabase.MetaOverlay;
			var graphic = GraphicDatabase.Get<Graphic_Single>("Things/Special/AirTransmitterOverlay", AirOverlayMat.TransmitterShader);
			AirOverlayMat.LinkedOverlayGraphic = GraphicDatabase.GetLinked(LinkDrawerType.Basic, graphic);
			graphic.MatSingle.renderQueue = 3800;
		}
	}
}
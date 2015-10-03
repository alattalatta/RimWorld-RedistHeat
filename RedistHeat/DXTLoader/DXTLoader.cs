using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedistHeat
{
	public static class DXTLoader
	{
		private const int DDSHeaderSize = 128;

		public static Texture2D LoadTextureDXT( string modName, string path, TextureFormat format, bool mipmap = true )
		{
			var a = Path.Combine( GenFilePaths.CoreModsFolderPath, LoadedModManager.LoadedMods.ToList().Find( s => s.name == modName ).name );
			var b = Path.Combine( a, "Textures" );
			var filePath = Path.Combine( b,  path + ".dds");
			var bytes = File.ReadAllBytes( filePath );
			
			if (format != TextureFormat.DXT1 && format != TextureFormat.DXT5)
				throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");

			var ddsSizeCheck = bytes[4];
			if (ddsSizeCheck != 124)
				throw new Exception("Invalid DDS DXT texture. Unable to read");  //this header byte should be 124 for DDS image files

			var height = bytes[13] * 256 + bytes[12];
			var width = bytes[17] * 256 + bytes[16];

			var dxtBytes = new byte[bytes.Length - DDSHeaderSize];
			Buffer.BlockCopy(bytes, DDSHeaderSize, dxtBytes, 0, bytes.Length - DDSHeaderSize);

			var texture = new Texture2D(width, height, format, mipmap);
			texture.LoadRawTextureData(dxtBytes);
			texture.Apply();

			return (texture);
		}
	}
}

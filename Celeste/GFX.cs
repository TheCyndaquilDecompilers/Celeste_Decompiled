using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200035B RID: 859
	public static class GFX
	{
		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06001B0A RID: 6922 RVA: 0x000B025B File Offset: 0x000AE45B
		// (set) Token: 0x06001B0B RID: 6923 RVA: 0x000B0262 File Offset: 0x000AE462
		public static bool Loaded { get; private set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06001B0C RID: 6924 RVA: 0x000B026A File Offset: 0x000AE46A
		// (set) Token: 0x06001B0D RID: 6925 RVA: 0x000B0271 File Offset: 0x000AE471
		public static bool DataLoaded { get; private set; }

		// Token: 0x06001B0E RID: 6926 RVA: 0x000B027C File Offset: 0x000AE47C
		public static void Load()
		{
			if (!GFX.Loaded)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				GFX.Game = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Gameplay"), Atlas.AtlasDataFormat.Packer);
				GFX.Opening = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Opening"), Atlas.AtlasDataFormat.PackerNoAtlas);
				GFX.Gui = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Gui"), Atlas.AtlasDataFormat.Packer);
				GFX.GuiSpriteBank = new SpriteBank(GFX.Gui, Path.Combine("Graphics", "SpritesGui.xml"));
				GFX.Misc = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Misc"), Atlas.AtlasDataFormat.PackerNoAtlas);
				GFX.Portraits = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Portraits"), Atlas.AtlasDataFormat.PackerNoAtlas);
				Draw.Particle = GFX.Game["util/particle"];
				Draw.Pixel = new MTexture(GFX.Game["util/pixel"], 1, 1, 1, 1);
				ParticleTypes.Load();
				GFX.ColorGrades = Atlas.FromDirectory(Path.Combine("Graphics", "ColorGrading"));
				GFX.MagicGlowNoise = VirtualContent.CreateTexture("glow-noise", 128, 128, Color.White);
				Color[] array = new Color[GFX.MagicGlowNoise.Width * GFX.MagicGlowNoise.Height];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Color(Calc.Random.NextFloat(), Calc.Random.NextFloat(), Calc.Random.NextFloat(), 0f);
				}
				GFX.MagicGlowNoise.Texture.SetData<Color>(array);
				Console.WriteLine(" - GFX LOAD: " + stopwatch.ElapsedMilliseconds + "ms");
			}
			GFX.Loaded = true;
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x000B044C File Offset: 0x000AE64C
		public static void LoadData()
		{
			if (!GFX.DataLoaded)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				GFX.PortraitsSpriteBank = new SpriteBank(GFX.Portraits, Path.Combine("Graphics", "Portraits.xml"));
				GFX.SpriteBank = new SpriteBank(GFX.Game, Path.Combine("Graphics", "Sprites.xml"));
				GFX.BGAutotiler = new Autotiler(Path.Combine("Graphics", "BackgroundTiles.xml"));
				GFX.FGAutotiler = new Autotiler(Path.Combine("Graphics", "ForegroundTiles.xml"));
				GFX.SceneryTiles = new Tileset(GFX.Game["tilesets/scenery"], 8, 8);
				PlayerSprite.ClearFramesMetadata();
				PlayerSprite.CreateFramesMetadata("player");
				PlayerSprite.CreateFramesMetadata("player_no_backpack");
				PlayerSprite.CreateFramesMetadata("badeline");
				PlayerSprite.CreateFramesMetadata("player_badeline");
				PlayerSprite.CreateFramesMetadata("player_playback");
				GFX.CompleteScreensXml = Calc.LoadContentXML(Path.Combine("Graphics", "CompleteScreens.xml"));
				GFX.AnimatedTilesBank = new AnimatedTilesBank();
				foreach (object obj in Calc.LoadContentXML(Path.Combine("Graphics", "AnimatedTiles.xml"))["Data"])
				{
					XmlElement xmlElement = (XmlElement)obj;
					if (xmlElement != null)
					{
						GFX.AnimatedTilesBank.Add(xmlElement.Attr("name"), xmlElement.AttrFloat("delay", 0f), xmlElement.AttrVector2("posX", "posY", Vector2.Zero), xmlElement.AttrVector2("origX", "origY", Vector2.Zero), GFX.Game.GetAtlasSubtextures(xmlElement.Attr("path")));
					}
				}
				Console.WriteLine(" - GFX DATA LOAD: " + stopwatch.ElapsedMilliseconds + "ms");
			}
			GFX.DataLoaded = true;
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x000B0638 File Offset: 0x000AE838
		public static void Unload()
		{
			if (GFX.Loaded)
			{
				GFX.Game.Dispose();
				GFX.Game = null;
				GFX.Gui.Dispose();
				GFX.Gui = null;
				GFX.Opening.Dispose();
				GFX.Opening = null;
				GFX.Misc.Dispose();
				GFX.Misc = null;
				GFX.ColorGrades.Dispose();
				GFX.ColorGrades = null;
				GFX.MagicGlowNoise.Dispose();
				GFX.MagicGlowNoise = null;
				GFX.Portraits.Dispose();
				GFX.Portraits = null;
			}
			GFX.Loaded = false;
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x000B06C2 File Offset: 0x000AE8C2
		public static void UnloadData()
		{
			if (GFX.DataLoaded)
			{
				GFX.GuiSpriteBank = null;
				GFX.PortraitsSpriteBank = null;
				GFX.SpriteBank = null;
				GFX.CompleteScreensXml = null;
				GFX.SceneryTiles = null;
				GFX.BGAutotiler = null;
				GFX.FGAutotiler = null;
			}
			GFX.DataLoaded = false;
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x000B06FC File Offset: 0x000AE8FC
		public static void LoadEffects()
		{
			GFX.FxMountain = GFX.LoadFx("MountainRender");
			GFX.FxGaussianBlur = GFX.LoadFx("GaussianBlur");
			GFX.FxDistort = GFX.LoadFx("Distort");
			GFX.FxDust = GFX.LoadFx("Dust");
			GFX.FxPrimitive = GFX.LoadFx("Primitive");
			GFX.FxDither = GFX.LoadFx("Dither");
			GFX.FxMagicGlow = GFX.LoadFx("MagicGlow");
			GFX.FxMirrors = GFX.LoadFx("Mirrors");
			GFX.FxColorGrading = GFX.LoadFx("ColorGrade");
			GFX.FxGlitch = GFX.LoadFx("Glitch");
			GFX.FxTexture = GFX.LoadFx("Texture");
			GFX.FxLighting = GFX.LoadFx("Lighting");
			GFX.FxDebug = new BasicEffect(Engine.Graphics.GraphicsDevice);
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x000B07D1 File Offset: 0x000AE9D1
		public static Effect LoadFx(string name)
		{
			return Engine.Instance.Content.Load<Effect>(Path.Combine("Effects", name));
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x000B07F0 File Offset: 0x000AE9F0
		public static void DrawVertices<T>(Matrix matrix, T[] vertices, int vertexCount, Effect effect = null, BlendState blendState = null) where T : struct, IVertexType
		{
			object obj = (effect != null) ? effect : GFX.FxPrimitive;
			BlendState blendState2 = (blendState != null) ? blendState : BlendState.AlphaBlend;
			Vector2 vector = new Vector2((float)Engine.Graphics.GraphicsDevice.Viewport.Width, (float)Engine.Graphics.GraphicsDevice.Viewport.Height);
			matrix *= Matrix.CreateScale(1f / vector.X * 2f, -(1f / vector.Y) * 2f, 1f);
			matrix *= Matrix.CreateTranslation(-1f, 1f, 0f);
			Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.GraphicsDevice.BlendState = blendState2;
			object obj2 = obj;
			obj2.Parameters["World"].SetValue(matrix);
			foreach (EffectPass effectPass in obj2.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Instance.GraphicsDevice.DrawUserPrimitives<T>(PrimitiveType.TriangleList, vertices, 0, vertexCount / 3);
			}
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000B0938 File Offset: 0x000AEB38
		public static void DrawIndexedVertices<T>(Matrix matrix, T[] vertices, int vertexCount, int[] indices, int primitiveCount, Effect effect = null, BlendState blendState = null) where T : struct, IVertexType
		{
			object obj = (effect != null) ? effect : GFX.FxPrimitive;
			BlendState blendState2 = (blendState != null) ? blendState : BlendState.AlphaBlend;
			Vector2 vector = new Vector2((float)Engine.Graphics.GraphicsDevice.Viewport.Width, (float)Engine.Graphics.GraphicsDevice.Viewport.Height);
			matrix *= Matrix.CreateScale(1f / vector.X * 2f, -(1f / vector.Y) * 2f, 1f);
			matrix *= Matrix.CreateTranslation(-1f, 1f, 0f);
			Engine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.GraphicsDevice.BlendState = blendState2;
			object obj2 = obj;
			obj2.Parameters["World"].SetValue(matrix);
			foreach (EffectPass effectPass in obj2.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives<T>(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, primitiveCount);
			}
		}

		// Token: 0x040017C4 RID: 6084
		public static Atlas Game;

		// Token: 0x040017C5 RID: 6085
		public static Atlas Gui;

		// Token: 0x040017C6 RID: 6086
		public static Atlas Opening;

		// Token: 0x040017C7 RID: 6087
		public static Atlas Misc;

		// Token: 0x040017C8 RID: 6088
		public static Atlas Portraits;

		// Token: 0x040017C9 RID: 6089
		public static Atlas ColorGrades;

		// Token: 0x040017CA RID: 6090
		public static VirtualTexture SplashScreen;

		// Token: 0x040017CB RID: 6091
		public static VirtualTexture MagicGlowNoise;

		// Token: 0x040017CC RID: 6092
		public static Effect FxMountain;

		// Token: 0x040017CD RID: 6093
		public static Effect FxDistort;

		// Token: 0x040017CE RID: 6094
		public static Effect FxGlitch;

		// Token: 0x040017CF RID: 6095
		public static Effect FxGaussianBlur;

		// Token: 0x040017D0 RID: 6096
		public static Effect FxPrimitive;

		// Token: 0x040017D1 RID: 6097
		public static Effect FxDust;

		// Token: 0x040017D2 RID: 6098
		public static Effect FxDither;

		// Token: 0x040017D3 RID: 6099
		public static Effect FxMagicGlow;

		// Token: 0x040017D4 RID: 6100
		public static Effect FxMirrors;

		// Token: 0x040017D5 RID: 6101
		public static Effect FxColorGrading;

		// Token: 0x040017D6 RID: 6102
		public static BasicEffect FxDebug;

		// Token: 0x040017D7 RID: 6103
		public static Effect FxTexture;

		// Token: 0x040017D8 RID: 6104
		public static Effect FxLighting;

		// Token: 0x040017D9 RID: 6105
		public static SpriteBank SpriteBank;

		// Token: 0x040017DA RID: 6106
		public static SpriteBank GuiSpriteBank;

		// Token: 0x040017DB RID: 6107
		public static SpriteBank PortraitsSpriteBank;

		// Token: 0x040017DC RID: 6108
		public static XmlDocument CompleteScreensXml;

		// Token: 0x040017DD RID: 6109
		public static AnimatedTilesBank AnimatedTilesBank;

		// Token: 0x040017DE RID: 6110
		public static Tileset SceneryTiles;

		// Token: 0x040017DF RID: 6111
		public static Autotiler BGAutotiler;

		// Token: 0x040017E0 RID: 6112
		public static Autotiler FGAutotiler;

		// Token: 0x040017E1 RID: 6113
		public const float PortraitSize = 240f;

		// Token: 0x040017E2 RID: 6114
		public static readonly BlendState Subtract = new BlendState
		{
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			ColorBlendFunction = BlendFunction.ReverseSubtract,
			AlphaSourceBlend = Blend.One,
			AlphaDestinationBlend = Blend.One,
			AlphaBlendFunction = BlendFunction.Add
		};

		// Token: 0x040017E3 RID: 6115
		public static readonly BlendState DestinationTransparencySubtract = new BlendState
		{
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			ColorBlendFunction = BlendFunction.ReverseSubtract,
			AlphaSourceBlend = Blend.Zero,
			AlphaDestinationBlend = Blend.One,
			AlphaBlendFunction = BlendFunction.Add
		};
	}
}

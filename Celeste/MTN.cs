using System;
using System.Diagnostics;
using System.IO;
using Monocle;

namespace Celeste
{
	// Token: 0x02000306 RID: 774
	public static class MTN
	{
		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001827 RID: 6183 RVA: 0x00097283 File Offset: 0x00095483
		// (set) Token: 0x06001828 RID: 6184 RVA: 0x0009728A File Offset: 0x0009548A
		public static bool Loaded { get; private set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001829 RID: 6185 RVA: 0x00097292 File Offset: 0x00095492
		// (set) Token: 0x0600182A RID: 6186 RVA: 0x00097299 File Offset: 0x00095499
		public static bool DataLoaded { get; private set; }

		// Token: 0x0600182B RID: 6187 RVA: 0x000972A4 File Offset: 0x000954A4
		public static void Load()
		{
			if (!MTN.Loaded)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				MTN.FileSelect = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "FileSelect"), Atlas.AtlasDataFormat.Packer);
				MTN.Journal = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Journal"), Atlas.AtlasDataFormat.Packer);
				MTN.Mountain = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Mountain"), Atlas.AtlasDataFormat.PackerNoAtlas);
				MTN.Checkpoints = Atlas.FromAtlas(Path.Combine("Graphics", "Atlases", "Checkpoints"), Atlas.AtlasDataFormat.PackerNoAtlas);
				MTN.MountainTerrainTextures = new VirtualTexture[3];
				MTN.MountainBuildingTextures = new VirtualTexture[3];
				MTN.MountainSkyboxTextures = new VirtualTexture[3];
				for (int i = 0; i < 3; i++)
				{
					MTN.MountainSkyboxTextures[i] = MTN.Mountain["skybox_" + i].Texture;
					MTN.MountainTerrainTextures[i] = MTN.Mountain["mountain_" + i].Texture;
					MTN.MountainBuildingTextures[i] = MTN.Mountain["buildings_" + i].Texture;
				}
				MTN.MountainMoonTexture = MTN.Mountain["moon"].Texture;
				MTN.MountainFogTexture = MTN.Mountain["fog"].Texture;
				MTN.MountainStarSky = MTN.Mountain["space"].Texture;
				MTN.MountainStars = MTN.Mountain["spacestars"].Texture;
				MTN.MountainStarStream = MTN.Mountain["starstream"].Texture;
				Console.WriteLine(" - MTN LOAD: " + stopwatch.ElapsedMilliseconds + "ms");
			}
			MTN.Loaded = true;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x00097480 File Offset: 0x00095680
		public static void LoadData()
		{
			if (!MTN.DataLoaded)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				string str = ".obj";
				MTN.MountainTerrain = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "mountain" + str));
				MTN.MountainBuildings = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "buildings" + str));
				MTN.MountainCoreWall = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "mountain_wall" + str));
				MTN.MountainMoon = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "moon" + str));
				MTN.MountainBird = ObjModel.Create(Path.Combine(Engine.ContentDirectory, "Overworld", "bird" + str));
				Console.WriteLine(" - MTN DATA LOAD: " + stopwatch.ElapsedMilliseconds + "ms");
			}
			MTN.DataLoaded = true;
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0009757C File Offset: 0x0009577C
		public static void Unload()
		{
			if (MTN.Loaded)
			{
				MTN.Journal.Dispose();
				MTN.Journal = null;
				MTN.Mountain.Dispose();
				MTN.Mountain = null;
				MTN.Checkpoints.Dispose();
				MTN.Checkpoints = null;
				MTN.FileSelect.Dispose();
				MTN.FileSelect = null;
			}
			MTN.Loaded = false;
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x000975D8 File Offset: 0x000957D8
		public static void UnloadData()
		{
			if (MTN.DataLoaded)
			{
				MTN.MountainTerrain.Dispose();
				MTN.MountainTerrain = null;
				MTN.MountainBuildings.Dispose();
				MTN.MountainBuildings = null;
				MTN.MountainCoreWall.Dispose();
				MTN.MountainCoreWall = null;
				MTN.MountainMoon.Dispose();
				MTN.MountainMoon = null;
				MTN.MountainBird.Dispose();
				MTN.MountainBird = null;
			}
			MTN.DataLoaded = false;
		}

		// Token: 0x040014FA RID: 5370
		public static Atlas FileSelect;

		// Token: 0x040014FB RID: 5371
		public static Atlas Journal;

		// Token: 0x040014FC RID: 5372
		public static Atlas Mountain;

		// Token: 0x040014FD RID: 5373
		public static Atlas Checkpoints;

		// Token: 0x040014FE RID: 5374
		public static ObjModel MountainTerrain;

		// Token: 0x040014FF RID: 5375
		public static ObjModel MountainBuildings;

		// Token: 0x04001500 RID: 5376
		public static ObjModel MountainCoreWall;

		// Token: 0x04001501 RID: 5377
		public static ObjModel MountainMoon;

		// Token: 0x04001502 RID: 5378
		public static ObjModel MountainBird;

		// Token: 0x04001503 RID: 5379
		public static VirtualTexture[] MountainTerrainTextures;

		// Token: 0x04001504 RID: 5380
		public static VirtualTexture[] MountainBuildingTextures;

		// Token: 0x04001505 RID: 5381
		public static VirtualTexture[] MountainSkyboxTextures;

		// Token: 0x04001506 RID: 5382
		public static VirtualTexture MountainFogTexture;

		// Token: 0x04001507 RID: 5383
		public static VirtualTexture MountainMoonTexture;

		// Token: 0x04001508 RID: 5384
		public static VirtualTexture MountainStarSky;

		// Token: 0x04001509 RID: 5385
		public static VirtualTexture MountainStars;

		// Token: 0x0400150A RID: 5386
		public static VirtualTexture MountainStarStream;
	}
}

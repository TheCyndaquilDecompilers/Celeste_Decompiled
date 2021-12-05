using System;
using System.Collections.Generic;
using Monocle;

namespace Celeste
{
	// Token: 0x02000387 RID: 903
	public class SurfaceIndex
	{
		// Token: 0x06001D6B RID: 7531 RVA: 0x000CCCBC File Offset: 0x000CAEBC
		public static Platform GetPlatformByPriority(List<Entity> platforms)
		{
			Platform platform = null;
			foreach (Entity entity in platforms)
			{
				if (entity is Platform && (platform == null || (entity as Platform).SurfaceSoundPriority > platform.SurfaceSoundPriority))
				{
					platform = (entity as Platform);
				}
			}
			return platform;
		}

		// Token: 0x04001E15 RID: 7701
		public const string Param = "surface_index";

		// Token: 0x04001E16 RID: 7702
		public const int Asphalt = 1;

		// Token: 0x04001E17 RID: 7703
		public const int Car = 2;

		// Token: 0x04001E18 RID: 7704
		public const int Dirt = 3;

		// Token: 0x04001E19 RID: 7705
		public const int Snow = 4;

		// Token: 0x04001E1A RID: 7706
		public const int Wood = 5;

		// Token: 0x04001E1B RID: 7707
		public const int Girder = 7;

		// Token: 0x04001E1C RID: 7708
		public const int Brick = 8;

		// Token: 0x04001E1D RID: 7709
		public const int ZipMover = 9;

		// Token: 0x04001E1E RID: 7710
		public const int ResortWood = 13;

		// Token: 0x04001E1F RID: 7711
		public const int DreamBlockInactive = 11;

		// Token: 0x04001E20 RID: 7712
		public const int DreamBlockActive = 12;

		// Token: 0x04001E21 RID: 7713
		public const int ResortRoof = 14;

		// Token: 0x04001E22 RID: 7714
		public const int ResortSinkingPlatforms = 15;

		// Token: 0x04001E23 RID: 7715
		public const int ResortLinens = 17;

		// Token: 0x04001E24 RID: 7716
		public const int ResortBoxes = 18;

		// Token: 0x04001E25 RID: 7717
		public const int ResortBooks = 19;

		// Token: 0x04001E26 RID: 7718
		public const int ClutterDoor = 20;

		// Token: 0x04001E27 RID: 7719
		public const int ClutterSwitch = 21;

		// Token: 0x04001E28 RID: 7720
		public const int ResortElevator = 22;

		// Token: 0x04001E29 RID: 7721
		public const int CliffsideSnow = 23;

		// Token: 0x04001E2A RID: 7722
		public const int CliffsideGrass = 25;

		// Token: 0x04001E2B RID: 7723
		public const int CliffsideWhiteBlock = 27;

		// Token: 0x04001E2C RID: 7724
		public const int Gondola = 28;

		// Token: 0x04001E2D RID: 7725
		public const int AuroraGlass = 32;

		// Token: 0x04001E2E RID: 7726
		public const int Grass = 33;

		// Token: 0x04001E2F RID: 7727
		public const int CassetteBlock = 35;

		// Token: 0x04001E30 RID: 7728
		public const int CoreIce = 36;

		// Token: 0x04001E31 RID: 7729
		public const int CoreMoltenRock = 37;

		// Token: 0x04001E32 RID: 7730
		public const int Glitch = 40;

		// Token: 0x04001E33 RID: 7731
		public const int MoonCafe = 42;

		// Token: 0x04001E34 RID: 7732
		public const int DreamClouds = 43;

		// Token: 0x04001E35 RID: 7733
		public const int Moon = 44;

		// Token: 0x04001E36 RID: 7734
		public const int StoneBridge = 6;

		// Token: 0x04001E37 RID: 7735
		public const int ResortBasementTile = 16;

		// Token: 0x04001E38 RID: 7736
		public const int ResortMagicButton = 21;

		// Token: 0x04001E39 RID: 7737
		public static Dictionary<char, int> TileToIndex = new Dictionary<char, int>
		{
			{
				'1',
				3
			},
			{
				'3',
				4
			},
			{
				'4',
				7
			},
			{
				'5',
				8
			},
			{
				'6',
				8
			},
			{
				'7',
				8
			},
			{
				'8',
				8
			},
			{
				'9',
				13
			},
			{
				'a',
				8
			},
			{
				'b',
				23
			},
			{
				'c',
				8
			},
			{
				'd',
				8
			},
			{
				'e',
				8
			},
			{
				'f',
				8
			},
			{
				'g',
				8
			},
			{
				'h',
				33
			},
			{
				'i',
				4
			},
			{
				'j',
				8
			},
			{
				'k',
				3
			},
			{
				'l',
				25
			},
			{
				'm',
				44
			},
			{
				'n',
				40
			},
			{
				'o',
				43
			}
		};
	}
}

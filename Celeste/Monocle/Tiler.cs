using System;

namespace Monocle
{
	// Token: 0x02000136 RID: 310
	public static class Tiler
	{
		// Token: 0x06000B24 RID: 2852 RVA: 0x0001EA68 File Offset: 0x0001CC68
		public static int[,] Tile(bool[,] bits, Func<int> tileDecider, Action<int> tileOutput, int tileWidth, int tileHeight, Tiler.EdgeBehavior edges)
		{
			int length = bits.GetLength(0);
			int length2 = bits.GetLength(1);
			int[,] array = new int[length, length2];
			Tiler.TileX = 0;
			while (Tiler.TileX < length)
			{
				Tiler.TileY = 0;
				while (Tiler.TileY < length2)
				{
					if (bits[Tiler.TileX, Tiler.TileY])
					{
						switch (edges)
						{
						case Tiler.EdgeBehavior.True:
							Tiler.Left = (Tiler.TileX == 0 || bits[Tiler.TileX - 1, Tiler.TileY]);
							Tiler.Right = (Tiler.TileX == length - 1 || bits[Tiler.TileX + 1, Tiler.TileY]);
							Tiler.Up = (Tiler.TileY == 0 || bits[Tiler.TileX, Tiler.TileY - 1]);
							Tiler.Down = (Tiler.TileY == length2 - 1 || bits[Tiler.TileX, Tiler.TileY + 1]);
							Tiler.UpLeft = (Tiler.TileX == 0 || Tiler.TileY == 0 || bits[Tiler.TileX - 1, Tiler.TileY - 1]);
							Tiler.UpRight = (Tiler.TileX == length - 1 || Tiler.TileY == 0 || bits[Tiler.TileX + 1, Tiler.TileY - 1]);
							Tiler.DownLeft = (Tiler.TileX == 0 || Tiler.TileY == length2 - 1 || bits[Tiler.TileX - 1, Tiler.TileY + 1]);
							Tiler.DownRight = (Tiler.TileX == length - 1 || Tiler.TileY == length2 - 1 || bits[Tiler.TileX + 1, Tiler.TileY + 1]);
							break;
						case Tiler.EdgeBehavior.False:
							Tiler.Left = (Tiler.TileX != 0 && bits[Tiler.TileX - 1, Tiler.TileY]);
							Tiler.Right = (Tiler.TileX != length - 1 && bits[Tiler.TileX + 1, Tiler.TileY]);
							Tiler.Up = (Tiler.TileY != 0 && bits[Tiler.TileX, Tiler.TileY - 1]);
							Tiler.Down = (Tiler.TileY != length2 - 1 && bits[Tiler.TileX, Tiler.TileY + 1]);
							Tiler.UpLeft = (Tiler.TileX != 0 && Tiler.TileY != 0 && bits[Tiler.TileX - 1, Tiler.TileY - 1]);
							Tiler.UpRight = (Tiler.TileX != length - 1 && Tiler.TileY != 0 && bits[Tiler.TileX + 1, Tiler.TileY - 1]);
							Tiler.DownLeft = (Tiler.TileX != 0 && Tiler.TileY != length2 - 1 && bits[Tiler.TileX - 1, Tiler.TileY + 1]);
							Tiler.DownRight = (Tiler.TileX != length - 1 && Tiler.TileY != length2 - 1 && bits[Tiler.TileX + 1, Tiler.TileY + 1]);
							break;
						case Tiler.EdgeBehavior.Wrap:
							Tiler.Left = bits[(Tiler.TileX + length - 1) % length, Tiler.TileY];
							Tiler.Right = bits[(Tiler.TileX + 1) % length, Tiler.TileY];
							Tiler.Up = bits[Tiler.TileX, (Tiler.TileY + length2 - 1) % length2];
							Tiler.Down = bits[Tiler.TileX, (Tiler.TileY + 1) % length2];
							Tiler.UpLeft = bits[(Tiler.TileX + length - 1) % length, (Tiler.TileY + length2 - 1) % length2];
							Tiler.UpRight = bits[(Tiler.TileX + 1) % length, (Tiler.TileY + length2 - 1) % length2];
							Tiler.DownLeft = bits[(Tiler.TileX + length - 1) % length, (Tiler.TileY + 1) % length2];
							Tiler.DownRight = bits[(Tiler.TileX + 1) % length, (Tiler.TileY + 1) % length2];
							break;
						}
						int num = tileDecider();
						tileOutput(num);
						array[Tiler.TileX, Tiler.TileY] = num;
					}
					Tiler.TileY++;
				}
				Tiler.TileX++;
			}
			return array;
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0001EE88 File Offset: 0x0001D088
		public static int[,] Tile(bool[,] bits, bool[,] mask, Func<int> tileDecider, Action<int> tileOutput, int tileWidth, int tileHeight, Tiler.EdgeBehavior edges)
		{
			int length = bits.GetLength(0);
			int length2 = bits.GetLength(1);
			int[,] array = new int[length, length2];
			Tiler.TileX = 0;
			while (Tiler.TileX < length)
			{
				Tiler.TileY = 0;
				while (Tiler.TileY < length2)
				{
					if (bits[Tiler.TileX, Tiler.TileY])
					{
						switch (edges)
						{
						case Tiler.EdgeBehavior.True:
							Tiler.Left = (Tiler.TileX == 0 || bits[Tiler.TileX - 1, Tiler.TileY] || mask[Tiler.TileX - 1, Tiler.TileY]);
							Tiler.Right = (Tiler.TileX == length - 1 || bits[Tiler.TileX + 1, Tiler.TileY] || mask[Tiler.TileX + 1, Tiler.TileY]);
							Tiler.Up = (Tiler.TileY == 0 || bits[Tiler.TileX, Tiler.TileY - 1] || mask[Tiler.TileX, Tiler.TileY - 1]);
							Tiler.Down = (Tiler.TileY == length2 - 1 || bits[Tiler.TileX, Tiler.TileY + 1] || mask[Tiler.TileX, Tiler.TileY + 1]);
							Tiler.UpLeft = (Tiler.TileX == 0 || Tiler.TileY == 0 || bits[Tiler.TileX - 1, Tiler.TileY - 1] || mask[Tiler.TileX - 1, Tiler.TileY - 1]);
							Tiler.UpRight = (Tiler.TileX == length - 1 || Tiler.TileY == 0 || bits[Tiler.TileX + 1, Tiler.TileY - 1] || mask[Tiler.TileX + 1, Tiler.TileY - 1]);
							Tiler.DownLeft = (Tiler.TileX == 0 || Tiler.TileY == length2 - 1 || bits[Tiler.TileX - 1, Tiler.TileY + 1] || mask[Tiler.TileX - 1, Tiler.TileY + 1]);
							Tiler.DownRight = (Tiler.TileX == length - 1 || Tiler.TileY == length2 - 1 || bits[Tiler.TileX + 1, Tiler.TileY + 1] || mask[Tiler.TileX + 1, Tiler.TileY + 1]);
							break;
						case Tiler.EdgeBehavior.False:
							Tiler.Left = (Tiler.TileX != 0 && (bits[Tiler.TileX - 1, Tiler.TileY] || mask[Tiler.TileX - 1, Tiler.TileY]));
							Tiler.Right = (Tiler.TileX != length - 1 && (bits[Tiler.TileX + 1, Tiler.TileY] || mask[Tiler.TileX + 1, Tiler.TileY]));
							Tiler.Up = (Tiler.TileY != 0 && (bits[Tiler.TileX, Tiler.TileY - 1] || mask[Tiler.TileX, Tiler.TileY - 1]));
							Tiler.Down = (Tiler.TileY != length2 - 1 && (bits[Tiler.TileX, Tiler.TileY + 1] || mask[Tiler.TileX, Tiler.TileY + 1]));
							Tiler.UpLeft = (Tiler.TileX != 0 && Tiler.TileY != 0 && (bits[Tiler.TileX - 1, Tiler.TileY - 1] || mask[Tiler.TileX - 1, Tiler.TileY - 1]));
							Tiler.UpRight = (Tiler.TileX != length - 1 && Tiler.TileY != 0 && (bits[Tiler.TileX + 1, Tiler.TileY - 1] || mask[Tiler.TileX + 1, Tiler.TileY - 1]));
							Tiler.DownLeft = (Tiler.TileX != 0 && Tiler.TileY != length2 - 1 && (bits[Tiler.TileX - 1, Tiler.TileY + 1] || mask[Tiler.TileX - 1, Tiler.TileY + 1]));
							Tiler.DownRight = (Tiler.TileX != length - 1 && Tiler.TileY != length2 - 1 && (bits[Tiler.TileX + 1, Tiler.TileY + 1] || mask[Tiler.TileX + 1, Tiler.TileY + 1]));
							break;
						case Tiler.EdgeBehavior.Wrap:
							Tiler.Left = (bits[(Tiler.TileX + length - 1) % length, Tiler.TileY] || mask[(Tiler.TileX + length - 1) % length, Tiler.TileY]);
							Tiler.Right = (bits[(Tiler.TileX + 1) % length, Tiler.TileY] || mask[(Tiler.TileX + 1) % length, Tiler.TileY]);
							Tiler.Up = (bits[Tiler.TileX, (Tiler.TileY + length2 - 1) % length2] || mask[Tiler.TileX, (Tiler.TileY + length2 - 1) % length2]);
							Tiler.Down = (bits[Tiler.TileX, (Tiler.TileY + 1) % length2] || mask[Tiler.TileX, (Tiler.TileY + 1) % length2]);
							Tiler.UpLeft = (bits[(Tiler.TileX + length - 1) % length, (Tiler.TileY + length2 - 1) % length2] || mask[(Tiler.TileX + length - 1) % length, (Tiler.TileY + length2 - 1) % length2]);
							Tiler.UpRight = (bits[(Tiler.TileX + 1) % length, (Tiler.TileY + length2 - 1) % length2] || mask[(Tiler.TileX + 1) % length, (Tiler.TileY + length2 - 1) % length2]);
							Tiler.DownLeft = (bits[(Tiler.TileX + length - 1) % length, (Tiler.TileY + 1) % length2] || mask[(Tiler.TileX + length - 1) % length, (Tiler.TileY + 1) % length2]);
							Tiler.DownRight = (bits[(Tiler.TileX + 1) % length, (Tiler.TileY + 1) % length2] || mask[(Tiler.TileX + 1) % length, (Tiler.TileY + 1) % length2]);
							break;
						}
						int num = tileDecider();
						tileOutput(num);
						array[Tiler.TileX, Tiler.TileY] = num;
					}
					Tiler.TileY++;
				}
				Tiler.TileX++;
			}
			return array;
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x0001F50C File Offset: 0x0001D70C
		public static int[,] Tile(bool[,] bits, AutotileData autotileData, Action<int> tileOutput, int tileWidth, int tileHeight, Tiler.EdgeBehavior edges)
		{
			return Tiler.Tile(bits, new Func<int>(autotileData.TileHandler), tileOutput, tileWidth, tileHeight, edges);
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x0001F526 File Offset: 0x0001D726
		public static int[,] Tile(bool[,] bits, bool[,] mask, AutotileData autotileData, Action<int> tileOutput, int tileWidth, int tileHeight, Tiler.EdgeBehavior edges)
		{
			return Tiler.Tile(bits, mask, new Func<int>(autotileData.TileHandler), tileOutput, tileWidth, tileHeight, edges);
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000B28 RID: 2856 RVA: 0x0001F542 File Offset: 0x0001D742
		// (set) Token: 0x06000B29 RID: 2857 RVA: 0x0001F549 File Offset: 0x0001D749
		public static int TileX { get; private set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000B2A RID: 2858 RVA: 0x0001F551 File Offset: 0x0001D751
		// (set) Token: 0x06000B2B RID: 2859 RVA: 0x0001F558 File Offset: 0x0001D758
		public static int TileY { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000B2C RID: 2860 RVA: 0x0001F560 File Offset: 0x0001D760
		// (set) Token: 0x06000B2D RID: 2861 RVA: 0x0001F567 File Offset: 0x0001D767
		public static bool Left { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000B2E RID: 2862 RVA: 0x0001F56F File Offset: 0x0001D76F
		// (set) Token: 0x06000B2F RID: 2863 RVA: 0x0001F576 File Offset: 0x0001D776
		public static bool Right { get; private set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000B30 RID: 2864 RVA: 0x0001F57E File Offset: 0x0001D77E
		// (set) Token: 0x06000B31 RID: 2865 RVA: 0x0001F585 File Offset: 0x0001D785
		public static bool Up { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000B32 RID: 2866 RVA: 0x0001F58D File Offset: 0x0001D78D
		// (set) Token: 0x06000B33 RID: 2867 RVA: 0x0001F594 File Offset: 0x0001D794
		public static bool Down { get; private set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000B34 RID: 2868 RVA: 0x0001F59C File Offset: 0x0001D79C
		// (set) Token: 0x06000B35 RID: 2869 RVA: 0x0001F5A3 File Offset: 0x0001D7A3
		public static bool UpLeft { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000B36 RID: 2870 RVA: 0x0001F5AB File Offset: 0x0001D7AB
		// (set) Token: 0x06000B37 RID: 2871 RVA: 0x0001F5B2 File Offset: 0x0001D7B2
		public static bool UpRight { get; private set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000B38 RID: 2872 RVA: 0x0001F5BA File Offset: 0x0001D7BA
		// (set) Token: 0x06000B39 RID: 2873 RVA: 0x0001F5C1 File Offset: 0x0001D7C1
		public static bool DownLeft { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000B3A RID: 2874 RVA: 0x0001F5C9 File Offset: 0x0001D7C9
		// (set) Token: 0x06000B3B RID: 2875 RVA: 0x0001F5D0 File Offset: 0x0001D7D0
		public static bool DownRight { get; private set; }

		// Token: 0x020003C0 RID: 960
		public enum EdgeBehavior
		{
			// Token: 0x04001F65 RID: 8037
			True,
			// Token: 0x04001F66 RID: 8038
			False,
			// Token: 0x04001F67 RID: 8039
			Wrap
		}
	}
}

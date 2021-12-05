using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000309 RID: 777
	public static class ClutterBlockGenerator
	{
		// Token: 0x06001837 RID: 6199 RVA: 0x00097BB4 File Offset: 0x00095DB4
		public static void Init(Level lvl)
		{
			if (!ClutterBlockGenerator.initialized)
			{
				ClutterBlockGenerator.initialized = true;
				ClutterBlockGenerator.level = lvl;
				ClutterBlockGenerator.columns = ClutterBlockGenerator.level.Bounds.Width / 8;
				ClutterBlockGenerator.rows = ClutterBlockGenerator.level.Bounds.Height / 8 + 1;
				if (ClutterBlockGenerator.tiles == null)
				{
					ClutterBlockGenerator.tiles = new ClutterBlockGenerator.Tile[200, 200];
				}
				for (int i = 0; i < ClutterBlockGenerator.columns; i++)
				{
					for (int j = 0; j < ClutterBlockGenerator.rows; j++)
					{
						ClutterBlockGenerator.tiles[i, j].Color = -1;
						ClutterBlockGenerator.tiles[i, j].Block = null;
					}
				}
				for (int k = 0; k < ClutterBlockGenerator.enabled.Length; k++)
				{
					ClutterBlockGenerator.enabled[k] = !ClutterBlockGenerator.level.Session.GetFlag("oshiro_clutter_cleared_" + k);
				}
				if (ClutterBlockGenerator.textures == null)
				{
					ClutterBlockGenerator.textures = new List<List<ClutterBlockGenerator.TextureSet>>();
					for (int l = 0; l < 3; l++)
					{
						List<ClutterBlockGenerator.TextureSet> list = new List<ClutterBlockGenerator.TextureSet>();
						Atlas game = GFX.Game;
						string str = "objects/resortclutter/";
						ClutterBlock.Colors colors = (ClutterBlock.Colors)l;
						foreach (MTexture mtexture in game.GetAtlasSubtextures(str + colors.ToString() + "_"))
						{
							int num = mtexture.Width / 8;
							int num2 = mtexture.Height / 8;
							ClutterBlockGenerator.TextureSet textureSet = null;
							foreach (ClutterBlockGenerator.TextureSet textureSet2 in list)
							{
								if (textureSet2.Columns == num && textureSet2.Rows == num2)
								{
									textureSet = textureSet2;
									break;
								}
							}
							if (textureSet == null)
							{
								List<ClutterBlockGenerator.TextureSet> list2 = list;
								ClutterBlockGenerator.TextureSet textureSet3 = new ClutterBlockGenerator.TextureSet();
								textureSet3.Columns = num;
								textureSet3.Rows = num2;
								textureSet = textureSet3;
								list2.Add(textureSet3);
							}
							textureSet.textures.Add(mtexture);
						}
						list.Sort((ClutterBlockGenerator.TextureSet a, ClutterBlockGenerator.TextureSet b) => -Math.Sign(a.Columns * a.Rows - b.Columns * b.Rows));
						ClutterBlockGenerator.textures.Add(list);
					}
				}
				Point levelSolidOffset = ClutterBlockGenerator.level.LevelSolidOffset;
				for (int m = 0; m < ClutterBlockGenerator.columns; m++)
				{
					for (int n = 0; n < ClutterBlockGenerator.rows; n++)
					{
						ClutterBlockGenerator.tiles[m, n].Wall = (ClutterBlockGenerator.level.SolidsData[levelSolidOffset.X + m, levelSolidOffset.Y + n] != '0');
					}
				}
			}
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x00097E7C File Offset: 0x0009607C
		public static void Dispose()
		{
			ClutterBlockGenerator.textures = null;
			ClutterBlockGenerator.tiles = null;
			ClutterBlockGenerator.initialized = false;
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x00097E90 File Offset: 0x00096090
		public static void Add(int x, int y, int w, int h, ClutterBlock.Colors color)
		{
			ClutterBlockGenerator.level.Add(new ClutterBlockBase(new Vector2((float)ClutterBlockGenerator.level.Bounds.X, (float)ClutterBlockGenerator.level.Bounds.Y) + new Vector2((float)x, (float)y) * 8f, w * 8, h * 8, ClutterBlockGenerator.enabled[(int)color], color));
			if (ClutterBlockGenerator.enabled[(int)color])
			{
				int i = Math.Max(0, x);
				int num = Math.Min(ClutterBlockGenerator.columns, x + w);
				while (i < num)
				{
					int j = Math.Max(0, y);
					int num2 = Math.Min(ClutterBlockGenerator.rows, y + h);
					while (j < num2)
					{
						Point point = new Point(i, j);
						ClutterBlockGenerator.tiles[point.X, point.Y].Color = (int)color;
						ClutterBlockGenerator.active.Add(point);
						j++;
					}
					i++;
				}
			}
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x00097F78 File Offset: 0x00096178
		public static void Generate()
		{
			if (!ClutterBlockGenerator.initialized)
			{
				return;
			}
			ClutterBlockGenerator.active.Shuffle<Point>();
			List<ClutterBlock> list = new List<ClutterBlock>();
			Rectangle bounds = ClutterBlockGenerator.level.Bounds;
			foreach (Point point in ClutterBlockGenerator.active)
			{
				if (ClutterBlockGenerator.tiles[point.X, point.Y].Block == null)
				{
					int num = 0;
					int color;
					ClutterBlockGenerator.TextureSet textureSet;
					for (;;)
					{
						color = ClutterBlockGenerator.tiles[point.X, point.Y].Color;
						textureSet = ClutterBlockGenerator.textures[color][num];
						bool flag = true;
						if (point.X + textureSet.Columns <= ClutterBlockGenerator.columns && point.Y + textureSet.Rows <= ClutterBlockGenerator.rows)
						{
							int num2 = point.X;
							int num3 = point.X + textureSet.Columns;
							while (flag && num2 < num3)
							{
								int num4 = point.Y;
								int num5 = point.Y + textureSet.Rows;
								while (flag && num4 < num5)
								{
									ClutterBlockGenerator.Tile tile = ClutterBlockGenerator.tiles[num2, num4];
									if (tile.Block != null || tile.Color != color)
									{
										flag = false;
									}
									num4++;
								}
								num2++;
							}
							if (flag)
							{
								break;
							}
						}
						num++;
					}
					ClutterBlock clutterBlock = new ClutterBlock(new Vector2((float)bounds.X, (float)bounds.Y) + new Vector2((float)point.X, (float)point.Y) * 8f, Calc.Random.Choose(textureSet.textures), (ClutterBlock.Colors)color);
					for (int i = point.X; i < point.X + textureSet.Columns; i++)
					{
						for (int j = point.Y; j < point.Y + textureSet.Rows; j++)
						{
							ClutterBlockGenerator.tiles[i, j].Block = clutterBlock;
						}
					}
					list.Add(clutterBlock);
					ClutterBlockGenerator.level.Add(clutterBlock);
				}
			}
			for (int k = 0; k < ClutterBlockGenerator.columns; k++)
			{
				for (int l = 0; l < ClutterBlockGenerator.rows; l++)
				{
					ClutterBlockGenerator.Tile tile2 = ClutterBlockGenerator.tiles[k, l];
					if (tile2.Block != null)
					{
						ClutterBlock block = tile2.Block;
						if (!block.TopSideOpen && (l == 0 || ClutterBlockGenerator.tiles[k, l - 1].Empty))
						{
							block.TopSideOpen = true;
						}
						if (!block.LeftSideOpen && (k == 0 || ClutterBlockGenerator.tiles[k - 1, l].Empty))
						{
							block.LeftSideOpen = true;
						}
						if (!block.RightSideOpen && (k == ClutterBlockGenerator.columns - 1 || ClutterBlockGenerator.tiles[k + 1, l].Empty))
						{
							block.RightSideOpen = true;
						}
						if (!block.OnTheGround && l < ClutterBlockGenerator.rows - 1)
						{
							ClutterBlockGenerator.Tile tile3 = ClutterBlockGenerator.tiles[k, l + 1];
							if (tile3.Wall)
							{
								block.OnTheGround = true;
							}
							else if (tile3.Block != null && tile3.Block != block && !block.HasBelow.Contains(tile3.Block))
							{
								block.HasBelow.Add(tile3.Block);
								block.Below.Add(tile3.Block);
								tile3.Block.Above.Add(block);
							}
						}
					}
				}
			}
			foreach (ClutterBlock clutterBlock2 in list)
			{
				if (clutterBlock2.OnTheGround)
				{
					ClutterBlockGenerator.SetAboveToOnGround(clutterBlock2);
				}
			}
			ClutterBlockGenerator.initialized = false;
			ClutterBlockGenerator.level = null;
			ClutterBlockGenerator.active.Clear();
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000983BC File Offset: 0x000965BC
		private static void SetAboveToOnGround(ClutterBlock block)
		{
			foreach (ClutterBlock clutterBlock in block.Above)
			{
				if (!clutterBlock.OnTheGround)
				{
					clutterBlock.OnTheGround = true;
					ClutterBlockGenerator.SetAboveToOnGround(clutterBlock);
				}
			}
		}

		// Token: 0x04001519 RID: 5401
		private static Level level;

		// Token: 0x0400151A RID: 5402
		private static ClutterBlockGenerator.Tile[,] tiles;

		// Token: 0x0400151B RID: 5403
		private static List<Point> active = new List<Point>();

		// Token: 0x0400151C RID: 5404
		private static List<List<ClutterBlockGenerator.TextureSet>> textures;

		// Token: 0x0400151D RID: 5405
		private static int columns;

		// Token: 0x0400151E RID: 5406
		private static int rows;

		// Token: 0x0400151F RID: 5407
		private static bool[] enabled = new bool[3];

		// Token: 0x04001520 RID: 5408
		private static bool initialized;

		// Token: 0x020006D4 RID: 1748
		private struct Tile
		{
			// Token: 0x1700066F RID: 1647
			// (get) Token: 0x06002D18 RID: 11544 RVA: 0x0011E287 File Offset: 0x0011C487
			public bool Empty
			{
				get
				{
					return !this.Wall && this.Color == -1;
				}
			}

			// Token: 0x04002C5A RID: 11354
			public int Color;

			// Token: 0x04002C5B RID: 11355
			public bool Wall;

			// Token: 0x04002C5C RID: 11356
			public ClutterBlock Block;
		}

		// Token: 0x020006D5 RID: 1749
		private class TextureSet
		{
			// Token: 0x04002C5D RID: 11357
			public int Columns;

			// Token: 0x04002C5E RID: 11358
			public int Rows;

			// Token: 0x04002C5F RID: 11359
			public List<MTexture> textures = new List<MTexture>();
		}
	}
}

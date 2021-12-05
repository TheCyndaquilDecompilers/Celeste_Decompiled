using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E4 RID: 740
	public class Autotiler
	{
		// Token: 0x060016CE RID: 5838 RVA: 0x00087448 File Offset: 0x00085648
		public Autotiler(string filename)
		{
			Dictionary<char, XmlElement> dictionary = new Dictionary<char, XmlElement>();
			foreach (object obj in Calc.LoadContentXML(filename).GetElementsByTagName("Tileset"))
			{
				XmlElement xmlElement = (XmlElement)obj;
				char c = xmlElement.AttrChar("id");
				Tileset tileset = new Tileset(GFX.Game["tilesets/" + xmlElement.Attr("path")], 8, 8);
				Autotiler.TerrainType terrainType = new Autotiler.TerrainType(c);
				this.ReadInto(terrainType, tileset, xmlElement);
				if (xmlElement.HasAttr("copy"))
				{
					char key = xmlElement.AttrChar("copy");
					if (!dictionary.ContainsKey(key))
					{
						throw new Exception("Copied tilesets must be defined before the tilesets that copy them!");
					}
					this.ReadInto(terrainType, tileset, dictionary[key]);
				}
				if (xmlElement.HasAttr("ignores"))
				{
					foreach (string text in xmlElement.Attr("ignores").Split(new char[]
					{
						','
					}))
					{
						if (text.Length > 0)
						{
							terrainType.Ignores.Add(text[0]);
						}
					}
				}
				dictionary.Add(c, xmlElement);
				this.lookup.Add(c, terrainType);
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x000875EC File Offset: 0x000857EC
		private void ReadInto(Autotiler.TerrainType data, Tileset tileset, XmlElement xml)
		{
			foreach (object obj in xml)
			{
				if (!(obj is XmlComment))
				{
					XmlElement xml2 = obj as XmlElement;
					string text = xml2.Attr("mask");
					Autotiler.Tiles tiles;
					if (text == "center")
					{
						tiles = data.Center;
					}
					else if (text == "padding")
					{
						tiles = data.Padded;
					}
					else
					{
						Autotiler.Masked masked = new Autotiler.Masked();
						tiles = masked.Tiles;
						int i = 0;
						int num = 0;
						while (i < text.Length)
						{
							if (text[i] == '0')
							{
								masked.Mask[num++] = 0;
							}
							else if (text[i] == '1')
							{
								masked.Mask[num++] = 1;
							}
							else if (text[i] == 'x' || text[i] == 'X')
							{
								masked.Mask[num++] = 2;
							}
							i++;
						}
						data.Masked.Add(masked);
					}
					string[] array = xml2.Attr("tiles").Split(new char[]
					{
						';'
					});
					for (int j = 0; j < array.Length; j++)
					{
						string[] array2 = array[j].Split(new char[]
						{
							','
						});
						int x = int.Parse(array2[0]);
						int y = int.Parse(array2[1]);
						MTexture item = tileset[x, y];
						tiles.Textures.Add(item);
					}
					if (xml2.HasAttr("sprites"))
					{
						foreach (string item2 in xml2.Attr("sprites").Split(new char[]
						{
							','
						}))
						{
							tiles.OverlapSprites.Add(item2);
						}
						tiles.HasOverlays = true;
					}
				}
			}
			data.Masked.Sort(delegate(Autotiler.Masked a, Autotiler.Masked b)
			{
				int num2 = 0;
				int num3 = 0;
				for (int k = 0; k < 9; k++)
				{
					if (a.Mask[k] == 2)
					{
						num2++;
					}
					if (b.Mask[k] == 2)
					{
						num3++;
					}
				}
				return num2 - num3;
			});
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0008782C File Offset: 0x00085A2C
		public Autotiler.Generated GenerateMap(VirtualMap<char> mapData, bool paddingIgnoreOutOfLevel)
		{
			Autotiler.Behaviour behaviour = new Autotiler.Behaviour
			{
				EdgesExtend = true,
				EdgesIgnoreOutOfLevel = false,
				PaddingIgnoreOutOfLevel = paddingIgnoreOutOfLevel
			};
			return this.Generate(mapData, 0, 0, mapData.Columns, mapData.Rows, false, '0', behaviour);
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x00087874 File Offset: 0x00085A74
		public Autotiler.Generated GenerateMap(VirtualMap<char> mapData, Autotiler.Behaviour behaviour)
		{
			return this.Generate(mapData, 0, 0, mapData.Columns, mapData.Rows, false, '0', behaviour);
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x0008789C File Offset: 0x00085A9C
		public Autotiler.Generated GenerateBox(char id, int tilesX, int tilesY)
		{
			return this.Generate(null, 0, 0, tilesX, tilesY, true, id, default(Autotiler.Behaviour));
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x000878C0 File Offset: 0x00085AC0
		public Autotiler.Generated GenerateOverlay(char id, int x, int y, int tilesX, int tilesY, VirtualMap<char> mapData)
		{
			Autotiler.Behaviour behaviour = new Autotiler.Behaviour
			{
				EdgesExtend = true,
				EdgesIgnoreOutOfLevel = true,
				PaddingIgnoreOutOfLevel = true
			};
			return this.Generate(mapData, x, y, tilesX, tilesY, true, id, behaviour);
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x00087900 File Offset: 0x00085B00
		private Autotiler.Generated Generate(VirtualMap<char> mapData, int startX, int startY, int tilesX, int tilesY, bool forceSolid, char forceID, Autotiler.Behaviour behaviour)
		{
			TileGrid tileGrid = new TileGrid(8, 8, tilesX, tilesY);
			AnimatedTiles animatedTiles = new AnimatedTiles(tilesX, tilesY, GFX.AnimatedTilesBank);
			Rectangle empty = Rectangle.Empty;
			if (forceSolid)
			{
				empty = new Rectangle(startX, startY, tilesX, tilesY);
			}
			if (mapData != null)
			{
				for (int i = startX; i < startX + tilesX; i += 50)
				{
					for (int j = startY; j < startY + tilesY; j += 50)
					{
						if (!mapData.AnyInSegmentAtTile(i, j))
						{
							j = j / 50 * 50;
						}
						else
						{
							int k = i;
							int num = Math.Min(i + 50, startX + tilesX);
							while (k < num)
							{
								int l = j;
								int num2 = Math.Min(j + 50, startY + tilesY);
								while (l < num2)
								{
									Autotiler.Tiles tiles = this.TileHandler(mapData, k, l, empty, forceID, behaviour);
									if (tiles != null)
									{
										tileGrid.Tiles[k - startX, l - startY] = Calc.Random.Choose(tiles.Textures);
										if (tiles.HasOverlays)
										{
											animatedTiles.Set(k - startX, l - startY, Calc.Random.Choose(tiles.OverlapSprites), 1f, 1f);
										}
									}
									l++;
								}
								k++;
							}
						}
					}
				}
			}
			else
			{
				for (int m = startX; m < startX + tilesX; m++)
				{
					for (int n = startY; n < startY + tilesY; n++)
					{
						Autotiler.Tiles tiles2 = this.TileHandler(null, m, n, empty, forceID, behaviour);
						if (tiles2 != null)
						{
							tileGrid.Tiles[m - startX, n - startY] = Calc.Random.Choose(tiles2.Textures);
							if (tiles2.HasOverlays)
							{
								animatedTiles.Set(m - startX, n - startY, Calc.Random.Choose(tiles2.OverlapSprites), 1f, 1f);
							}
						}
					}
				}
			}
			return new Autotiler.Generated
			{
				TileGrid = tileGrid,
				SpriteOverlay = animatedTiles
			};
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x00087AFC File Offset: 0x00085CFC
		private Autotiler.Tiles TileHandler(VirtualMap<char> mapData, int x, int y, Rectangle forceFill, char forceID, Autotiler.Behaviour behaviour)
		{
			char tile = this.GetTile(mapData, x, y, forceFill, forceID, behaviour);
			if (this.IsEmpty(tile))
			{
				return null;
			}
			Autotiler.TerrainType terrainType = this.lookup[tile];
			bool flag = true;
			int num = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					bool flag2 = this.CheckTile(terrainType, mapData, x + j, y + i, forceFill, behaviour);
					if (!flag2 && behaviour.EdgesIgnoreOutOfLevel && !this.CheckForSameLevel(x, y, x + j, y + i))
					{
						flag2 = true;
					}
					this.adjacent[num++] = (flag2 ? 1 : 0);
					if (!flag2)
					{
						flag = false;
					}
				}
			}
			if (!flag)
			{
				foreach (Autotiler.Masked masked in terrainType.Masked)
				{
					bool flag3 = true;
					int num2 = 0;
					while (num2 < 9 && flag3)
					{
						if (masked.Mask[num2] != 2 && masked.Mask[num2] != this.adjacent[num2])
						{
							flag3 = false;
						}
						num2++;
					}
					if (flag3)
					{
						return masked.Tiles;
					}
				}
				return null;
			}
			bool flag4;
			if (!behaviour.PaddingIgnoreOutOfLevel)
			{
				flag4 = (!this.CheckTile(terrainType, mapData, x - 2, y, forceFill, behaviour) || !this.CheckTile(terrainType, mapData, x + 2, y, forceFill, behaviour) || !this.CheckTile(terrainType, mapData, x, y - 2, forceFill, behaviour) || !this.CheckTile(terrainType, mapData, x, y + 2, forceFill, behaviour));
			}
			else
			{
				flag4 = ((!this.CheckTile(terrainType, mapData, x - 2, y, forceFill, behaviour) && this.CheckForSameLevel(x, y, x - 2, y)) || (!this.CheckTile(terrainType, mapData, x + 2, y, forceFill, behaviour) && this.CheckForSameLevel(x, y, x + 2, y)) || (!this.CheckTile(terrainType, mapData, x, y - 2, forceFill, behaviour) && this.CheckForSameLevel(x, y, x, y - 2)) || (!this.CheckTile(terrainType, mapData, x, y + 2, forceFill, behaviour) && this.CheckForSameLevel(x, y, x, y + 2)));
			}
			if (flag4)
			{
				return this.lookup[tile].Padded;
			}
			return this.lookup[tile].Center;
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x00087D50 File Offset: 0x00085F50
		private bool CheckForSameLevel(int x1, int y1, int x2, int y2)
		{
			foreach (Rectangle rectangle in this.LevelBounds)
			{
				if (rectangle.Contains(x1, y1) && rectangle.Contains(x2, y2))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x00087DBC File Offset: 0x00085FBC
		private bool CheckTile(Autotiler.TerrainType set, VirtualMap<char> mapData, int x, int y, Rectangle forceFill, Autotiler.Behaviour behaviour)
		{
			if (forceFill.Contains(x, y))
			{
				return true;
			}
			if (mapData == null)
			{
				return behaviour.EdgesExtend;
			}
			if (x >= 0 && y >= 0 && x < mapData.Columns && y < mapData.Rows)
			{
				char c = mapData[x, y];
				return !this.IsEmpty(c) && !set.Ignore(c);
			}
			if (!behaviour.EdgesExtend)
			{
				return false;
			}
			char c2 = mapData[Calc.Clamp(x, 0, mapData.Columns - 1), Calc.Clamp(y, 0, mapData.Rows - 1)];
			return !this.IsEmpty(c2) && !set.Ignore(c2);
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x00087E64 File Offset: 0x00086064
		private char GetTile(VirtualMap<char> mapData, int x, int y, Rectangle forceFill, char forceID, Autotiler.Behaviour behaviour)
		{
			if (forceFill.Contains(x, y))
			{
				return forceID;
			}
			if (mapData == null)
			{
				if (!behaviour.EdgesExtend)
				{
					return '0';
				}
				return forceID;
			}
			else
			{
				if (x >= 0 && y >= 0 && x < mapData.Columns && y < mapData.Rows)
				{
					return mapData[x, y];
				}
				if (!behaviour.EdgesExtend)
				{
					return '0';
				}
				int x2 = Calc.Clamp(x, 0, mapData.Columns - 1);
				int y2 = Calc.Clamp(y, 0, mapData.Rows - 1);
				return mapData[x2, y2];
			}
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00087EE8 File Offset: 0x000860E8
		private bool IsEmpty(char id)
		{
			return id == '0' || id == '\0';
		}

		// Token: 0x04001356 RID: 4950
		public List<Rectangle> LevelBounds = new List<Rectangle>();

		// Token: 0x04001357 RID: 4951
		private Dictionary<char, Autotiler.TerrainType> lookup = new Dictionary<char, Autotiler.TerrainType>();

		// Token: 0x04001358 RID: 4952
		private byte[] adjacent = new byte[9];

		// Token: 0x02000689 RID: 1673
		private class TerrainType
		{
			// Token: 0x06002BF7 RID: 11255 RVA: 0x00118BB9 File Offset: 0x00116DB9
			public TerrainType(char id)
			{
				this.ID = id;
			}

			// Token: 0x06002BF8 RID: 11256 RVA: 0x00118BF4 File Offset: 0x00116DF4
			public bool Ignore(char c)
			{
				return this.ID != c && (this.Ignores.Contains(c) || this.Ignores.Contains('*'));
			}

			// Token: 0x04002B1E RID: 11038
			public char ID;

			// Token: 0x04002B1F RID: 11039
			public HashSet<char> Ignores = new HashSet<char>();

			// Token: 0x04002B20 RID: 11040
			public List<Autotiler.Masked> Masked = new List<Autotiler.Masked>();

			// Token: 0x04002B21 RID: 11041
			public Autotiler.Tiles Center = new Autotiler.Tiles();

			// Token: 0x04002B22 RID: 11042
			public Autotiler.Tiles Padded = new Autotiler.Tiles();
		}

		// Token: 0x0200068A RID: 1674
		private class Masked
		{
			// Token: 0x04002B23 RID: 11043
			public byte[] Mask = new byte[9];

			// Token: 0x04002B24 RID: 11044
			public Autotiler.Tiles Tiles = new Autotiler.Tiles();
		}

		// Token: 0x0200068B RID: 1675
		private class Tiles
		{
			// Token: 0x04002B25 RID: 11045
			public List<MTexture> Textures = new List<MTexture>();

			// Token: 0x04002B26 RID: 11046
			public List<string> OverlapSprites = new List<string>();

			// Token: 0x04002B27 RID: 11047
			public bool HasOverlays;
		}

		// Token: 0x0200068C RID: 1676
		public struct Generated
		{
			// Token: 0x04002B28 RID: 11048
			public TileGrid TileGrid;

			// Token: 0x04002B29 RID: 11049
			public AnimatedTiles SpriteOverlay;
		}

		// Token: 0x0200068D RID: 1677
		public struct Behaviour
		{
			// Token: 0x04002B2A RID: 11050
			public bool PaddingIgnoreOutOfLevel;

			// Token: 0x04002B2B RID: 11051
			public bool EdgesIgnoreOutOfLevel;

			// Token: 0x04002B2C RID: 11052
			public bool EdgesExtend;
		}
	}
}

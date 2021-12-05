using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000FA RID: 250
	public class TileGrid : Component
	{
		// Token: 0x060006AE RID: 1710 RVA: 0x0000AF5C File Offset: 0x0000915C
		public TileGrid(int tileWidth, int tileHeight, int tilesX, int tilesY) : base(false, true)
		{
			this.TileWidth = tileWidth;
			this.TileHeight = tileHeight;
			this.Tiles = new VirtualMap<MTexture>(tilesX, tilesY, null);
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x0000AF99 File Offset: 0x00009199
		// (set) Token: 0x060006B0 RID: 1712 RVA: 0x0000AFA1 File Offset: 0x000091A1
		public int TileWidth { get; private set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x0000AFAA File Offset: 0x000091AA
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x0000AFB2 File Offset: 0x000091B2
		public int TileHeight { get; private set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0000AFBB File Offset: 0x000091BB
		public int TilesX
		{
			get
			{
				return this.Tiles.Columns;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x0000AFC8 File Offset: 0x000091C8
		public int TilesY
		{
			get
			{
				return this.Tiles.Rows;
			}
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0000AFD8 File Offset: 0x000091D8
		public void Populate(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
		{
			int num = 0;
			while (num < tiles.GetLength(0) && num + offsetX < this.TilesX)
			{
				int num2 = 0;
				while (num2 < tiles.GetLength(1) && num2 + offsetY < this.TilesY)
				{
					this.Tiles[num + offsetX, num2 + offsetY] = tileset[tiles[num, num2]];
					num2++;
				}
				num++;
			}
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x0000B040 File Offset: 0x00009240
		public void Overlay(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
		{
			int num = 0;
			while (num < tiles.GetLength(0) && num + offsetX < this.TilesX)
			{
				int num2 = 0;
				while (num2 < tiles.GetLength(1) && num2 + offsetY < this.TilesY)
				{
					if (tiles[num, num2] >= 0)
					{
						this.Tiles[num + offsetX, num2 + offsetY] = tileset[tiles[num, num2]];
					}
					num2++;
				}
				num++;
			}
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0000B0B4 File Offset: 0x000092B4
		public void Extend(int left, int right, int up, int down)
		{
			this.Position -= new Vector2((float)(left * this.TileWidth), (float)(up * this.TileHeight));
			int num = this.TilesX + left + right;
			int num2 = this.TilesY + up + down;
			if (num <= 0 || num2 <= 0)
			{
				this.Tiles = new VirtualMap<MTexture>(0, 0, null);
				return;
			}
			VirtualMap<MTexture> virtualMap = new VirtualMap<MTexture>(num, num2, null);
			for (int i = 0; i < this.TilesX; i++)
			{
				for (int j = 0; j < this.TilesY; j++)
				{
					int num3 = i + left;
					int num4 = j + up;
					if (num3 >= 0 && num3 < num && num4 >= 0 && num4 < num2)
					{
						virtualMap[num3, num4] = this.Tiles[i, j];
					}
				}
			}
			for (int k = 0; k < left; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					virtualMap[k, l] = this.Tiles[0, Calc.Clamp(l - up, 0, this.TilesY - 1)];
				}
			}
			for (int m = num - right; m < num; m++)
			{
				for (int n = 0; n < num2; n++)
				{
					virtualMap[m, n] = this.Tiles[this.TilesX - 1, Calc.Clamp(n - up, 0, this.TilesY - 1)];
				}
			}
			for (int num5 = 0; num5 < up; num5++)
			{
				for (int num6 = 0; num6 < num; num6++)
				{
					virtualMap[num6, num5] = this.Tiles[Calc.Clamp(num6 - left, 0, this.TilesX - 1), 0];
				}
			}
			for (int num7 = num2 - down; num7 < num2; num7++)
			{
				for (int num8 = 0; num8 < num; num8++)
				{
					virtualMap[num8, num7] = this.Tiles[Calc.Clamp(num8 - left, 0, this.TilesX - 1), this.TilesY - 1];
				}
			}
			this.Tiles = virtualMap;
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0000B2B8 File Offset: 0x000094B8
		public void FillRect(int x, int y, int columns, int rows, MTexture tile)
		{
			int num = Math.Max(0, x);
			int num2 = Math.Max(0, y);
			int num3 = Math.Min(this.TilesX, x + columns);
			int num4 = Math.Min(this.TilesY, y + rows);
			for (int i = num; i < num3; i++)
			{
				for (int j = num2; j < num4; j++)
				{
					this.Tiles[i, j] = tile;
				}
			}
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0000B320 File Offset: 0x00009520
		public void Clear()
		{
			for (int i = 0; i < this.TilesX; i++)
			{
				for (int j = 0; j < this.TilesY; j++)
				{
					this.Tiles[i, j] = null;
				}
			}
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0000B360 File Offset: 0x00009560
		public Rectangle GetClippedRenderTiles()
		{
			Vector2 vector = base.Entity.Position + this.Position;
			int num;
			int num2;
			int num3;
			int num4;
			if (this.ClipCamera == null)
			{
				num = -this.VisualExtend;
				num2 = -this.VisualExtend;
				num3 = this.TilesX + this.VisualExtend;
				num4 = this.TilesY + this.VisualExtend;
			}
			else
			{
				Camera clipCamera = this.ClipCamera;
				num = (int)Math.Max(0.0, Math.Floor((double)((clipCamera.Left - vector.X) / (float)this.TileWidth)) - (double)this.VisualExtend);
				num2 = (int)Math.Max(0.0, Math.Floor((double)((clipCamera.Top - vector.Y) / (float)this.TileHeight)) - (double)this.VisualExtend);
				num3 = (int)Math.Min((double)this.TilesX, Math.Ceiling((double)((clipCamera.Right - vector.X) / (float)this.TileWidth)) + (double)this.VisualExtend);
				num4 = (int)Math.Min((double)this.TilesY, Math.Ceiling((double)((clipCamera.Bottom - vector.Y) / (float)this.TileHeight)) + (double)this.VisualExtend);
			}
			num = Math.Max(num, 0);
			num2 = Math.Max(num2, 0);
			num3 = Math.Min(num3, this.TilesX);
			num4 = Math.Min(num4, this.TilesY);
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0000B4CD File Offset: 0x000096CD
		public override void Render()
		{
			this.RenderAt(base.Entity.Position + this.Position);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0000B4EC File Offset: 0x000096EC
		public void RenderAt(Vector2 position)
		{
			if (this.Alpha <= 0f)
			{
				return;
			}
			Rectangle clippedRenderTiles = this.GetClippedRenderTiles();
			Color color = this.Color * this.Alpha;
			for (int i = clippedRenderTiles.Left; i < clippedRenderTiles.Right; i++)
			{
				for (int j = clippedRenderTiles.Top; j < clippedRenderTiles.Bottom; j++)
				{
					MTexture mtexture = this.Tiles[i, j];
					if (mtexture != null)
					{
						mtexture.Draw(position + new Vector2((float)(i * this.TileWidth), (float)(j * this.TileHeight)), Vector2.Zero, color);
					}
				}
			}
		}

		// Token: 0x040004F5 RID: 1269
		public Vector2 Position;

		// Token: 0x040004F6 RID: 1270
		public Color Color = Color.White;

		// Token: 0x040004F7 RID: 1271
		public int VisualExtend;

		// Token: 0x040004F8 RID: 1272
		public VirtualMap<MTexture> Tiles;

		// Token: 0x040004F9 RID: 1273
		public Camera ClipCamera;

		// Token: 0x040004FA RID: 1274
		public float Alpha = 1f;
	}
}

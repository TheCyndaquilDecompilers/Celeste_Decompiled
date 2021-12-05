using System;

namespace Monocle
{
	// Token: 0x0200010C RID: 268
	public class Tileset
	{
		// Token: 0x06000848 RID: 2120 RVA: 0x000123F4 File Offset: 0x000105F4
		public Tileset(MTexture texture, int tileWidth, int tileHeight)
		{
			this.Texture = texture;
			this.TileWidth = tileWidth;
			this.TileHeight = this.TileHeight;
			this.tiles = new MTexture[this.Texture.Width / tileWidth, this.Texture.Height / tileHeight];
			for (int i = 0; i < this.Texture.Width / tileWidth; i++)
			{
				for (int j = 0; j < this.Texture.Height / tileHeight; j++)
				{
					this.tiles[i, j] = new MTexture(this.Texture, i * tileWidth, j * tileHeight, tileWidth, tileHeight);
				}
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x00012496 File Offset: 0x00010696
		// (set) Token: 0x0600084A RID: 2122 RVA: 0x0001249E File Offset: 0x0001069E
		public MTexture Texture { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600084B RID: 2123 RVA: 0x000124A7 File Offset: 0x000106A7
		// (set) Token: 0x0600084C RID: 2124 RVA: 0x000124AF File Offset: 0x000106AF
		public int TileWidth { get; private set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600084D RID: 2125 RVA: 0x000124B8 File Offset: 0x000106B8
		// (set) Token: 0x0600084E RID: 2126 RVA: 0x000124C0 File Offset: 0x000106C0
		public int TileHeight { get; private set; }

		// Token: 0x170000BD RID: 189
		public MTexture this[int x, int y]
		{
			get
			{
				return this.tiles[x, y];
			}
		}

		// Token: 0x170000BE RID: 190
		public MTexture this[int index]
		{
			get
			{
				if (index < 0)
				{
					return null;
				}
				return this.tiles[index % this.tiles.GetLength(0), index / this.tiles.GetLength(0)];
			}
		}

		// Token: 0x04000585 RID: 1413
		private MTexture[,] tiles;
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000380 RID: 896
	[Tracked(false)]
	public class SolidTiles : Solid
	{
		// Token: 0x06001D5B RID: 7515 RVA: 0x000CC5F4 File Offset: 0x000CA7F4
		public SolidTiles(Vector2 position, VirtualMap<char> data) : base(position, 0f, 0f, true)
		{
			base.Tag = Tags.Global;
			base.Depth = -10000;
			this.tileTypes = data;
			this.EnableAssistModeChecks = false;
			this.AllowStaticMovers = false;
			base.Collider = (this.Grid = new Grid(data.Columns, data.Rows, 8f, 8f));
			for (int i = 0; i < data.Columns; i += 50)
			{
				for (int j = 0; j < data.Rows; j += 50)
				{
					if (data.AnyInSegmentAtTile(i, j))
					{
						int k = i;
						int num = Math.Min(k + 50, data.Columns);
						while (k < num)
						{
							int l = j;
							int num2 = Math.Min(l + 50, data.Rows);
							while (l < num2)
							{
								if (data[k, l] != '0')
								{
									this.Grid[k, l] = true;
								}
								l++;
							}
							k++;
						}
					}
				}
			}
			Autotiler.Generated generated = GFX.FGAutotiler.GenerateMap(data, true);
			this.Tiles = generated.TileGrid;
			this.Tiles.VisualExtend = 1;
			base.Add(this.Tiles);
			base.Add(this.AnimatedTiles = generated.SpriteOverlay);
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x000CC74D File Offset: 0x000CA94D
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Tiles.ClipCamera = base.SceneAs<Level>().Camera;
			this.AnimatedTiles.ClipCamera = this.Tiles.ClipCamera;
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x000CC784 File Offset: 0x000CA984
		private int CoreTileSurfaceIndex()
		{
			Level level = base.Scene as Level;
			if (level.CoreMode == Session.CoreModes.Hot)
			{
				return 37;
			}
			if (level.CoreMode == Session.CoreModes.Cold)
			{
				return 36;
			}
			return 3;
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x000CC7B8 File Offset: 0x000CA9B8
		private int SurfaceSoundIndexAt(Vector2 readPosition)
		{
			int num = (int)((readPosition.X - base.X) / 8f);
			int num2 = (int)((readPosition.Y - base.Y) / 8f);
			if (num >= 0 && num2 >= 0 && num < this.Grid.CellsX && num2 < this.Grid.CellsY)
			{
				char c = this.tileTypes[num, num2];
				if (c == 'k')
				{
					return this.CoreTileSurfaceIndex();
				}
				if (c != '0' && SurfaceIndex.TileToIndex.ContainsKey(c))
				{
					return SurfaceIndex.TileToIndex[c];
				}
			}
			return -1;
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x000CC84C File Offset: 0x000CAA4C
		public override int GetWallSoundIndex(Player player, int side)
		{
			int num = this.SurfaceSoundIndexAt(player.Center + Vector2.UnitX * (float)side * 8f);
			if (num < 0)
			{
				num = this.SurfaceSoundIndexAt(player.Center + new Vector2((float)(side * 8), -6f));
			}
			if (num < 0)
			{
				num = this.SurfaceSoundIndexAt(player.Center + new Vector2((float)(side * 8), 6f));
			}
			return num;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x000CC8CC File Offset: 0x000CAACC
		public override int GetStepSoundIndex(Entity entity)
		{
			int num = this.SurfaceSoundIndexAt(entity.BottomCenter + Vector2.UnitY * 4f);
			if (num == -1)
			{
				num = this.SurfaceSoundIndexAt(entity.BottomLeft + Vector2.UnitY * 4f);
			}
			if (num == -1)
			{
				num = this.SurfaceSoundIndexAt(entity.BottomRight + Vector2.UnitY * 4f);
			}
			return num;
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x000CC948 File Offset: 0x000CAB48
		public override int GetLandSoundIndex(Entity entity)
		{
			int num = this.SurfaceSoundIndexAt(entity.BottomCenter + Vector2.UnitY * 4f);
			if (num == -1)
			{
				num = this.SurfaceSoundIndexAt(entity.BottomLeft + Vector2.UnitY * 4f);
			}
			if (num == -1)
			{
				num = this.SurfaceSoundIndexAt(entity.BottomRight + Vector2.UnitY * 4f);
			}
			return num;
		}

		// Token: 0x04001AED RID: 6893
		public TileGrid Tiles;

		// Token: 0x04001AEE RID: 6894
		public AnimatedTiles AnimatedTiles;

		// Token: 0x04001AEF RID: 6895
		public Grid Grid;

		// Token: 0x04001AF0 RID: 6896
		private VirtualMap<char> tileTypes;
	}
}

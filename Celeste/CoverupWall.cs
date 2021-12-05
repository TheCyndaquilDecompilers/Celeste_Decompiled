using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000265 RID: 613
	[Tracked(false)]
	public class CoverupWall : Entity
	{
		// Token: 0x06001328 RID: 4904 RVA: 0x00067F60 File Offset: 0x00066160
		public CoverupWall(Vector2 position, char tile, float width, float height) : base(position)
		{
			this.fillTile = tile;
			base.Depth = -13000;
			base.Collider = new Hitbox(width, height, 0f, 0f);
			base.Add(this.cutout = new EffectCutout());
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x00067FB2 File Offset: 0x000661B2
		public CoverupWall(EntityData data, Vector2 offset) : this(data.Position + offset, data.Char("tiletype", '3'), (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x00067FE4 File Offset: 0x000661E4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			int tilesX = (int)base.Width / 8;
			int tilesY = (int)base.Height / 8;
			Level level = base.SceneAs<Level>();
			Rectangle tileBounds = level.Session.MapData.TileBounds;
			VirtualMap<char> solidsData = level.SolidsData;
			int x = (int)base.X / 8 - tileBounds.Left;
			int y = (int)base.Y / 8 - tileBounds.Top;
			base.Add(this.tiles = GFX.FGAutotiler.GenerateOverlay(this.fillTile, x, y, tilesX, tilesY, solidsData).TileGrid);
			base.Add(new TileInterceptor(this.tiles, false));
		}

		// Token: 0x04000F16 RID: 3862
		private char fillTile;

		// Token: 0x04000F17 RID: 3863
		private TileGrid tiles;

		// Token: 0x04000F18 RID: 3864
		private EffectCutout cutout;
	}
}

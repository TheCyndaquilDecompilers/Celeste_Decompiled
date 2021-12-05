using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000359 RID: 857
	public class BackgroundTiles : Entity
	{
		// Token: 0x06001AF4 RID: 6900 RVA: 0x000AF184 File Offset: 0x000AD384
		public BackgroundTiles(Vector2 position, VirtualMap<char> data)
		{
			this.Position = position;
			base.Tag = Tags.Global;
			this.Tiles = GFX.BGAutotiler.GenerateMap(data, false).TileGrid;
			this.Tiles.VisualExtend = 1;
			base.Add(this.Tiles);
			base.Depth = 10000;
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x000AF1E8 File Offset: 0x000AD3E8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.Tiles.ClipCamera = base.SceneAs<Level>().Camera;
		}

		// Token: 0x040017AF RID: 6063
		public TileGrid Tiles;
	}
}

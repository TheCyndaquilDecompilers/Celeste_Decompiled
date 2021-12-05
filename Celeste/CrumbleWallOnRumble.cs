using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200014A RID: 330
	[Tracked(false)]
	public class CrumbleWallOnRumble : Solid
	{
		// Token: 0x06000C07 RID: 3079 RVA: 0x00025550 File Offset: 0x00023750
		public CrumbleWallOnRumble(Vector2 position, char tileType, float width, float height, bool blendIn, bool persistent, EntityID id) : base(position, width, height, true)
		{
			base.Depth = -12999;
			this.id = id;
			this.tileType = tileType;
			this.blendIn = blendIn;
			this.permanent = persistent;
			this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[this.tileType];
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x000255A8 File Offset: 0x000237A8
		public CrumbleWallOnRumble(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, data.Char("tiletype", 'm'), (float)data.Width, (float)data.Height, data.Bool("blendin", false), data.Bool("persistent", false), id)
		{
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x000255FC File Offset: 0x000237FC
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			TileGrid tileGrid;
			if (!this.blendIn)
			{
				tileGrid = GFX.FGAutotiler.GenerateBox(this.tileType, (int)base.Width / 8, (int)base.Height / 8).TileGrid;
			}
			else
			{
				Level level = base.SceneAs<Level>();
				Rectangle tileBounds = level.Session.MapData.TileBounds;
				VirtualMap<char> solidsData = level.SolidsData;
				int x = (int)(base.X / 8f) - tileBounds.Left;
				int y = (int)(base.Y / 8f) - tileBounds.Top;
				int tilesX = (int)base.Width / 8;
				int tilesY = (int)base.Height / 8;
				tileGrid = GFX.FGAutotiler.GenerateOverlay(this.tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
				base.Depth = -10501;
			}
			base.Add(tileGrid);
			base.Add(new TileInterceptor(tileGrid, true));
			base.Add(new LightOcclude(1f));
			if (base.CollideCheck<Player>())
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x00025700 File Offset: 0x00023900
		public void Break()
		{
			if (this.Collidable && base.Scene != null)
			{
				Audio.Play("event:/new_content/game/10_farewell/quake_rockbreak", this.Position);
				this.Collidable = false;
				int num = 0;
				while ((float)num < base.Width / 8f)
				{
					int num2 = 0;
					while ((float)num2 < base.Height / 8f)
					{
						if (!base.Scene.CollideCheck<Solid>(new Rectangle((int)base.X + num * 8, (int)base.Y + num2 * 8, 8, 8)))
						{
							base.Scene.Add(Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float)(4 + num * 8), (float)(4 + num2 * 8)), this.tileType, true).BlastFrom(base.TopCenter));
						}
						num2++;
					}
					num++;
				}
				if (this.permanent)
				{
					base.SceneAs<Level>().Session.DoNotLoad.Add(this.id);
				}
				base.RemoveSelf();
			}
		}

		// Token: 0x04000756 RID: 1878
		private bool permanent;

		// Token: 0x04000757 RID: 1879
		private EntityID id;

		// Token: 0x04000758 RID: 1880
		private char tileType;

		// Token: 0x04000759 RID: 1881
		private bool blendIn;
	}
}

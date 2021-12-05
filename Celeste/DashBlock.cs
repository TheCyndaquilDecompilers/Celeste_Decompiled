using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200032D RID: 813
	[Tracked(false)]
	public class DashBlock : Solid
	{
		// Token: 0x06001982 RID: 6530 RVA: 0x000A4538 File Offset: 0x000A2738
		public DashBlock(Vector2 position, char tiletype, float width, float height, bool blendIn, bool permanent, bool canDash, EntityID id) : base(position, width, height, true)
		{
			base.Depth = -12999;
			this.id = id;
			this.permanent = permanent;
			this.width = width;
			this.height = height;
			this.blendIn = blendIn;
			this.canDash = canDash;
			this.tileType = tiletype;
			this.OnDashCollide = new DashCollision(this.OnDashed);
			this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[this.tileType];
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x000A45BC File Offset: 0x000A27BC
		public DashBlock(EntityData data, Vector2 offset, EntityID id) : this(data.Position + offset, data.Char("tiletype", '3'), (float)data.Width, (float)data.Height, data.Bool("blendin", false), data.Bool("permanent", true), data.Bool("canDash", true), id)
		{
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x000A461C File Offset: 0x000A281C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			TileGrid tileGrid;
			if (!this.blendIn)
			{
				tileGrid = GFX.FGAutotiler.GenerateBox(this.tileType, (int)this.width / 8, (int)this.height / 8).TileGrid;
				base.Add(new LightOcclude(1f));
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
				base.Add(new EffectCutout());
				base.Depth = -10501;
			}
			base.Add(tileGrid);
			base.Add(new TileInterceptor(tileGrid, true));
			if (base.CollideCheck<Player>())
			{
				base.RemoveSelf();
			}
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x000A472B File Offset: 0x000A292B
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			Celeste.Freeze(0.05f);
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x000A4740 File Offset: 0x000A2940
		public void Break(Vector2 from, Vector2 direction, bool playSound = true, bool playDebrisSound = true)
		{
			if (playSound)
			{
				if (this.tileType == '1')
				{
					Audio.Play("event:/game/general/wall_break_dirt", this.Position);
				}
				else if (this.tileType == '3')
				{
					Audio.Play("event:/game/general/wall_break_ice", this.Position);
				}
				else if (this.tileType == '9')
				{
					Audio.Play("event:/game/general/wall_break_wood", this.Position);
				}
				else
				{
					Audio.Play("event:/game/general/wall_break_stone", this.Position);
				}
			}
			int num = 0;
			while ((float)num < base.Width / 8f)
			{
				int num2 = 0;
				while ((float)num2 < base.Height / 8f)
				{
					base.Scene.Add(Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float)(4 + num * 8), (float)(4 + num2 * 8)), this.tileType, playDebrisSound).BlastFrom(from));
					num2++;
				}
				num++;
			}
			this.Collidable = false;
			if (this.permanent)
			{
				this.RemoveAndFlagAsGone();
				return;
			}
			base.RemoveSelf();
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x000A4848 File Offset: 0x000A2A48
		public void RemoveAndFlagAsGone()
		{
			base.RemoveSelf();
			base.SceneAs<Level>().Session.DoNotLoad.Add(this.id);
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x000A486C File Offset: 0x000A2A6C
		private DashCollisionResults OnDashed(Player player, Vector2 direction)
		{
			if (!this.canDash && player.StateMachine.State != 5 && player.StateMachine.State != 10)
			{
				return DashCollisionResults.NormalCollision;
			}
			this.Break(player.Center, direction, true, true);
			return DashCollisionResults.Rebound;
		}

		// Token: 0x0400163E RID: 5694
		private bool permanent;

		// Token: 0x0400163F RID: 5695
		private EntityID id;

		// Token: 0x04001640 RID: 5696
		private char tileType;

		// Token: 0x04001641 RID: 5697
		private float width;

		// Token: 0x04001642 RID: 5698
		private float height;

		// Token: 0x04001643 RID: 5699
		private bool blendIn;

		// Token: 0x04001644 RID: 5700
		private bool canDash;

		// Token: 0x020006E0 RID: 1760
		public enum Modes
		{
			// Token: 0x04002CA1 RID: 11425
			Dash,
			// Token: 0x04002CA2 RID: 11426
			FinalBoss,
			// Token: 0x04002CA3 RID: 11427
			Crusher
		}
	}
}

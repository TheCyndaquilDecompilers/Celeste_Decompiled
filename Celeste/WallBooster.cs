using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001AE RID: 430
	[Tracked(false)]
	public class WallBooster : Entity
	{
		// Token: 0x06000F06 RID: 3846 RVA: 0x0003B260 File Offset: 0x00039460
		public WallBooster(Vector2 position, float height, bool left, bool notCoreMode) : base(position)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Depth = 1999;
			this.notCoreMode = notCoreMode;
			if (left)
			{
				this.Facing = Facings.Left;
				base.Collider = new Hitbox(2f, height, 0f, 0f);
			}
			else
			{
				this.Facing = Facings.Right;
				base.Collider = new Hitbox(2f, height, 6f, 0f);
			}
			base.Add(new CoreModeListener(new Action<Session.CoreModes>(this.OnChangeMode)));
			base.Add(this.staticMover = new StaticMover());
			base.Add(this.climbBlocker = new ClimbBlocker(false));
			base.Add(this.idleSfx = new SoundSource());
			this.tiles = this.BuildSprite(left);
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0003B341 File Offset: 0x00039541
		public WallBooster(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Height, data.Bool("left", false), data.Bool("notCoreMode", false))
		{
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0003B374 File Offset: 0x00039574
		private List<Sprite> BuildSprite(bool left)
		{
			List<Sprite> list = new List<Sprite>();
			int num = 0;
			while ((float)num < base.Height)
			{
				string id;
				if (num == 0)
				{
					id = "WallBoosterTop";
				}
				else if ((float)(num + 16) > base.Height)
				{
					id = "WallBoosterBottom";
				}
				else
				{
					id = "WallBoosterMid";
				}
				Sprite sprite = GFX.SpriteBank.Create(id);
				if (!left)
				{
					sprite.FlipX = true;
					sprite.Position = new Vector2(4f, (float)num);
				}
				else
				{
					sprite.Position = new Vector2(0f, (float)num);
				}
				list.Add(sprite);
				base.Add(sprite);
				num += 8;
			}
			return list;
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0003B40C File Offset: 0x0003960C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Session.CoreModes mode = Session.CoreModes.None;
			if (base.SceneAs<Level>().CoreMode == Session.CoreModes.Cold || this.notCoreMode)
			{
				mode = Session.CoreModes.Cold;
			}
			this.OnChangeMode(mode);
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x0003B444 File Offset: 0x00039644
		private void OnChangeMode(Session.CoreModes mode)
		{
			this.IceMode = (mode == Session.CoreModes.Cold);
			this.climbBlocker.Blocking = this.IceMode;
			this.tiles.ForEach(delegate(Sprite t)
			{
				t.Play(this.IceMode ? "ice" : "hot", false, false);
			});
			if (this.IceMode)
			{
				this.idleSfx.Stop(true);
				return;
			}
			if (!this.idleSfx.Playing)
			{
				this.idleSfx.Play("event:/env/local/09_core/conveyor_idle", null, 0f);
			}
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x0003B4BD File Offset: 0x000396BD
		public override void Update()
		{
			this.PositionIdleSfx();
			if ((base.Scene as Level).Transitioning)
			{
				return;
			}
			base.Update();
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x0003B4E0 File Offset: 0x000396E0
		private void PositionIdleSfx()
		{
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.idleSfx.Position = Calc.ClosestPointOnLine(this.Position, this.Position + new Vector2(0f, base.Height), entity.Center) - this.Position;
				this.idleSfx.UpdateSfxPosition();
			}
		}

		// Token: 0x04000A57 RID: 2647
		public Facings Facing;

		// Token: 0x04000A58 RID: 2648
		private StaticMover staticMover;

		// Token: 0x04000A59 RID: 2649
		private ClimbBlocker climbBlocker;

		// Token: 0x04000A5A RID: 2650
		private SoundSource idleSfx;

		// Token: 0x04000A5B RID: 2651
		public bool IceMode;

		// Token: 0x04000A5C RID: 2652
		private bool notCoreMode;

		// Token: 0x04000A5D RID: 2653
		private List<Sprite> tiles;
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200033F RID: 831
	[Tracked(false)]
	public class FakeWall : Entity
	{
		// Token: 0x06001A1D RID: 6685 RVA: 0x000A7FF4 File Offset: 0x000A61F4
		public FakeWall(EntityID eid, Vector2 position, char tile, float width, float height, FakeWall.Modes mode) : base(position)
		{
			this.mode = mode;
			this.eid = eid;
			this.fillTile = tile;
			base.Collider = new Hitbox(width, height, 0f, 0f);
			base.Depth = -13000;
			base.Add(this.cutout = new EffectCutout());
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x000A8058 File Offset: 0x000A6258
		public FakeWall(EntityID eid, EntityData data, Vector2 offset, FakeWall.Modes mode) : this(eid, data.Position + offset, data.Char("tiletype", '3'), (float)data.Width, (float)data.Height, mode)
		{
			this.playRevealWhenTransitionedInto = data.Bool("playTransitionReveal", false);
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x000A80A8 File Offset: 0x000A62A8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			int tilesX = (int)base.Width / 8;
			int tilesY = (int)base.Height / 8;
			if (this.mode == FakeWall.Modes.Wall)
			{
				Level level = base.SceneAs<Level>();
				Rectangle tileBounds = level.Session.MapData.TileBounds;
				VirtualMap<char> solidsData = level.SolidsData;
				int x = (int)base.X / 8 - tileBounds.Left;
				int y = (int)base.Y / 8 - tileBounds.Top;
				this.tiles = GFX.FGAutotiler.GenerateOverlay(this.fillTile, x, y, tilesX, tilesY, solidsData).TileGrid;
			}
			else if (this.mode == FakeWall.Modes.Block)
			{
				this.tiles = GFX.FGAutotiler.GenerateBox(this.fillTile, tilesX, tilesY).TileGrid;
			}
			base.Add(this.tiles);
			base.Add(new TileInterceptor(this.tiles, false));
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x000A8184 File Offset: 0x000A6384
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (base.CollideCheck<Player>())
			{
				this.tiles.Alpha = 0f;
				this.fade = true;
				this.cutout.Visible = false;
				if (this.playRevealWhenTransitionedInto)
				{
					Audio.Play("event:/game/general/secret_revealed", base.Center);
				}
				base.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
				return;
			}
			base.Add(new TransitionListener
			{
				OnOut = new Action<float>(this.OnTransitionOut),
				OnOutBegin = new Action(this.OnTransitionOutBegin),
				OnIn = new Action<float>(this.OnTransitionIn),
				OnInBegin = new Action(this.OnTransitionInBegin)
			});
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x000A824E File Offset: 0x000A644E
		private void OnTransitionOutBegin()
		{
			if (Collide.CheckRect(this, base.SceneAs<Level>().Bounds))
			{
				this.transitionFade = true;
				this.transitionStartAlpha = this.tiles.Alpha;
				return;
			}
			this.transitionFade = false;
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x000A8283 File Offset: 0x000A6483
		private void OnTransitionOut(float percent)
		{
			if (this.transitionFade)
			{
				this.tiles.Alpha = this.transitionStartAlpha * (1f - percent);
			}
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x000A82A8 File Offset: 0x000A64A8
		private void OnTransitionInBegin()
		{
			Level level = base.SceneAs<Level>();
			if (level.PreviousBounds != null && Collide.CheckRect(this, level.PreviousBounds.Value))
			{
				this.transitionFade = true;
				this.tiles.Alpha = 0f;
				return;
			}
			this.transitionFade = false;
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x000A8301 File Offset: 0x000A6501
		private void OnTransitionIn(float percent)
		{
			if (this.transitionFade)
			{
				this.tiles.Alpha = percent;
			}
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x000A8318 File Offset: 0x000A6518
		public override void Update()
		{
			base.Update();
			if (this.fade)
			{
				this.tiles.Alpha = Calc.Approach(this.tiles.Alpha, 0f, 2f * Engine.DeltaTime);
				this.cutout.Alpha = this.tiles.Alpha;
				if (this.tiles.Alpha <= 0f)
				{
					base.RemoveSelf();
					return;
				}
			}
			else
			{
				Player player = base.CollideFirst<Player>();
				if (player != null && player.StateMachine.State != 9)
				{
					base.SceneAs<Level>().Session.DoNotLoad.Add(this.eid);
					this.fade = true;
					Audio.Play("event:/game/general/secret_revealed", base.Center);
				}
			}
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x000A83DC File Offset: 0x000A65DC
		public override void Render()
		{
			if (this.mode == FakeWall.Modes.Wall)
			{
				Level level = base.Scene as Level;
				if (level.ShakeVector.X < 0f && level.Camera.X <= (float)level.Bounds.Left && base.X <= (float)level.Bounds.Left)
				{
					this.tiles.RenderAt(this.Position + new Vector2(-3f, 0f));
				}
				if (level.ShakeVector.X > 0f && level.Camera.X + 320f >= (float)level.Bounds.Right && base.X + base.Width >= (float)level.Bounds.Right)
				{
					this.tiles.RenderAt(this.Position + new Vector2(3f, 0f));
				}
				if (level.ShakeVector.Y < 0f && level.Camera.Y <= (float)level.Bounds.Top && base.Y <= (float)level.Bounds.Top)
				{
					this.tiles.RenderAt(this.Position + new Vector2(0f, -3f));
				}
				if (level.ShakeVector.Y > 0f && level.Camera.Y + 180f >= (float)level.Bounds.Bottom && base.Y + base.Height >= (float)level.Bounds.Bottom)
				{
					this.tiles.RenderAt(this.Position + new Vector2(0f, 3f));
				}
			}
			base.Render();
		}

		// Token: 0x040016C3 RID: 5827
		private FakeWall.Modes mode;

		// Token: 0x040016C4 RID: 5828
		private char fillTile;

		// Token: 0x040016C5 RID: 5829
		private TileGrid tiles;

		// Token: 0x040016C6 RID: 5830
		private bool fade;

		// Token: 0x040016C7 RID: 5831
		private EffectCutout cutout;

		// Token: 0x040016C8 RID: 5832
		private float transitionStartAlpha;

		// Token: 0x040016C9 RID: 5833
		private bool transitionFade;

		// Token: 0x040016CA RID: 5834
		private EntityID eid;

		// Token: 0x040016CB RID: 5835
		private bool playRevealWhenTransitionedInto;

		// Token: 0x020006EF RID: 1775
		public enum Modes
		{
			// Token: 0x04002CEC RID: 11500
			Wall,
			// Token: 0x04002CED RID: 11501
			Block
		}
	}
}

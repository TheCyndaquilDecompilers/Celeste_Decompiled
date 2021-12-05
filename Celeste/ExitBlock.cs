using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000323 RID: 803
	[Tracked(false)]
	public class ExitBlock : Solid
	{
		// Token: 0x0600194E RID: 6478 RVA: 0x000A25B0 File Offset: 0x000A07B0
		public ExitBlock(Vector2 position, float width, float height, char tileType) : base(position, width, height, true)
		{
			base.Depth = -13000;
			this.tileType = tileType;
			this.tl = new TransitionListener();
			this.tl.OnOutBegin = new Action(this.OnTransitionOutBegin);
			this.tl.OnInBegin = new Action(this.OnTransitionInBegin);
			base.Add(this.tl);
			base.Add(this.cutout = new EffectCutout());
			this.SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
			this.EnableAssistModeChecks = false;
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x000A264C File Offset: 0x000A084C
		public ExitBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Char("tileType", '3'))
		{
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x000A267B File Offset: 0x000A087B
		private void OnTransitionOutBegin()
		{
			if (Collide.CheckRect(this, base.SceneAs<Level>().Bounds))
			{
				this.tl.OnOut = new Action<float>(this.OnTransitionOut);
				this.startAlpha = this.tiles.Alpha;
			}
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x000A26B8 File Offset: 0x000A08B8
		private void OnTransitionOut(float percent)
		{
			this.cutout.Alpha = (this.tiles.Alpha = MathHelper.Lerp(this.startAlpha, 0f, percent));
			this.cutout.Update();
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x000A26FC File Offset: 0x000A08FC
		private void OnTransitionInBegin()
		{
			if (Collide.CheckRect(this, base.SceneAs<Level>().PreviousBounds.Value) && !base.CollideCheck<Player>())
			{
				this.cutout.Alpha = 0f;
				this.tiles.Alpha = 0f;
				this.tl.OnIn = new Action<float>(this.OnTransitionIn);
			}
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x000A2764 File Offset: 0x000A0964
		private void OnTransitionIn(float percent)
		{
			EffectCutout effectCutout = this.cutout;
			this.tiles.Alpha = percent;
			effectCutout.Alpha = percent;
			this.cutout.Update();
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x000A2798 File Offset: 0x000A0998
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Level level = base.SceneAs<Level>();
			Rectangle tileBounds = level.Session.MapData.TileBounds;
			VirtualMap<char> solidsData = level.SolidsData;
			int x = (int)(base.X / 8f) - tileBounds.Left;
			int y = (int)(base.Y / 8f) - tileBounds.Top;
			int tilesX = (int)base.Width / 8;
			int tilesY = (int)base.Height / 8;
			this.tiles = GFX.FGAutotiler.GenerateOverlay(this.tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
			base.Add(this.tiles);
			base.Add(new TileInterceptor(this.tiles, false));
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x000A284C File Offset: 0x000A0A4C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (base.CollideCheck<Player>())
			{
				this.cutout.Alpha = (this.tiles.Alpha = 0f);
				this.Collidable = false;
			}
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x000A2890 File Offset: 0x000A0A90
		public override void Update()
		{
			base.Update();
			if (this.Collidable)
			{
				this.cutout.Alpha = (this.tiles.Alpha = Calc.Approach(this.tiles.Alpha, 1f, Engine.DeltaTime));
				return;
			}
			if (!base.CollideCheck<Player>())
			{
				this.Collidable = true;
				Audio.Play("event:/game/general/passage_closed_behind", base.Center);
			}
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x000A2900 File Offset: 0x000A0B00
		public override void Render()
		{
			if (this.tiles.Alpha >= 1f)
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

		// Token: 0x0400160D RID: 5645
		private TileGrid tiles;

		// Token: 0x0400160E RID: 5646
		private TransitionListener tl;

		// Token: 0x0400160F RID: 5647
		private EffectCutout cutout;

		// Token: 0x04001610 RID: 5648
		private float startAlpha;

		// Token: 0x04001611 RID: 5649
		private char tileType;
	}
}

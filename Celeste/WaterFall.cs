using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000293 RID: 659
	[Tracked(false)]
	public class WaterFall : Entity
	{
		// Token: 0x0600146B RID: 5227 RVA: 0x0006F4DE File Offset: 0x0006D6DE
		public WaterFall(Vector2 position) : base(position)
		{
			base.Depth = -9999;
			base.Tag = Tags.TransitionUpdate;
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x0006F502 File Offset: 0x0006D702
		public WaterFall(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x0006F518 File Offset: 0x0006D718
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = base.Scene as Level;
			bool flag = false;
			this.height = 8f;
			while (base.Y + this.height < (float)level.Bounds.Bottom && (this.water = base.Scene.CollideFirst<Water>(new Rectangle((int)base.X, (int)(base.Y + this.height), 8, 8))) == null && ((this.solid = base.Scene.CollideFirst<Solid>(new Rectangle((int)base.X, (int)(base.Y + this.height), 8, 8))) == null || !this.solid.BlockWaterfalls))
			{
				this.height += 8f;
				this.solid = null;
			}
			if (this.water != null && !base.Scene.CollideCheck<Solid>(new Rectangle((int)base.X, (int)(base.Y + this.height), 8, 16)))
			{
				flag = true;
			}
			base.Add(this.loopingSfx = new SoundSource());
			this.loopingSfx.Play("event:/env/local/waterfall_small_main", null, 0f);
			base.Add(this.enteringSfx = new SoundSource());
			this.enteringSfx.Play(flag ? "event:/env/local/waterfall_small_in_deep" : "event:/env/local/waterfall_small_in_shallow", null, 0f);
			this.enteringSfx.Position.Y = this.height;
			base.Add(new DisplacementRenderHook(new Action(this.RenderDisplacement)));
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0006F6BC File Offset: 0x0006D8BC
		public override void Update()
		{
			Vector2 position = (base.Scene as Level).Camera.Position;
			this.loopingSfx.Position.Y = Calc.Clamp(position.Y + 90f, base.Y, this.height);
			if (this.water != null && base.Scene.OnInterval(0.3f))
			{
				this.water.TopSurface.DoRipple(new Vector2(base.X + 4f, this.water.Y), 0.75f);
			}
			if (this.water != null || this.solid != null)
			{
				Vector2 position2 = new Vector2(base.X + 4f, base.Y + this.height + 2f);
				(base.Scene as Level).ParticlesFG.Emit(Water.P_Splash, 1, position2, new Vector2(8f, 2f), new Vector2(0f, -1f).Angle());
			}
			base.Update();
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x0006F7D2 File Offset: 0x0006D9D2
		public void RenderDisplacement()
		{
			Draw.Rect(base.X, base.Y, 8f, this.height, new Color(0.5f, 0.5f, 0.8f, 1f));
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x0006F80C File Offset: 0x0006DA0C
		public override void Render()
		{
			if (this.water == null || this.water.TopSurface == null)
			{
				Draw.Rect(base.X + 1f, base.Y, 6f, this.height, Water.FillColor);
				Draw.Rect(base.X - 1f, base.Y, 2f, this.height, Water.SurfaceColor);
				Draw.Rect(base.X + 7f, base.Y, 2f, this.height, Water.SurfaceColor);
				return;
			}
			Water.Surface topSurface = this.water.TopSurface;
			float num = this.height + this.water.TopSurface.Position.Y - this.water.Y;
			for (int i = 0; i < 6; i++)
			{
				Draw.Rect(base.X + (float)i + 1f, base.Y, 1f, num - topSurface.GetSurfaceHeight(new Vector2(base.X + 1f + (float)i, this.water.Y)), Water.FillColor);
			}
			Draw.Rect(base.X - 1f, base.Y, 2f, num - topSurface.GetSurfaceHeight(new Vector2(base.X, this.water.Y)), Water.SurfaceColor);
			Draw.Rect(base.X + 7f, base.Y, 2f, num - topSurface.GetSurfaceHeight(new Vector2(base.X + 8f, this.water.Y)), Water.SurfaceColor);
		}

		// Token: 0x04001020 RID: 4128
		private float height;

		// Token: 0x04001021 RID: 4129
		private Water water;

		// Token: 0x04001022 RID: 4130
		private Solid solid;

		// Token: 0x04001023 RID: 4131
		private SoundSource loopingSfx;

		// Token: 0x04001024 RID: 4132
		private SoundSource enteringSfx;
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000151 RID: 337
	[Tracked(false)]
	public class StrawberryPoints : Entity
	{
		// Token: 0x06000C44 RID: 3140 RVA: 0x00027FA4 File Offset: 0x000261A4
		public StrawberryPoints(Vector2 position, bool ghostberry, int index, bool moonberry) : base(position)
		{
			base.Add(this.sprite = GFX.SpriteBank.Create("strawberry"));
			base.Add(this.light = new VertexLight(Color.White, 1f, 16, 24));
			base.Add(this.bloom = new BloomPoint(1f, 12f));
			base.Depth = -2000100;
			base.Tag = (Tags.Persistent | Tags.TransitionUpdate | Tags.FrozenUpdate);
			this.ghostberry = ghostberry;
			this.moonberry = moonberry;
			this.index = index;
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x00028060 File Offset: 0x00026260
		public override void Added(Scene scene)
		{
			this.index = Math.Min(5, this.index);
			if (this.index >= 5)
			{
				Achievements.Register(Achievement.ONEUP);
			}
			if (this.moonberry)
			{
				this.sprite.Play("fade_wow", false, false);
			}
			else
			{
				this.sprite.Play("fade" + this.index, false, false);
			}
			this.sprite.OnFinish = delegate(string a)
			{
				base.RemoveSelf();
			};
			base.Added(scene);
			foreach (Entity entity in base.Scene.Tracker.GetEntities<StrawberryPoints>())
			{
				if (entity != this && Vector2.DistanceSquared(entity.Position, this.Position) <= 256f)
				{
					entity.RemoveSelf();
				}
			}
			this.burst = (scene as Level).Displacement.AddBurst(this.Position, 0.3f, 16f, 24f, 0.3f, null, null);
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x00028188 File Offset: 0x00026388
		public override void Update()
		{
			Level level = base.Scene as Level;
			if (level.Frozen)
			{
				if (this.burst != null)
				{
					this.burst.AlphaFrom = (this.burst.AlphaTo = 0f);
					this.burst.Percent = this.burst.Duration;
				}
				return;
			}
			base.Update();
			Camera camera = level.Camera;
			base.Y -= 8f * Engine.DeltaTime;
			base.X = Calc.Clamp(base.X, camera.Left + 8f, camera.Right - 8f);
			base.Y = Calc.Clamp(base.Y, camera.Top + 8f, camera.Bottom - 8f);
			this.light.Alpha = Calc.Approach(this.light.Alpha, 0f, Engine.DeltaTime * 4f);
			this.bloom.Alpha = this.light.Alpha;
			ParticleType particleType = this.ghostberry ? Strawberry.P_GhostGlow : Strawberry.P_Glow;
			if (this.moonberry && !this.ghostberry)
			{
				particleType = Strawberry.P_MoonGlow;
			}
			if (base.Scene.OnInterval(0.05f))
			{
				if (this.sprite.Color == particleType.Color2)
				{
					this.sprite.Color = particleType.Color;
				}
				else
				{
					this.sprite.Color = particleType.Color2;
				}
			}
			if (base.Scene.OnInterval(0.06f) && this.sprite.CurrentAnimationFrame > 11)
			{
				level.ParticlesFG.Emit(particleType, 1, this.Position + Vector2.UnitY * -2f, new Vector2(8f, 4f));
			}
		}

		// Token: 0x040007A4 RID: 1956
		private Sprite sprite;

		// Token: 0x040007A5 RID: 1957
		private bool ghostberry;

		// Token: 0x040007A6 RID: 1958
		private bool moonberry;

		// Token: 0x040007A7 RID: 1959
		private VertexLight light;

		// Token: 0x040007A8 RID: 1960
		private BloomPoint bloom;

		// Token: 0x040007A9 RID: 1961
		private int index;

		// Token: 0x040007AA RID: 1962
		private DisplacementRenderer.Burst burst;
	}
}

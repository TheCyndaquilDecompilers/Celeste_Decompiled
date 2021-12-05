using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B4 RID: 436
	[Tracked(false)]
	public class FallEffects : Entity
	{
		// Token: 0x06000F31 RID: 3889 RVA: 0x0003D268 File Offset: 0x0003B468
		public FallEffects()
		{
			base.Tag = Tags.Global;
			base.Depth = -1000000;
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
				this.particles[i].Speed = (float)Calc.Random.Range(120, 240);
				this.particles[i].Color = Calc.Random.Next(FallEffects.colors.Length);
			}
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0003D33C File Offset: 0x0003B53C
		public static void Show(bool visible)
		{
			FallEffects fallEffects = Engine.Scene.Tracker.GetEntity<FallEffects>();
			if (fallEffects == null && visible)
			{
				Engine.Scene.Add(fallEffects = new FallEffects());
			}
			if (fallEffects != null)
			{
				fallEffects.enabled = visible;
			}
			FallEffects.SpeedMultiplier = 1f;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0003D388 File Offset: 0x0003B588
		public override void Update()
		{
			base.Update();
			for (int i = 0; i < this.particles.Length; i++)
			{
				FallEffects.Particle[] array = this.particles;
				int num = i;
				array[num].Position = array[num].Position - Vector2.UnitY * this.particles[i].Speed * FallEffects.SpeedMultiplier * Engine.DeltaTime;
			}
			this.fade = Calc.Approach(this.fade, this.enabled ? 1f : 0f, (float)(this.enabled ? 1 : 4) * Engine.DeltaTime);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0003D438 File Offset: 0x0003B638
		public override void Render()
		{
			if (this.fade <= 0f)
			{
				return;
			}
			Camera camera = (base.Scene as Level).Camera;
			for (int i = 0; i < FallEffects.faded.Length; i++)
			{
				FallEffects.faded[i] = FallEffects.colors[i] * this.fade;
			}
			for (int j = 0; j < this.particles.Length; j++)
			{
				float num = 8f * FallEffects.SpeedMultiplier;
				Vector2 value = new Vector2
				{
					X = this.mod(this.particles[j].Position.X - camera.X, 320f),
					Y = this.mod(this.particles[j].Position.Y - camera.Y - 16f, 212f)
				} + camera.Position;
				Draw.Rect(value - new Vector2(0f, num / 2f), 1f, num, FallEffects.faded[this.particles[j].Color]);
			}
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000A91 RID: 2705
		private static readonly Color[] colors = new Color[]
		{
			Color.White,
			Color.LightGray
		};

		// Token: 0x04000A92 RID: 2706
		private static readonly Color[] faded = new Color[2];

		// Token: 0x04000A93 RID: 2707
		private FallEffects.Particle[] particles = new FallEffects.Particle[50];

		// Token: 0x04000A94 RID: 2708
		private float fade;

		// Token: 0x04000A95 RID: 2709
		private bool enabled;

		// Token: 0x04000A96 RID: 2710
		public static float SpeedMultiplier = 1f;

		// Token: 0x020004C1 RID: 1217
		private struct Particle
		{
			// Token: 0x0400236E RID: 9070
			public Vector2 Position;

			// Token: 0x0400236F RID: 9071
			public float Speed;

			// Token: 0x04002370 RID: 9072
			public int Color;
		}
	}
}

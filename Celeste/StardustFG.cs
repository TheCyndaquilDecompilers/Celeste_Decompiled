using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F4 RID: 500
	public class StardustFG : Backdrop
	{
		// Token: 0x06001069 RID: 4201 RVA: 0x00049598 File Offset: 0x00047798
		public StardustFG()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.Reset(i, Calc.Random.NextFloat());
			}
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x000495E8 File Offset: 0x000477E8
		private void Reset(int i, float p)
		{
			this.particles[i].Percent = p;
			this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
			this.particles[i].Speed = (float)Calc.Random.Range(4, 14);
			this.particles[i].Spin = Calc.Random.Range(0.25f, 18.849556f);
			this.particles[i].Duration = Calc.Random.Range(1f, 4f);
			this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
			this.particles[i].Color = Calc.Random.Next(StardustFG.colors.Length);
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x000496F4 File Offset: 0x000478F4
		public override void Update(Scene scene)
		{
			base.Update(scene);
			Level level = scene as Level;
			bool flag = level.Wind.Y == 0f;
			Vector2 zero = Vector2.Zero;
			if (flag)
			{
				this.scale.X = Math.Max(1f, Math.Abs(level.Wind.X) / 100f);
				this.scale.Y = 1f;
				zero = new Vector2(level.Wind.X, 0f);
			}
			else
			{
				this.scale.X = 1f;
				this.scale.Y = Math.Max(1f, Math.Abs(level.Wind.Y) / 40f);
				zero = new Vector2(0f, level.Wind.Y * 2f);
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				if (this.particles[i].Percent >= 1f)
				{
					this.Reset(i, 0f);
				}
				StardustFG.Particle[] array = this.particles;
				int num = i;
				array[num].Percent = array[num].Percent + Engine.DeltaTime / this.particles[i].Duration;
				StardustFG.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].Position = array2[num2].Position + (this.particles[i].Direction * this.particles[i].Speed + zero) * Engine.DeltaTime;
				this.particles[i].Direction.Rotate(this.particles[i].Spin * Engine.DeltaTime);
			}
			this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0f, Engine.DeltaTime);
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x000498F0 File Offset: 0x00047AF0
		public override void Render(Scene level)
		{
			if (this.fade <= 0f)
			{
				return;
			}
			Camera camera = (level as Level).Camera;
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 position = default(Vector2);
				position.X = this.mod(this.particles[i].Position.X - camera.X, 320f);
				position.Y = this.mod(this.particles[i].Position.Y - camera.Y, 180f);
				float percent = this.particles[i].Percent;
				float num;
				if (percent < 0.7f)
				{
					num = Calc.ClampedMap(percent, 0f, 0.3f, 0f, 1f);
				}
				else
				{
					num = Calc.ClampedMap(percent, 0.7f, 1f, 1f, 0f);
				}
				num *= this.FadeAlphaMultiplier;
				Draw.Rect(position, this.scale.X, this.scale.Y, StardustFG.colors[this.particles[i].Color] * (this.fade * num));
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000BEC RID: 3052
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("4cccef"),
			Calc.HexToColor("f243bd"),
			Calc.HexToColor("42f1dd")
		};

		// Token: 0x04000BED RID: 3053
		private StardustFG.Particle[] particles = new StardustFG.Particle[50];

		// Token: 0x04000BEE RID: 3054
		private float fade;

		// Token: 0x04000BEF RID: 3055
		private Vector2 scale = Vector2.One;

		// Token: 0x020004F3 RID: 1267
		private struct Particle
		{
			// Token: 0x04002450 RID: 9296
			public Vector2 Position;

			// Token: 0x04002451 RID: 9297
			public float Percent;

			// Token: 0x04002452 RID: 9298
			public float Duration;

			// Token: 0x04002453 RID: 9299
			public Vector2 Direction;

			// Token: 0x04002454 RID: 9300
			public float Speed;

			// Token: 0x04002455 RID: 9301
			public float Spin;

			// Token: 0x04002456 RID: 9302
			public int Color;
		}
	}
}

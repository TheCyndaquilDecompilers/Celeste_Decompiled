using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000213 RID: 531
	public class ReflectionFG : Backdrop
	{
		// Token: 0x0600113B RID: 4411 RVA: 0x000544AC File Offset: 0x000526AC
		public ReflectionFG()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.Reset(i, Calc.Random.NextFloat());
			}
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x000544F0 File Offset: 0x000526F0
		private void Reset(int i, float p)
		{
			this.particles[i].Percent = p;
			this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
			this.particles[i].Speed = (float)Calc.Random.Range(4, 14);
			this.particles[i].Spin = Calc.Random.Range(0.25f, 18.849556f);
			this.particles[i].Duration = Calc.Random.Range(1f, 4f);
			this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
			this.particles[i].Color = Calc.Random.Next(ReflectionFG.colors.Length);
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x000545FC File Offset: 0x000527FC
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < this.particles.Length; i++)
			{
				if (this.particles[i].Percent >= 1f)
				{
					this.Reset(i, 0f);
				}
				ReflectionFG.Particle[] array = this.particles;
				int num = i;
				array[num].Percent = array[num].Percent + Engine.DeltaTime / this.particles[i].Duration;
				ReflectionFG.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].Position = array2[num2].Position + this.particles[i].Direction * this.particles[i].Speed * Engine.DeltaTime;
				this.particles[i].Direction.Rotate(this.particles[i].Spin * Engine.DeltaTime);
			}
			this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0f, Engine.DeltaTime);
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00054724 File Offset: 0x00052924
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
				Draw.Rect(position, 1f, 1f, ReflectionFG.colors[this.particles[i].Color] * (this.fade * num));
			}
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CE2 RID: 3298
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("f52b63")
		};

		// Token: 0x04000CE3 RID: 3299
		private ReflectionFG.Particle[] particles = new ReflectionFG.Particle[50];

		// Token: 0x04000CE4 RID: 3300
		private float fade;

		// Token: 0x02000520 RID: 1312
		private struct Particle
		{
			// Token: 0x04002524 RID: 9508
			public Vector2 Position;

			// Token: 0x04002525 RID: 9509
			public float Percent;

			// Token: 0x04002526 RID: 9510
			public float Duration;

			// Token: 0x04002527 RID: 9511
			public Vector2 Direction;

			// Token: 0x04002528 RID: 9512
			public float Speed;

			// Token: 0x04002529 RID: 9513
			public float Spin;

			// Token: 0x0400252A RID: 9514
			public int Color;
		}
	}
}

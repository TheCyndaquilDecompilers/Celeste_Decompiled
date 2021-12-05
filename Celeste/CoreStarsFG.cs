using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000212 RID: 530
	public class CoreStarsFG : Backdrop
	{
		// Token: 0x06001135 RID: 4405 RVA: 0x000540C8 File Offset: 0x000522C8
		public CoreStarsFG()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.Reset(i, Calc.Random.NextFloat());
			}
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x0005410C File Offset: 0x0005230C
		private void Reset(int i, float p)
		{
			this.particles[i].Percent = p;
			this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
			this.particles[i].Speed = Calc.Random.Range(2f, 5f);
			this.particles[i].Spin = Calc.Random.Range(0.25f, 18.849556f);
			this.particles[i].Duration = Calc.Random.Range(1f, 4f);
			this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
			this.particles[i].Color = Calc.Random.Next(CoreStarsFG.colors.Length);
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00054220 File Offset: 0x00052420
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < this.particles.Length; i++)
			{
				if (this.particles[i].Percent >= 1f)
				{
					this.Reset(i, 0f);
				}
				CoreStarsFG.Particle[] array = this.particles;
				int num = i;
				array[num].Percent = array[num].Percent + Engine.DeltaTime / this.particles[i].Duration;
				CoreStarsFG.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].Position = array2[num2].Position + this.particles[i].Direction * this.particles[i].Speed * Engine.DeltaTime;
				this.particles[i].Direction.Rotate(this.particles[i].Spin * Engine.DeltaTime);
			}
			this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0f, Engine.DeltaTime);
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00054348 File Offset: 0x00052548
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
				Draw.Rect(position, 1f, 1f, CoreStarsFG.colors[this.particles[i].Color] * (this.fade * num));
			}
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CDF RID: 3295
		private static readonly Color[] colors = new Color[]
		{
			Color.White,
			Calc.HexToColor("d8baf8")
		};

		// Token: 0x04000CE0 RID: 3296
		private CoreStarsFG.Particle[] particles = new CoreStarsFG.Particle[50];

		// Token: 0x04000CE1 RID: 3297
		private float fade;

		// Token: 0x0200051F RID: 1311
		private struct Particle
		{
			// Token: 0x0400251D RID: 9501
			public Vector2 Position;

			// Token: 0x0400251E RID: 9502
			public float Percent;

			// Token: 0x0400251F RID: 9503
			public float Duration;

			// Token: 0x04002520 RID: 9504
			public Vector2 Direction;

			// Token: 0x04002521 RID: 9505
			public float Speed;

			// Token: 0x04002522 RID: 9506
			public float Spin;

			// Token: 0x04002523 RID: 9507
			public int Color;
		}
	}
}

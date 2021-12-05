using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000214 RID: 532
	public class MirrorFG : Backdrop
	{
		// Token: 0x06001141 RID: 4417 RVA: 0x0005487C File Offset: 0x00052A7C
		public MirrorFG()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.Reset(i, Calc.Random.NextFloat());
			}
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x000548C0 File Offset: 0x00052AC0
		private void Reset(int i, float p)
		{
			this.particles[i].Percent = p;
			this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
			this.particles[i].Speed = (float)Calc.Random.Range(4, 14);
			this.particles[i].Spin = Calc.Random.Range(0.25f, 18.849556f);
			this.particles[i].Duration = Calc.Random.Range(1f, 4f);
			this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
			this.particles[i].Color = Calc.Random.Next(MirrorFG.colors.Length);
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x000549CC File Offset: 0x00052BCC
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < this.particles.Length; i++)
			{
				if (this.particles[i].Percent >= 1f)
				{
					this.Reset(i, 0f);
				}
				MirrorFG.Particle[] array = this.particles;
				int num = i;
				array[num].Percent = array[num].Percent + Engine.DeltaTime / this.particles[i].Duration;
				MirrorFG.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].Position = array2[num2].Position + this.particles[i].Direction * this.particles[i].Speed * Engine.DeltaTime;
				this.particles[i].Direction.Rotate(this.particles[i].Spin * Engine.DeltaTime);
			}
			this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0f, Engine.DeltaTime);
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00054AF4 File Offset: 0x00052CF4
		public override void Render(Scene level)
		{
			if (this.fade <= 0f)
			{
				return;
			}
			Camera camera = (level as Level).Camera;
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 position = new Vector2
				{
					X = this.Mod(this.particles[i].Position.X - camera.X, 320f),
					Y = this.Mod(this.particles[i].Position.Y - camera.Y, 180f)
				};
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
				Draw.Rect(position, 1f, 1f, MirrorFG.colors[this.particles[i].Color] * (this.fade * num));
			}
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CE5 RID: 3301
		private static readonly Color[] colors = new Color[]
		{
			Color.Red
		};

		// Token: 0x04000CE6 RID: 3302
		private MirrorFG.Particle[] particles = new MirrorFG.Particle[50];

		// Token: 0x04000CE7 RID: 3303
		private float fade;

		// Token: 0x02000521 RID: 1313
		private struct Particle
		{
			// Token: 0x0400252B RID: 9515
			public Vector2 Position;

			// Token: 0x0400252C RID: 9516
			public float Percent;

			// Token: 0x0400252D RID: 9517
			public float Duration;

			// Token: 0x0400252E RID: 9518
			public Vector2 Direction;

			// Token: 0x0400252F RID: 9519
			public float Speed;

			// Token: 0x04002530 RID: 9520
			public float Spin;

			// Token: 0x04002531 RID: 9521
			public int Color;
		}
	}
}

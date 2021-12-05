using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000211 RID: 529
	public class Petals : Backdrop
	{
		// Token: 0x0600112F RID: 4399 RVA: 0x00053D6C File Offset: 0x00051F6C
		public Petals()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.Reset(i);
			}
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x00053DA8 File Offset: 0x00051FA8
		private void Reset(int i)
		{
			this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 352), (float)Calc.Random.Range(0, 212));
			this.particles[i].Speed = Calc.Random.Range(6f, 16f);
			this.particles[i].Spin = Calc.Random.Range(8f, 12f) * 0.2f;
			this.particles[i].Color = Calc.Random.Next(Petals.colors.Length);
			this.particles[i].RotationCounter = Calc.Random.NextAngle();
			this.particles[i].MaxRotate = Calc.Random.Range(0.3f, 0.6f) * 1.5707964f;
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00053EA8 File Offset: 0x000520A8
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < this.particles.Length; i++)
			{
				Petals.Particle[] array = this.particles;
				int num = i;
				array[num].Position.Y = array[num].Position.Y + this.particles[i].Speed * Engine.DeltaTime;
				Petals.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].RotationCounter = array2[num2].RotationCounter + this.particles[i].Spin * Engine.DeltaTime;
			}
			this.fade = Calc.Approach(this.fade, this.Visible ? 1f : 0f, Engine.DeltaTime);
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00053F58 File Offset: 0x00052158
		public override void Render(Scene level)
		{
			if (this.fade <= 0f)
			{
				return;
			}
			Camera camera = (level as Level).Camera;
			MTexture mtexture = GFX.Game["particles/petal"];
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 vector = default(Vector2);
				vector.X = -16f + this.Mod(this.particles[i].Position.X - camera.X, 352f);
				vector.Y = -16f + this.Mod(this.particles[i].Position.Y - camera.Y, 212f);
				float num = (float)(1.5707963705062866 + Math.Sin((double)(this.particles[i].RotationCounter * this.particles[i].MaxRotate)) * 1.0);
				vector += Calc.AngleToVector(num, 4f);
				mtexture.DrawCentered(vector, Petals.colors[this.particles[i].Color] * this.fade, 1f, num - 0.8f);
			}
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CDC RID: 3292
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("ff3aa3")
		};

		// Token: 0x04000CDD RID: 3293
		private Petals.Particle[] particles = new Petals.Particle[40];

		// Token: 0x04000CDE RID: 3294
		private float fade;

		// Token: 0x0200051E RID: 1310
		private struct Particle
		{
			// Token: 0x04002517 RID: 9495
			public Vector2 Position;

			// Token: 0x04002518 RID: 9496
			public float Speed;

			// Token: 0x04002519 RID: 9497
			public float Spin;

			// Token: 0x0400251A RID: 9498
			public float MaxRotate;

			// Token: 0x0400251B RID: 9499
			public int Color;

			// Token: 0x0400251C RID: 9500
			public float RotationCounter;
		}
	}
}

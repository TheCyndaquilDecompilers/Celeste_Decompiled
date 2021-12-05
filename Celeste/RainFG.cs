using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F3 RID: 499
	public class RainFG : Backdrop
	{
		// Token: 0x06001065 RID: 4197 RVA: 0x00049330 File Offset: 0x00047530
		public RainFG()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Init();
			}
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x00049398 File Offset: 0x00047598
		public override void Update(Scene scene)
		{
			base.Update(scene);
			bool flag = base.IsVisible(scene as Level);
			(scene as Level).Raining = flag;
			this.visibleFade = Calc.Approach(this.visibleFade, (float)(flag ? 1 : 0), Engine.DeltaTime * (flag ? 10f : 0.25f));
			if (this.FadeX != null)
			{
				this.linearFade = this.FadeX.Value((scene as Level).Camera.X + 160f);
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				RainFG.Particle[] array = this.particles;
				int num = i;
				array[num].Position = array[num].Position + this.particles[i].Speed * Engine.DeltaTime;
			}
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x00049474 File Offset: 0x00047674
		public override void Render(Scene scene)
		{
			if (this.Alpha <= 0f || this.visibleFade <= 0f || this.linearFade <= 0f)
			{
				return;
			}
			Color color = Calc.HexToColor("161933") * 0.5f * this.Alpha * this.linearFade * this.visibleFade;
			Camera camera = (scene as Level).Camera;
			for (int i = 0; i < this.particles.Length; i++)
			{
				Vector2 position = new Vector2(this.mod(this.particles[i].Position.X - camera.X - 32f, 384f), this.mod(this.particles[i].Position.Y - camera.Y - 32f, 244f));
				Draw.Pixel.DrawCentered(position, color, this.particles[i].Scale, this.particles[i].Rotation);
			}
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000BE8 RID: 3048
		public float Alpha = 1f;

		// Token: 0x04000BE9 RID: 3049
		private float visibleFade = 1f;

		// Token: 0x04000BEA RID: 3050
		private float linearFade = 1f;

		// Token: 0x04000BEB RID: 3051
		private RainFG.Particle[] particles = new RainFG.Particle[240];

		// Token: 0x020004F2 RID: 1266
		private struct Particle
		{
			// Token: 0x060024A8 RID: 9384 RVA: 0x000F4AA4 File Offset: 0x000F2CA4
			public void Init()
			{
				this.Position = new Vector2(-32f + Calc.Random.NextFloat(384f), -32f + Calc.Random.NextFloat(244f));
				this.Rotation = 1.5707964f + Calc.Random.Range(-0.05f, 0.05f);
				this.Speed = Calc.AngleToVector(this.Rotation, Calc.Random.Range(200f, 600f));
				this.Scale = new Vector2(4f + (this.Speed.Length() - 200f) / 400f * 12f, 1f);
			}

			// Token: 0x0400244C RID: 9292
			public Vector2 Position;

			// Token: 0x0400244D RID: 9293
			public Vector2 Speed;

			// Token: 0x0400244E RID: 9294
			public float Rotation;

			// Token: 0x0400244F RID: 9295
			public Vector2 Scale;
		}
	}
}

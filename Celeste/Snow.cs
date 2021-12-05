using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200036A RID: 874
	public class Snow : Backdrop
	{
		// Token: 0x06001B79 RID: 7033 RVA: 0x000B3944 File Offset: 0x000B1B44
		public Snow(bool foreground)
		{
			this.colors = (foreground ? Snow.ForegroundColors : Snow.BackgroundColors);
			this.blendedColors = new Color[this.colors.Length];
			int num = foreground ? 120 : 40;
			int num2 = foreground ? 300 : 100;
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.particles[i].Init(this.colors.Length, (float)num, (float)num2);
			}
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x000B39F4 File Offset: 0x000B1BF4
		public override void Update(Scene scene)
		{
			base.Update(scene);
			this.visibleFade = Calc.Approach(this.visibleFade, (float)(base.IsVisible(scene as Level) ? 1 : 0), Engine.DeltaTime * 2f);
			if (this.FadeX != null)
			{
				this.linearFade = this.FadeX.Value((scene as Level).Camera.X + 160f);
			}
			for (int i = 0; i < this.particles.Length; i++)
			{
				Snow.Particle[] array = this.particles;
				int num = i;
				array[num].Position.X = array[num].Position.X - this.particles[i].Speed * Engine.DeltaTime;
				Snow.Particle[] array2 = this.particles;
				int num2 = i;
				array2[num2].Position.Y = array2[num2].Position.Y + (float)Math.Sin((double)this.particles[i].Sin) * this.particles[i].Speed * 0.2f * Engine.DeltaTime;
				Snow.Particle[] array3 = this.particles;
				int num3 = i;
				array3[num3].Sin = array3[num3].Sin + Engine.DeltaTime;
			}
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x000B3B1C File Offset: 0x000B1D1C
		public override void Render(Scene scene)
		{
			if (this.Alpha <= 0f || this.visibleFade <= 0f || this.linearFade <= 0f)
			{
				return;
			}
			for (int i = 0; i < this.blendedColors.Length; i++)
			{
				this.blendedColors[i] = this.colors[i] * (this.Alpha * this.visibleFade * this.linearFade);
			}
			Camera camera = (scene as Level).Camera;
			for (int j = 0; j < this.particles.Length; j++)
			{
				Vector2 position = new Vector2(this.mod(this.particles[j].Position.X - camera.X, 320f), this.mod(this.particles[j].Position.Y - camera.Y, 180f));
				Color color = this.blendedColors[this.particles[j].Color];
				Draw.Pixel.DrawCentered(position, color);
			}
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04001886 RID: 6278
		public static readonly Color[] ForegroundColors = new Color[]
		{
			Color.White,
			Color.CornflowerBlue
		};

		// Token: 0x04001887 RID: 6279
		public static readonly Color[] BackgroundColors = new Color[]
		{
			new Color(0.2f, 0.2f, 0.2f, 1f),
			new Color(0.1f, 0.2f, 0.5f, 1f)
		};

		// Token: 0x04001888 RID: 6280
		public float Alpha = 1f;

		// Token: 0x04001889 RID: 6281
		private float visibleFade = 1f;

		// Token: 0x0400188A RID: 6282
		private float linearFade = 1f;

		// Token: 0x0400188B RID: 6283
		private Color[] colors;

		// Token: 0x0400188C RID: 6284
		private Color[] blendedColors;

		// Token: 0x0400188D RID: 6285
		private Snow.Particle[] particles = new Snow.Particle[60];

		// Token: 0x0200071F RID: 1823
		private struct Particle
		{
			// Token: 0x06002E51 RID: 11857 RVA: 0x001243A4 File Offset: 0x001225A4
			public void Init(int maxColors, float speedMin, float speedMax)
			{
				this.Position = new Vector2(Calc.Random.NextFloat(320f), Calc.Random.NextFloat(180f));
				this.Color = Calc.Random.Next(maxColors);
				this.Speed = Calc.Random.Range(speedMin, speedMax);
				this.Sin = Calc.Random.NextFloat(6.2831855f);
			}

			// Token: 0x04002DD3 RID: 11731
			public Vector2 Position;

			// Token: 0x04002DD4 RID: 11732
			public int Color;

			// Token: 0x04002DD5 RID: 11733
			public float Speed;

			// Token: 0x04002DD6 RID: 11734
			public float Sin;
		}
	}
}

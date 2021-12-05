using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E6 RID: 742
	public class WindSnowFG : Backdrop
	{
		// Token: 0x060016DD RID: 5853 RVA: 0x000882F4 File Offset: 0x000864F4
		public WindSnowFG()
		{
			this.Color = Color.White;
			this.positions = new Vector2[240];
			for (int i = 0; i < this.positions.Length; i++)
			{
				this.positions[i] = Calc.Random.Range(new Vector2(0f, 0f), new Vector2(this.loopWidth, this.loopHeight));
			}
			this.sines = new SineWave[16];
			for (int j = 0; j < this.sines.Length; j++)
			{
				this.sines[j] = new SineWave(Calc.Random.Range(0.8f, 1.2f), 0f);
				this.sines[j].Randomize();
			}
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x00088404 File Offset: 0x00086604
		public override void Update(Scene scene)
		{
			base.Update(scene);
			this.visibleFade = Calc.Approach(this.visibleFade, (float)(base.IsVisible(scene as Level) ? 1 : 0), Engine.DeltaTime * 2f);
			Level level = scene as Level;
			SineWave[] array = this.sines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Update();
			}
			bool flag = level.Wind.Y == 0f;
			if (flag)
			{
				this.scale.X = Math.Max(1f, Math.Abs(level.Wind.X) / 100f);
				this.rotation = Calc.Approach(this.rotation, 0f, Engine.DeltaTime * 8f);
			}
			else
			{
				this.scale.X = Math.Max(1f, Math.Abs(level.Wind.Y) / 40f);
				this.rotation = Calc.Approach(this.rotation, -1.5707964f, Engine.DeltaTime * 8f);
			}
			this.scale.Y = 1f / Math.Max(1f, this.scale.X * 0.25f);
			for (int j = 0; j < this.positions.Length; j++)
			{
				float value = this.sines[j % this.sines.Length].Value;
				Vector2 zero = Vector2.Zero;
				if (flag)
				{
					zero = new Vector2(level.Wind.X + value * 10f, 20f);
				}
				else
				{
					zero = new Vector2(0f, level.Wind.Y * 3f + value * 10f);
				}
				this.positions[j] += zero * Engine.DeltaTime;
			}
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x000885F8 File Offset: 0x000867F8
		public override void Render(Scene scene)
		{
			if (this.Alpha <= 0f)
			{
				return;
			}
			Color color = this.Color * this.visibleFade * this.Alpha;
			int num = (int)(((scene as Level).Wind.Y == 0f) ? ((float)this.positions.Length) : ((float)this.positions.Length * 0.6f));
			int num2 = 0;
			foreach (Vector2 vector in this.positions)
			{
				vector.Y -= (scene as Level).Camera.Y + this.CameraOffset.Y;
				vector.Y %= this.loopHeight;
				if (vector.Y < 0f)
				{
					vector.Y += this.loopHeight;
				}
				vector.X -= (scene as Level).Camera.X + this.CameraOffset.X;
				vector.X %= this.loopWidth;
				if (vector.X < 0f)
				{
					vector.X += this.loopWidth;
				}
				if (num2 < num)
				{
					GFX.Game["particles/snow"].DrawCentered(vector, color, this.scale, this.rotation);
				}
				num2++;
			}
		}

		// Token: 0x0400135F RID: 4959
		public Vector2 CameraOffset = Vector2.Zero;

		// Token: 0x04001360 RID: 4960
		public float Alpha = 1f;

		// Token: 0x04001361 RID: 4961
		private Vector2[] positions;

		// Token: 0x04001362 RID: 4962
		private SineWave[] sines;

		// Token: 0x04001363 RID: 4963
		private Vector2 scale = Vector2.One;

		// Token: 0x04001364 RID: 4964
		private float rotation;

		// Token: 0x04001365 RID: 4965
		private float loopWidth = 640f;

		// Token: 0x04001366 RID: 4966
		private float loopHeight = 360f;

		// Token: 0x04001367 RID: 4967
		private float visibleFade = 1f;
	}
}

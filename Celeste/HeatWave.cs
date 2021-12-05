using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200020F RID: 527
	public class HeatWave : Backdrop
	{
		// Token: 0x06001120 RID: 4384 RVA: 0x00052CB8 File Offset: 0x00050EB8
		public HeatWave()
		{
			for (int i = 0; i < this.particles.Length; i++)
			{
				this.Reset(i, Calc.Random.NextFloat());
			}
			this.currentColors = new Color[HeatWave.hotColors.Length];
			this.colorLerp = 1f;
			this.mist1 = new Parallax(GFX.Misc["mist"]);
			this.mist2 = new Parallax(GFX.Misc["mist"]);
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x00052D50 File Offset: 0x00050F50
		private void Reset(int i, float p)
		{
			this.particles[i].Percent = p;
			this.particles[i].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
			this.particles[i].Speed = (float)Calc.Random.Range(4, 14);
			this.particles[i].Spin = Calc.Random.Range(0.25f, 18.849556f);
			this.particles[i].Duration = Calc.Random.Range(1f, 4f);
			this.particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
			this.particles[i].Color = Calc.Random.Next(HeatWave.hotColors.Length);
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00052E5C File Offset: 0x0005105C
		public override void Update(Scene scene)
		{
			Level level = scene as Level;
			this.show = (base.IsVisible(level) && level.CoreMode > Session.CoreModes.None);
			if (this.show)
			{
				if (!this.wasShow)
				{
					this.colorLerp = (float)((level.CoreMode == Session.CoreModes.Hot) ? 1 : 0);
					level.NextColorGrade((level.CoreMode == Session.CoreModes.Hot) ? "hot" : "cold", 1f);
				}
				else
				{
					level.SnapColorGrade((level.CoreMode == Session.CoreModes.Hot) ? "hot" : "cold");
				}
				this.colorLerp = Calc.Approach(this.colorLerp, (float)((level.CoreMode == Session.CoreModes.Hot) ? 1 : 0), Engine.DeltaTime * 100f);
				for (int i = 0; i < this.currentColors.Length; i++)
				{
					this.currentColors[i] = Color.Lerp(HeatWave.coldColors[i], HeatWave.hotColors[i], this.colorLerp);
				}
			}
			else
			{
				level.NextColorGrade("none", 1f);
			}
			for (int j = 0; j < this.particles.Length; j++)
			{
				if (this.particles[j].Percent >= 1f)
				{
					this.Reset(j, 0f);
				}
				float scaleFactor = 1f;
				if (level.CoreMode == Session.CoreModes.Cold)
				{
					scaleFactor = 0.25f;
				}
				HeatWave.Particle[] array = this.particles;
				int num = j;
				array[num].Percent = array[num].Percent + Engine.DeltaTime / this.particles[j].Duration;
				HeatWave.Particle[] array2 = this.particles;
				int num2 = j;
				array2[num2].Position = array2[num2].Position + this.particles[j].Direction * this.particles[j].Speed * scaleFactor * Engine.DeltaTime;
				this.particles[j].Direction.Rotate(this.particles[j].Spin * Engine.DeltaTime);
				if (level.CoreMode == Session.CoreModes.Hot)
				{
					HeatWave.Particle[] array3 = this.particles;
					int num3 = j;
					array3[num3].Position.Y = array3[num3].Position.Y - 10f * Engine.DeltaTime;
				}
			}
			this.fade = Calc.Approach(this.fade, this.show ? 1f : 0f, Engine.DeltaTime);
			this.heat = Calc.Approach(this.heat, (this.show && level.CoreMode == Session.CoreModes.Hot) ? 1f : 0f, Engine.DeltaTime * 100f);
			this.mist1.Color = Color.Lerp(Calc.HexToColor("639bff"), Calc.HexToColor("f1b22b"), this.heat) * this.fade * 0.7f;
			this.mist2.Color = Color.Lerp(Calc.HexToColor("5fcde4"), Calc.HexToColor("f12b3a"), this.heat) * this.fade * 0.7f;
			this.mist1.Speed = new Vector2(4f, -20f) * this.heat;
			this.mist2.Speed = new Vector2(4f, -40f) * this.heat;
			this.mist1.Update(scene);
			this.mist2.Update(scene);
			if (this.heat > 0f)
			{
				Distort.WaterSineDirection = -1f;
				Distort.WaterAlpha = this.heat * 0.5f;
			}
			else
			{
				Distort.WaterAlpha = 1f;
			}
			this.wasShow = this.show;
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00053220 File Offset: 0x00051420
		public void RenderDisplacement(Level level)
		{
			if (this.heat > 0f)
			{
				Color color = new Color(0.5f, 0.5f, 0.1f, 1f);
				Draw.Rect(level.Camera.X - 5f, level.Camera.Y - 5f, 370f, 190f, color);
			}
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00053288 File Offset: 0x00051488
		public override void Render(Scene scene)
		{
			if (this.fade <= 0f)
			{
				return;
			}
			Camera camera = (scene as Level).Camera;
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
				Draw.Rect(position, 1f, 1f, this.currentColors[this.particles[i].Color] * (this.fade * num));
			}
			this.mist1.Render(scene);
			this.mist2.Render(scene);
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CC8 RID: 3272
		private static readonly Color[] hotColors = new Color[]
		{
			Color.Red,
			Color.Orange
		};

		// Token: 0x04000CC9 RID: 3273
		private static readonly Color[] coldColors = new Color[]
		{
			Color.LightSkyBlue,
			Color.Teal
		};

		// Token: 0x04000CCA RID: 3274
		private Color[] currentColors;

		// Token: 0x04000CCB RID: 3275
		private float colorLerp;

		// Token: 0x04000CCC RID: 3276
		private HeatWave.Particle[] particles = new HeatWave.Particle[50];

		// Token: 0x04000CCD RID: 3277
		private float fade;

		// Token: 0x04000CCE RID: 3278
		private float heat;

		// Token: 0x04000CCF RID: 3279
		private Parallax mist1;

		// Token: 0x04000CD0 RID: 3280
		private Parallax mist2;

		// Token: 0x04000CD1 RID: 3281
		private bool show;

		// Token: 0x04000CD2 RID: 3282
		private bool wasShow;

		// Token: 0x0200051A RID: 1306
		private struct Particle
		{
			// Token: 0x04002502 RID: 9474
			public Vector2 Position;

			// Token: 0x04002503 RID: 9475
			public float Percent;

			// Token: 0x04002504 RID: 9476
			public float Duration;

			// Token: 0x04002505 RID: 9477
			public Vector2 Direction;

			// Token: 0x04002506 RID: 9478
			public float Speed;

			// Token: 0x04002507 RID: 9479
			public float Spin;

			// Token: 0x04002508 RID: 9480
			public int Color;
		}
	}
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200020D RID: 525
	public class FinalBossStarfield : Backdrop
	{
		// Token: 0x06001117 RID: 4375 RVA: 0x00052110 File Offset: 0x00050310
		public FinalBossStarfield()
		{
			this.UseSpritebatch = false;
			for (int i = 0; i < 200; i++)
			{
				this.particles[i].Speed = Calc.Random.Range(500f, 1200f);
				this.particles[i].Direction = new Vector2(-1f, 0f);
				this.particles[i].DirectionApproach = Calc.Random.Range(0.25f, 4f);
				this.particles[i].Position.X = (float)Calc.Random.Range(0, 384);
				this.particles[i].Position.Y = (float)Calc.Random.Range(0, 244);
				this.particles[i].Color = Calc.Random.Choose(FinalBossStarfield.colors);
			}
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00052244 File Offset: 0x00050444
		public override void Update(Scene scene)
		{
			base.Update(scene);
			if (this.Visible && this.Alpha > 0f)
			{
				Vector2 vector = new Vector2(-1f, 0f);
				Level level = scene as Level;
				if (level.Bounds.Height > level.Bounds.Width)
				{
					vector = new Vector2(0f, -1f);
				}
				float target = vector.Angle();
				for (int i = 0; i < 200; i++)
				{
					FinalBossStarfield.Particle[] array = this.particles;
					int num = i;
					array[num].Position = array[num].Position + this.particles[i].Direction * this.particles[i].Speed * Engine.DeltaTime;
					float num2 = this.particles[i].Direction.Angle();
					num2 = Calc.AngleApproach(num2, target, this.particles[i].DirectionApproach * Engine.DeltaTime);
					this.particles[i].Direction = Calc.AngleToVector(num2, 1f);
				}
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0005237C File Offset: 0x0005057C
		public override void Render(Scene scene)
		{
			Vector2 position = (scene as Level).Camera.Position;
			Color color = Color.Black * this.Alpha;
			this.verts[0].Color = color;
			this.verts[0].Position = new Vector3(-10f, -10f, 0f);
			this.verts[1].Color = color;
			this.verts[1].Position = new Vector3(330f, -10f, 0f);
			this.verts[2].Color = color;
			this.verts[2].Position = new Vector3(330f, 190f, 0f);
			this.verts[3].Color = color;
			this.verts[3].Position = new Vector3(-10f, -10f, 0f);
			this.verts[4].Color = color;
			this.verts[4].Position = new Vector3(330f, 190f, 0f);
			this.verts[5].Color = color;
			this.verts[5].Position = new Vector3(-10f, 190f, 0f);
			for (int i = 0; i < 200; i++)
			{
				int num = (i + 1) * 6;
				float scaleFactor = Calc.ClampedMap(this.particles[i].Speed, 0f, 1200f, 1f, 64f);
				float scaleFactor2 = Calc.ClampedMap(this.particles[i].Speed, 0f, 1200f, 3f, 0.6f);
				Vector2 direction = this.particles[i].Direction;
				Vector2 value = direction.Perpendicular();
				Vector2 position2 = this.particles[i].Position;
				position2.X = -32f + this.Mod(position2.X - position.X * 0.9f, 384f);
				position2.Y = -32f + this.Mod(position2.Y - position.Y * 0.9f, 244f);
				Vector2 value2 = position2 - direction * scaleFactor * 0.5f - value * scaleFactor2;
				Vector2 value3 = position2 + direction * scaleFactor * 1f - value * scaleFactor2;
				Vector2 value4 = position2 + direction * scaleFactor * 0.5f + value * scaleFactor2;
				Vector2 value5 = position2 - direction * scaleFactor * 1f + value * scaleFactor2;
				Color color2 = this.particles[i].Color * this.Alpha;
				this.verts[num].Color = color2;
				this.verts[num].Position = new Vector3(value2, 0f);
				this.verts[num + 1].Color = color2;
				this.verts[num + 1].Position = new Vector3(value3, 0f);
				this.verts[num + 2].Color = color2;
				this.verts[num + 2].Position = new Vector3(value4, 0f);
				this.verts[num + 3].Color = color2;
				this.verts[num + 3].Position = new Vector3(value2, 0f);
				this.verts[num + 4].Color = color2;
				this.verts[num + 4].Position = new Vector3(value4, 0f);
				this.verts[num + 5].Color = color2;
				this.verts[num + 5].Position = new Vector3(value5, 0f);
			}
			GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.verts, this.verts.Length, null, null);
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CBD RID: 3261
		public float Alpha = 1f;

		// Token: 0x04000CBE RID: 3262
		private const int particleCount = 200;

		// Token: 0x04000CBF RID: 3263
		private FinalBossStarfield.Particle[] particles = new FinalBossStarfield.Particle[200];

		// Token: 0x04000CC0 RID: 3264
		private VertexPositionColor[] verts = new VertexPositionColor[1206];

		// Token: 0x04000CC1 RID: 3265
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("030c1b"),
			Calc.HexToColor("0b031b"),
			Calc.HexToColor("1b0319"),
			Calc.HexToColor("0f0301")
		};

		// Token: 0x02000518 RID: 1304
		private struct Particle
		{
			// Token: 0x040024F7 RID: 9463
			public Vector2 Position;

			// Token: 0x040024F8 RID: 9464
			public Vector2 Direction;

			// Token: 0x040024F9 RID: 9465
			public float Speed;

			// Token: 0x040024FA RID: 9466
			public Color Color;

			// Token: 0x040024FB RID: 9467
			public float DirectionApproach;
		}
	}
}

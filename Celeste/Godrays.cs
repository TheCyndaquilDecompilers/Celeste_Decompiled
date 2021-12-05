using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200020E RID: 526
	public class Godrays : Backdrop
	{
		// Token: 0x0600111C RID: 4380 RVA: 0x00052860 File Offset: 0x00050A60
		public Godrays()
		{
			this.UseSpritebatch = false;
			for (int i = 0; i < this.rays.Length; i++)
			{
				this.rays[i].Reset();
				this.rays[i].Percent = Calc.Random.NextFloat();
			}
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x000528EC File Offset: 0x00050AEC
		public override void Update(Scene scene)
		{
			Level level = scene as Level;
			bool flag = base.IsVisible(level);
			this.fade = Calc.Approach(this.fade, (float)(flag ? 1 : 0), Engine.DeltaTime);
			this.Visible = (this.fade > 0f);
			if (!this.Visible)
			{
				return;
			}
			Player entity = level.Tracker.GetEntity<Player>();
			Vector2 vector = Calc.AngleToVector(-1.6707964f, 1f);
			Vector2 value = new Vector2(-vector.Y, vector.X);
			int num = 0;
			for (int i = 0; i < this.rays.Length; i++)
			{
				if (this.rays[i].Percent >= 1f)
				{
					this.rays[i].Reset();
				}
				Godrays.Ray[] array = this.rays;
				int num2 = i;
				array[num2].Percent = array[num2].Percent + Engine.DeltaTime / this.rays[i].Duration;
				Godrays.Ray[] array2 = this.rays;
				int num3 = i;
				array2[num3].Y = array2[num3].Y + 8f * Engine.DeltaTime;
				float percent = this.rays[i].Percent;
				float num4 = -32f + this.Mod(this.rays[i].X - level.Camera.X * 0.9f, 384f);
				float num5 = -32f + this.Mod(this.rays[i].Y - level.Camera.Y * 0.9f, 244f);
				float width = this.rays[i].Width;
				float length = this.rays[i].Length;
				Vector2 value2 = new Vector2((float)((int)num4), (float)((int)num5));
				Color color = this.rayColor * Ease.CubeInOut(Calc.Clamp(((percent < 0.5f) ? percent : (1f - percent)) * 2f, 0f, 1f)) * this.fade;
				if (entity != null)
				{
					float num6 = (value2 + level.Camera.Position - entity.Position).Length();
					if (num6 < 64f)
					{
						color *= 0.25f + 0.75f * (num6 / 64f);
					}
				}
				VertexPositionColor vertexPositionColor = new VertexPositionColor(new Vector3(value2 + value * width + vector * length, 0f), color);
				VertexPositionColor vertexPositionColor2 = new VertexPositionColor(new Vector3(value2 - value * width, 0f), color);
				VertexPositionColor vertexPositionColor3 = new VertexPositionColor(new Vector3(value2 + value * width, 0f), color);
				VertexPositionColor vertexPositionColor4 = new VertexPositionColor(new Vector3(value2 - value * width - vector * length, 0f), color);
				this.vertices[num++] = vertexPositionColor;
				this.vertices[num++] = vertexPositionColor2;
				this.vertices[num++] = vertexPositionColor3;
				this.vertices[num++] = vertexPositionColor2;
				this.vertices[num++] = vertexPositionColor3;
				this.vertices[num++] = vertexPositionColor4;
			}
			this.vertexCount = num;
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x00052C88 File Offset: 0x00050E88
		public override void Render(Scene scene)
		{
			if (this.vertexCount > 0 && this.fade > 0f)
			{
				GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.vertices, this.vertexCount, null, null);
			}
		}

		// Token: 0x04000CC2 RID: 3266
		private const int RayCount = 6;

		// Token: 0x04000CC3 RID: 3267
		private VertexPositionColor[] vertices = new VertexPositionColor[36];

		// Token: 0x04000CC4 RID: 3268
		private int vertexCount;

		// Token: 0x04000CC5 RID: 3269
		private Color rayColor = Calc.HexToColor("f52b63") * 0.5f;

		// Token: 0x04000CC6 RID: 3270
		private Godrays.Ray[] rays = new Godrays.Ray[6];

		// Token: 0x04000CC7 RID: 3271
		private float fade;

		// Token: 0x02000519 RID: 1305
		private struct Ray
		{
			// Token: 0x06002534 RID: 9524 RVA: 0x000F7B34 File Offset: 0x000F5D34
			public void Reset()
			{
				this.Percent = 0f;
				this.X = Calc.Random.NextFloat(384f);
				this.Y = Calc.Random.NextFloat(244f);
				this.Duration = 4f + Calc.Random.NextFloat() * 8f;
				this.Width = (float)Calc.Random.Next(8, 16);
				this.Length = (float)Calc.Random.Next(20, 40);
			}

			// Token: 0x040024FC RID: 9468
			public float X;

			// Token: 0x040024FD RID: 9469
			public float Y;

			// Token: 0x040024FE RID: 9470
			public float Percent;

			// Token: 0x040024FF RID: 9471
			public float Duration;

			// Token: 0x04002500 RID: 9472
			public float Width;

			// Token: 0x04002501 RID: 9473
			public float Length;
		}
	}
}

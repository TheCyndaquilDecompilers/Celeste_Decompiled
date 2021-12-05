using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000215 RID: 533
	public class Tentacles : Backdrop
	{
		// Token: 0x06001147 RID: 4423 RVA: 0x00054C44 File Offset: 0x00052E44
		public Tentacles(Tentacles.Side side, Color color, float outwardsOffset)
		{
			this.side = side;
			this.outwardsOffset = outwardsOffset;
			this.UseSpritebatch = false;
			switch (side)
			{
			case Tentacles.Side.Right:
				this.outwards = new Vector2(-1f, 0f);
				this.width = 180f;
				this.origin = new Vector2(320f, 90f);
				break;
			case Tentacles.Side.Left:
				this.outwards = new Vector2(1f, 0f);
				this.width = 180f;
				this.origin = new Vector2(0f, 90f);
				break;
			case Tentacles.Side.Top:
				this.outwards = new Vector2(0f, 1f);
				this.width = 320f;
				this.origin = new Vector2(160f, 0f);
				break;
			case Tentacles.Side.Bottom:
				this.outwards = new Vector2(0f, -1f);
				this.width = 320f;
				this.origin = new Vector2(160f, 180f);
				break;
			}
			float num = 0f;
			this.tentacles = new Tentacles.Tentacle[100];
			int num2 = 0;
			while (num2 < this.tentacles.Length && num < this.width + 40f)
			{
				this.tentacles[num2].Length = Calc.Random.NextFloat();
				this.tentacles[num2].Offset = Calc.Random.NextFloat();
				this.tentacles[num2].Step = Calc.Random.NextFloat();
				this.tentacles[num2].Position = -200f;
				this.tentacles[num2].Approach = Calc.Random.NextFloat();
				num += (this.tentacles[num2].Width = 6f + Calc.Random.NextFloat(20f));
				this.tentacleCount++;
				num2++;
			}
			this.vertices = new VertexPositionColor[this.tentacleCount * 11 * 6];
			for (int i = 0; i < this.vertices.Length; i++)
			{
				this.vertices[i].Color = color;
			}
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00054EA4 File Offset: 0x000530A4
		public override void Update(Scene scene)
		{
			bool flag = base.IsVisible(scene as Level);
			float num = 0f;
			if (flag)
			{
				Camera camera = (scene as Level).Camera;
				Player entity = scene.Tracker.GetEntity<Player>();
				if (entity != null)
				{
					if (this.side == Tentacles.Side.Right)
					{
						num = 320f - (entity.X - camera.X) - 160f;
					}
					else if (this.side == Tentacles.Side.Bottom)
					{
						num = 180f - (entity.Y - camera.Y) - 180f;
					}
				}
				this.hideTimer = 0f;
			}
			else
			{
				num = -200f;
				this.hideTimer += Engine.DeltaTime;
			}
			num += this.outwardsOffset;
			this.Visible = (this.hideTimer < 5f);
			if (this.Visible)
			{
				Vector2 value = -this.outwards.Perpendicular();
				int num2 = 0;
				Vector2 vector = this.origin - value * (this.width / 2f + 2f);
				for (int i = 0; i < this.tentacleCount; i++)
				{
					vector += value * this.tentacles[i].Width * 0.5f;
					Tentacles.Tentacle[] array = this.tentacles;
					int num3 = i;
					array[num3].Position = array[num3].Position + (num - this.tentacles[i].Position) * (1f - (float)Math.Pow((double)(0.5f * (0.5f + this.tentacles[i].Approach * 0.5f)), (double)Engine.DeltaTime));
					Vector2 value2 = (this.tentacles[i].Position + (float)Math.Sin((double)(scene.TimeActive + this.tentacles[i].Offset * 4f)) * 8f + (this.origin - vector).Length() * 0.7f) * this.outwards;
					Vector2 vector2 = vector + value2;
					float num4 = 2f + this.tentacles[i].Length * 8f;
					Vector2 value3 = vector2;
					Vector2 value4 = value * this.tentacles[i].Width * 0.5f;
					this.vertices[num2++].Position = new Vector3(vector + value4, 0f);
					this.vertices[num2++].Position = new Vector3(vector - value4, 0f);
					this.vertices[num2++].Position = new Vector3(vector2 - value4, 0f);
					this.vertices[num2++].Position = new Vector3(vector2 - value4, 0f);
					this.vertices[num2++].Position = new Vector3(vector + value4, 0f);
					this.vertices[num2++].Position = new Vector3(vector2 + value4, 0f);
					for (int j = 1; j < 10; j++)
					{
						double num5 = (double)(scene.TimeActive * this.tentacles[i].Offset * (float)Math.Pow(1.100000023841858, (double)j) * 2f);
						float num6 = this.tentacles[i].Offset * 3f + (float)j * (0.1f + this.tentacles[i].Step * 0.2f) + num4 * (float)j * 0.1f;
						float scaleFactor = 2f + 4f * ((float)j / 10f);
						Vector2 value5 = (float)Math.Sin(num5 + (double)num6) * value * scaleFactor;
						float scaleFactor2 = (1f - (float)j / 10f) * this.tentacles[i].Width * 0.5f;
						Vector2 vector3 = value3 + this.outwards * num4 + value5;
						Vector2 vector4 = (value3 - vector3).SafeNormalize().Perpendicular() * scaleFactor2;
						this.vertices[num2++].Position = new Vector3(value3 + value4, 0f);
						this.vertices[num2++].Position = new Vector3(value3 - value4, 0f);
						this.vertices[num2++].Position = new Vector3(vector3 - vector4, 0f);
						this.vertices[num2++].Position = new Vector3(vector3 - vector4, 0f);
						this.vertices[num2++].Position = new Vector3(value3 + value4, 0f);
						this.vertices[num2++].Position = new Vector3(vector3 + vector4, 0f);
						value3 = vector3;
						value4 = vector4;
					}
					vector += value * this.tentacles[i].Width * 0.5f;
				}
				this.vertexCount = num2;
			}
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x00055469 File Offset: 0x00053669
		public override void Render(Scene scene)
		{
			if (this.vertexCount > 0)
			{
				GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.vertices, this.vertexCount, null, null);
			}
		}

		// Token: 0x04000CE8 RID: 3304
		private const int NodesPerTentacle = 10;

		// Token: 0x04000CE9 RID: 3305
		private Tentacles.Side side;

		// Token: 0x04000CEA RID: 3306
		private float width;

		// Token: 0x04000CEB RID: 3307
		private Vector2 origin;

		// Token: 0x04000CEC RID: 3308
		private Vector2 outwards;

		// Token: 0x04000CED RID: 3309
		private float outwardsOffset;

		// Token: 0x04000CEE RID: 3310
		private VertexPositionColor[] vertices;

		// Token: 0x04000CEF RID: 3311
		private int vertexCount;

		// Token: 0x04000CF0 RID: 3312
		private Tentacles.Tentacle[] tentacles;

		// Token: 0x04000CF1 RID: 3313
		private int tentacleCount;

		// Token: 0x04000CF2 RID: 3314
		private float hideTimer = 5f;

		// Token: 0x02000522 RID: 1314
		public enum Side
		{
			// Token: 0x04002533 RID: 9523
			Right,
			// Token: 0x04002534 RID: 9524
			Left,
			// Token: 0x04002535 RID: 9525
			Top,
			// Token: 0x04002536 RID: 9526
			Bottom
		}

		// Token: 0x02000523 RID: 1315
		private struct Tentacle
		{
			// Token: 0x04002537 RID: 9527
			public float Length;

			// Token: 0x04002538 RID: 9528
			public float Offset;

			// Token: 0x04002539 RID: 9529
			public float Step;

			// Token: 0x0400253A RID: 9530
			public float Position;

			// Token: 0x0400253B RID: 9531
			public float Approach;

			// Token: 0x0400253C RID: 9532
			public float Width;
		}
	}
}

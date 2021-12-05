using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000279 RID: 633
	public class Flagline : Component
	{
		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06001397 RID: 5015 RVA: 0x0006A9FF File Offset: 0x00068BFF
		public Vector2 From
		{
			get
			{
				return base.Entity.Position;
			}
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x0006AA0C File Offset: 0x00068C0C
		public Flagline(Vector2 to, Color lineColor, Color pinColor, Color[] colors, int minFlagHeight, int maxFlagHeight, int minFlagLength, int maxFlagLength, int minSpace, int maxSpace) : base(true, true)
		{
			this.To = to;
			this.colors = colors;
			this.lineColor = lineColor;
			this.pinColor = pinColor;
			this.waveTimer = Calc.Random.NextFloat() * 6.2831855f;
			this.highlights = new Color[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				this.highlights[i] = Color.Lerp(colors[i], Color.White, 0.1f);
			}
			this.clothes = new Flagline.Cloth[10];
			for (int j = 0; j < this.clothes.Length; j++)
			{
				this.clothes[j] = new Flagline.Cloth
				{
					Color = Calc.Random.Next(colors.Length),
					Height = Calc.Random.Next(minFlagHeight, maxFlagHeight),
					Length = Calc.Random.Next(minFlagLength, maxFlagLength),
					Step = Calc.Random.Next(minSpace, maxSpace)
				};
			}
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x0006AB29 File Offset: 0x00068D29
		public override void Update()
		{
			this.waveTimer += Engine.DeltaTime;
			base.Update();
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x0006AB44 File Offset: 0x00068D44
		public override void Render()
		{
			Vector2 vector = (this.From.X < this.To.X) ? this.From : this.To;
			Vector2 vector2 = (this.From.X < this.To.X) ? this.To : this.From;
			float num = (vector - vector2).Length();
			float num2 = num / 8f;
			SimpleCurve simpleCurve = new SimpleCurve(vector, vector2, (vector2 + vector) / 2f + Vector2.UnitY * (num2 + (float)Math.Sin((double)this.waveTimer) * num2 * 0.3f));
			Vector2 vector3 = vector;
			float num3 = 0f;
			int num4 = 0;
			bool flag = false;
			while (num3 < 1f)
			{
				Flagline.Cloth cloth = this.clothes[num4 % this.clothes.Length];
				num3 += (float)(flag ? cloth.Length : cloth.Step) / num;
				Vector2 point = simpleCurve.GetPoint(num3);
				Draw.Line(vector3, point, this.lineColor);
				if (num3 < 1f && flag)
				{
					float num5 = (float)cloth.Length * this.ClothDroopAmount;
					SimpleCurve simpleCurve2 = new SimpleCurve(vector3, point, (vector3 + point) / 2f + new Vector2(0f, num5 + (float)Math.Sin((double)(this.waveTimer * 2f + num3)) * num5 * 0.4f));
					Vector2 vector4 = vector3;
					for (float num6 = 1f; num6 <= (float)cloth.Length; num6 += 1f)
					{
						Vector2 point2 = simpleCurve2.GetPoint(num6 / (float)cloth.Length);
						if (point2.X != vector4.X)
						{
							Draw.Rect(vector4.X, vector4.Y, point2.X - vector4.X + 1f, (float)cloth.Height, this.colors[cloth.Color]);
							vector4 = point2;
						}
					}
					Draw.Rect(vector3.X, vector3.Y, 1f, (float)cloth.Height, this.highlights[cloth.Color]);
					Draw.Rect(point.X, point.Y, 1f, (float)cloth.Height, this.highlights[cloth.Color]);
					Draw.Rect(vector3.X, vector3.Y - 1f, 1f, 3f, this.pinColor);
					Draw.Rect(point.X, point.Y - 1f, 1f, 3f, this.pinColor);
					num4++;
				}
				vector3 = point;
				flag = !flag;
			}
		}

		// Token: 0x04000F6B RID: 3947
		private Color[] colors;

		// Token: 0x04000F6C RID: 3948
		private Color[] highlights;

		// Token: 0x04000F6D RID: 3949
		private Color lineColor;

		// Token: 0x04000F6E RID: 3950
		private Color pinColor;

		// Token: 0x04000F6F RID: 3951
		private Flagline.Cloth[] clothes;

		// Token: 0x04000F70 RID: 3952
		private float waveTimer;

		// Token: 0x04000F71 RID: 3953
		public float ClothDroopAmount = 0.6f;

		// Token: 0x04000F72 RID: 3954
		public Vector2 To;

		// Token: 0x020005BA RID: 1466
		private struct Cloth
		{
			// Token: 0x040027C0 RID: 10176
			public int Color;

			// Token: 0x040027C1 RID: 10177
			public int Height;

			// Token: 0x040027C2 RID: 10178
			public int Length;

			// Token: 0x040027C3 RID: 10179
			public int Step;
		}
	}
}

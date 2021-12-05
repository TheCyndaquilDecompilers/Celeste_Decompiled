using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000296 RID: 662
	public class DreamWipe : ScreenWipe
	{
		// Token: 0x06001477 RID: 5239 RVA: 0x0006FAAC File Offset: 0x0006DCAC
		public DreamWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			if (DreamWipe.vertexBuffer == null)
			{
				DreamWipe.vertexBuffer = new VertexPositionColor[(this.circleColumns + 2) * (this.circleRows + 2) * 32 * 3];
			}
			if (DreamWipe.circles == null)
			{
				DreamWipe.circles = new DreamWipe.Circle[(this.circleColumns + 2) * (this.circleRows + 2)];
			}
			for (int i = 0; i < DreamWipe.vertexBuffer.Length; i++)
			{
				DreamWipe.vertexBuffer[i].Color = ScreenWipe.WipeColor;
			}
			int num = 1920 / this.circleColumns;
			int num2 = 1080 / this.circleRows;
			int j = 0;
			int num3 = 0;
			while (j < this.circleColumns + 2)
			{
				for (int k = 0; k < this.circleRows + 2; k++)
				{
					DreamWipe.circles[num3].Position = new Vector2(((float)(j - 1) + 0.2f + Calc.Random.NextFloat(0.6f)) * (float)num, ((float)(k - 1) + 0.2f + Calc.Random.NextFloat(0.6f)) * (float)num2);
					DreamWipe.circles[num3].Delay = Calc.Random.NextFloat(0.05f) + (float)(this.WipeIn ? (this.circleColumns - j) : j) * 0.018f;
					DreamWipe.circles[num3].Radius = (this.WipeIn ? (400f * (this.Duration - DreamWipe.circles[num3].Delay)) : 0f);
					num3++;
				}
				j++;
			}
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x0006FC64 File Offset: 0x0006DE64
		public override void Update(Scene scene)
		{
			base.Update(scene);
			for (int i = 0; i < DreamWipe.circles.Length; i++)
			{
				if (!this.WipeIn)
				{
					DreamWipe.Circle[] array = DreamWipe.circles;
					int num = i;
					array[num].Delay = array[num].Delay - Engine.DeltaTime;
					if (DreamWipe.circles[i].Delay <= 0f)
					{
						DreamWipe.Circle[] array2 = DreamWipe.circles;
						int num2 = i;
						array2[num2].Radius = array2[num2].Radius + Engine.DeltaTime * 400f;
					}
				}
				else if (DreamWipe.circles[i].Radius > 0f)
				{
					DreamWipe.Circle[] array3 = DreamWipe.circles;
					int num3 = i;
					array3[num3].Radius = array3[num3].Radius - Engine.DeltaTime * 400f;
				}
				else
				{
					DreamWipe.circles[i].Radius = 0f;
				}
			}
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0006FD38 File Offset: 0x0006DF38
		public override void Render(Scene scene)
		{
			int num = 0;
			for (int i = 0; i < DreamWipe.circles.Length; i++)
			{
				DreamWipe.Circle circle = DreamWipe.circles[i];
				Vector2 value = new Vector2(1f, 0f);
				for (float num2 = 0f; num2 < 32f; num2 += 1f)
				{
					Vector2 vector = Calc.AngleToVector((num2 + 1f) / 32f * 6.2831855f, 1f);
					DreamWipe.vertexBuffer[num++].Position = new Vector3(circle.Position, 0f);
					DreamWipe.vertexBuffer[num++].Position = new Vector3(circle.Position + value * circle.Radius, 0f);
					DreamWipe.vertexBuffer[num++].Position = new Vector3(circle.Position + vector * circle.Radius, 0f);
					value = vector;
				}
			}
			ScreenWipe.DrawPrimitives(DreamWipe.vertexBuffer);
		}

		// Token: 0x04001026 RID: 4134
		private readonly int circleColumns = 15;

		// Token: 0x04001027 RID: 4135
		private readonly int circleRows = 8;

		// Token: 0x04001028 RID: 4136
		private const int circleSegments = 32;

		// Token: 0x04001029 RID: 4137
		private const float circleFillSpeed = 400f;

		// Token: 0x0400102A RID: 4138
		private static DreamWipe.Circle[] circles;

		// Token: 0x0400102B RID: 4139
		private static VertexPositionColor[] vertexBuffer;

		// Token: 0x02000612 RID: 1554
		private struct Circle
		{
			// Token: 0x04002926 RID: 10534
			public Vector2 Position;

			// Token: 0x04002927 RID: 10535
			public float Radius;

			// Token: 0x04002928 RID: 10536
			public float Delay;
		}
	}
}

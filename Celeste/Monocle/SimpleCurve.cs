using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000134 RID: 308
	public struct SimpleCurve
	{
		// Token: 0x06000B16 RID: 2838 RVA: 0x0001E844 File Offset: 0x0001CA44
		public SimpleCurve(Vector2 begin, Vector2 end, Vector2 control)
		{
			this.Begin = begin;
			this.End = end;
			this.Control = control;
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0001E85C File Offset: 0x0001CA5C
		public void DoubleControl()
		{
			Vector2 value = this.End - this.Begin;
			Vector2 value2 = this.Begin + value / 2f;
			Vector2 value3 = this.Control - value2;
			this.Control += value3;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0001E8B4 File Offset: 0x0001CAB4
		public Vector2 GetPoint(float percent)
		{
			float num = 1f - percent;
			return num * num * this.Begin + 2f * num * percent * this.Control + percent * percent * this.End;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0001E904 File Offset: 0x0001CB04
		public float GetLengthParametric(int resolution)
		{
			Vector2 value = this.Begin;
			float num = 0f;
			for (int i = 1; i <= resolution; i++)
			{
				Vector2 point = this.GetPoint((float)i / (float)resolution);
				num += (point - value).Length();
				value = point;
			}
			return num;
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0001E94C File Offset: 0x0001CB4C
		public void Render(Vector2 offset, Color color, int resolution)
		{
			Vector2 start = offset + this.Begin;
			for (int i = 1; i <= resolution; i++)
			{
				Vector2 vector = offset + this.GetPoint((float)i / (float)resolution);
				Draw.Line(start, vector, color);
				start = vector;
			}
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0001E990 File Offset: 0x0001CB90
		public void Render(Vector2 offset, Color color, int resolution, float thickness)
		{
			Vector2 start = offset + this.Begin;
			for (int i = 1; i <= resolution; i++)
			{
				Vector2 vector = offset + this.GetPoint((float)i / (float)resolution);
				Draw.Line(start, vector, color, thickness);
				start = vector;
			}
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x0001E9D4 File Offset: 0x0001CBD4
		public void Render(Color color, int resolution)
		{
			this.Render(Vector2.Zero, color, resolution);
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0001E9E3 File Offset: 0x0001CBE3
		public void Render(Color color, int resolution, float thickness)
		{
			this.Render(Vector2.Zero, color, resolution, thickness);
		}

		// Token: 0x0400069D RID: 1693
		public Vector2 Begin;

		// Token: 0x0400069E RID: 1694
		public Vector2 End;

		// Token: 0x0400069F RID: 1695
		public Vector2 Control;
	}
}

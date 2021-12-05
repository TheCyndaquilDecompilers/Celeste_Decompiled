using System;

namespace Monocle
{
	// Token: 0x02000130 RID: 304
	public struct Pnt
	{
		// Token: 0x06000AF7 RID: 2807 RVA: 0x0001E258 File Offset: 0x0001C458
		public Pnt(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x0001E268 File Offset: 0x0001C468
		public static bool operator ==(Pnt a, Pnt b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x0001E288 File Offset: 0x0001C488
		public static bool operator !=(Pnt a, Pnt b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0001E2AB File Offset: 0x0001C4AB
		public static Pnt operator +(Pnt a, Pnt b)
		{
			return new Pnt(a.X + b.X, a.Y + b.Y);
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x0001E2CC File Offset: 0x0001C4CC
		public static Pnt operator -(Pnt a, Pnt b)
		{
			return new Pnt(a.X - b.X, a.Y - b.Y);
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0001E2ED File Offset: 0x0001C4ED
		public static Pnt operator *(Pnt a, Pnt b)
		{
			return new Pnt(a.X * b.X, a.Y * b.Y);
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x0001E30E File Offset: 0x0001C50E
		public static Pnt operator /(Pnt a, Pnt b)
		{
			return new Pnt(a.X / b.X, a.Y / b.Y);
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x0001E32F File Offset: 0x0001C52F
		public static Pnt operator %(Pnt a, Pnt b)
		{
			return new Pnt(a.X % b.X, a.Y % b.Y);
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x0001E350 File Offset: 0x0001C550
		public static bool operator ==(Pnt a, int b)
		{
			return a.X == b && a.Y == b;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x0001E366 File Offset: 0x0001C566
		public static bool operator !=(Pnt a, int b)
		{
			return a.X != b || a.Y != b;
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x0001E37F File Offset: 0x0001C57F
		public static Pnt operator +(Pnt a, int b)
		{
			return new Pnt(a.X + b, a.Y + b);
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x0001E396 File Offset: 0x0001C596
		public static Pnt operator -(Pnt a, int b)
		{
			return new Pnt(a.X - b, a.Y - b);
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x0001E3AD File Offset: 0x0001C5AD
		public static Pnt operator *(Pnt a, int b)
		{
			return new Pnt(a.X * b, a.Y * b);
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x0001E3C4 File Offset: 0x0001C5C4
		public static Pnt operator /(Pnt a, int b)
		{
			return new Pnt(a.X / b, a.Y / b);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x0001E3DB File Offset: 0x0001C5DB
		public static Pnt operator %(Pnt a, int b)
		{
			return new Pnt(a.X % b, a.Y % b);
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool Equals(object obj)
		{
			return false;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x0001E3F2 File Offset: 0x0001C5F2
		public override int GetHashCode()
		{
			return this.X * 10000 + this.Y;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0001E408 File Offset: 0x0001C608
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"{ X: ",
				this.X,
				", Y: ",
				this.Y,
				" }"
			});
		}

		// Token: 0x04000696 RID: 1686
		public static readonly Pnt Zero = new Pnt(0, 0);

		// Token: 0x04000697 RID: 1687
		public static readonly Pnt UnitX = new Pnt(1, 0);

		// Token: 0x04000698 RID: 1688
		public static readonly Pnt UnitY = new Pnt(0, 1);

		// Token: 0x04000699 RID: 1689
		public static readonly Pnt One = new Pnt(1, 1);

		// Token: 0x0400069A RID: 1690
		public int X;

		// Token: 0x0400069B RID: 1691
		public int Y;
	}
}

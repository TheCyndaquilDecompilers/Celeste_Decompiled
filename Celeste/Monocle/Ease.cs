using System;

namespace Monocle
{
	// Token: 0x0200012A RID: 298
	public static class Ease
	{
		// Token: 0x06000AD4 RID: 2772 RVA: 0x0001CE4E File Offset: 0x0001B04E
		public static Ease.Easer Invert(Ease.Easer easer)
		{
			return (float t) => 1f - easer(1f - t);
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x0001CE67 File Offset: 0x0001B067
		public static Ease.Easer Follow(Ease.Easer first, Ease.Easer second)
		{
			return delegate(float t)
			{
				if (t > 0.5f)
				{
					return second(t * 2f - 1f) / 2f + 0.5f;
				}
				return first(t * 2f) / 2f;
			};
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x00017C9F File Offset: 0x00015E9F
		public static float UpDown(float eased)
		{
			if (eased <= 0.5f)
			{
				return eased * 2f;
			}
			return 1f - (eased - 0.5f) * 2f;
		}

		// Token: 0x04000662 RID: 1634
		public static readonly Ease.Easer Linear = (float t) => t;

		// Token: 0x04000663 RID: 1635
		public static readonly Ease.Easer SineIn = (float t) => -(float)Math.Cos((double)(1.5707964f * t)) + 1f;

		// Token: 0x04000664 RID: 1636
		public static readonly Ease.Easer SineOut = (float t) => (float)Math.Sin((double)(1.5707964f * t));

		// Token: 0x04000665 RID: 1637
		public static readonly Ease.Easer SineInOut = (float t) => -(float)Math.Cos((double)(3.1415927f * t)) / 2f + 0.5f;

		// Token: 0x04000666 RID: 1638
		public static readonly Ease.Easer QuadIn = (float t) => t * t;

		// Token: 0x04000667 RID: 1639
		public static readonly Ease.Easer QuadOut = Ease.Invert(Ease.QuadIn);

		// Token: 0x04000668 RID: 1640
		public static readonly Ease.Easer QuadInOut = Ease.Follow(Ease.QuadIn, Ease.QuadOut);

		// Token: 0x04000669 RID: 1641
		public static readonly Ease.Easer CubeIn = (float t) => t * t * t;

		// Token: 0x0400066A RID: 1642
		public static readonly Ease.Easer CubeOut = Ease.Invert(Ease.CubeIn);

		// Token: 0x0400066B RID: 1643
		public static readonly Ease.Easer CubeInOut = Ease.Follow(Ease.CubeIn, Ease.CubeOut);

		// Token: 0x0400066C RID: 1644
		public static readonly Ease.Easer QuintIn = (float t) => t * t * t * t * t;

		// Token: 0x0400066D RID: 1645
		public static readonly Ease.Easer QuintOut = Ease.Invert(Ease.QuintIn);

		// Token: 0x0400066E RID: 1646
		public static readonly Ease.Easer QuintInOut = Ease.Follow(Ease.QuintIn, Ease.QuintOut);

		// Token: 0x0400066F RID: 1647
		public static readonly Ease.Easer ExpoIn = (float t) => (float)Math.Pow(2.0, (double)(10f * (t - 1f)));

		// Token: 0x04000670 RID: 1648
		public static readonly Ease.Easer ExpoOut = Ease.Invert(Ease.ExpoIn);

		// Token: 0x04000671 RID: 1649
		public static readonly Ease.Easer ExpoInOut = Ease.Follow(Ease.ExpoIn, Ease.ExpoOut);

		// Token: 0x04000672 RID: 1650
		public static readonly Ease.Easer BackIn = (float t) => t * t * (2.70158f * t - 1.70158f);

		// Token: 0x04000673 RID: 1651
		public static readonly Ease.Easer BackOut = Ease.Invert(Ease.BackIn);

		// Token: 0x04000674 RID: 1652
		public static readonly Ease.Easer BackInOut = Ease.Follow(Ease.BackIn, Ease.BackOut);

		// Token: 0x04000675 RID: 1653
		public static readonly Ease.Easer BigBackIn = (float t) => t * t * (4f * t - 3f);

		// Token: 0x04000676 RID: 1654
		public static readonly Ease.Easer BigBackOut = Ease.Invert(Ease.BigBackIn);

		// Token: 0x04000677 RID: 1655
		public static readonly Ease.Easer BigBackInOut = Ease.Follow(Ease.BigBackIn, Ease.BigBackOut);

		// Token: 0x04000678 RID: 1656
		public static readonly Ease.Easer ElasticIn = delegate(float t)
		{
			float num = t * t;
			float num2 = num * t;
			return 33f * num2 * num + -59f * num * num + 32f * num2 + -5f * num;
		};

		// Token: 0x04000679 RID: 1657
		public static readonly Ease.Easer ElasticOut = delegate(float t)
		{
			float num = t * t;
			float num2 = num * t;
			return 33f * num2 * num + -106f * num * num + 126f * num2 + -67f * num + 15f * t;
		};

		// Token: 0x0400067A RID: 1658
		public static readonly Ease.Easer ElasticInOut = Ease.Follow(Ease.ElasticIn, Ease.ElasticOut);

		// Token: 0x0400067B RID: 1659
		private const float B1 = 0.36363637f;

		// Token: 0x0400067C RID: 1660
		private const float B2 = 0.72727275f;

		// Token: 0x0400067D RID: 1661
		private const float B3 = 0.54545456f;

		// Token: 0x0400067E RID: 1662
		private const float B4 = 0.90909094f;

		// Token: 0x0400067F RID: 1663
		private const float B5 = 0.8181818f;

		// Token: 0x04000680 RID: 1664
		private const float B6 = 0.95454544f;

		// Token: 0x04000681 RID: 1665
		public static readonly Ease.Easer BounceIn = delegate(float t)
		{
			t = 1f - t;
			if (t < 0.36363637f)
			{
				return 1f - 7.5625f * t * t;
			}
			if (t < 0.72727275f)
			{
				return 1f - (7.5625f * (t - 0.54545456f) * (t - 0.54545456f) + 0.75f);
			}
			if (t < 0.90909094f)
			{
				return 1f - (7.5625f * (t - 0.8181818f) * (t - 0.8181818f) + 0.9375f);
			}
			return 1f - (7.5625f * (t - 0.95454544f) * (t - 0.95454544f) + 0.984375f);
		};

		// Token: 0x04000682 RID: 1666
		public static readonly Ease.Easer BounceOut = delegate(float t)
		{
			if (t < 0.36363637f)
			{
				return 7.5625f * t * t;
			}
			if (t < 0.72727275f)
			{
				return 7.5625f * (t - 0.54545456f) * (t - 0.54545456f) + 0.75f;
			}
			if (t < 0.90909094f)
			{
				return 7.5625f * (t - 0.8181818f) * (t - 0.8181818f) + 0.9375f;
			}
			return 7.5625f * (t - 0.95454544f) * (t - 0.95454544f) + 0.984375f;
		};

		// Token: 0x04000683 RID: 1667
		public static readonly Ease.Easer BounceInOut = delegate(float t)
		{
			if (t < 0.5f)
			{
				t = 1f - t * 2f;
				if (t < 0.36363637f)
				{
					return (1f - 7.5625f * t * t) / 2f;
				}
				if (t < 0.72727275f)
				{
					return (1f - (7.5625f * (t - 0.54545456f) * (t - 0.54545456f) + 0.75f)) / 2f;
				}
				if (t < 0.90909094f)
				{
					return (1f - (7.5625f * (t - 0.8181818f) * (t - 0.8181818f) + 0.9375f)) / 2f;
				}
				return (1f - (7.5625f * (t - 0.95454544f) * (t - 0.95454544f) + 0.984375f)) / 2f;
			}
			else
			{
				t = t * 2f - 1f;
				if (t < 0.36363637f)
				{
					return 7.5625f * t * t / 2f + 0.5f;
				}
				if (t < 0.72727275f)
				{
					return (7.5625f * (t - 0.54545456f) * (t - 0.54545456f) + 0.75f) / 2f + 0.5f;
				}
				if (t < 0.90909094f)
				{
					return (7.5625f * (t - 0.8181818f) * (t - 0.8181818f) + 0.9375f) / 2f + 0.5f;
				}
				return (7.5625f * (t - 0.95454544f) * (t - 0.95454544f) + 0.984375f) / 2f + 0.5f;
			}
		};

		// Token: 0x020003BA RID: 954
		// (Invoke) Token: 0x06001EAE RID: 7854
		public delegate float Easer(float t);
	}
}

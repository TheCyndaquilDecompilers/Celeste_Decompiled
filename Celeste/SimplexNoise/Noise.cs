using System;

namespace SimplexNoise
{
	// Token: 0x02000002 RID: 2
	public class Noise
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static float Generate(float x)
		{
			int num = Noise.FastFloor(x);
			int num2 = num + 1;
			float num3 = x - (float)num;
			float num4 = num3 - 1f;
			float num5 = 1f - num3 * num3;
			float num6 = num5 * num5;
			float num7 = num6 * num6 * Noise.grad((int)Noise.perm[num & 255], num3);
			float num8 = 1f - num4 * num4;
			float num9 = num8 * num8;
			float num10 = num9 * num9 * Noise.grad((int)Noise.perm[num2 & 255], num4);
			return 0.395f * (num7 + num10);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020C8 File Offset: 0x000002C8
		public static float Generate(float x, float y)
		{
			float num = (x + y) * 0.3660254f;
			float x2 = x + num;
			float x3 = y + num;
			int num2 = Noise.FastFloor(x2);
			int num3 = Noise.FastFloor(x3);
			float num4 = (float)(num2 + num3) * 0.21132487f;
			float num5 = (float)num2 - num4;
			float num6 = (float)num3 - num4;
			float num7 = x - num5;
			float num8 = y - num6;
			int num9;
			int num10;
			if (num7 > num8)
			{
				num9 = 1;
				num10 = 0;
			}
			else
			{
				num9 = 0;
				num10 = 1;
			}
			float num11 = num7 - (float)num9 + 0.21132487f;
			float num12 = num8 - (float)num10 + 0.21132487f;
			float num13 = num7 - 1f + 0.42264974f;
			float num14 = num8 - 1f + 0.42264974f;
			int num15 = num2 % 256;
			int num16 = num3 % 256;
			float num17 = 0.5f - num7 * num7 - num8 * num8;
			float num18;
			if (num17 < 0f)
			{
				num18 = 0f;
			}
			else
			{
				num17 *= num17;
				num18 = num17 * num17 * Noise.grad((int)Noise.perm[num15 + (int)Noise.perm[num16]], num7, num8);
			}
			float num19 = 0.5f - num11 * num11 - num12 * num12;
			float num20;
			if (num19 < 0f)
			{
				num20 = 0f;
			}
			else
			{
				num19 *= num19;
				num20 = num19 * num19 * Noise.grad((int)Noise.perm[num15 + num9 + (int)Noise.perm[num16 + num10]], num11, num12);
			}
			float num21 = 0.5f - num13 * num13 - num14 * num14;
			float num22;
			if (num21 < 0f)
			{
				num22 = 0f;
			}
			else
			{
				num21 *= num21;
				num22 = num21 * num21 * Noise.grad((int)Noise.perm[num15 + 1 + (int)Noise.perm[num16 + 1]], num13, num14);
			}
			return 40f * (num18 + num20 + num22);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002278 File Offset: 0x00000478
		public static float Generate(float x, float y, float z)
		{
			float num = (x + y + z) * 0.33333334f;
			float x2 = x + num;
			float x3 = y + num;
			float x4 = z + num;
			int num2 = Noise.FastFloor(x2);
			int num3 = Noise.FastFloor(x3);
			int num4 = Noise.FastFloor(x4);
			float num5 = (float)(num2 + num3 + num4) * 0.16666667f;
			float num6 = (float)num2 - num5;
			float num7 = (float)num3 - num5;
			float num8 = (float)num4 - num5;
			float num9 = x - num6;
			float num10 = y - num7;
			float num11 = z - num8;
			int num12;
			int num13;
			int num14;
			int num15;
			int num16;
			int num17;
			if (num9 >= num10)
			{
				if (num10 >= num11)
				{
					num12 = 1;
					num13 = 0;
					num14 = 0;
					num15 = 1;
					num16 = 1;
					num17 = 0;
				}
				else if (num9 >= num11)
				{
					num12 = 1;
					num13 = 0;
					num14 = 0;
					num15 = 1;
					num16 = 0;
					num17 = 1;
				}
				else
				{
					num12 = 0;
					num13 = 0;
					num14 = 1;
					num15 = 1;
					num16 = 0;
					num17 = 1;
				}
			}
			else if (num10 < num11)
			{
				num12 = 0;
				num13 = 0;
				num14 = 1;
				num15 = 0;
				num16 = 1;
				num17 = 1;
			}
			else if (num9 < num11)
			{
				num12 = 0;
				num13 = 1;
				num14 = 0;
				num15 = 0;
				num16 = 1;
				num17 = 1;
			}
			else
			{
				num12 = 0;
				num13 = 1;
				num14 = 0;
				num15 = 1;
				num16 = 1;
				num17 = 0;
			}
			float num18 = num9 - (float)num12 + 0.16666667f;
			float num19 = num10 - (float)num13 + 0.16666667f;
			float num20 = num11 - (float)num14 + 0.16666667f;
			float num21 = num9 - (float)num15 + 0.33333334f;
			float num22 = num10 - (float)num16 + 0.33333334f;
			float num23 = num11 - (float)num17 + 0.33333334f;
			float num24 = num9 - 1f + 0.5f;
			float num25 = num10 - 1f + 0.5f;
			float num26 = num11 - 1f + 0.5f;
			int num27 = Noise.Mod(num2, 256);
			int num28 = Noise.Mod(num3, 256);
			int num29 = Noise.Mod(num4, 256);
			float num30 = 0.6f - num9 * num9 - num10 * num10 - num11 * num11;
			float num31;
			if (num30 < 0f)
			{
				num31 = 0f;
			}
			else
			{
				num30 *= num30;
				num31 = num30 * num30 * Noise.grad((int)Noise.perm[num27 + (int)Noise.perm[num28 + (int)Noise.perm[num29]]], num9, num10, num11);
			}
			float num32 = 0.6f - num18 * num18 - num19 * num19 - num20 * num20;
			float num33;
			if (num32 < 0f)
			{
				num33 = 0f;
			}
			else
			{
				num32 *= num32;
				num33 = num32 * num32 * Noise.grad((int)Noise.perm[num27 + num12 + (int)Noise.perm[num28 + num13 + (int)Noise.perm[num29 + num14]]], num18, num19, num20);
			}
			float num34 = 0.6f - num21 * num21 - num22 * num22 - num23 * num23;
			float num35;
			if (num34 < 0f)
			{
				num35 = 0f;
			}
			else
			{
				num34 *= num34;
				num35 = num34 * num34 * Noise.grad((int)Noise.perm[num27 + num15 + (int)Noise.perm[num28 + num16 + (int)Noise.perm[num29 + num17]]], num21, num22, num23);
			}
			float num36 = 0.6f - num24 * num24 - num25 * num25 - num26 * num26;
			float num37;
			if (num36 < 0f)
			{
				num37 = 0f;
			}
			else
			{
				num36 *= num36;
				num37 = num36 * num36 * Noise.grad((int)Noise.perm[num27 + 1 + (int)Noise.perm[num28 + 1 + (int)Noise.perm[num29 + 1]]], num24, num25, num26);
			}
			return 32f * (num31 + num33 + num35 + num37);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000025CB File Offset: 0x000007CB
		private static int FastFloor(float x)
		{
			if (x <= 0f)
			{
				return (int)x - 1;
			}
			return (int)x;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000025DC File Offset: 0x000007DC
		private static int Mod(int x, int m)
		{
			int num = x % m;
			if (num >= 0)
			{
				return num;
			}
			return num + m;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000025F8 File Offset: 0x000007F8
		private static float grad(int hash, float x)
		{
			int num = hash & 15;
			float num2 = 1f + (float)(num & 7);
			if ((num & 8) != 0)
			{
				num2 = -num2;
			}
			return num2 * x;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002620 File Offset: 0x00000820
		private static float grad(int hash, float x, float y)
		{
			int num = hash & 7;
			float num2 = (num < 4) ? x : y;
			float num3 = (num < 4) ? y : x;
			return (((num & 1) != 0) ? (-num2) : num2) + (((num & 2) != 0) ? (-2f * num3) : (2f * num3));
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002664 File Offset: 0x00000864
		private static float grad(int hash, float x, float y, float z)
		{
			int num = hash & 15;
			float num2 = (num < 8) ? x : y;
			float num3 = (num < 4) ? y : ((num == 12 || num == 14) ? x : z);
			return (((num & 1) != 0) ? (-num2) : num2) + (((num & 2) != 0) ? (-num3) : num3);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000026AC File Offset: 0x000008AC
		private static float grad(int hash, float x, float y, float z, float t)
		{
			int num = hash & 31;
			float num2 = (num < 24) ? x : y;
			float num3 = (num < 16) ? y : z;
			float num4 = (num < 8) ? z : t;
			return (((num & 1) != 0) ? (-num2) : num2) + (((num & 2) != 0) ? (-num3) : num3) + (((num & 4) != 0) ? (-num4) : num4);
		}

		// Token: 0x04000001 RID: 1
		public static byte[] perm = new byte[]
		{
			151,
			160,
			137,
			91,
			90,
			15,
			131,
			13,
			201,
			95,
			96,
			53,
			194,
			233,
			7,
			225,
			140,
			36,
			103,
			30,
			69,
			142,
			8,
			99,
			37,
			240,
			21,
			10,
			23,
			190,
			6,
			148,
			247,
			120,
			234,
			75,
			0,
			26,
			197,
			62,
			94,
			252,
			219,
			203,
			117,
			35,
			11,
			32,
			57,
			177,
			33,
			88,
			237,
			149,
			56,
			87,
			174,
			20,
			125,
			136,
			171,
			168,
			68,
			175,
			74,
			165,
			71,
			134,
			139,
			48,
			27,
			166,
			77,
			146,
			158,
			231,
			83,
			111,
			229,
			122,
			60,
			211,
			133,
			230,
			220,
			105,
			92,
			41,
			55,
			46,
			245,
			40,
			244,
			102,
			143,
			54,
			65,
			25,
			63,
			161,
			1,
			216,
			80,
			73,
			209,
			76,
			132,
			187,
			208,
			89,
			18,
			169,
			200,
			196,
			135,
			130,
			116,
			188,
			159,
			86,
			164,
			100,
			109,
			198,
			173,
			186,
			3,
			64,
			52,
			217,
			226,
			250,
			124,
			123,
			5,
			202,
			38,
			147,
			118,
			126,
			byte.MaxValue,
			82,
			85,
			212,
			207,
			206,
			59,
			227,
			47,
			16,
			58,
			17,
			182,
			189,
			28,
			42,
			223,
			183,
			170,
			213,
			119,
			248,
			152,
			2,
			44,
			154,
			163,
			70,
			221,
			153,
			101,
			155,
			167,
			43,
			172,
			9,
			129,
			22,
			39,
			253,
			19,
			98,
			108,
			110,
			79,
			113,
			224,
			232,
			178,
			185,
			112,
			104,
			218,
			246,
			97,
			228,
			251,
			34,
			242,
			193,
			238,
			210,
			144,
			12,
			191,
			179,
			162,
			241,
			81,
			51,
			145,
			235,
			249,
			14,
			239,
			107,
			49,
			192,
			214,
			31,
			181,
			199,
			106,
			157,
			184,
			84,
			204,
			176,
			115,
			121,
			50,
			45,
			127,
			4,
			150,
			254,
			138,
			236,
			205,
			93,
			222,
			114,
			67,
			29,
			24,
			72,
			243,
			141,
			128,
			195,
			78,
			66,
			215,
			61,
			156,
			180,
			151,
			160,
			137,
			91,
			90,
			15,
			131,
			13,
			201,
			95,
			96,
			53,
			194,
			233,
			7,
			225,
			140,
			36,
			103,
			30,
			69,
			142,
			8,
			99,
			37,
			240,
			21,
			10,
			23,
			190,
			6,
			148,
			247,
			120,
			234,
			75,
			0,
			26,
			197,
			62,
			94,
			252,
			219,
			203,
			117,
			35,
			11,
			32,
			57,
			177,
			33,
			88,
			237,
			149,
			56,
			87,
			174,
			20,
			125,
			136,
			171,
			168,
			68,
			175,
			74,
			165,
			71,
			134,
			139,
			48,
			27,
			166,
			77,
			146,
			158,
			231,
			83,
			111,
			229,
			122,
			60,
			211,
			133,
			230,
			220,
			105,
			92,
			41,
			55,
			46,
			245,
			40,
			244,
			102,
			143,
			54,
			65,
			25,
			63,
			161,
			1,
			216,
			80,
			73,
			209,
			76,
			132,
			187,
			208,
			89,
			18,
			169,
			200,
			196,
			135,
			130,
			116,
			188,
			159,
			86,
			164,
			100,
			109,
			198,
			173,
			186,
			3,
			64,
			52,
			217,
			226,
			250,
			124,
			123,
			5,
			202,
			38,
			147,
			118,
			126,
			byte.MaxValue,
			82,
			85,
			212,
			207,
			206,
			59,
			227,
			47,
			16,
			58,
			17,
			182,
			189,
			28,
			42,
			223,
			183,
			170,
			213,
			119,
			248,
			152,
			2,
			44,
			154,
			163,
			70,
			221,
			153,
			101,
			155,
			167,
			43,
			172,
			9,
			129,
			22,
			39,
			253,
			19,
			98,
			108,
			110,
			79,
			113,
			224,
			232,
			178,
			185,
			112,
			104,
			218,
			246,
			97,
			228,
			251,
			34,
			242,
			193,
			238,
			210,
			144,
			12,
			191,
			179,
			162,
			241,
			81,
			51,
			145,
			235,
			249,
			14,
			239,
			107,
			49,
			192,
			214,
			31,
			181,
			199,
			106,
			157,
			184,
			84,
			204,
			176,
			115,
			121,
			50,
			45,
			127,
			4,
			150,
			254,
			138,
			236,
			205,
			93,
			222,
			114,
			67,
			29,
			24,
			72,
			243,
			141,
			128,
			195,
			78,
			66,
			215,
			61,
			156,
			180
		};
	}
}

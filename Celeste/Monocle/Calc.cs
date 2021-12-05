using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x02000121 RID: 289
	public static class Calc
	{
		// Token: 0x06000963 RID: 2403 RVA: 0x0001703C File Offset: 0x0001523C
		public static int EnumLength(Type e)
		{
			return Enum.GetNames(e).Length;
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00017046 File Offset: 0x00015246
		public static T StringToEnum<T>(string str) where T : struct
		{
			if (Enum.IsDefined(typeof(T), str))
			{
				return (T)((object)Enum.Parse(typeof(T), str));
			}
			throw new Exception("The string cannot be converted to the enum type.");
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0001707C File Offset: 0x0001527C
		public static T[] StringsToEnums<T>(string[] strs) where T : struct
		{
			T[] array = new T[strs.Length];
			for (int i = 0; i < strs.Length; i++)
			{
				array[i] = Calc.StringToEnum<T>(strs[i]);
			}
			return array;
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x000170B0 File Offset: 0x000152B0
		public static bool EnumHasString<T>(string str) where T : struct
		{
			return Enum.IsDefined(typeof(T), str);
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x000170C2 File Offset: 0x000152C2
		public static bool StartsWith(this string str, string match)
		{
			return str.IndexOf(match) == 0;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x000170CE File Offset: 0x000152CE
		public static bool EndsWith(this string str, string match)
		{
			return str.LastIndexOf(match) == str.Length - match.Length;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x000170E8 File Offset: 0x000152E8
		public static bool IsIgnoreCase(this string str, params string[] matches)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			foreach (string value in matches)
			{
				if (str.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x00017120 File Offset: 0x00015320
		public static string ToString(this int num, int minDigits)
		{
			string text = num.ToString();
			while (text.Length < minDigits)
			{
				text = "0" + text;
			}
			return text;
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x00017150 File Offset: 0x00015350
		public static string[] SplitLines(string text, SpriteFont font, int maxLineWidth, char newLine = '\n')
		{
			List<string> list = new List<string>();
			foreach (string text2 in text.Split(new char[]
			{
				newLine
			}))
			{
				string text3 = "";
				foreach (string text4 in text2.Split(new char[]
				{
					' '
				}))
				{
					if (font.MeasureString(text3 + " " + text4).X > (float)maxLineWidth)
					{
						list.Add(text3);
						text3 = text4;
					}
					else
					{
						if (text3 != "")
						{
							text3 += " ";
						}
						text3 += text4;
					}
				}
				list.Add(text3);
			}
			return list.ToArray();
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00017214 File Offset: 0x00015414
		public static int Count<T>(T target, T a, T b)
		{
			int num = 0;
			if (a.Equals(target))
			{
				num++;
			}
			if (b.Equals(target))
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x00017258 File Offset: 0x00015458
		public static int Count<T>(T target, T a, T b, T c)
		{
			int num = 0;
			if (a.Equals(target))
			{
				num++;
			}
			if (b.Equals(target))
			{
				num++;
			}
			if (c.Equals(target))
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x000172B4 File Offset: 0x000154B4
		public static int Count<T>(T target, T a, T b, T c, T d)
		{
			int num = 0;
			if (a.Equals(target))
			{
				num++;
			}
			if (b.Equals(target))
			{
				num++;
			}
			if (c.Equals(target))
			{
				num++;
			}
			if (d.Equals(target))
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x00017328 File Offset: 0x00015528
		public static int Count<T>(T target, T a, T b, T c, T d, T e)
		{
			int num = 0;
			if (a.Equals(target))
			{
				num++;
			}
			if (b.Equals(target))
			{
				num++;
			}
			if (c.Equals(target))
			{
				num++;
			}
			if (d.Equals(target))
			{
				num++;
			}
			if (e.Equals(target))
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x000173B8 File Offset: 0x000155B8
		public static int Count<T>(T target, T a, T b, T c, T d, T e, T f)
		{
			int num = 0;
			if (a.Equals(target))
			{
				num++;
			}
			if (b.Equals(target))
			{
				num++;
			}
			if (c.Equals(target))
			{
				num++;
			}
			if (d.Equals(target))
			{
				num++;
			}
			if (e.Equals(target))
			{
				num++;
			}
			if (f.Equals(target))
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0001745E File Offset: 0x0001565E
		public static T GiveMe<T>(int index, T a, T b)
		{
			if (index == 0)
			{
				return a;
			}
			if (index != 1)
			{
				throw new Exception("Index was out of range!");
			}
			return b;
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x00017475 File Offset: 0x00015675
		public static T GiveMe<T>(int index, T a, T b, T c)
		{
			switch (index)
			{
			case 0:
				return a;
			case 1:
				return b;
			case 2:
				return c;
			default:
				throw new Exception("Index was out of range!");
			}
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x00017499 File Offset: 0x00015699
		public static T GiveMe<T>(int index, T a, T b, T c, T d)
		{
			switch (index)
			{
			case 0:
				return a;
			case 1:
				return b;
			case 2:
				return c;
			case 3:
				return d;
			default:
				throw new Exception("Index was out of range!");
			}
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x000174C4 File Offset: 0x000156C4
		public static T GiveMe<T>(int index, T a, T b, T c, T d, T e)
		{
			switch (index)
			{
			case 0:
				return a;
			case 1:
				return b;
			case 2:
				return c;
			case 3:
				return d;
			case 4:
				return e;
			default:
				throw new Exception("Index was out of range!");
			}
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x000174F6 File Offset: 0x000156F6
		public static T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f)
		{
			switch (index)
			{
			case 0:
				return a;
			case 1:
				return b;
			case 2:
				return c;
			case 3:
				return d;
			case 4:
				return e;
			case 5:
				return f;
			default:
				throw new Exception("Index was out of range!");
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0001752F File Offset: 0x0001572F
		public static void PushRandom(int newSeed)
		{
			Calc.randomStack.Push(Calc.Random);
			Calc.Random = new Random(newSeed);
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0001754B File Offset: 0x0001574B
		public static void PushRandom(Random random)
		{
			Calc.randomStack.Push(Calc.Random);
			Calc.Random = random;
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x00017562 File Offset: 0x00015762
		public static void PushRandom()
		{
			Calc.randomStack.Push(Calc.Random);
			Calc.Random = new Random();
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0001757D File Offset: 0x0001577D
		public static void PopRandom()
		{
			Calc.Random = Calc.randomStack.Pop();
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0001758E File Offset: 0x0001578E
		public static T Choose<T>(this Random random, T a, T b)
		{
			return Calc.GiveMe<T>(random.Next(2), a, b);
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0001759E File Offset: 0x0001579E
		public static T Choose<T>(this Random random, T a, T b, T c)
		{
			return Calc.GiveMe<T>(random.Next(3), a, b, c);
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x000175AF File Offset: 0x000157AF
		public static T Choose<T>(this Random random, T a, T b, T c, T d)
		{
			return Calc.GiveMe<T>(random.Next(4), a, b, c, d);
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x000175C2 File Offset: 0x000157C2
		public static T Choose<T>(this Random random, T a, T b, T c, T d, T e)
		{
			return Calc.GiveMe<T>(random.Next(5), a, b, c, d, e);
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x000175D7 File Offset: 0x000157D7
		public static T Choose<T>(this Random random, T a, T b, T c, T d, T e, T f)
		{
			return Calc.GiveMe<T>(random.Next(6), a, b, c, d, e, f);
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x000175EE File Offset: 0x000157EE
		public static T Choose<T>(this Random random, params T[] choices)
		{
			return choices[random.Next(choices.Length)];
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x000175FF File Offset: 0x000157FF
		public static T Choose<T>(this Random random, List<T> choices)
		{
			return choices[random.Next(choices.Count)];
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x00017613 File Offset: 0x00015813
		public static int Range(this Random random, int min, int max)
		{
			return min + random.Next(max - min);
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00017620 File Offset: 0x00015820
		public static float Range(this Random random, float min, float max)
		{
			return min + random.NextFloat(max - min);
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0001762D File Offset: 0x0001582D
		public static Vector2 Range(this Random random, Vector2 min, Vector2 max)
		{
			return min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x00017660 File Offset: 0x00015860
		public static int Facing(this Random random)
		{
			if (random.NextFloat() >= 0.5f)
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x00017672 File Offset: 0x00015872
		public static bool Chance(this Random random, float chance)
		{
			return random.NextFloat() < chance;
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0001767D File Offset: 0x0001587D
		public static float NextFloat(this Random random)
		{
			return (float)random.NextDouble();
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x00017686 File Offset: 0x00015886
		public static float NextFloat(this Random random, float max)
		{
			return random.NextFloat() * max;
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00017690 File Offset: 0x00015890
		public static float NextAngle(this Random random)
		{
			return random.NextFloat() * 6.2831855f;
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0001769E File Offset: 0x0001589E
		public static Vector2 ShakeVector(this Random random)
		{
			return new Vector2((float)random.Choose(Calc.shakeVectorOffsets), (float)random.Choose(Calc.shakeVectorOffsets));
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x000176C0 File Offset: 0x000158C0
		public static Vector2 ClosestTo(this List<Vector2> list, Vector2 to)
		{
			Vector2 result = list[0];
			float num = Vector2.DistanceSquared(list[0], to);
			for (int i = 1; i < list.Count; i++)
			{
				float num2 = Vector2.DistanceSquared(list[i], to);
				if (num2 < num)
				{
					num = num2;
					result = list[i];
				}
			}
			return result;
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x00017714 File Offset: 0x00015914
		public static Vector2 ClosestTo(this Vector2[] list, Vector2 to)
		{
			Vector2 result = list[0];
			float num = Vector2.DistanceSquared(list[0], to);
			for (int i = 1; i < list.Length; i++)
			{
				float num2 = Vector2.DistanceSquared(list[i], to);
				if (num2 < num)
				{
					num = num2;
					result = list[i];
				}
			}
			return result;
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x00017764 File Offset: 0x00015964
		public static Vector2 ClosestTo(this Vector2[] list, Vector2 to, out int index)
		{
			index = 0;
			Vector2 result = list[0];
			float num = Vector2.DistanceSquared(list[0], to);
			for (int i = 1; i < list.Length; i++)
			{
				float num2 = Vector2.DistanceSquared(list[i], to);
				if (num2 < num)
				{
					index = i;
					num = num2;
					result = list[i];
				}
			}
			return result;
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x000177B8 File Offset: 0x000159B8
		public static void Shuffle<T>(this List<T> list, Random random)
		{
			int num = list.Count;
			while (--num > 0)
			{
				T value = list[num];
				int index;
				list[num] = list[index = random.Next(num + 1)];
				list[index] = value;
			}
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x000177FE File Offset: 0x000159FE
		public static void Shuffle<T>(this List<T> list)
		{
			list.Shuffle(Calc.Random);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0001780C File Offset: 0x00015A0C
		public static void ShuffleSetFirst<T>(this List<T> list, Random random, T first)
		{
			int num = 0;
			while (list.Contains(first))
			{
				list.Remove(first);
				num++;
			}
			list.Shuffle(random);
			for (int i = 0; i < num; i++)
			{
				list.Insert(0, first);
			}
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x0001784D File Offset: 0x00015A4D
		public static void ShuffleSetFirst<T>(this List<T> list, T first)
		{
			list.ShuffleSetFirst(Calc.Random, first);
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0001785C File Offset: 0x00015A5C
		public static void ShuffleNotFirst<T>(this List<T> list, Random random, T notFirst)
		{
			int num = 0;
			while (list.Contains(notFirst))
			{
				list.Remove(notFirst);
				num++;
			}
			list.Shuffle(random);
			for (int i = 0; i < num; i++)
			{
				list.Insert(random.Next(list.Count - 1) + 1, notFirst);
			}
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x000178AC File Offset: 0x00015AAC
		public static void ShuffleNotFirst<T>(this List<T> list, T notFirst)
		{
			list.ShuffleNotFirst(Calc.Random, notFirst);
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x000178BA File Offset: 0x00015ABA
		public static Color Invert(this Color color)
		{
			return new Color((int)(byte.MaxValue - color.R), (int)(byte.MaxValue - color.G), (int)(byte.MaxValue - color.B), (int)color.A);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x000178F0 File Offset: 0x00015AF0
		public static Color HexToColor(string hex)
		{
			int num = 0;
			if (hex.Length >= 1 && hex[0] == '#')
			{
				num = 1;
			}
			if (hex.Length - num >= 6)
			{
				float r = (float)(Calc.HexToByte(hex[num]) * 16 + Calc.HexToByte(hex[num + 1])) / 255f;
				float g = (float)(Calc.HexToByte(hex[num + 2]) * 16 + Calc.HexToByte(hex[num + 3])) / 255f;
				float b = (float)(Calc.HexToByte(hex[num + 4]) * 16 + Calc.HexToByte(hex[num + 5])) / 255f;
				return new Color(r, g, b);
			}
			int hex2;
			if (int.TryParse(hex.Substring(num), out hex2))
			{
				return Calc.HexToColor(hex2);
			}
			return Color.White;
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x000179BC File Offset: 0x00015BBC
		public static Color HexToColor(int hex)
		{
			return new Color
			{
				A = byte.MaxValue,
				R = (byte)(hex >> 16),
				G = (byte)(hex >> 8),
				B = (byte)hex
			};
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x00017A00 File Offset: 0x00015C00
		public static Color HsvToColor(float hue, float s, float v)
		{
			int num = (int)(hue * 360f);
			float num2 = s * v;
			float num3 = num2 * (1f - Math.Abs((float)num / 60f % 2f - 1f));
			float num4 = v - num2;
			if (num < 60)
			{
				return new Color(num4 + num2, num4 + num3, num4);
			}
			if (num < 120)
			{
				return new Color(num4 + num3, num4 + num2, num4);
			}
			if (num < 180)
			{
				return new Color(num4, num4 + num2, num4 + num3);
			}
			if (num < 240)
			{
				return new Color(num4, num4 + num3, num4 + num2);
			}
			if (num < 300)
			{
				return new Color(num4 + num3, num4, num4 + num2);
			}
			return new Color(num4 + num2, num4, num4 + num3);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x00017AB0 File Offset: 0x00015CB0
		public static string ShortGameplayFormat(this TimeSpan time)
		{
			if (time.TotalHours >= 1.0)
			{
				return (int)time.TotalHours + ":" + time.ToString("mm\\:ss\\.fff");
			}
			return time.ToString("m\\:ss\\.fff");
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00017B00 File Offset: 0x00015D00
		public static string LongGameplayFormat(this TimeSpan time)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (time.TotalDays >= 2.0)
			{
				stringBuilder.Append((int)time.TotalDays);
				stringBuilder.Append(" days, ");
			}
			else if (time.TotalDays >= 1.0)
			{
				stringBuilder.Append("1 day, ");
			}
			stringBuilder.Append((time.TotalHours - (double)((int)time.TotalDays * 24)).ToString("0.0"));
			stringBuilder.Append(" hours");
			return stringBuilder.ToString();
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00017B9C File Offset: 0x00015D9C
		public static int Digits(this int num)
		{
			int num2 = 1;
			int num3 = 10;
			while (num >= num3)
			{
				num2++;
				num3 *= 10;
			}
			return num2;
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00017BBE File Offset: 0x00015DBE
		public static byte HexToByte(char c)
		{
			return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00017BD1 File Offset: 0x00015DD1
		public static float Percent(float num, float zeroAt, float oneAt)
		{
			return MathHelper.Clamp((num - zeroAt) / (oneAt - zeroAt), 0f, 1f);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00017BE9 File Offset: 0x00015DE9
		public static float SignThreshold(float value, float threshold)
		{
			if (Math.Abs(value) >= threshold)
			{
				return (float)Math.Sign(value);
			}
			return 0f;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00017C04 File Offset: 0x00015E04
		public static float Min(params float[] values)
		{
			float num = values[0];
			for (int i = 1; i < values.Length; i++)
			{
				num = MathHelper.Min(values[i], num);
			}
			return num;
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00017C30 File Offset: 0x00015E30
		public static float Max(params float[] values)
		{
			float num = values[0];
			for (int i = 1; i < values.Length; i++)
			{
				num = MathHelper.Max(values[i], num);
			}
			return num;
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00017C5A File Offset: 0x00015E5A
		public static int Max(int a, int b, int c, int d)
		{
			return Math.Max(Math.Max(Math.Max(a, b), c), d);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00017C6F File Offset: 0x00015E6F
		public static float ToRad(this float f)
		{
			return f * 0.017453292f;
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00017C78 File Offset: 0x00015E78
		public static float ToDeg(this float f)
		{
			return f * 57.295776f;
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x0001273C File Offset: 0x0001093C
		public static int Axis(bool negative, bool positive, int both = 0)
		{
			if (negative)
			{
				if (positive)
				{
					return both;
				}
				return -1;
			}
			else
			{
				if (positive)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00017C81 File Offset: 0x00015E81
		public static int Clamp(int value, int min, int max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00017C90 File Offset: 0x00015E90
		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00017C9F File Offset: 0x00015E9F
		public static float YoYo(float value)
		{
			if (value <= 0.5f)
			{
				return value * 2f;
			}
			return 1f - (value - 0.5f) * 2f;
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00017CC4 File Offset: 0x00015EC4
		public static float Map(float val, float min, float max, float newMin = 0f, float newMax = 1f)
		{
			return (val - min) / (max - min) * (newMax - newMin) + newMin;
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00017CD4 File Offset: 0x00015ED4
		public static float SineMap(float counter, float newMin, float newMax)
		{
			return Calc.Map((float)Math.Sin((double)counter), -1f, 1f, newMin, newMax);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00017CEF File Offset: 0x00015EEF
		public static float ClampedMap(float val, float min, float max, float newMin = 0f, float newMax = 1f)
		{
			return MathHelper.Clamp((val - min) / (max - min), 0f, 1f) * (newMax - newMin) + newMin;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00017D10 File Offset: 0x00015F10
		public static float LerpSnap(float value1, float value2, float amount, float snapThreshold = 0.1f)
		{
			float num = MathHelper.Lerp(value1, value2, amount);
			if (Math.Abs(num - value2) < snapThreshold)
			{
				return value2;
			}
			return num;
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00017D34 File Offset: 0x00015F34
		public static float LerpClamp(float value1, float value2, float lerp)
		{
			return MathHelper.Lerp(value1, value2, MathHelper.Clamp(lerp, 0f, 1f));
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00017D50 File Offset: 0x00015F50
		public static Vector2 LerpSnap(Vector2 value1, Vector2 value2, float amount, float snapThresholdSq = 0.1f)
		{
			Vector2 vector = Vector2.Lerp(value1, value2, amount);
			if ((vector - value2).LengthSquared() < snapThresholdSq)
			{
				return value2;
			}
			return vector;
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00017D7B File Offset: 0x00015F7B
		public static Vector2 Sign(this Vector2 vec)
		{
			return new Vector2((float)Math.Sign(vec.X), (float)Math.Sign(vec.Y));
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00017D9A File Offset: 0x00015F9A
		public static Vector2 SafeNormalize(this Vector2 vec)
		{
			return vec.SafeNormalize(Vector2.Zero);
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00017DA7 File Offset: 0x00015FA7
		public static Vector2 SafeNormalize(this Vector2 vec, float length)
		{
			return vec.SafeNormalize(Vector2.Zero, length);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x00017DB5 File Offset: 0x00015FB5
		public static Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero)
		{
			if (vec == Vector2.Zero)
			{
				return ifZero;
			}
			vec.Normalize();
			return vec;
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00017DCE File Offset: 0x00015FCE
		public static Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero, float length)
		{
			if (vec == Vector2.Zero)
			{
				return ifZero * length;
			}
			vec.Normalize();
			return vec * length;
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00017DF3 File Offset: 0x00015FF3
		public static Vector2 TurnRight(this Vector2 vec)
		{
			return new Vector2(-vec.Y, vec.X);
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00017E07 File Offset: 0x00016007
		public static float ReflectAngle(float angle, float axis = 0f)
		{
			return -(angle + axis) - axis;
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00017E0F File Offset: 0x0001600F
		public static float ReflectAngle(float angleRadians, Vector2 axis)
		{
			return Calc.ReflectAngle(angleRadians, axis.Angle());
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00017E20 File Offset: 0x00016020
		public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
		{
			Vector2 vector = lineB - lineA;
			float num = Vector2.Dot(closestTo - lineA, vector) / Vector2.Dot(vector, vector);
			num = MathHelper.Clamp(num, 0f, 1f);
			return lineA + vector * num;
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00017E69 File Offset: 0x00016069
		public static Vector2 Round(this Vector2 vec)
		{
			return new Vector2((float)Math.Round((double)vec.X), (float)Math.Round((double)vec.Y));
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00017E8A File Offset: 0x0001608A
		public static float Snap(float value, float increment)
		{
			return (float)Math.Round((double)(value / increment)) * increment;
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00017E98 File Offset: 0x00016098
		public static float Snap(float value, float increment, float offset)
		{
			return (float)Math.Round((double)((value - offset) / increment)) * increment + offset;
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00017EAA File Offset: 0x000160AA
		public static float WrapAngleDeg(float angleDegrees)
		{
			return ((angleDegrees * (float)Math.Sign(angleDegrees) + 180f) % 360f - 180f) * (float)Math.Sign(angleDegrees);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00017ECF File Offset: 0x000160CF
		public static float WrapAngle(float angleRadians)
		{
			return ((angleRadians * (float)Math.Sign(angleRadians) + 3.1415927f) % 6.2831855f - 3.1415927f) * (float)Math.Sign(angleRadians);
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00017EF4 File Offset: 0x000160F4
		public static Vector2 AngleToVector(float angleRadians, float length)
		{
			return new Vector2((float)Math.Cos((double)angleRadians) * length, (float)Math.Sin((double)angleRadians) * length);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x00017F10 File Offset: 0x00016110
		public static float AngleApproach(float val, float target, float maxMove)
		{
			float value = Calc.AngleDiff(val, target);
			if (Math.Abs(value) < maxMove)
			{
				return target;
			}
			return val + MathHelper.Clamp(value, -maxMove, maxMove);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00017F3B File Offset: 0x0001613B
		public static float AngleLerp(float startAngle, float endAngle, float percent)
		{
			return startAngle + Calc.AngleDiff(startAngle, endAngle) * percent;
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00017F48 File Offset: 0x00016148
		public static float Approach(float val, float target, float maxMove)
		{
			if (val <= target)
			{
				return Math.Min(val + maxMove, target);
			}
			return Math.Max(val - maxMove, target);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00017F64 File Offset: 0x00016164
		public static float AngleDiff(float radiansA, float radiansB)
		{
			float num;
			for (num = radiansB - radiansA; num > 3.1415927f; num -= 6.2831855f)
			{
			}
			while (num <= -3.1415927f)
			{
				num += 6.2831855f;
			}
			return num;
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00017F9A File Offset: 0x0001619A
		public static float AbsAngleDiff(float radiansA, float radiansB)
		{
			return Math.Abs(Calc.AngleDiff(radiansA, radiansB));
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00017FA8 File Offset: 0x000161A8
		public static int SignAngleDiff(float radiansA, float radiansB)
		{
			return Math.Sign(Calc.AngleDiff(radiansA, radiansB));
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00017FB6 File Offset: 0x000161B6
		public static float Angle(Vector2 from, Vector2 to)
		{
			return (float)Math.Atan2((double)(to.Y - from.Y), (double)(to.X - from.X));
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x00017FDA File Offset: 0x000161DA
		public static Color ToggleColors(Color current, Color a, Color b)
		{
			if (current == a)
			{
				return b;
			}
			return a;
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00017FE8 File Offset: 0x000161E8
		public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB)
		{
			if (Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < Math.Abs(Calc.AngleDiff(currentAngle, angleB)))
			{
				return angleA;
			}
			return angleB;
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00018007 File Offset: 0x00016207
		public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB, float angleC)
		{
			if (Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < Math.Abs(Calc.AngleDiff(currentAngle, angleB)))
			{
				return Calc.ShorterAngleDifference(currentAngle, angleA, angleC);
			}
			return Calc.ShorterAngleDifference(currentAngle, angleB, angleC);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00018034 File Offset: 0x00016234
		public static bool IsInRange<T>(this T[] array, int index)
		{
			return index >= 0 && index < array.Length;
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00018042 File Offset: 0x00016242
		public static bool IsInRange<T>(this List<T> list, int index)
		{
			return index >= 0 && index < list.Count;
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x00018053 File Offset: 0x00016253
		public static T[] Array<T>(params T[] items)
		{
			return items;
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x00018058 File Offset: 0x00016258
		public static T[] VerifyLength<T>(this T[] array, int length)
		{
			if (array == null)
			{
				return new T[length];
			}
			if (array.Length != length)
			{
				T[] array2 = new T[length];
				for (int i = 0; i < Math.Min(length, array.Length); i++)
				{
					array2[i] = array[i];
				}
				return array2;
			}
			return array;
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x000180A4 File Offset: 0x000162A4
		public static T[][] VerifyLength<T>(this T[][] array, int length0, int length1)
		{
			array = array.VerifyLength(length0);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].VerifyLength(length1);
			}
			return array;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x000180D5 File Offset: 0x000162D5
		public static bool BetweenInterval(float val, float interval)
		{
			return val % (interval * 2f) > interval;
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x000180E3 File Offset: 0x000162E3
		public static bool OnInterval(float val, float prevVal, float interval)
		{
			return (int)(prevVal / interval) != (int)(val / interval);
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x000180F2 File Offset: 0x000162F2
		public static Vector2 Toward(Vector2 from, Vector2 to, float length)
		{
			if (from == to)
			{
				return Vector2.Zero;
			}
			return (to - from).SafeNormalize(length);
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00018110 File Offset: 0x00016310
		public static Vector2 Toward(Entity from, Entity to, float length)
		{
			return Calc.Toward(from.Position, to.Position, length);
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00017DF3 File Offset: 0x00015FF3
		public static Vector2 Perpendicular(this Vector2 vector)
		{
			return new Vector2(-vector.Y, vector.X);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x00018124 File Offset: 0x00016324
		public static float Angle(this Vector2 vector)
		{
			return (float)Math.Atan2((double)vector.Y, (double)vector.X);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x0001813A File Offset: 0x0001633A
		public static Vector2 Clamp(this Vector2 val, float minX, float minY, float maxX, float maxY)
		{
			return new Vector2(MathHelper.Clamp(val.X, minX, maxX), MathHelper.Clamp(val.Y, minY, maxY));
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x0001815C File Offset: 0x0001635C
		public static Vector2 Floor(this Vector2 val)
		{
			return new Vector2((float)((int)Math.Floor((double)val.X)), (float)((int)Math.Floor((double)val.Y)));
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x0001817F File Offset: 0x0001637F
		public static Vector2 Ceiling(this Vector2 val)
		{
			return new Vector2((float)((int)Math.Ceiling((double)val.X)), (float)((int)Math.Ceiling((double)val.Y)));
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x000181A2 File Offset: 0x000163A2
		public static Vector2 Abs(this Vector2 val)
		{
			return new Vector2(Math.Abs(val.X), Math.Abs(val.Y));
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x000181C0 File Offset: 0x000163C0
		public static Vector2 Approach(Vector2 val, Vector2 target, float maxMove)
		{
			if (maxMove == 0f || val == target)
			{
				return val;
			}
			Vector2 value = target - val;
			if (value.Length() < maxMove)
			{
				return target;
			}
			value.Normalize();
			return val + value * maxMove;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x00018208 File Offset: 0x00016408
		public static Vector2 FourWayNormal(this Vector2 vec)
		{
			if (vec == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			vec = Calc.AngleToVector((float)Math.Floor((double)((vec.Angle() + 0.7853982f) / 1.5707964f)) * 1.5707964f, 1f);
			if (Math.Abs(vec.X) < 0.5f)
			{
				vec.X = 0f;
			}
			else
			{
				vec.X = (float)Math.Sign(vec.X);
			}
			if (Math.Abs(vec.Y) < 0.5f)
			{
				vec.Y = 0f;
			}
			else
			{
				vec.Y = (float)Math.Sign(vec.Y);
			}
			return vec;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x000182BC File Offset: 0x000164BC
		public static Vector2 EightWayNormal(this Vector2 vec)
		{
			if (vec == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			vec = Calc.AngleToVector((float)Math.Floor((double)((vec.Angle() + 0.3926991f) / 0.7853982f)) * 0.7853982f, 1f);
			if (Math.Abs(vec.X) < 0.5f)
			{
				vec.X = 0f;
			}
			else if (Math.Abs(vec.Y) < 0.5f)
			{
				vec.Y = 0f;
			}
			return vec;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x00018348 File Offset: 0x00016548
		public static Vector2 SnappedNormal(this Vector2 vec, float slices)
		{
			float num = 6.2831855f / slices;
			return Calc.AngleToVector((float)Math.Floor((double)((vec.Angle() + num / 2f) / num)) * num, 1f);
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00018380 File Offset: 0x00016580
		public static Vector2 Snapped(this Vector2 vec, float slices)
		{
			float num = 6.2831855f / slices;
			return Calc.AngleToVector((float)Math.Floor((double)((vec.Angle() + num / 2f) / num)) * num, vec.Length());
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x000183BA File Offset: 0x000165BA
		public static Vector2 XComp(this Vector2 vec)
		{
			return Vector2.UnitX * vec.X;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x000183CC File Offset: 0x000165CC
		public static Vector2 YComp(this Vector2 vec)
		{
			return Vector2.UnitY * vec.Y;
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x000183E0 File Offset: 0x000165E0
		public static Vector2[] ParseVector2List(string list, char seperator = '|')
		{
			string[] array = list.Split(new char[]
			{
				seperator
			});
			Vector2[] array2 = new Vector2[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[]
				{
					','
				});
				array2[i] = new Vector2((float)Convert.ToInt32(array3[0]), (float)Convert.ToInt32(array3[1]));
			}
			return array2;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00018448 File Offset: 0x00016648
		public static Vector2 Rotate(this Vector2 vec, float angleRadians)
		{
			return Calc.AngleToVector(vec.Angle() + angleRadians, vec.Length());
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x0001845E File Offset: 0x0001665E
		public static Vector2 RotateTowards(this Vector2 vec, float targetAngleRadians, float maxMoveRadians)
		{
			return Calc.AngleToVector(Calc.AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians), vec.Length());
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x0001847C File Offset: 0x0001667C
		public static Vector3 RotateTowards(this Vector3 from, Vector3 target, float maxRotationRadians)
		{
			Vector3 vector = Vector3.Cross(from, target);
			float num = from.Length();
			float num2 = target.Length();
			float w = (float)Math.Sqrt((double)(num * num * (num2 * num2))) + Vector3.Dot(from, target);
			Quaternion quaternion = new Quaternion(vector.X, vector.Y, vector.Z, w);
			if (quaternion.Length() <= maxRotationRadians)
			{
				return target;
			}
			quaternion.Normalize();
			quaternion *= maxRotationRadians;
			return Vector3.Transform(from, quaternion);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x000184F2 File Offset: 0x000166F2
		public static Vector2 XZ(this Vector3 vector)
		{
			return new Vector2(vector.X, vector.Z);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00018508 File Offset: 0x00016708
		public static Vector3 Approach(this Vector3 v, Vector3 target, float amount)
		{
			if (amount > (target - v).Length())
			{
				return target;
			}
			return v + (target - v).SafeNormalize() * amount;
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00018544 File Offset: 0x00016744
		public static Vector3 SafeNormalize(this Vector3 v)
		{
			float num = v.Length();
			if (num > 0f)
			{
				return v / num;
			}
			return Vector3.Zero;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00018570 File Offset: 0x00016770
		public static int[,] ReadCSVIntGrid(string csv, int width, int height)
		{
			int[,] array = new int[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					array[i, j] = -1;
				}
			}
			string[] array2 = csv.Split(new char[]
			{
				'\n'
			});
			int num = 0;
			while (num < height && num < array2.Length)
			{
				string[] array3 = array2[num].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				int num2 = 0;
				while (num2 < width && num2 < array3.Length)
				{
					array[num2, num] = Convert.ToInt32(array3[num2]);
					num2++;
				}
				num++;
			}
			return array;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00018614 File Offset: 0x00016814
		public static int[] ReadCSVInt(string csv)
		{
			if (csv == "")
			{
				return new int[0];
			}
			string[] array = csv.Split(new char[]
			{
				','
			});
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Convert.ToInt32(array[i].Trim());
			}
			return array2;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00018670 File Offset: 0x00016870
		public static int[] ReadCSVIntWithTricks(string csv)
		{
			if (csv == "")
			{
				return new int[0];
			}
			string[] array = csv.Split(new char[]
			{
				','
			});
			List<int> list = new List<int>();
			foreach (string text in array)
			{
				if (text.IndexOf('-') != -1)
				{
					string[] array3 = text.Split(new char[]
					{
						'-'
					});
					int num = Convert.ToInt32(array3[0]);
					int num2 = Convert.ToInt32(array3[1]);
					for (int num3 = num; num3 != num2; num3 += Math.Sign(num2 - num))
					{
						list.Add(num3);
					}
					list.Add(num2);
				}
				else if (text.IndexOf('*') != -1)
				{
					string[] array4 = text.Split(new char[]
					{
						'*'
					});
					int item = Convert.ToInt32(array4[0]);
					int num4 = Convert.ToInt32(array4[1]);
					for (int j = 0; j < num4; j++)
					{
						list.Add(item);
					}
				}
				else
				{
					list.Add(Convert.ToInt32(text));
				}
			}
			return list.ToArray();
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0001877C File Offset: 0x0001697C
		public static string[] ReadCSV(string csv)
		{
			if (csv == "")
			{
				return new string[0];
			}
			string[] array = csv.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			return array;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x000187CC File Offset: 0x000169CC
		public static string IntGridToCSV(int[,] data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<int> list = new List<int>();
			int num = 0;
			for (int i = 0; i < data.GetLength(1); i++)
			{
				int num2 = 0;
				for (int j = 0; j < data.GetLength(0); j++)
				{
					if (data[j, i] == -1)
					{
						num2++;
					}
					else
					{
						for (int k = 0; k < num; k++)
						{
							stringBuilder.Append('\n');
						}
						for (int l = 0; l < num2; l++)
						{
							list.Add(-1);
						}
						num = (num2 = 0);
						list.Add(data[j, i]);
					}
				}
				if (list.Count > 0)
				{
					stringBuilder.Append(string.Join<int>(",", list));
					list.Clear();
				}
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0001889C File Offset: 0x00016A9C
		public static bool[,] GetBitData(string data, char rowSep = '\n')
		{
			int num = 0;
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == '1' || data[i] == '0')
				{
					num++;
				}
				else if (data[i] == rowSep)
				{
					break;
				}
			}
			int num2 = data.Count((char c) => c == '\n') + 1;
			bool[,] array = new bool[num, num2];
			int num3 = 0;
			int num4 = 0;
			foreach (char c2 in data)
			{
				if (c2 != '\n')
				{
					if (c2 != '0')
					{
						if (c2 == '1')
						{
							array[num3, num4] = true;
							num3++;
						}
					}
					else
					{
						array[num3, num4] = false;
						num3++;
					}
				}
				else
				{
					num3 = 0;
					num4++;
				}
			}
			return array;
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x00018978 File Offset: 0x00016B78
		public static void CombineBitData(bool[,] combineInto, string data, char rowSep = '\n')
		{
			int num = 0;
			int num2 = 0;
			foreach (char c in data)
			{
				if (c != '\n')
				{
					if (c != '0')
					{
						if (c == '1')
						{
							combineInto[num, num2] = true;
							num++;
						}
					}
					else
					{
						num++;
					}
				}
				else
				{
					num = 0;
					num2++;
				}
			}
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x000189CC File Offset: 0x00016BCC
		public static void CombineBitData(bool[,] combineInto, bool[,] data)
		{
			for (int i = 0; i < combineInto.GetLength(0); i++)
			{
				for (int j = 0; j < combineInto.GetLength(1); j++)
				{
					if (data[i, j])
					{
						combineInto[i, j] = true;
					}
				}
			}
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00018A10 File Offset: 0x00016C10
		public static int[] ConvertStringArrayToIntArray(string[] strings)
		{
			int[] array = new int[strings.Length];
			for (int i = 0; i < strings.Length; i++)
			{
				array[i] = Convert.ToInt32(strings[i]);
			}
			return array;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00018A40 File Offset: 0x00016C40
		public static float[] ConvertStringArrayToFloatArray(string[] strings)
		{
			float[] array = new float[strings.Length];
			for (int i = 0; i < strings.Length; i++)
			{
				array[i] = Convert.ToSingle(strings[i], CultureInfo.InvariantCulture);
			}
			return array;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00018A75 File Offset: 0x00016C75
		public static bool FileExists(string filename)
		{
			return File.Exists(filename);
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x00018A80 File Offset: 0x00016C80
		public static bool SaveFile<T>(T obj, string filename) where T : new()
		{
			Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
			bool result;
			try
			{
				new XmlSerializer(typeof(T)).Serialize(stream, obj);
				stream.Close();
				result = true;
			}
			catch
			{
				stream.Close();
				result = false;
			}
			return result;
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00018AD8 File Offset: 0x00016CD8
		public static bool LoadFile<T>(string filename, ref T data) where T : new()
		{
			Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			bool result;
			try
			{
				T t = (T)((object)new XmlSerializer(typeof(T)).Deserialize(stream));
				stream.Close();
				data = t;
				result = true;
			}
			catch
			{
				stream.Close();
				result = false;
			}
			return result;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x00018B38 File Offset: 0x00016D38
		public static XmlDocument LoadContentXML(string filename)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (Stream stream = TitleContainer.OpenStream(Path.Combine(Engine.Instance.Content.RootDirectory, filename)))
			{
				xmlDocument.Load(stream);
			}
			return xmlDocument;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00018B8C File Offset: 0x00016D8C
		public static XmlDocument LoadXML(string filename)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (FileStream fileStream = File.OpenRead(filename))
			{
				xmlDocument.Load(fileStream);
			}
			return xmlDocument;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00018BCC File Offset: 0x00016DCC
		public static bool ContentXMLExists(string filename)
		{
			return File.Exists(Path.Combine(Engine.ContentDirectory, filename));
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00018A75 File Offset: 0x00016C75
		public static bool XMLExists(string filename)
		{
			return File.Exists(filename);
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x00018BDE File Offset: 0x00016DDE
		public static bool HasAttr(this XmlElement xml, string attributeName)
		{
			return xml.Attributes[attributeName] != null;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00018BEF File Offset: 0x00016DEF
		public static string Attr(this XmlElement xml, string attributeName)
		{
			return xml.Attributes[attributeName].InnerText;
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00018C02 File Offset: 0x00016E02
		public static string Attr(this XmlElement xml, string attributeName, string defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return xml.Attributes[attributeName].InnerText;
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x00018C20 File Offset: 0x00016E20
		public static int AttrInt(this XmlElement xml, string attributeName)
		{
			return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00018C38 File Offset: 0x00016E38
		public static int AttrInt(this XmlElement xml, string attributeName, int defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x00018C5B File Offset: 0x00016E5B
		public static float AttrFloat(this XmlElement xml, string attributeName)
		{
			return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x00018C78 File Offset: 0x00016E78
		public static float AttrFloat(this XmlElement xml, string attributeName, float defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00018CA0 File Offset: 0x00016EA0
		public static Vector3 AttrVector3(this XmlElement xml, string attributeName)
		{
			string[] array = xml.Attr(attributeName).Split(new char[]
			{
				','
			});
			float x = float.Parse(array[0].Trim(), CultureInfo.InvariantCulture);
			float y = float.Parse(array[1].Trim(), CultureInfo.InvariantCulture);
			float z = float.Parse(array[2].Trim(), CultureInfo.InvariantCulture);
			return new Vector3(x, y, z);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00018D04 File Offset: 0x00016F04
		public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName)
		{
			return new Vector2(xml.AttrFloat(xAttributeName), xml.AttrFloat(yAttributeName));
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00018D19 File Offset: 0x00016F19
		public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName, Vector2 defaultValue)
		{
			return new Vector2(xml.AttrFloat(xAttributeName, defaultValue.X), xml.AttrFloat(yAttributeName, defaultValue.Y));
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00018D3A File Offset: 0x00016F3A
		public static bool AttrBool(this XmlElement xml, string attributeName)
		{
			return Convert.ToBoolean(xml.Attributes[attributeName].InnerText);
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00018D52 File Offset: 0x00016F52
		public static bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return xml.AttrBool(attributeName);
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00018D66 File Offset: 0x00016F66
		public static char AttrChar(this XmlElement xml, string attributeName)
		{
			return Convert.ToChar(xml.Attributes[attributeName].InnerText);
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00018D7E File Offset: 0x00016F7E
		public static char AttrChar(this XmlElement xml, string attributeName, char defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return xml.AttrChar(attributeName);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00018D94 File Offset: 0x00016F94
		public static T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
		{
			if (Enum.IsDefined(typeof(T), xml.Attributes[attributeName].InnerText))
			{
				return (T)((object)Enum.Parse(typeof(T), xml.Attributes[attributeName].InnerText));
			}
			throw new Exception("The attribute value cannot be converted to the enum type.");
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00018DF3 File Offset: 0x00016FF3
		public static T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return xml.AttrEnum(attributeName);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00018E07 File Offset: 0x00017007
		public static Color AttrHexColor(this XmlElement xml, string attributeName)
		{
			return Calc.HexToColor(xml.Attr(attributeName));
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00018E15 File Offset: 0x00017015
		public static Color AttrHexColor(this XmlElement xml, string attributeName, Color defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return defaultValue;
			}
			return xml.AttrHexColor(attributeName);
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00018E29 File Offset: 0x00017029
		public static Color AttrHexColor(this XmlElement xml, string attributeName, string defaultValue)
		{
			if (!xml.HasAttr(attributeName))
			{
				return Calc.HexToColor(defaultValue);
			}
			return xml.AttrHexColor(attributeName);
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00018E42 File Offset: 0x00017042
		public static Vector2 Position(this XmlElement xml)
		{
			return new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00018E5F File Offset: 0x0001705F
		public static Vector2 Position(this XmlElement xml, Vector2 defaultPosition)
		{
			return new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00018E88 File Offset: 0x00017088
		public static int X(this XmlElement xml)
		{
			return xml.AttrInt("x");
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00018E95 File Offset: 0x00017095
		public static int X(this XmlElement xml, int defaultX)
		{
			return xml.AttrInt("x", defaultX);
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00018EA3 File Offset: 0x000170A3
		public static int Y(this XmlElement xml)
		{
			return xml.AttrInt("y");
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00018EB0 File Offset: 0x000170B0
		public static int Y(this XmlElement xml, int defaultY)
		{
			return xml.AttrInt("y", defaultY);
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00018EBE File Offset: 0x000170BE
		public static int Width(this XmlElement xml)
		{
			return xml.AttrInt("width");
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00018ECB File Offset: 0x000170CB
		public static int Width(this XmlElement xml, int defaultWidth)
		{
			return xml.AttrInt("width", defaultWidth);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00018ED9 File Offset: 0x000170D9
		public static int Height(this XmlElement xml)
		{
			return xml.AttrInt("height");
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00018EE6 File Offset: 0x000170E6
		public static int Height(this XmlElement xml, int defaultHeight)
		{
			return xml.AttrInt("height", defaultHeight);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00018EF4 File Offset: 0x000170F4
		public static Rectangle Rect(this XmlElement xml)
		{
			return new Rectangle(xml.X(), xml.Y(), xml.Width(), xml.Height());
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00018F13 File Offset: 0x00017113
		public static int ID(this XmlElement xml)
		{
			return xml.AttrInt("id");
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x00018F20 File Offset: 0x00017120
		public static int InnerInt(this XmlElement xml)
		{
			return Convert.ToInt32(xml.InnerText);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00018F2D File Offset: 0x0001712D
		public static float InnerFloat(this XmlElement xml)
		{
			return Convert.ToSingle(xml.InnerText, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00018F3F File Offset: 0x0001713F
		public static bool InnerBool(this XmlElement xml)
		{
			return Convert.ToBoolean(xml.InnerText);
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00018F4C File Offset: 0x0001714C
		public static T InnerEnum<T>(this XmlElement xml) where T : struct
		{
			if (Enum.IsDefined(typeof(T), xml.InnerText))
			{
				return (T)((object)Enum.Parse(typeof(T), xml.InnerText));
			}
			throw new Exception("The attribute value cannot be converted to the enum type.");
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00018F8A File Offset: 0x0001718A
		public static Color InnerHexColor(this XmlElement xml)
		{
			return Calc.HexToColor(xml.InnerText);
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00018F97 File Offset: 0x00017197
		public static bool HasChild(this XmlElement xml, string childName)
		{
			return xml[childName] != null;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00018FA3 File Offset: 0x000171A3
		public static string ChildText(this XmlElement xml, string childName)
		{
			return xml[childName].InnerText;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00018FB1 File Offset: 0x000171B1
		public static string ChildText(this XmlElement xml, string childName, string defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return xml[childName].InnerText;
			}
			return defaultValue;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00018FCA File Offset: 0x000171CA
		public static int ChildInt(this XmlElement xml, string childName)
		{
			return xml[childName].InnerInt();
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00018FD8 File Offset: 0x000171D8
		public static int ChildInt(this XmlElement xml, string childName, int defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return xml[childName].InnerInt();
			}
			return defaultValue;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00018FF1 File Offset: 0x000171F1
		public static float ChildFloat(this XmlElement xml, string childName)
		{
			return xml[childName].InnerFloat();
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00018FFF File Offset: 0x000171FF
		public static float ChildFloat(this XmlElement xml, string childName, float defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return xml[childName].InnerFloat();
			}
			return defaultValue;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00019018 File Offset: 0x00017218
		public static bool ChildBool(this XmlElement xml, string childName)
		{
			return xml[childName].InnerBool();
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00019026 File Offset: 0x00017226
		public static bool ChildBool(this XmlElement xml, string childName, bool defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return xml[childName].InnerBool();
			}
			return defaultValue;
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x00019040 File Offset: 0x00017240
		public static T ChildEnum<T>(this XmlElement xml, string childName) where T : struct
		{
			if (Enum.IsDefined(typeof(T), xml[childName].InnerText))
			{
				return (T)((object)Enum.Parse(typeof(T), xml[childName].InnerText));
			}
			throw new Exception("The attribute value cannot be converted to the enum type.");
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x00019098 File Offset: 0x00017298
		public static T ChildEnum<T>(this XmlElement xml, string childName, T defaultValue) where T : struct
		{
			if (!xml.HasChild(childName))
			{
				return defaultValue;
			}
			if (Enum.IsDefined(typeof(T), xml[childName].InnerText))
			{
				return (T)((object)Enum.Parse(typeof(T), xml[childName].InnerText));
			}
			throw new Exception("The attribute value cannot be converted to the enum type.");
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x000190F8 File Offset: 0x000172F8
		public static Color ChildHexColor(this XmlElement xml, string childName)
		{
			return Calc.HexToColor(xml[childName].InnerText);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0001910B File Offset: 0x0001730B
		public static Color ChildHexColor(this XmlElement xml, string childName, Color defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return Calc.HexToColor(xml[childName].InnerText);
			}
			return defaultValue;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00019129 File Offset: 0x00017329
		public static Color ChildHexColor(this XmlElement xml, string childName, string defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return Calc.HexToColor(xml[childName].InnerText);
			}
			return Calc.HexToColor(defaultValue);
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x0001914C File Offset: 0x0001734C
		public static Vector2 ChildPosition(this XmlElement xml, string childName)
		{
			return xml[childName].Position();
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x0001915A File Offset: 0x0001735A
		public static Vector2 ChildPosition(this XmlElement xml, string childName, Vector2 defaultValue)
		{
			if (xml.HasChild(childName))
			{
				return xml[childName].Position(defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00019174 File Offset: 0x00017374
		public static Vector2 FirstNode(this XmlElement xml)
		{
			if (xml["node"] == null)
			{
				return Vector2.Zero;
			}
			return new Vector2((float)((int)xml["node"].AttrFloat("x")), (float)((int)xml["node"].AttrFloat("y")));
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x000191C8 File Offset: 0x000173C8
		public static Vector2? FirstNodeNullable(this XmlElement xml)
		{
			if (xml["node"] == null)
			{
				return null;
			}
			return new Vector2?(new Vector2((float)((int)xml["node"].AttrFloat("x")), (float)((int)xml["node"].AttrFloat("y"))));
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00019224 File Offset: 0x00017424
		public static Vector2? FirstNodeNullable(this XmlElement xml, Vector2 offset)
		{
			if (xml["node"] == null)
			{
				return null;
			}
			return new Vector2?(new Vector2((float)((int)xml["node"].AttrFloat("x")), (float)((int)xml["node"].AttrFloat("y"))) + offset);
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00019288 File Offset: 0x00017488
		public static Vector2[] Nodes(this XmlElement xml, bool includePosition = false)
		{
			XmlNodeList elementsByTagName = xml.GetElementsByTagName("node");
			if (elementsByTagName != null)
			{
				Vector2[] array;
				if (includePosition)
				{
					array = new Vector2[elementsByTagName.Count + 1];
					array[0] = xml.Position();
					for (int i = 0; i < elementsByTagName.Count; i++)
					{
						array[i + 1] = new Vector2((float)Convert.ToInt32(elementsByTagName[i].Attributes["x"].InnerText), (float)Convert.ToInt32(elementsByTagName[i].Attributes["y"].InnerText));
					}
				}
				else
				{
					array = new Vector2[elementsByTagName.Count];
					for (int j = 0; j < elementsByTagName.Count; j++)
					{
						array[j] = new Vector2((float)Convert.ToInt32(elementsByTagName[j].Attributes["x"].InnerText), (float)Convert.ToInt32(elementsByTagName[j].Attributes["y"].InnerText));
					}
				}
				return array;
			}
			if (!includePosition)
			{
				return new Vector2[0];
			}
			return new Vector2[]
			{
				xml.Position()
			};
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x000193B0 File Offset: 0x000175B0
		public static Vector2[] Nodes(this XmlElement xml, Vector2 offset, bool includePosition = false)
		{
			Vector2[] array = xml.Nodes(includePosition);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] += offset;
			}
			return array;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x000193EC File Offset: 0x000175EC
		public static Vector2 GetNode(this XmlElement xml, int nodeNum)
		{
			return xml.Nodes(false)[nodeNum];
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x000193FC File Offset: 0x000175FC
		public static Vector2? GetNodeNullable(this XmlElement xml, int nodeNum)
		{
			if (xml.Nodes(false).Length > nodeNum)
			{
				return new Vector2?(xml.Nodes(false)[nodeNum]);
			}
			return null;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00019434 File Offset: 0x00017634
		public static void SetAttr(this XmlElement xml, string attributeName, object setTo)
		{
			XmlAttribute xmlAttribute;
			if (xml.HasAttr(attributeName))
			{
				xmlAttribute = xml.Attributes[attributeName];
			}
			else
			{
				xmlAttribute = xml.OwnerDocument.CreateAttribute(attributeName);
				xml.Attributes.Append(xmlAttribute);
			}
			xmlAttribute.Value = setTo.ToString();
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00019480 File Offset: 0x00017680
		public static void SetChild(this XmlElement xml, string childName, object setTo)
		{
			XmlElement xmlElement;
			if (xml.HasChild(childName))
			{
				xmlElement = xml[childName];
			}
			else
			{
				xmlElement = xml.OwnerDocument.CreateElement(null, childName, xml.NamespaceURI);
				xml.AppendChild(xmlElement);
			}
			xmlElement.InnerText = setTo.ToString();
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x000194C8 File Offset: 0x000176C8
		public static XmlElement CreateChild(this XmlDocument doc, string childName)
		{
			XmlElement xmlElement = doc.CreateElement(null, childName, doc.NamespaceURI);
			doc.AppendChild(xmlElement);
			return xmlElement;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x000194F0 File Offset: 0x000176F0
		public static XmlElement CreateChild(this XmlElement xml, string childName)
		{
			XmlElement xmlElement = xml.OwnerDocument.CreateElement(null, childName, xml.NamespaceURI);
			xml.AppendChild(xmlElement);
			return xmlElement;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x0001951A File Offset: 0x0001771A
		public static int SortLeftToRight(Entity a, Entity b)
		{
			return (int)((a.X - b.X) * 100f);
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00019530 File Offset: 0x00017730
		public static int SortRightToLeft(Entity a, Entity b)
		{
			return (int)((b.X - a.X) * 100f);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x00019546 File Offset: 0x00017746
		public static int SortTopToBottom(Entity a, Entity b)
		{
			return (int)((a.Y - b.Y) * 100f);
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x0001955C File Offset: 0x0001775C
		public static int SortBottomToTop(Entity a, Entity b)
		{
			return (int)((b.Y - a.Y) * 100f);
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00019572 File Offset: 0x00017772
		public static int SortByDepth(Entity a, Entity b)
		{
			return a.Depth - b.Depth;
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x00019581 File Offset: 0x00017781
		public static int SortByDepthReversed(Entity a, Entity b)
		{
			return b.Depth - a.Depth;
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void Log()
		{
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void TimeLog()
		{
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00019590 File Offset: 0x00017790
		public static void Log(params object[] obj)
		{
			foreach (object obj2 in obj)
			{
			}
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void TimeLog(object obj)
		{
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x000195B4 File Offset: 0x000177B4
		public static void LogEach<T>(IEnumerable<T> collection)
		{
			foreach (T t in collection)
			{
			}
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x000195F8 File Offset: 0x000177F8
		public static void Dissect(object obj)
		{
			foreach (FieldInfo fieldInfo in obj.GetType().GetFields())
			{
			}
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00019623 File Offset: 0x00017823
		public static void StartTimer()
		{
			Calc.stopwatch = new Stopwatch();
			Calc.stopwatch.Start();
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x0001963C File Offset: 0x0001783C
		public static void EndTimer()
		{
			if (Calc.stopwatch != null)
			{
				Calc.stopwatch.Stop();
				string.Concat(new object[]
				{
					"Timer: ",
					Calc.stopwatch.ElapsedTicks,
					" ticks, or ",
					TimeSpan.FromTicks(Calc.stopwatch.ElapsedTicks).TotalSeconds.ToString("00.0000000"),
					" seconds"
				});
				Calc.stopwatch = null;
			}
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x000196BD File Offset: 0x000178BD
		public static Delegate GetMethod<T>(object obj, string method) where T : class
		{
			if (obj.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null)
			{
				return null;
			}
			return Delegate.CreateDelegate(typeof(T), obj, method);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x000196E8 File Offset: 0x000178E8
		public static T At<T>(this T[,] arr, Pnt at)
		{
			return arr[at.X, at.Y];
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x000196FC File Offset: 0x000178FC
		public static string ConvertPath(string path)
		{
			return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x00019718 File Offset: 0x00017918
		public static string ReadNullTerminatedString(this BinaryReader stream)
		{
			string text = "";
			char c;
			while ((c = stream.ReadChar()) != '\0')
			{
				text += c.ToString();
			}
			return text;
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00019746 File Offset: 0x00017946
		public static IEnumerator Do(params IEnumerator[] numerators)
		{
			if (numerators.Length == 0)
			{
				yield break;
			}
			if (numerators.Length == 1)
			{
				yield return numerators[0];
			}
			else
			{
				List<Coroutine> routines = new List<Coroutine>();
				foreach (IEnumerator functionCall in numerators)
				{
					routines.Add(new Coroutine(functionCall, true));
				}
				for (;;)
				{
					bool flag = false;
					foreach (Coroutine coroutine in routines)
					{
						coroutine.Update();
						if (!coroutine.Finished)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						break;
					}
					yield return null;
				}
				routines = null;
			}
			yield break;
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00019758 File Offset: 0x00017958
		public static Rectangle ClampTo(this Rectangle rect, Rectangle clamp)
		{
			if (rect.X < clamp.X)
			{
				rect.Width -= clamp.X - rect.X;
				rect.X = clamp.X;
			}
			if (rect.Y < clamp.Y)
			{
				rect.Height -= clamp.Y - rect.Y;
				rect.Y = clamp.Y;
			}
			if (rect.Right > clamp.Right)
			{
				rect.Width = clamp.Right - rect.X;
			}
			if (rect.Bottom > clamp.Bottom)
			{
				rect.Height = clamp.Bottom - rect.Y;
			}
			return rect;
		}

		// Token: 0x0400061E RID: 1566
		public static Random Random = new Random();

		// Token: 0x0400061F RID: 1567
		private static Stack<Random> randomStack = new Stack<Random>();

		// Token: 0x04000620 RID: 1568
		private static int[] shakeVectorOffsets = new int[]
		{
			-1,
			-1,
			0,
			1,
			1
		};

		// Token: 0x04000621 RID: 1569
		public const float Right = 0f;

		// Token: 0x04000622 RID: 1570
		public const float Up = -1.5707964f;

		// Token: 0x04000623 RID: 1571
		public const float Left = 3.1415927f;

		// Token: 0x04000624 RID: 1572
		public const float Down = 1.5707964f;

		// Token: 0x04000625 RID: 1573
		public const float UpRight = -0.7853982f;

		// Token: 0x04000626 RID: 1574
		public const float UpLeft = -2.3561945f;

		// Token: 0x04000627 RID: 1575
		public const float DownRight = 0.7853982f;

		// Token: 0x04000628 RID: 1576
		public const float DownLeft = 2.3561945f;

		// Token: 0x04000629 RID: 1577
		public const float DegToRad = 0.017453292f;

		// Token: 0x0400062A RID: 1578
		public const float RadToDeg = 57.295776f;

		// Token: 0x0400062B RID: 1579
		public const float DtR = 0.017453292f;

		// Token: 0x0400062C RID: 1580
		public const float RtD = 57.295776f;

		// Token: 0x0400062D RID: 1581
		public const float Circle = 6.2831855f;

		// Token: 0x0400062E RID: 1582
		public const float HalfCircle = 3.1415927f;

		// Token: 0x0400062F RID: 1583
		public const float QuarterCircle = 1.5707964f;

		// Token: 0x04000630 RID: 1584
		public const float EighthCircle = 0.7853982f;

		// Token: 0x04000631 RID: 1585
		private const string Hex = "0123456789ABCDEF";

		// Token: 0x04000632 RID: 1586
		private static Stopwatch stopwatch;
	}
}

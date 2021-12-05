using System;
using System.Collections.Generic;
using System.Globalization;

namespace Monocle
{
	// Token: 0x02000126 RID: 294
	public class Chooser<T>
	{
		// Token: 0x06000A7C RID: 2684 RVA: 0x0001A3FD File Offset: 0x000185FD
		public Chooser()
		{
			this.choices = new List<Chooser<T>.Choice>();
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x0001A410 File Offset: 0x00018610
		public Chooser(T firstChoice, float weight) : this()
		{
			this.Add(firstChoice, weight);
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x0001A424 File Offset: 0x00018624
		public Chooser(params T[] choices) : this()
		{
			foreach (T choice in choices)
			{
				this.Add(choice, 1f);
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000A7F RID: 2687 RVA: 0x0001A45C File Offset: 0x0001865C
		public int Count
		{
			get
			{
				return this.choices.Count;
			}
		}

		// Token: 0x170000E6 RID: 230
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return this.choices[index].Value;
			}
			set
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException();
				}
				this.choices[index].Value = value;
			}
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x0001A4B6 File Offset: 0x000186B6
		public Chooser<T> Add(T choice, float weight)
		{
			weight = Math.Max(weight, 0f);
			this.choices.Add(new Chooser<T>.Choice(choice, weight));
			this.TotalWeight += weight;
			return this;
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0001A4E8 File Offset: 0x000186E8
		public T Choose()
		{
			if (this.TotalWeight <= 0f)
			{
				return default(T);
			}
			if (this.choices.Count == 1)
			{
				return this.choices[0].Value;
			}
			double num = Calc.Random.NextDouble() * (double)this.TotalWeight;
			float num2 = 0f;
			for (int i = 0; i < this.choices.Count - 1; i++)
			{
				num2 += this.choices[i].Weight;
				if (num < (double)num2)
				{
					return this.choices[i].Value;
				}
			}
			return this.choices[this.choices.Count - 1].Value;
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000A84 RID: 2692 RVA: 0x0001A5A5 File Offset: 0x000187A5
		// (set) Token: 0x06000A85 RID: 2693 RVA: 0x0001A5AD File Offset: 0x000187AD
		public float TotalWeight { get; private set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000A86 RID: 2694 RVA: 0x0001A5B6 File Offset: 0x000187B6
		public bool CanChoose
		{
			get
			{
				return this.TotalWeight > 0f;
			}
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0001A5C8 File Offset: 0x000187C8
		public static Chooser<TT> FromString<TT>(string data) where TT : IConvertible
		{
			Chooser<TT> chooser = new Chooser<TT>();
			string[] array = data.Split(new char[]
			{
				','
			});
			if (array.Length == 1 && array[0].IndexOf(':') == -1)
			{
				chooser.Add((TT)((object)Convert.ChangeType(array[0], typeof(TT))), 1f);
				return chooser;
			}
			foreach (string text in array)
			{
				if (text.IndexOf(':') == -1)
				{
					chooser.Add((TT)((object)Convert.ChangeType(text, typeof(TT))), 1f);
				}
				else
				{
					string[] array3 = text.Split(new char[]
					{
						':'
					});
					string value = array3[0].Trim();
					string value2 = array3[1].Trim();
					chooser.Add((TT)((object)Convert.ChangeType(value, typeof(TT))), Convert.ToSingle(value2, CultureInfo.InvariantCulture));
				}
			}
			return chooser;
		}

		// Token: 0x04000642 RID: 1602
		private List<Chooser<T>.Choice> choices;

		// Token: 0x020003B5 RID: 949
		private class Choice
		{
			// Token: 0x06001EA7 RID: 7847 RVA: 0x000D58B4 File Offset: 0x000D3AB4
			public Choice(T value, float weight)
			{
				this.Value = value;
				this.Weight = weight;
			}

			// Token: 0x04001F4F RID: 8015
			public T Value;

			// Token: 0x04001F50 RID: 8016
			public float Weight;
		}
	}
}

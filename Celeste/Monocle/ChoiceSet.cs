using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x02000125 RID: 293
	public class ChoiceSet<T>
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000A72 RID: 2674 RVA: 0x0001A123 File Offset: 0x00018323
		// (set) Token: 0x06000A73 RID: 2675 RVA: 0x0001A12B File Offset: 0x0001832B
		public int TotalWeight { get; private set; }

		// Token: 0x06000A74 RID: 2676 RVA: 0x0001A134 File Offset: 0x00018334
		public ChoiceSet()
		{
			this.choices = new Dictionary<T, int>();
			this.TotalWeight = 0;
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x0001A150 File Offset: 0x00018350
		public void Set(T choice, int weight)
		{
			int num = 0;
			this.choices.TryGetValue(choice, out num);
			this.TotalWeight -= num;
			if (weight <= 0)
			{
				if (this.choices.ContainsKey(choice))
				{
					this.choices.Remove(choice);
					return;
				}
			}
			else
			{
				this.TotalWeight += weight;
				this.choices[choice] = weight;
			}
		}

		// Token: 0x170000E4 RID: 228
		public int this[T choice]
		{
			get
			{
				int result = 0;
				this.choices.TryGetValue(choice, out result);
				return result;
			}
			set
			{
				this.Set(choice, value);
			}
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x0001A1E4 File Offset: 0x000183E4
		public void Set(T choice, float chance)
		{
			int num = 0;
			this.choices.TryGetValue(choice, out num);
			this.TotalWeight -= num;
			int num2 = (int)Math.Round((double)((float)this.TotalWeight / (1f - chance)));
			if (num2 <= 0 && chance > 0f)
			{
				num2 = 1;
			}
			if (num2 <= 0)
			{
				if (this.choices.ContainsKey(choice))
				{
					this.choices.Remove(choice);
					return;
				}
			}
			else
			{
				this.TotalWeight += num2;
				this.choices[choice] = num2;
			}
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x0001A270 File Offset: 0x00018470
		public void SetMany(float totalChance, params T[] choices)
		{
			if (choices.Length != 0)
			{
				float num = totalChance / (float)choices.Length;
				int num2 = 0;
				foreach (T key in choices)
				{
					int num3 = 0;
					this.choices.TryGetValue(key, out num3);
					num2 += num3;
				}
				this.TotalWeight -= num2;
				int num4 = (int)Math.Round((double)((float)this.TotalWeight / (1f - totalChance) / (float)choices.Length));
				if (num4 <= 0 && totalChance > 0f)
				{
					num4 = 1;
				}
				if (num4 <= 0)
				{
					foreach (T key2 in choices)
					{
						if (this.choices.ContainsKey(key2))
						{
							this.choices.Remove(key2);
						}
					}
					return;
				}
				this.TotalWeight += num4 * choices.Length;
				foreach (T key3 in choices)
				{
					this.choices[key3] = num4;
				}
			}
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x0001A36C File Offset: 0x0001856C
		public T Get(Random random)
		{
			int num = random.Next(this.TotalWeight);
			foreach (KeyValuePair<T, int> keyValuePair in this.choices)
			{
				if (num < keyValuePair.Value)
				{
					return keyValuePair.Key;
				}
				num -= keyValuePair.Value;
			}
			throw new Exception("Random choice error!");
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x0001A3F0 File Offset: 0x000185F0
		public T Get()
		{
			return this.Get(Calc.Random);
		}

		// Token: 0x04000641 RID: 1601
		private Dictionary<T, int> choices;

		// Token: 0x020003B4 RID: 948
		private struct Choice
		{
			// Token: 0x06001EA6 RID: 7846 RVA: 0x000D58A4 File Offset: 0x000D3AA4
			public Choice(T data, int weight)
			{
				this.Data = data;
				this.Weight = weight;
			}

			// Token: 0x04001F4D RID: 8013
			public T Data;

			// Token: 0x04001F4E RID: 8014
			public int Weight;
		}
	}
}

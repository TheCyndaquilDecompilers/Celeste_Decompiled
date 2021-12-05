using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x020000FE RID: 254
	public class CounterSet<T> : Component
	{
		// Token: 0x060006D9 RID: 1753 RVA: 0x0000BB16 File Offset: 0x00009D16
		public CounterSet() : base(true, false)
		{
			this.counters = new Dictionary<T, float>();
		}

		// Token: 0x17000075 RID: 117
		public float this[T index]
		{
			get
			{
				float num;
				if (this.counters.TryGetValue(index, out num))
				{
					return Math.Max(num - this.timer, 0f);
				}
				return 0f;
			}
			set
			{
				this.counters[index] = this.timer + value;
			}
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x0000BB78 File Offset: 0x00009D78
		public bool Check(T index)
		{
			float num;
			return this.counters.TryGetValue(index, out num) && num - this.timer > 0f;
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x0000BBA6 File Offset: 0x00009DA6
		public override void Update()
		{
			this.timer += Engine.DeltaTime;
		}

		// Token: 0x0400050C RID: 1292
		private Dictionary<T, float> counters;

		// Token: 0x0400050D RID: 1293
		private float timer;
	}
}

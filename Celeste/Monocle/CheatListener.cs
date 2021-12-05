using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x02000124 RID: 292
	public class CheatListener : Entity
	{
		// Token: 0x06000A6E RID: 2670 RVA: 0x00019F36 File Offset: 0x00018136
		public CheatListener()
		{
			this.Visible = false;
			this.CurrentInput = "";
			this.inputs = new List<Tuple<char, Func<bool>>>();
			this.cheats = new List<Tuple<string, Action>>();
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x00019F68 File Offset: 0x00018168
		public override void Update()
		{
			bool flag = false;
			foreach (Tuple<char, Func<bool>> tuple in this.inputs)
			{
				if (tuple.Item2())
				{
					this.CurrentInput += tuple.Item1.ToString();
					flag = true;
				}
			}
			if (flag)
			{
				if (this.CurrentInput.Length > this.maxInput)
				{
					this.CurrentInput = this.CurrentInput.Substring(this.CurrentInput.Length - this.maxInput);
				}
				if (this.Logging)
				{
					Calc.Log(new object[]
					{
						this.CurrentInput
					});
				}
				foreach (Tuple<string, Action> tuple2 in this.cheats)
				{
					if (this.CurrentInput.Contains(tuple2.Item1))
					{
						this.CurrentInput = "";
						if (tuple2.Item2 != null)
						{
							tuple2.Item2();
						}
						this.cheats.Remove(tuple2);
						if (this.Logging)
						{
							Calc.Log(new object[]
							{
								"Cheat Activated: " + tuple2.Item1
							});
							break;
						}
						break;
					}
				}
			}
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0001A0E4 File Offset: 0x000182E4
		public void AddCheat(string code, Action onEntered = null)
		{
			this.cheats.Add(new Tuple<string, Action>(code, onEntered));
			this.maxInput = Math.Max(code.Length, this.maxInput);
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x0001A10F File Offset: 0x0001830F
		public void AddInput(char id, Func<bool> checker)
		{
			this.inputs.Add(new Tuple<char, Func<bool>>(id, checker));
		}

		// Token: 0x0400063B RID: 1595
		public string CurrentInput;

		// Token: 0x0400063C RID: 1596
		public bool Logging;

		// Token: 0x0400063D RID: 1597
		private List<Tuple<char, Func<bool>>> inputs;

		// Token: 0x0400063E RID: 1598
		private List<Tuple<string, Action>> cheats;

		// Token: 0x0400063F RID: 1599
		private int maxInput;
	}
}

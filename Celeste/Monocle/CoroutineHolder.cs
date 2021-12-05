using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x020000FD RID: 253
	public class CoroutineHolder : Component
	{
		// Token: 0x060006D4 RID: 1748 RVA: 0x0000B8FD File Offset: 0x00009AFD
		public CoroutineHolder() : base(true, false)
		{
			this.coroutineList = new List<CoroutineHolder.CoroutineData>();
			this.toRemove = new HashSet<CoroutineHolder.CoroutineData>();
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0000B920 File Offset: 0x00009B20
		public override void Update()
		{
			this.isRunning = true;
			for (int i = 0; i < this.coroutineList.Count; i++)
			{
				IEnumerator enumerator = this.coroutineList[i].Data.Peek();
				if (enumerator.MoveNext())
				{
					if (enumerator.Current is IEnumerator)
					{
						this.coroutineList[i].Data.Push(enumerator.Current as IEnumerator);
					}
				}
				else
				{
					this.coroutineList[i].Data.Pop();
					if (this.coroutineList[i].Data.Count == 0)
					{
						this.toRemove.Add(this.coroutineList[i]);
					}
				}
			}
			this.isRunning = false;
			if (this.toRemove.Count > 0)
			{
				foreach (CoroutineHolder.CoroutineData item in this.toRemove)
				{
					this.coroutineList.Remove(item);
				}
				this.toRemove.Clear();
			}
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0000BA54 File Offset: 0x00009C54
		public void EndCoroutine(int id)
		{
			foreach (CoroutineHolder.CoroutineData coroutineData in this.coroutineList)
			{
				if (coroutineData.ID == id)
				{
					if (this.isRunning)
					{
						this.toRemove.Add(coroutineData);
						break;
					}
					this.coroutineList.Remove(coroutineData);
					break;
				}
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0000BAD0 File Offset: 0x00009CD0
		public int StartCoroutine(IEnumerator functionCall)
		{
			int num = this.nextID;
			this.nextID = num + 1;
			CoroutineHolder.CoroutineData coroutineData = new CoroutineHolder.CoroutineData(num, functionCall);
			this.coroutineList.Add(coroutineData);
			return coroutineData.ID;
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x0000BB07 File Offset: 0x00009D07
		public static IEnumerator WaitForFrames(int frames)
		{
			int num;
			for (int i = 0; i < frames; i = num + 1)
			{
				yield return 0;
				num = i;
			}
			yield break;
		}

		// Token: 0x04000508 RID: 1288
		private List<CoroutineHolder.CoroutineData> coroutineList;

		// Token: 0x04000509 RID: 1289
		private HashSet<CoroutineHolder.CoroutineData> toRemove;

		// Token: 0x0400050A RID: 1290
		private int nextID;

		// Token: 0x0400050B RID: 1291
		private bool isRunning;

		// Token: 0x020003A1 RID: 929
		private class CoroutineData
		{
			// Token: 0x06001E16 RID: 7702 RVA: 0x000D3814 File Offset: 0x000D1A14
			public CoroutineData(int id, IEnumerator functionCall)
			{
				this.ID = id;
				this.Data = new Stack<IEnumerator>();
				this.Data.Push(functionCall);
			}

			// Token: 0x04001F02 RID: 7938
			public int ID;

			// Token: 0x04001F03 RID: 7939
			public Stack<IEnumerator> Data;
		}
	}
}

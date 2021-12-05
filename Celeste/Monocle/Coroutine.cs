using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x020000FC RID: 252
	public class Coroutine : Component
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x0000B723 File Offset: 0x00009923
		// (set) Token: 0x060006CE RID: 1742 RVA: 0x0000B72B File Offset: 0x0000992B
		public bool Finished { get; private set; }

		// Token: 0x060006CF RID: 1743 RVA: 0x0000B734 File Offset: 0x00009934
		public Coroutine(IEnumerator functionCall, bool removeOnComplete = true) : base(true, false)
		{
			this.enumerators = new Stack<IEnumerator>();
			this.enumerators.Push(functionCall);
			this.RemoveOnComplete = removeOnComplete;
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0000B763 File Offset: 0x00009963
		public Coroutine(bool removeOnComplete = true) : base(false, false)
		{
			this.RemoveOnComplete = removeOnComplete;
			this.enumerators = new Stack<IEnumerator>();
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0000B788 File Offset: 0x00009988
		public override void Update()
		{
			this.ended = false;
			if (this.waitTimer > 0f)
			{
				this.waitTimer -= (this.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);
				return;
			}
			if (this.enumerators.Count > 0)
			{
				IEnumerator enumerator = this.enumerators.Peek();
				if (enumerator.MoveNext() && !this.ended)
				{
					if (enumerator.Current is int)
					{
						this.waitTimer = (float)((int)enumerator.Current);
					}
					if (enumerator.Current is float)
					{
						this.waitTimer = (float)enumerator.Current;
						return;
					}
					if (enumerator.Current is IEnumerator)
					{
						this.enumerators.Push(enumerator.Current as IEnumerator);
						return;
					}
				}
				else if (!this.ended)
				{
					this.enumerators.Pop();
					if (this.enumerators.Count == 0)
					{
						this.Finished = true;
						this.Active = false;
						if (this.RemoveOnComplete)
						{
							base.RemoveSelf();
						}
					}
				}
			}
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x0000B897 File Offset: 0x00009A97
		public void Cancel()
		{
			this.Active = false;
			this.Finished = true;
			this.waitTimer = 0f;
			this.enumerators.Clear();
			this.ended = true;
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x0000B8C4 File Offset: 0x00009AC4
		public void Replace(IEnumerator functionCall)
		{
			this.Active = true;
			this.Finished = false;
			this.waitTimer = 0f;
			this.enumerators.Clear();
			this.enumerators.Push(functionCall);
			this.ended = true;
		}

		// Token: 0x04000503 RID: 1283
		public bool RemoveOnComplete = true;

		// Token: 0x04000504 RID: 1284
		public bool UseRawDeltaTime;

		// Token: 0x04000505 RID: 1285
		private Stack<IEnumerator> enumerators;

		// Token: 0x04000506 RID: 1286
		private float waitTimer;

		// Token: 0x04000507 RID: 1287
		private bool ended;
	}
}

using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200018B RID: 395
	[Tracked(false)]
	public class BeforeRenderHook : Component
	{
		// Token: 0x06000DDA RID: 3546 RVA: 0x00031948 File Offset: 0x0002FB48
		public BeforeRenderHook(Action callback) : base(false, true)
		{
			this.Callback = callback;
		}

		// Token: 0x04000926 RID: 2342
		public Action Callback;
	}
}

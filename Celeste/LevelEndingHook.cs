using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000159 RID: 345
	[Tracked(false)]
	public class LevelEndingHook : Component
	{
		// Token: 0x06000C6E RID: 3182 RVA: 0x00029B55 File Offset: 0x00027D55
		public LevelEndingHook(Action onEnd) : base(false, false)
		{
			this.OnEnd = onEnd;
		}

		// Token: 0x040007DA RID: 2010
		public Action OnEnd;
	}
}

using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200018D RID: 397
	[Tracked(false)]
	public class CustomBloom : Component
	{
		// Token: 0x06000DDD RID: 3549 RVA: 0x00031995 File Offset: 0x0002FB95
		public CustomBloom(Action onRenderBloom) : base(false, true)
		{
			this.OnRenderBloom = onRenderBloom;
		}

		// Token: 0x0400092B RID: 2347
		public Action OnRenderBloom;
	}
}

using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000278 RID: 632
	[Tracked(false)]
	public class DustEdge : Component
	{
		// Token: 0x06001396 RID: 5014 RVA: 0x0006A9EE File Offset: 0x00068BEE
		public DustEdge(Action onRenderDust) : base(false, true)
		{
			this.RenderDust = onRenderDust;
		}

		// Token: 0x04000F6A RID: 3946
		public Action RenderDust;
	}
}

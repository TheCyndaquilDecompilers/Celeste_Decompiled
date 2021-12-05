using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200018E RID: 398
	[Tracked(false)]
	public class DisplacementRenderHook : Component
	{
		// Token: 0x06000DDE RID: 3550 RVA: 0x000319A6 File Offset: 0x0002FBA6
		public DisplacementRenderHook(Action render) : base(false, true)
		{
			this.RenderDisplacement = render;
		}

		// Token: 0x0400092C RID: 2348
		public Action RenderDisplacement;
	}
}

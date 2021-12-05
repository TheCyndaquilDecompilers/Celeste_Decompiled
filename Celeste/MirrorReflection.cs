using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200021D RID: 541
	[Tracked(false)]
	public class MirrorReflection : Component
	{
		// Token: 0x06001177 RID: 4471 RVA: 0x00056A08 File Offset: 0x00054C08
		public MirrorReflection() : base(false, true)
		{
		}

		// Token: 0x04000D18 RID: 3352
		public bool IgnoreEntityVisible;

		// Token: 0x04000D19 RID: 3353
		public bool IsRendering;
	}
}

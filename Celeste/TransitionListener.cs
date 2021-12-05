using System;
using Monocle;

namespace Celeste
{
	// Token: 0x020002BE RID: 702
	[Tracked(false)]
	public class TransitionListener : Component
	{
		// Token: 0x060015A6 RID: 5542 RVA: 0x0000B61B File Offset: 0x0000981B
		public TransitionListener() : base(false, false)
		{
		}

		// Token: 0x040011C3 RID: 4547
		public Action OnInBegin;

		// Token: 0x040011C4 RID: 4548
		public Action OnInEnd;

		// Token: 0x040011C5 RID: 4549
		public Action<float> OnIn;

		// Token: 0x040011C6 RID: 4550
		public Action OnOutBegin;

		// Token: 0x040011C7 RID: 4551
		public Action<float> OnOut;
	}
}

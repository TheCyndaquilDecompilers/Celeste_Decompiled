using System;

namespace Monocle
{
	// Token: 0x02000139 RID: 313
	public class Tracked : Attribute
	{
		// Token: 0x06000B62 RID: 2914 RVA: 0x000203E0 File Offset: 0x0001E5E0
		public Tracked(bool inherited = false)
		{
			this.Inherited = inherited;
		}

		// Token: 0x040006C5 RID: 1733
		public bool Inherited;
	}
}

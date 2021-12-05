using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200015A RID: 346
	[Tracked(false)]
	public class PostUpdateHook : Component
	{
		// Token: 0x06000C6F RID: 3183 RVA: 0x00029B66 File Offset: 0x00027D66
		public PostUpdateHook(Action onPostUpdate) : base(false, false)
		{
			this.OnPostUpdate = onPostUpdate;
		}

		// Token: 0x040007DB RID: 2011
		public Action OnPostUpdate;
	}
}

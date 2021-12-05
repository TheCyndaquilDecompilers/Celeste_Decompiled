using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000155 RID: 341
	[Tracked(false)]
	public class CoreModeListener : Component
	{
		// Token: 0x06000C58 RID: 3160 RVA: 0x00028A34 File Offset: 0x00026C34
		public CoreModeListener(Action<Session.CoreModes> onChange) : base(false, false)
		{
			this.OnChange = onChange;
		}

		// Token: 0x040007BA RID: 1978
		public Action<Session.CoreModes> OnChange;
	}
}

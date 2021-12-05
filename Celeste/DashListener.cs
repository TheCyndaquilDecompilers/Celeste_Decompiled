using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B7 RID: 695
	[Tracked(false)]
	public class DashListener : Component
	{
		// Token: 0x06001573 RID: 5491 RVA: 0x0000B61B File Offset: 0x0000981B
		public DashListener() : base(false, false)
		{
		}

		// Token: 0x04001189 RID: 4489
		public Action<Vector2> OnDash;

		// Token: 0x0400118A RID: 4490
		public Action OnSet;
	}
}

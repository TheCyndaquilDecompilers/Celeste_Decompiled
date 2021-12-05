using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200021E RID: 542
	[Tracked(false)]
	public class HoldableCollider : Component
	{
		// Token: 0x06001178 RID: 4472 RVA: 0x00056A12 File Offset: 0x00054C12
		public HoldableCollider(Action<Holdable> onCollide, Collider collider = null) : base(false, false)
		{
			this.collider = collider;
			this.OnCollide = onCollide;
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x00056A2C File Offset: 0x00054C2C
		public bool Check(Holdable holdable)
		{
			Collider collider = base.Entity.Collider;
			if (this.collider != null)
			{
				base.Entity.Collider = this.collider;
			}
			bool result = holdable.Entity.CollideCheck(base.Entity);
			base.Entity.Collider = collider;
			return result;
		}

		// Token: 0x04000D1A RID: 3354
		private Collider collider;

		// Token: 0x04000D1B RID: 3355
		public Action<Holdable> OnCollide;
	}
}

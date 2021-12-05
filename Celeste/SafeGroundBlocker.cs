using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002BA RID: 698
	[Tracked(false)]
	public class SafeGroundBlocker : Component
	{
		// Token: 0x06001585 RID: 5509 RVA: 0x0007C410 File Offset: 0x0007A610
		public SafeGroundBlocker(Collider checkWith = null) : base(false, false)
		{
			this.CheckWith = checkWith;
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x0007C428 File Offset: 0x0007A628
		public bool Check(Player player)
		{
			if (!this.Blocking)
			{
				return false;
			}
			Collider collider = base.Entity.Collider;
			if (this.CheckWith != null)
			{
				base.Entity.Collider = this.CheckWith;
			}
			bool result = player.CollideCheck(base.Entity);
			base.Entity.Collider = collider;
			return result;
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x0007C47C File Offset: 0x0007A67C
		public override void DebugRender(Camera camera)
		{
			Collider collider = base.Entity.Collider;
			if (this.CheckWith != null)
			{
				base.Entity.Collider = this.CheckWith;
			}
			base.Entity.Collider.Render(camera, Color.Aqua);
			base.Entity.Collider = collider;
		}

		// Token: 0x040011AC RID: 4524
		public bool Blocking = true;

		// Token: 0x040011AD RID: 4525
		public Collider CheckWith;
	}
}

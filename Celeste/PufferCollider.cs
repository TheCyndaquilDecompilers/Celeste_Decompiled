using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200015B RID: 347
	[Tracked(false)]
	public class PufferCollider : Component
	{
		// Token: 0x06000C70 RID: 3184 RVA: 0x00029B77 File Offset: 0x00027D77
		public PufferCollider(Action<Puffer> onCollide, Collider collider = null) : base(false, false)
		{
			this.OnCollide = onCollide;
			this.Collider = null;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00029B90 File Offset: 0x00027D90
		public void Check(Puffer puffer)
		{
			if (this.OnCollide != null)
			{
				Collider collider = base.Entity.Collider;
				if (this.Collider != null)
				{
					base.Entity.Collider = this.Collider;
				}
				if (puffer.CollideCheck(base.Entity))
				{
					this.OnCollide(puffer);
				}
				base.Entity.Collider = collider;
			}
		}

		// Token: 0x040007DC RID: 2012
		public Action<Puffer> OnCollide;

		// Token: 0x040007DD RID: 2013
		public Collider Collider;
	}
}

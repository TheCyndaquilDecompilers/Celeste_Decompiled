using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200015C RID: 348
	[Tracked(false)]
	public class SeekerCollider : Component
	{
		// Token: 0x06000C72 RID: 3186 RVA: 0x00029BF0 File Offset: 0x00027DF0
		public SeekerCollider(Action<Seeker> onCollide, Collider collider = null) : base(false, false)
		{
			this.OnCollide = onCollide;
			this.Collider = null;
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x00029C08 File Offset: 0x00027E08
		public void Check(Seeker seeker)
		{
			if (this.OnCollide != null)
			{
				Collider collider = base.Entity.Collider;
				if (this.Collider != null)
				{
					base.Entity.Collider = this.Collider;
				}
				if (seeker.CollideCheck(base.Entity))
				{
					this.OnCollide(seeker);
				}
				base.Entity.Collider = collider;
			}
		}

		// Token: 0x040007DE RID: 2014
		public Action<Seeker> OnCollide;

		// Token: 0x040007DF RID: 2015
		public Collider Collider;
	}
}

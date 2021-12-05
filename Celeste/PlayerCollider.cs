using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200034F RID: 847
	[Tracked(false)]
	public class PlayerCollider : Component
	{
		// Token: 0x06001A90 RID: 6800 RVA: 0x000AB7E8 File Offset: 0x000A99E8
		public PlayerCollider(Action<Player> onCollide, Collider collider = null, Collider featherCollider = null) : base(false, false)
		{
			this.OnCollide = onCollide;
			this.Collider = collider;
			this.FeatherCollider = featherCollider;
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x000AB808 File Offset: 0x000A9A08
		public bool Check(Player player)
		{
			Collider collider = this.Collider;
			if (this.FeatherCollider != null && player.StateMachine.State == 19)
			{
				collider = this.FeatherCollider;
			}
			if (collider == null)
			{
				if (player.CollideCheck(base.Entity))
				{
					this.OnCollide(player);
					return true;
				}
				return false;
			}
			else
			{
				Collider collider2 = base.Entity.Collider;
				base.Entity.Collider = collider;
				bool flag = player.CollideCheck(base.Entity);
				base.Entity.Collider = collider2;
				if (flag)
				{
					this.OnCollide(player);
					return true;
				}
				return false;
			}
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x000AB89C File Offset: 0x000A9A9C
		public override void DebugRender(Camera camera)
		{
			if (this.Collider != null)
			{
				Collider collider = base.Entity.Collider;
				base.Entity.Collider = this.Collider;
				this.Collider.Render(camera, Color.HotPink);
				base.Entity.Collider = collider;
			}
		}

		// Token: 0x0400172E RID: 5934
		public Action<Player> OnCollide;

		// Token: 0x0400172F RID: 5935
		public Collider Collider;

		// Token: 0x04001730 RID: 5936
		public Collider FeatherCollider;
	}
}

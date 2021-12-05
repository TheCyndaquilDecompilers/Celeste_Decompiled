using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200032A RID: 810
	[Tracked(false)]
	public class RespawnTargetTrigger : Entity
	{
		// Token: 0x0600196F RID: 6511 RVA: 0x000A3918 File Offset: 0x000A1B18
		public RespawnTargetTrigger(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Collider = new Hitbox((float)data.Width, (float)data.Height, 0f, 0f);
			this.Target = data.Nodes[0] + offset;
			this.Visible = (this.Active = false);
		}

		// Token: 0x04001630 RID: 5680
		public Vector2 Target;
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D3 RID: 467
	[Tracked(false)]
	public class SpawnFacingTrigger : Entity
	{
		// Token: 0x06000FC8 RID: 4040 RVA: 0x00043020 File Offset: 0x00041220
		public SpawnFacingTrigger(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Collider = new Hitbox((float)data.Width, (float)data.Height, 0f, 0f);
			this.Facing = data.Enum<Facings>("facing", (Facings)0);
			this.Visible = (this.Active = false);
		}

		// Token: 0x04000B2E RID: 2862
		public Facings Facing;
	}
}

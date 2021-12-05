using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200032E RID: 814
	[Tracked(false)]
	public class InvisibleBarrier : Solid
	{
		// Token: 0x06001989 RID: 6537 RVA: 0x000A48A5 File Offset: 0x000A2AA5
		public InvisibleBarrier(Vector2 position, float width, float height) : base(position, width, height, true)
		{
			base.Tag = Tags.TransitionUpdate;
			this.Collidable = false;
			this.Visible = false;
			base.Add(new ClimbBlocker(true));
			this.SurfaceSoundIndex = 33;
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x000A48E3 File Offset: 0x000A2AE3
		public InvisibleBarrier(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x000A4905 File Offset: 0x000A2B05
		public override void Update()
		{
			this.Collidable = true;
			if (base.CollideCheck<Player>())
			{
				this.Collidable = false;
			}
			if (!this.Collidable)
			{
				this.Active = false;
			}
		}
	}
}

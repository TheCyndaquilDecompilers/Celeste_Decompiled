using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001E7 RID: 487
	[Tracked(false)]
	public class LookoutBlocker : Entity
	{
		// Token: 0x06001032 RID: 4146 RVA: 0x00046486 File Offset: 0x00044686
		public LookoutBlocker(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Collider = new Hitbox((float)data.Width, (float)data.Height, 0f, 0f);
		}
	}
}

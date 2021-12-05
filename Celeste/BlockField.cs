using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002AE RID: 686
	[Tracked(false)]
	public class BlockField : Entity
	{
		// Token: 0x06001530 RID: 5424 RVA: 0x00079436 File Offset: 0x00077636
		public BlockField(Vector2 position, int width, int height) : base(position)
		{
			base.Collider = new Hitbox((float)width, (float)height, 0f, 0f);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x00079458 File Offset: 0x00077658
		public BlockField(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height)
		{
		}
	}
}

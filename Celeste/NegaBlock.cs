using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000331 RID: 817
	[Tracked(false)]
	public class NegaBlock : Solid
	{
		// Token: 0x060019A0 RID: 6560 RVA: 0x000A510B File Offset: 0x000A330B
		public NegaBlock(Vector2 position, float width, float height) : base(position, width, height, false)
		{
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x000A5117 File Offset: 0x000A3317
		public NegaBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height)
		{
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x000A5139 File Offset: 0x000A3339
		public override void Render()
		{
			base.Render();
			Draw.Rect(base.Collider, Color.Red);
		}
	}
}

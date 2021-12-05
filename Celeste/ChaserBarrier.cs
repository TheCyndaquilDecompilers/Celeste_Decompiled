using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000335 RID: 821
	[Tracked(false)]
	public class ChaserBarrier : Entity
	{
		// Token: 0x060019BA RID: 6586 RVA: 0x00079436 File Offset: 0x00077636
		public ChaserBarrier(Vector2 position, int width, int height) : base(position)
		{
			base.Collider = new Hitbox((float)width, (float)height, 0f, 0f);
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x000A5E7D File Offset: 0x000A407D
		public ChaserBarrier(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height)
		{
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x000A5E9D File Offset: 0x000A409D
		public override void Render()
		{
			base.Render();
			Draw.Rect(base.Collider, Color.Red * 0.3f);
		}
	}
}

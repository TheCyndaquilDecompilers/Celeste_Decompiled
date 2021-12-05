using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000346 RID: 838
	public class Lamp : Entity
	{
		// Token: 0x06001A5F RID: 6751 RVA: 0x000A9740 File Offset: 0x000A7940
		public Lamp(Vector2 position, bool broken)
		{
			this.Position = position;
			base.Depth = 5;
			base.Add(this.sprite = new Image(GFX.Game["scenery/lamp"].GetSubtexture(broken ? 16 : 0, 0, 16, 80, null)));
			this.sprite.Origin = new Vector2(this.sprite.Width / 2f, this.sprite.Height);
			if (!broken)
			{
				base.Add(new BloomPoint(new Vector2(0f, -66f), 1f, 16f));
			}
		}

		// Token: 0x040016F4 RID: 5876
		private Image sprite;
	}
}

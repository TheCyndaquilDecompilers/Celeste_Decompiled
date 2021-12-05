using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001CD RID: 461
	public class TheoPhone : Entity
	{
		// Token: 0x06000FB3 RID: 4019 RVA: 0x00042754 File Offset: 0x00040954
		public TheoPhone(Vector2 position) : base(position)
		{
			base.Add(this.light = new VertexLight(Color.LawnGreen, 1f, 8, 16));
			base.Add(new Image(GFX.Game["characters/theo/phone"]).JustifyOrigin(0.5f, 1f));
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x000427B2 File Offset: 0x000409B2
		public override void Update()
		{
			if (base.Scene.OnInterval(0.5f))
			{
				this.light.Visible = !this.light.Visible;
			}
			base.Update();
		}

		// Token: 0x04000B1F RID: 2847
		private VertexLight light;
	}
}

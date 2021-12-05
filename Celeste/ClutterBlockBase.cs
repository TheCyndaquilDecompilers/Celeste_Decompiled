using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B3 RID: 691
	public class ClutterBlockBase : Solid
	{
		// Token: 0x06001557 RID: 5463 RVA: 0x0007AB98 File Offset: 0x00078D98
		public ClutterBlockBase(Vector2 position, int width, int height, bool enabled, ClutterBlock.Colors blockColor) : base(position, (float)width, (float)height, true)
		{
			this.EnableAssistModeChecks = false;
			this.BlockColor = blockColor;
			base.Depth = 8999;
			this.enabled = enabled;
			this.color = (enabled ? ClutterBlockBase.enabledColor : ClutterBlockBase.disabledColor);
			if (enabled)
			{
				base.Add(this.occluder = new LightOcclude(1f));
			}
			else
			{
				this.Collidable = false;
			}
			if (blockColor == ClutterBlock.Colors.Green)
			{
				this.SurfaceSoundIndex = 19;
				return;
			}
			if (blockColor == ClutterBlock.Colors.Red)
			{
				this.SurfaceSoundIndex = 17;
				return;
			}
			if (blockColor == ClutterBlock.Colors.Yellow)
			{
				this.SurfaceSoundIndex = 18;
			}
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0007AC37 File Offset: 0x00078E37
		public void Deactivate()
		{
			this.Collidable = false;
			this.color = ClutterBlockBase.disabledColor;
			this.enabled = false;
			if (this.occluder != null)
			{
				base.Remove(this.occluder);
				this.occluder = null;
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x0007AC6D File Offset: 0x00078E6D
		public override void Render()
		{
			Draw.Rect(base.X, base.Y, base.Width, base.Height + (float)(this.enabled ? 2 : 0), this.color);
		}

		// Token: 0x0400116B RID: 4459
		private static readonly Color enabledColor = Color.Black * 0.7f;

		// Token: 0x0400116C RID: 4460
		private static readonly Color disabledColor = Color.Black * 0.3f;

		// Token: 0x0400116D RID: 4461
		public ClutterBlock.Colors BlockColor;

		// Token: 0x0400116E RID: 4462
		private Color color;

		// Token: 0x0400116F RID: 4463
		private bool enabled;

		// Token: 0x04001170 RID: 4464
		private LightOcclude occluder;
	}
}

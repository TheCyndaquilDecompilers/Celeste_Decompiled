using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000203 RID: 515
	public class Plateau : Solid
	{
		// Token: 0x060010DB RID: 4315 RVA: 0x0004F714 File Offset: 0x0004D914
		public Plateau(EntityData e, Vector2 offset) : base(e.Position + offset, 104f, 4f, true)
		{
			base.Collider.Left += 8f;
			base.Add(this.sprite = new Image(GFX.Game["scenery/fallplateau"]));
			base.Add(this.Occluder = new LightOcclude(1f));
			this.SurfaceSoundIndex = 23;
			this.EnableAssistModeChecks = false;
		}

		// Token: 0x04000C7A RID: 3194
		private Image sprite;

		// Token: 0x04000C7B RID: 3195
		public LightOcclude Occluder;
	}
}

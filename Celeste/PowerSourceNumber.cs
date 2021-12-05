using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200017F RID: 383
	public class PowerSourceNumber : Entity
	{
		// Token: 0x06000D92 RID: 3474 RVA: 0x0002F8AC File Offset: 0x0002DAAC
		public PowerSourceNumber(Vector2 position, int index, bool gotCollectables)
		{
			this.Position = position;
			base.Depth = -10010;
			base.Add(this.image = new Image(GFX.Game["scenery/powersource_numbers/1"]));
			base.Add(this.glow = new Image(GFX.Game["scenery/powersource_numbers/1_glow"]));
			this.glow.Color = Color.Transparent;
			this.gotKey = gotCollectables;
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x0002F930 File Offset: 0x0002DB30
		public override void Update()
		{
			base.Update();
			if ((base.Scene as Level).Session.GetFlag("disable_lightning") && !this.gotKey)
			{
				this.timer += Engine.DeltaTime;
				this.ease = Calc.Approach(this.ease, 1f, Engine.DeltaTime * 4f);
				this.glow.Color = Color.White * this.ease * Calc.SineMap(this.timer * 2f, 0.5f, 0.9f);
			}
		}

		// Token: 0x040008C8 RID: 2248
		private readonly Image image;

		// Token: 0x040008C9 RID: 2249
		private readonly Image glow;

		// Token: 0x040008CA RID: 2250
		private float ease;

		// Token: 0x040008CB RID: 2251
		private float timer;

		// Token: 0x040008CC RID: 2252
		private bool gotKey;
	}
}

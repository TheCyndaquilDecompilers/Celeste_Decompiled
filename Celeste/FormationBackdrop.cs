using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200022E RID: 558
	public class FormationBackdrop : Entity
	{
		// Token: 0x060011C9 RID: 4553 RVA: 0x00058C52 File Offset: 0x00056E52
		public FormationBackdrop()
		{
			base.Tag = (Tags.FrozenUpdate | Tags.Global);
			base.Depth = -1999900;
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x00058C8C File Offset: 0x00056E8C
		public override void Update()
		{
			this.fade = Calc.Approach(this.fade, (float)(this.Display ? 1 : 0), Engine.RawDeltaTime * 3f);
			if (this.Display)
			{
				this.wasDisplayed = true;
			}
			if (this.wasDisplayed)
			{
				Level level = base.Scene as Level;
				Snow snow = level.Foreground.Get<Snow>();
				if (snow != null)
				{
					snow.Alpha = 1f - this.fade;
				}
				WindSnowFG windSnowFG = level.Foreground.Get<WindSnowFG>();
				if (windSnowFG != null)
				{
					windSnowFG.Alpha = 1f - this.fade;
				}
				if (this.fade <= 0f)
				{
					this.wasDisplayed = false;
				}
			}
			base.Update();
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x00058D40 File Offset: 0x00056F40
		public override void Render()
		{
			Level level = base.Scene as Level;
			if (this.fade > 0f)
			{
				Draw.Rect(level.Camera.Left - 1f, level.Camera.Top - 1f, 322f, 182f, Color.Black * this.fade * this.Alpha * 0.85f);
			}
		}

		// Token: 0x04000D6B RID: 3435
		public bool Display;

		// Token: 0x04000D6C RID: 3436
		public float Alpha = 1f;

		// Token: 0x04000D6D RID: 3437
		private bool wasDisplayed;

		// Token: 0x04000D6E RID: 3438
		private float fade;
	}
}

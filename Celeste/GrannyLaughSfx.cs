using System;
using Monocle;

namespace Celeste
{
	// Token: 0x02000157 RID: 343
	public class GrannyLaughSfx : Component
	{
		// Token: 0x06000C5D RID: 3165 RVA: 0x00028CFC File Offset: 0x00026EFC
		public GrannyLaughSfx(Sprite sprite) : base(true, false)
		{
			this.sprite = sprite;
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x00028D14 File Offset: 0x00026F14
		public override void Update()
		{
			if (this.sprite.CurrentAnimationID == "laugh" && this.sprite.CurrentAnimationFrame == 0 && this.ready)
			{
				if (this.FirstPlay)
				{
					Audio.Play("event:/char/granny/laugh_firstphrase", base.Entity.Position);
				}
				else
				{
					Audio.Play("event:/char/granny/laugh_oneha", base.Entity.Position);
				}
				this.ready = false;
			}
			if (!this.FirstPlay && (this.sprite.CurrentAnimationID != "laugh" || this.sprite.CurrentAnimationFrame > 0))
			{
				this.ready = true;
			}
		}

		// Token: 0x040007C1 RID: 1985
		public bool FirstPlay;

		// Token: 0x040007C2 RID: 1986
		private Sprite sprite;

		// Token: 0x040007C3 RID: 1987
		private bool ready = true;
	}
}

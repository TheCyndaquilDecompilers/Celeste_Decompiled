using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001BE RID: 446
	public class BackgroundFadeIn : Entity
	{
		// Token: 0x06000F58 RID: 3928 RVA: 0x0003E538 File Offset: 0x0003C738
		public BackgroundFadeIn(Color color, float delay, float duration)
		{
			base.Tag = (Tags.Persistent | Tags.TransitionUpdate);
			base.Depth = 10100;
			this.Color = color;
			this.Delay = delay;
			this.Duration = duration;
			this.Percent = 0f;
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0003E594 File Offset: 0x0003C794
		public override void Update()
		{
			if (this.Delay <= 0f)
			{
				if (this.Percent >= 1f)
				{
					base.RemoveSelf();
				}
				this.Percent += Engine.DeltaTime / this.Duration;
			}
			else
			{
				this.Delay -= Engine.DeltaTime;
			}
			base.Update();
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0003E5F4 File Offset: 0x0003C7F4
		public override void Render()
		{
			Vector2 position = (base.Scene as Level).Camera.Position;
			Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, this.Color * (1f - this.Percent));
		}

		// Token: 0x04000ABC RID: 2748
		public Color Color;

		// Token: 0x04000ABD RID: 2749
		public float Duration;

		// Token: 0x04000ABE RID: 2750
		public float Delay;

		// Token: 0x04000ABF RID: 2751
		public float Percent;
	}
}

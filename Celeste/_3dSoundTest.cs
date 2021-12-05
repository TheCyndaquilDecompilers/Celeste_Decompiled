using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002AD RID: 685
	public class _3dSoundTest : Entity
	{
		// Token: 0x0600152E RID: 5422 RVA: 0x00079328 File Offset: 0x00077528
		public _3dSoundTest(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Add(this.sfx = new SoundSource());
			this.sfx.Play("event:/3d_testing", null, 0f);
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x00079374 File Offset: 0x00077574
		public override void Render()
		{
			Draw.Rect(base.X - 8f, base.Y - 8f, 16f, 16f, Color.Yellow);
			Camera camera = (base.Scene as Level).Camera;
			Draw.HollowRect(base.X - 320f, camera.Y, 640f, 180f, Color.Red);
			Draw.HollowRect(base.X - 160f, camera.Y, 320f, 180f, Color.Yellow);
			Draw.HollowRect(base.X - 160f - 320f, camera.Y, 960f, 180f, Color.Yellow);
		}

		// Token: 0x0400112D RID: 4397
		public SoundSource sfx;
	}
}

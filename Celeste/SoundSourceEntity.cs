using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B6 RID: 438
	public class SoundSourceEntity : Entity
	{
		// Token: 0x06000F3F RID: 3903 RVA: 0x0003DCBC File Offset: 0x0003BEBC
		public SoundSourceEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Tag = Tags.TransitionUpdate;
			base.Add(this.sfx = new SoundSource());
			this.eventName = SFX.EventnameByHandle(data.Attr("sound", ""));
			this.Visible = true;
			base.Depth = -8500;
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0003DD2C File Offset: 0x0003BF2C
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			this.sfx.Play(this.eventName, null, 0f);
		}

		// Token: 0x04000AAA RID: 2730
		private string eventName;

		// Token: 0x04000AAB RID: 2731
		private SoundSource sfx;
	}
}

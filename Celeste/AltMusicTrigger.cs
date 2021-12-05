using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020002DC RID: 732
	public class AltMusicTrigger : Trigger
	{
		// Token: 0x060016A8 RID: 5800 RVA: 0x00086672 File Offset: 0x00084872
		public AltMusicTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Track = data.Attr("track", "");
			this.ResetOnLeave = data.Bool("resetOnLeave", true);
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x000866A4 File Offset: 0x000848A4
		public override void OnEnter(Player player)
		{
			Audio.SetAltMusic(SFX.EventnameByHandle(this.Track));
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x000866B6 File Offset: 0x000848B6
		public override void OnLeave(Player player)
		{
			if (this.ResetOnLeave)
			{
				Audio.SetAltMusic(null);
			}
		}

		// Token: 0x0400132D RID: 4909
		public string Track;

		// Token: 0x0400132E RID: 4910
		public bool ResetOnLeave;
	}
}

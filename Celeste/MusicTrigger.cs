using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020002DD RID: 733
	public class MusicTrigger : Trigger
	{
		// Token: 0x060016AB RID: 5803 RVA: 0x000866C8 File Offset: 0x000848C8
		public MusicTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			this.Track = data.Attr("track", "");
			this.ResetOnLeave = data.Bool("resetOnLeave", true);
			this.Progress = data.Int("progress", 0);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x00086718 File Offset: 0x00084918
		public override void OnEnter(Player player)
		{
			if (this.ResetOnLeave)
			{
				this.oldTrack = Audio.CurrentMusic;
			}
			Session session = base.SceneAs<Level>().Session;
			session.Audio.Music.Event = SFX.EventnameByHandle(this.Track);
			if (this.Progress != 0)
			{
				session.Audio.Music.Progress = this.Progress;
			}
			session.Audio.Apply(false);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00086789 File Offset: 0x00084989
		public override void OnLeave(Player player)
		{
			if (this.ResetOnLeave)
			{
				Session session = base.SceneAs<Level>().Session;
				session.Audio.Music.Event = this.oldTrack;
				session.Audio.Apply(false);
			}
		}

		// Token: 0x0400132F RID: 4911
		public string Track;

		// Token: 0x04001330 RID: 4912
		public bool SetInSession;

		// Token: 0x04001331 RID: 4913
		public bool ResetOnLeave;

		// Token: 0x04001332 RID: 4914
		public int Progress;

		// Token: 0x04001333 RID: 4915
		private string oldTrack;
	}
}

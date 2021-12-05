using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C5 RID: 453
	public class OshiroLobbyBell : Entity
	{
		// Token: 0x06000F7B RID: 3963 RVA: 0x0003F894 File Offset: 0x0003DA94
		public OshiroLobbyBell(Vector2 position) : base(position)
		{
			base.Add(this.talker = new TalkComponent(new Rectangle(-8, -8, 16, 16), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			this.talker.Enabled = false;
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0003F8F1 File Offset: 0x0003DAF1
		private void OnTalk(Player player)
		{
			Audio.Play("event:/game/03_resort/deskbell_again", this.Position);
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x0003F904 File Offset: 0x0003DB04
		public override void Update()
		{
			if (!this.talker.Enabled && base.Scene.Entities.FindFirst<NPC03_Oshiro_Lobby>() == null)
			{
				this.talker.Enabled = true;
			}
			base.Update();
		}

		// Token: 0x04000ADD RID: 2781
		private TalkComponent talker;
	}
}

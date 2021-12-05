using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200026D RID: 621
	public class NPC03_Oshiro_Lobby : NPC
	{
		// Token: 0x06001357 RID: 4951 RVA: 0x0006935C File Offset: 0x0006755C
		public NPC03_Oshiro_Lobby(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = new OshiroSprite(-1));
			this.Sprite.Visible = false;
			MTexture texture = GFX.Gui["hover/resort"];
			if (GFX.Gui.Has("hover/resort_" + Settings.Instance.Language))
			{
				texture = GFX.Gui["hover/resort_" + Settings.Instance.Language];
			}
			base.Add(this.Talker = new TalkComponent(new Rectangle(-30, -16, 42, 32), new Vector2(-12f, -24f), new Action<Player>(this.OnTalk), new TalkComponent.HoverDisplay
			{
				Texture = texture,
				InputPosition = new Vector2(0f, -75f),
				SfxIn = "event:/ui/game/hotspot_note_in",
				SfxOut = "event:/ui/game/hotspot_note_out"
			}));
			this.Talker.PlayerMustBeFacing = false;
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
			base.Depth = 9001;
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x00069480 File Offset: 0x00067680
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (base.Session.GetFlag("oshiro_resort_talked_1"))
			{
				base.Session.Audio.Music.Event = "event:/music/lvl3/explore";
				base.Session.Audio.Music.Progress = 1;
				base.Session.Audio.Apply(false);
				base.RemoveSelf();
			}
			else
			{
				base.Session.Audio.Music.Event = null;
				base.Session.Audio.Apply(false);
			}
			scene.Add(new OshiroLobbyBell(new Vector2(base.X - 14f, base.Y)));
			this.startX = this.Position.X;
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x00069549 File Offset: 0x00067749
		private void OnTalk(Player player)
		{
			base.Scene.Add(new CS03_OshiroLobby(player, this));
			this.Talker.Enabled = false;
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x00069569 File Offset: 0x00067769
		public override void Update()
		{
			base.Update();
			if (base.X >= this.startX + 12f)
			{
				base.Depth = 1000;
			}
		}

		// Token: 0x04000F40 RID: 3904
		public static ParticleType P_AppearSpark;

		// Token: 0x04000F41 RID: 3905
		private float startX;
	}
}

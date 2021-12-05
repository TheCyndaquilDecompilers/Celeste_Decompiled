using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000347 RID: 839
	public class Memorial : Entity
	{
		// Token: 0x06001A60 RID: 6752 RVA: 0x000A97EC File Offset: 0x000A79EC
		public Memorial(Vector2 position) : base(position)
		{
			base.Tag = Tags.PauseUpdate;
			base.Add(this.sprite = new Image(GFX.Game["scenery/memorial/memorial"]));
			this.sprite.Origin = new Vector2(this.sprite.Width / 2f, this.sprite.Height);
			base.Depth = 100;
			base.Collider = new Hitbox(60f, 80f, -30f, -60f);
			base.Add(this.loopingSfx = new SoundSource());
		}

		// Token: 0x06001A61 RID: 6753 RVA: 0x000A989A File Offset: 0x000A7A9A
		public Memorial(EntityData data, Vector2 offset) : this(data.Position + offset)
		{
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x000A98B0 File Offset: 0x000A7AB0
		public override void Added(Scene scene)
		{
			base.Added(scene);
			Level level = scene as Level;
			level.Add(this.text = new MemorialText(this, level.Session.Dreaming));
			if (level.Session.Dreaming)
			{
				base.Add(this.dreamyText = new Sprite(GFX.Game, "scenery/memorial/floatytext"));
				this.dreamyText.AddLoop("dreamy", "", 0.1f);
				this.dreamyText.Play("dreamy", false, false);
				this.dreamyText.Position = new Vector2(-this.dreamyText.Width / 2f, -33f);
			}
			if (level.Session.Area.ID == 1 && level.Session.Area.Mode == AreaMode.Normal)
			{
				Audio.SetMusicParam("end", 1f);
			}
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x000A99A0 File Offset: 0x000A7BA0
		public override void Update()
		{
			base.Update();
			Level level = base.Scene as Level;
			if (level.Paused)
			{
				this.loopingSfx.Pause();
				return;
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			bool dreaming = level.Session.Dreaming;
			this.wasShowing = this.text.Show;
			this.text.Show = (entity != null && base.CollideCheck(entity));
			if (this.text.Show && !this.wasShowing)
			{
				Audio.Play(dreaming ? "event:/ui/game/memorial_dream_text_in" : "event:/ui/game/memorial_text_in");
				if (dreaming)
				{
					this.loopingSfx.Play("event:/ui/game/memorial_dream_loop", null, 0f);
					this.loopingSfx.Param("end", 0f);
				}
			}
			else if (!this.text.Show && this.wasShowing)
			{
				Audio.Play(dreaming ? "event:/ui/game/memorial_dream_text_out" : "event:/ui/game/memorial_text_out");
				this.loopingSfx.Param("end", 1f);
				this.loopingSfx.Stop(true);
			}
			this.loopingSfx.Resume();
		}

		// Token: 0x040016F5 RID: 5877
		private Image sprite;

		// Token: 0x040016F6 RID: 5878
		private MemorialText text;

		// Token: 0x040016F7 RID: 5879
		private Sprite dreamyText;

		// Token: 0x040016F8 RID: 5880
		private bool wasShowing;

		// Token: 0x040016F9 RID: 5881
		private SoundSource loopingSfx;
	}
}

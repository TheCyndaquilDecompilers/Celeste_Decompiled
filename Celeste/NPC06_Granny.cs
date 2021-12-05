using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001A4 RID: 420
	public class NPC06_Granny : NPC
	{
		// Token: 0x06000EAB RID: 3755 RVA: 0x000372BC File Offset: 0x000354BC
		public NPC06_Granny(EntityData data, Vector2 position) : base(data.Position + position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Scale.X = -1f;
			this.Sprite.Play("idle", false, false);
			base.Add(new GrannyLaughSfx(this.Sprite));
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x00037334 File Offset: 0x00035534
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, null));
			this.Hahaha.Enabled = false;
			while (base.Session.GetFlag("granny_" + this.cutsceneIndex))
			{
				this.cutsceneIndex++;
			}
			base.Add(this.Talker = new TalkComponent(new Rectangle(-20, -8, 30, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			this.Talker.Enabled = (this.cutsceneIndex > 0 && this.cutsceneIndex < 3);
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x00037420 File Offset: 0x00035620
		public override void Update()
		{
			if (this.cutsceneIndex == 0)
			{
				Player entity = this.Level.Tracker.GetEntity<Player>();
				if (entity != null && entity.X > base.X - 60f)
				{
					this.OnTalk(entity);
				}
			}
			this.Hahaha.Enabled = (this.Sprite.CurrentAnimationID == "laugh");
			base.Update();
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0003748C File Offset: 0x0003568C
		private void OnTalk(Player player)
		{
			base.Scene.Add(new CS06_Granny(this, player, this.cutsceneIndex));
			this.cutsceneIndex++;
			this.Talker.Enabled = (this.cutsceneIndex > 0 && this.cutsceneIndex < 3);
		}

		// Token: 0x040009E4 RID: 2532
		public Hahaha Hahaha;

		// Token: 0x040009E5 RID: 2533
		private int cutsceneIndex;
	}
}

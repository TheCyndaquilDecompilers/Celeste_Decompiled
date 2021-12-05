using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000267 RID: 615
	public class NPC04_Granny : NPC
	{
		// Token: 0x0600132E RID: 4910 RVA: 0x0006826C File Offset: 0x0006646C
		public NPC04_Granny(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Scale.X = -1f;
			this.Sprite.Play("idle", false, false);
			base.Add(new GrannyLaughSfx(this.Sprite));
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x000682D8 File Offset: 0x000664D8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, null));
			this.Hahaha.Enabled = false;
			if (base.Session.GetFlag("granny_1") && !base.Session.GetFlag("granny_2"))
			{
				this.Sprite.Play("laugh", false, false);
			}
			if (!base.Session.GetFlag("granny_3"))
			{
				base.Add(this.Talker = new TalkComponent(new Rectangle(-20, -16, 40, 16), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
				if (!base.Session.GetFlag("granny_1"))
				{
					this.Talker.Enabled = false;
				}
			}
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x000683D8 File Offset: 0x000665D8
		public override void Update()
		{
			Player entity = this.Level.Tracker.GetEntity<Player>();
			if (entity != null && !base.Session.GetFlag("granny_1") && !this.cutscene && entity.X > base.X - 40f)
			{
				this.cutscene = true;
				base.Scene.Add(new CS04_Granny(this, entity));
				if (this.Talker != null)
				{
					this.Talker.Enabled = true;
				}
			}
			this.Hahaha.Enabled = (this.Sprite.CurrentAnimationID == "laugh");
			base.Update();
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x0006847C File Offset: 0x0006667C
		private void OnTalk(Player player)
		{
			this.Level.StartCutscene(new Action<Level>(this.TalkEnd), true, false, true);
			base.Add(this.talkRoutine = new Coroutine(this.TalkRoutine(player), true));
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x000684BF File Offset: 0x000666BF
		private IEnumerator TalkRoutine(Player player)
		{
			this.Sprite.Play("idle", false, false);
			player.ForceCameraUpdate = true;
			yield return base.PlayerApproachLeftSide(player, true, new float?((float)20));
			yield return this.Level.ZoomTo(new Vector2((player.X + base.X) / 2f - this.Level.Camera.X, 116f), 2f, 0.5f);
			if (!base.Session.GetFlag("granny_2"))
			{
				yield return Textbox.Say("CH4_GRANNY_2", new Func<IEnumerator>[0]);
			}
			else
			{
				yield return Textbox.Say("CH4_GRANNY_3", new Func<IEnumerator>[0]);
			}
			yield return this.Level.ZoomBack(0.5f);
			this.Level.EndCutscene();
			this.TalkEnd(this.Level);
			yield break;
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x000684D8 File Offset: 0x000666D8
		private void TalkEnd(Level level)
		{
			if (!base.Session.GetFlag("granny_2"))
			{
				base.Session.SetFlag("granny_2", true);
			}
			else if (!base.Session.GetFlag("granny_3"))
			{
				base.Session.SetFlag("granny_3", true);
				base.Remove(this.Talker);
			}
			if (this.talkRoutine != null)
			{
				this.talkRoutine.RemoveSelf();
				this.talkRoutine = null;
			}
			Player entity = this.Level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StateMachine.Locked = false;
				entity.StateMachine.State = 0;
				entity.ForceCameraUpdate = false;
			}
		}

		// Token: 0x04000F1C RID: 3868
		public Hahaha Hahaha;

		// Token: 0x04000F1D RID: 3869
		private bool cutscene;

		// Token: 0x04000F1E RID: 3870
		private Coroutine talkRoutine;

		// Token: 0x04000F1F RID: 3871
		private const string talkedFlagA = "granny_2";

		// Token: 0x04000F20 RID: 3872
		private const string talkedFlagB = "granny_3";
	}
}

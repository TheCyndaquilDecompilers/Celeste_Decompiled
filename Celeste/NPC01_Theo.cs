using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000272 RID: 626
	public class NPC01_Theo : NPC
	{
		// Token: 0x06001370 RID: 4976 RVA: 0x00069C64 File Offset: 0x00067E64
		public NPC01_Theo(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("theo"));
			this.Sprite.Play("idle", false, false);
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x00069CA8 File Offset: 0x00067EA8
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.currentConversation = base.Session.GetCounter("theo");
			if (!base.Session.GetFlag("theoDoneTalking"))
			{
				base.Add(this.Talker = new TalkComponent(new Rectangle(-8, -8, 88, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			}
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x00069D20 File Offset: 0x00067F20
		private void OnTalk(Player player)
		{
			this.Level.StartCutscene(new Action<Level>(this.OnTalkEnd), true, false, true);
			base.Add(this.talkRoutine = new Coroutine(this.Talk(player), true));
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x00069D63 File Offset: 0x00067F63
		private IEnumerator Talk(Player player)
		{
			if (this.currentConversation == 0)
			{
				yield return base.PlayerApproachRightSide(player, true, null);
				yield return Textbox.Say("CH1_THEO_A", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(base.PlayerApproach48px)
				});
			}
			else if (this.currentConversation == 1)
			{
				yield return base.PlayerApproachRightSide(player, true, null);
				yield return 0.2f;
				yield return base.PlayerApproach(player, true, new float?((float)48), null);
				yield return Textbox.Say("CH1_THEO_B", new Func<IEnumerator>[0]);
			}
			else if (this.currentConversation == 2)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH1_THEO_C", new Func<IEnumerator>[0]);
			}
			else if (this.currentConversation == 3)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH1_THEO_D", new Func<IEnumerator>[0]);
			}
			else if (this.currentConversation == 4)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH1_THEO_E", new Func<IEnumerator>[0]);
			}
			else if (this.currentConversation == 5)
			{
				yield return base.PlayerApproachRightSide(player, true, new float?((float)48));
				yield return Textbox.Say("CH1_THEO_F", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.Yolo)
				});
				this.Sprite.Play("yoloEnd", false, false);
				base.Remove(this.Talker);
				yield return this.Level.ZoomBack(0.5f);
			}
			this.Level.EndCutscene();
			this.OnTalkEnd(this.Level);
			yield break;
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x00069D7C File Offset: 0x00067F7C
		private void OnTalkEnd(Level level)
		{
			if (this.currentConversation == 0)
			{
				SaveData.Instance.SetFlag("MetTheo");
			}
			else if (this.currentConversation == 1)
			{
				SaveData.Instance.SetFlag("TheoKnowsName");
			}
			else if (this.currentConversation == 5)
			{
				base.Session.SetFlag("theoDoneTalking", true);
				base.Remove(this.Talker);
			}
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				entity.StateMachine.Locked = false;
				entity.StateMachine.State = 0;
			}
			base.Session.IncrementCounter("theo");
			this.currentConversation++;
			this.talkRoutine.Cancel();
			this.talkRoutine.RemoveSelf();
			this.Sprite.Play("idle", false, false);
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x00069E54 File Offset: 0x00068054
		private IEnumerator Yolo()
		{
			yield return this.Level.ZoomTo(new Vector2(128f, 128f), 2f, 0.5f);
			yield return 0.2f;
			Audio.Play("event:/char/theo/yolo_fist", this.Position);
			this.Sprite.Play("yolo", false, false);
			yield return 0.1f;
			this.Level.DirectionalShake(-Vector2.UnitY, 0.3f);
			this.Level.ParticlesFG.Emit(NPC01_Theo.P_YOLO, 6, this.Position + new Vector2(-3f, -24f), Vector2.One * 4f);
			yield return 0.5f;
			yield break;
		}

		// Token: 0x04000F4D RID: 3917
		public static ParticleType P_YOLO;

		// Token: 0x04000F4E RID: 3918
		private const string DoneTalking = "theoDoneTalking";

		// Token: 0x04000F4F RID: 3919
		private int currentConversation;

		// Token: 0x04000F50 RID: 3920
		private Coroutine talkRoutine;
	}
}

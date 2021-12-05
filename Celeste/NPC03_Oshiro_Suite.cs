using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200026E RID: 622
	public class NPC03_Oshiro_Suite : NPC
	{
		// Token: 0x0600135B RID: 4955 RVA: 0x00069590 File Offset: 0x00067790
		public NPC03_Oshiro_Suite(Vector2 position) : base(position)
		{
			base.Add(this.Sprite = new OshiroSprite(1));
			base.Add(this.Light = new VertexLight(-Vector2.UnitY * 16f, Color.White, 1f, 32, 64));
			base.Add(this.Talker = new TalkComponent(new Rectangle(-16, -8, 32, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			this.Talker.Enabled = false;
			this.MoveAnim = "move";
			this.IdleAnim = "idle";
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x00069650 File Offset: 0x00067850
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (!base.Session.GetFlag("oshiro_resort_suite"))
			{
				base.Scene.Add(new CS03_OshiroMasterSuite(this));
				return;
			}
			this.Sprite.Play("idle_ground", false, false);
			this.Talker.Enabled = true;
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x000696A6 File Offset: 0x000678A6
		private void OnTalk(Player player)
		{
			this.finishedTalking = false;
			this.Level.StartCutscene(new Action<Level>(this.EndTalking), true, false, true);
			base.Add(new Coroutine(this.Talk(player), true));
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x000696DC File Offset: 0x000678DC
		private IEnumerator Talk(Player player)
		{
			int conversation = base.Session.GetCounter("oshiroSuiteSadConversation");
			yield return base.PlayerApproach(player, false, new float?((float)12), null);
			yield return Textbox.Say("CH3_OSHIRO_SUITE_SAD" + conversation, new Func<IEnumerator>[0]);
			yield return base.PlayerLeave(player, null);
			this.EndTalking(base.SceneAs<Level>());
			yield break;
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x000696F4 File Offset: 0x000678F4
		private void EndTalking(Level level)
		{
			Player player = base.Scene.Entities.FindFirst<Player>();
			if (player != null)
			{
				player.StateMachine.Locked = false;
				player.StateMachine.State = 0;
			}
			if (!this.finishedTalking)
			{
				int num = base.Session.GetCounter("oshiroSuiteSadConversation");
				num++;
				num %= 7;
				if (num == 0)
				{
					num++;
				}
				base.Session.SetCounter("oshiroSuiteSadConversation", num);
				this.finishedTalking = true;
			}
		}

		// Token: 0x04000F42 RID: 3906
		private const string ConversationCounter = "oshiroSuiteSadConversation";

		// Token: 0x04000F43 RID: 3907
		private bool finishedTalking;
	}
}

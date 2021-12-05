using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000183 RID: 387
	public class NPC07X_Granny_Ending : NPC
	{
		// Token: 0x06000DA9 RID: 3497 RVA: 0x0003034C File Offset: 0x0002E54C
		public NPC07X_Granny_Ending(EntityData data, Vector2 offset, bool ch9EasterEgg = false) : base(data.Position + offset)
		{
			base.Add(this.Sprite = GFX.SpriteBank.Create("granny"));
			this.Sprite.Play("idle", false, false);
			this.Sprite.Scale.X = -1f;
			base.Add(this.LaughSfx = new GrannyLaughSfx(this.Sprite));
			base.Add(this.talker = new TalkComponent(new Rectangle(-20, -8, 40, 8), new Vector2(0f, -24f), new Action<Player>(this.OnTalk), null));
			this.MoveAnim = "walk";
			this.Maxspeed = 40f;
			this.ch9EasterEgg = ch9EasterEgg;
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x00030424 File Offset: 0x0002E624
		public override void Added(Scene scene)
		{
			base.Added(scene);
			scene.Add(this.Hahaha = new Hahaha(this.Position + new Vector2(8f, -4f), "", false, null));
			this.Hahaha.Enabled = false;
		}

		// Token: 0x06000DAB RID: 3499 RVA: 0x00030481 File Offset: 0x0002E681
		public override void Update()
		{
			this.Hahaha.Enabled = (this.Sprite.CurrentAnimationID == "laugh");
			base.Update();
		}

		// Token: 0x06000DAC RID: 3500 RVA: 0x000304AC File Offset: 0x0002E6AC
		private void OnTalk(Player player)
		{
			this.player = player;
			(base.Scene as Level).StartCutscene(new Action<Level>(this.EndTalking), true, false, true);
			base.Add(this.talkRoutine = new Coroutine(this.TalkRoutine(player), true));
		}

		// Token: 0x06000DAD RID: 3501 RVA: 0x000304FB File Offset: 0x0002E6FB
		private IEnumerator TalkRoutine(Player player)
		{
			player.StateMachine.State = 11;
			player.ForceCameraUpdate = true;
			while (!player.OnGround(1))
			{
				yield return null;
			}
			yield return player.DummyWalkToExact((int)base.X - 16, false, 1f, false);
			player.Facing = Facings.Right;
			if (this.ch9EasterEgg)
			{
				yield return 0.5f;
				yield return this.Level.ZoomTo(this.Position - this.Level.Camera.Position + new Vector2(0f, -32f), 2f, 0.5f);
				Dialog.Language.Dialog["CH10_GRANNY_EASTEREGG"] = "{portrait GRANNY right mock} I see you have discovered Debug Mode.";
				yield return Textbox.Say("CH10_GRANNY_EASTEREGG", new Func<IEnumerator>[0]);
				this.talker.Enabled = false;
			}
			else if (this.conversation == 0)
			{
				yield return 0.5f;
				yield return this.Level.ZoomTo(this.Position - this.Level.Camera.Position + new Vector2(0f, -32f), 2f, 0.5f);
				yield return Textbox.Say("CH7_CSIDE_OLDLADY", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.StartLaughing),
					new Func<IEnumerator>(this.StopLaughing)
				});
			}
			else if (this.conversation == 1)
			{
				yield return 0.5f;
				yield return this.Level.ZoomTo(this.Position - this.Level.Camera.Position + new Vector2(0f, -32f), 2f, 0.5f);
				yield return Textbox.Say("CH7_CSIDE_OLDLADY_B", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.StartLaughing),
					new Func<IEnumerator>(this.StopLaughing)
				});
				this.talker.Enabled = false;
			}
			yield return this.Level.ZoomBack(0.5f);
			this.Level.EndCutscene();
			this.EndTalking(this.Level);
			yield break;
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x00030511 File Offset: 0x0002E711
		private IEnumerator StartLaughing()
		{
			this.Sprite.Play("laugh", false, false);
			yield return null;
			yield break;
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x00030520 File Offset: 0x0002E720
		private IEnumerator StopLaughing()
		{
			this.Sprite.Play("idle", false, false);
			yield return null;
			yield break;
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x00030530 File Offset: 0x0002E730
		private void EndTalking(Level level)
		{
			if (this.player != null)
			{
				this.player.StateMachine.State = 0;
				this.player.ForceCameraUpdate = false;
			}
			this.conversation++;
			if (this.talkRoutine != null)
			{
				this.talkRoutine.RemoveSelf();
				this.talkRoutine = null;
			}
			this.Sprite.Play("idle", false, false);
		}

		// Token: 0x040008E5 RID: 2277
		public Hahaha Hahaha;

		// Token: 0x040008E6 RID: 2278
		public GrannyLaughSfx LaughSfx;

		// Token: 0x040008E7 RID: 2279
		private Player player;

		// Token: 0x040008E8 RID: 2280
		private TalkComponent talker;

		// Token: 0x040008E9 RID: 2281
		private Coroutine talkRoutine;

		// Token: 0x040008EA RID: 2282
		private int conversation;

		// Token: 0x040008EB RID: 2283
		private bool ch9EasterEgg;
	}
}

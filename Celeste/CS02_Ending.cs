using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x0200027E RID: 638
	public class CS02_Ending : CutsceneEntity
	{
		// Token: 0x060013BD RID: 5053 RVA: 0x0006B308 File Offset: 0x00069508
		public CS02_Ending(Player player) : base(false, true)
		{
			this.player = player;
			base.Add(this.phoneSfx = new SoundSource());
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x0006B338 File Offset: 0x00069538
		public override void OnBegin(Level level)
		{
			level.RegisterAreaComplete();
			this.payphone = base.Scene.Tracker.GetEntity<Payphone>();
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x0006B369 File Offset: 0x00069569
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.Dashes = 1;
			while (this.player.Light.Alpha > 0f)
			{
				this.player.Light.Alpha -= Engine.DeltaTime * 1.25f;
				yield return null;
			}
			yield return 1f;
			yield return this.player.DummyWalkTo(this.payphone.X - 4f, false, 1f, false);
			yield return 0.2f;
			this.player.Facing = Facings.Right;
			yield return 0.5f;
			this.player.Visible = false;
			Audio.Play("event:/game/02_old_site/sequence_phone_pickup", this.player.Position);
			yield return this.payphone.Sprite.PlayRoutine("pickUp", false);
			yield return 0.25f;
			this.phoneSfx.Position = this.player.Position;
			this.phoneSfx.Play("event:/game/02_old_site/sequence_phone_ringtone_loop", null, 0f);
			yield return 6f;
			this.phoneSfx.Stop(true);
			this.payphone.Sprite.Play("talkPhone", false, false);
			yield return Textbox.Say("CH2_END_PHONECALL", new Func<IEnumerator>[0]);
			yield return 0.3f;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x060013C0 RID: 5056 RVA: 0x0006B2FB File Offset: 0x000694FB
		public override void OnEnd(Level level)
		{
			level.CompleteArea(true, false, false);
		}

		// Token: 0x04000F85 RID: 3973
		private Player player;

		// Token: 0x04000F86 RID: 3974
		private Payphone payphone;

		// Token: 0x04000F87 RID: 3975
		private SoundSource phoneSfx;
	}
}

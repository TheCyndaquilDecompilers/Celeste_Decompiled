using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200028A RID: 650
	public class CS04_Granny : CutsceneEntity
	{
		// Token: 0x06001415 RID: 5141 RVA: 0x0006C673 File Offset: 0x0006A873
		public CS04_Granny(NPC04_Granny granny, Player player) : base(true, false)
		{
			this.granny = granny;
			this.player = player;
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0006C68B File Offset: 0x0006A88B
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0006C6A0 File Offset: 0x0006A8A0
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.ForceCameraUpdate = true;
			yield return this.player.DummyWalkTo(this.granny.X - 30f, false, 1f, false);
			this.player.Facing = Facings.Right;
			yield return Textbox.Say("CH4_GRANNY_1", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.Laughs),
				new Func<IEnumerator>(this.StopLaughing),
				new Func<IEnumerator>(this.WaitABeat),
				new Func<IEnumerator>(this.ZoomIn),
				new Func<IEnumerator>(this.MaddyTurnsAround),
				new Func<IEnumerator>(this.MaddyApproaches),
				new Func<IEnumerator>(this.MaddyWalksPastGranny)
			});
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x0006C6B6 File Offset: 0x0006A8B6
		private IEnumerator Laughs()
		{
			this.granny.Sprite.Play("laugh", false, false);
			yield return 1f;
			yield break;
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x0006C6C5 File Offset: 0x0006A8C5
		private IEnumerator StopLaughing()
		{
			this.granny.Sprite.Play("idle", false, false);
			yield return 0.25f;
			yield break;
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x0006C6D4 File Offset: 0x0006A8D4
		private IEnumerator WaitABeat()
		{
			yield return 1.2f;
			yield break;
		}

		// Token: 0x0600141B RID: 5147 RVA: 0x0006C6DC File Offset: 0x0006A8DC
		private IEnumerator ZoomIn()
		{
			yield return this.Level.ZoomTo(new Vector2(123f, 116f), 2f, 0.5f);
			yield break;
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x0006C6EB File Offset: 0x0006A8EB
		private IEnumerator MaddyTurnsAround()
		{
			yield return 0.2f;
			this.player.Facing = Facings.Left;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x0006C6FA File Offset: 0x0006A8FA
		private IEnumerator MaddyApproaches()
		{
			yield return this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
			yield break;
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0006C709 File Offset: 0x0006A909
		private IEnumerator MaddyWalksPastGranny()
		{
			yield return this.player.DummyWalkToExact((int)this.granny.X + 30, false, 1f, false);
			yield break;
		}

		// Token: 0x0600141F RID: 5151 RVA: 0x0006C718 File Offset: 0x0006A918
		public override void OnEnd(Level level)
		{
			this.player.X = this.granny.X + 30f;
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.player.ForceCameraUpdate = false;
			if (this.WasSkipped)
			{
				level.Camera.Position = this.player.CameraTarget;
			}
			this.granny.Sprite.Play("laugh", false, false);
			level.Session.SetFlag("granny_1", true);
		}

		// Token: 0x04000FC5 RID: 4037
		public const string Flag = "granny_1";

		// Token: 0x04000FC6 RID: 4038
		private NPC04_Granny granny;

		// Token: 0x04000FC7 RID: 4039
		private Player player;
	}
}

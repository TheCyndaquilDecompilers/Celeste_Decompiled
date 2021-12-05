using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000195 RID: 405
	public class CS06_Granny : CutsceneEntity
	{
		// Token: 0x06000E11 RID: 3601 RVA: 0x00032782 File Offset: 0x00030982
		public CS06_Granny(NPC06_Granny granny, Player player, int index) : base(true, false)
		{
			this.granny = granny;
			this.player = player;
			this.index = index;
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x000327A1 File Offset: 0x000309A1
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x000327B6 File Offset: 0x000309B6
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.ForceCameraUpdate = true;
			if (this.index == 0)
			{
				yield return this.player.DummyWalkTo(this.granny.X - 40f, false, 1f, false);
				this.startX = this.player.X;
				this.player.Facing = Facings.Right;
				this.firstLaugh = true;
				yield return Textbox.Say("ch6_oldlady", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.ZoomIn),
					new Func<IEnumerator>(this.Laughs),
					new Func<IEnumerator>(this.StopLaughing),
					new Func<IEnumerator>(this.MaddyWalksRight),
					new Func<IEnumerator>(this.MaddyWalksLeft),
					new Func<IEnumerator>(this.WaitABit),
					new Func<IEnumerator>(this.MaddyTurnsRight)
				});
			}
			else if (this.index == 1)
			{
				yield return this.ZoomIn();
				yield return this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
				this.player.Facing = Facings.Right;
				yield return Textbox.Say("ch6_oldlady_b", new Func<IEnumerator>[0]);
			}
			else if (this.index == 2)
			{
				yield return this.ZoomIn();
				yield return this.player.DummyWalkTo(this.granny.X - 20f, false, 1f, false);
				this.player.Facing = Facings.Right;
				yield return Textbox.Say("ch6_oldlady_c", new Func<IEnumerator>[0]);
			}
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x000327CC File Offset: 0x000309CC
		private IEnumerator ZoomIn()
		{
			Vector2 screenSpaceFocusPoint = Vector2.Lerp(this.granny.Position, this.player.Position, 0.5f) - this.Level.Camera.Position + new Vector2(0f, -20f);
			yield return this.Level.ZoomTo(screenSpaceFocusPoint, 2f, 0.5f);
			yield break;
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x000327DB File Offset: 0x000309DB
		private IEnumerator Laughs()
		{
			if (this.firstLaugh)
			{
				this.firstLaugh = false;
				yield return 0.5f;
			}
			this.granny.Sprite.Play("laugh", false, false);
			yield return 1f;
			yield break;
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x000327EA File Offset: 0x000309EA
		private IEnumerator StopLaughing()
		{
			this.granny.Sprite.Play("idle", false, false);
			yield return 0.25f;
			yield break;
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x000327F9 File Offset: 0x000309F9
		private IEnumerator MaddyWalksLeft()
		{
			yield return 0.1f;
			this.player.Facing = Facings.Left;
			yield return this.player.DummyWalkToExact((int)this.player.X - 8, false, 1f, false);
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x00032808 File Offset: 0x00030A08
		private IEnumerator MaddyWalksRight()
		{
			yield return 0.1f;
			this.player.Facing = Facings.Right;
			yield return this.player.DummyWalkToExact((int)this.player.X + 8, false, 1f, false);
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x00032817 File Offset: 0x00030A17
		private IEnumerator WaitABit()
		{
			yield return 0.8f;
			yield break;
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x0003281F File Offset: 0x00030A1F
		private IEnumerator MaddyTurnsRight()
		{
			yield return 0.1f;
			this.player.Facing = Facings.Right;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x00032830 File Offset: 0x00030A30
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.player.ForceCameraUpdate = false;
			this.granny.Sprite.Play("idle", false, false);
			level.Session.SetFlag("granny_" + this.index, true);
		}

		// Token: 0x04000953 RID: 2387
		public const string FlagPrefix = "granny_";

		// Token: 0x04000954 RID: 2388
		private NPC06_Granny granny;

		// Token: 0x04000955 RID: 2389
		private Player player;

		// Token: 0x04000956 RID: 2390
		private float startX;

		// Token: 0x04000957 RID: 2391
		private int index;

		// Token: 0x04000958 RID: 2392
		private bool firstLaugh;
	}
}

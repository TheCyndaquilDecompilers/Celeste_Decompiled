using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000192 RID: 402
	public class CS06_Ending : CutsceneEntity
	{
		// Token: 0x06000DF0 RID: 3568 RVA: 0x00031BE4 File Offset: 0x0002FDE4
		public CS06_Ending(Player player, NPC granny) : base(false, true)
		{
			this.player = player;
			this.granny = granny;
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x00031BFC File Offset: 0x0002FDFC
		public override void OnBegin(Level level)
		{
			level.RegisterAreaComplete();
			this.theo = base.Scene.Entities.FindFirst<NPC06_Theo_Ending>();
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x00031C2D File Offset: 0x0002FE2D
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return 1f;
			this.player.Dashes = 1;
			level.Session.Inventory.Dashes = 1;
			level.Add(this.badeline = new BadelineDummy(this.player.Center));
			this.badeline.Appear(level, true);
			this.badeline.FloatSpeed = 80f;
			this.badeline.Sprite.Scale.X = -1f;
			Audio.Play("event:/char/badeline/maddy_split", this.player.Center);
			yield return this.badeline.FloatTo(this.player.Position + new Vector2(24f, -20f), new int?(-1), false, false, false);
			yield return level.ZoomTo(new Vector2(160f, 120f), 2f, 1f);
			yield return Textbox.Say("ch6_ending", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.GrannyEnter),
				new Func<IEnumerator>(this.TheoEnter),
				new Func<IEnumerator>(this.MaddyTurnsRight),
				new Func<IEnumerator>(this.BadelineTurnsRight),
				new Func<IEnumerator>(this.BadelineTurnsLeft),
				new Func<IEnumerator>(this.WaitAbit),
				new Func<IEnumerator>(this.TurnToLeft),
				new Func<IEnumerator>(this.TheoRaiseFist),
				new Func<IEnumerator>(this.TheoStopTired)
			});
			Audio.Play("event:/char/madeline/backpack_drop", this.player.Position);
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("bagdown", false, false);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x00031C43 File Offset: 0x0002FE43
		private IEnumerator GrannyEnter()
		{
			yield return 0.25f;
			this.badeline.Sprite.Scale.X = 1f;
			yield return 0.1f;
			this.granny.Visible = true;
			base.Add(new Coroutine(this.badeline.FloatTo(new Vector2(this.badeline.X - 10f, this.badeline.Y), new int?(1), false, false, false), true));
			yield return this.granny.MoveTo(this.player.Position + new Vector2(40f, 0f), false, null, false);
			yield break;
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x00031C52 File Offset: 0x0002FE52
		private IEnumerator TheoEnter()
		{
			this.player.Facing = Facings.Left;
			this.badeline.Sprite.Scale.X = -1f;
			yield return 0.25f;
			yield return CutsceneEntity.CameraTo(new Vector2(this.Level.Camera.X - 40f, this.Level.Camera.Y), 1f, null, 0f);
			this.theo.Visible = true;
			base.Add(new Coroutine(CutsceneEntity.CameraTo(new Vector2(this.Level.Camera.X + 40f, this.Level.Camera.Y), 2f, null, 1f), true));
			base.Add(new Coroutine(this.badeline.FloatTo(new Vector2(this.badeline.X + 6f, this.badeline.Y + 4f), new int?(-1), false, false, false), true));
			yield return this.theo.MoveTo(this.player.Position + new Vector2(-32f, 0f), false, null, false);
			this.theo.Sprite.Play("tired", false, false);
			yield break;
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x00031C61 File Offset: 0x0002FE61
		private IEnumerator MaddyTurnsRight()
		{
			yield return 0.1f;
			this.player.Facing = Facings.Right;
			yield return 0.1f;
			yield return this.badeline.FloatTo(this.badeline.Position + new Vector2(-2f, 10f), new int?(-1), false, false, false);
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x00031C70 File Offset: 0x0002FE70
		private IEnumerator BadelineTurnsRight()
		{
			yield return 0.1f;
			this.badeline.Sprite.Scale.X = 1f;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x00031C7F File Offset: 0x0002FE7F
		private IEnumerator BadelineTurnsLeft()
		{
			yield return 0.1f;
			this.badeline.Sprite.Scale.X = -1f;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x00031C8E File Offset: 0x0002FE8E
		private IEnumerator WaitAbit()
		{
			yield return 0.4f;
			yield break;
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00031C96 File Offset: 0x0002FE96
		private IEnumerator TurnToLeft()
		{
			yield return 0.1f;
			this.player.Facing = Facings.Left;
			yield return 0.05f;
			this.badeline.Sprite.Scale.X = -1f;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00031CA5 File Offset: 0x0002FEA5
		private IEnumerator TheoRaiseFist()
		{
			this.theo.Sprite.Play("yolo", false, false);
			base.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
			{
				this.theo.Sprite.Play("yoloEnd", false, false);
			}, 0.8f, true));
			yield return null;
			yield break;
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00031CB4 File Offset: 0x0002FEB4
		private IEnumerator TheoStopTired()
		{
			this.theo.Sprite.Play("idle", false, false);
			yield return null;
			yield break;
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00031CC3 File Offset: 0x0002FEC3
		public override void OnEnd(Level level)
		{
			level.CompleteArea(true, false, false);
			SpotlightWipe.FocusPoint += new Vector2(0f, -20f);
		}

		// Token: 0x04000939 RID: 2361
		private Player player;

		// Token: 0x0400093A RID: 2362
		private BadelineDummy badeline;

		// Token: 0x0400093B RID: 2363
		private NPC granny;

		// Token: 0x0400093C RID: 2364
		private NPC theo;
	}
}

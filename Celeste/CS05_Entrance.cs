using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000222 RID: 546
	public class CS05_Entrance : CutsceneEntity
	{
		// Token: 0x0600118C RID: 4492 RVA: 0x00056EDE File Offset: 0x000550DE
		public CS05_Entrance(NPC theo) : base(true, false)
		{
			this.theo = theo;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00056EEF File Offset: 0x000550EF
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00056F04 File Offset: 0x00055104
		private IEnumerator Cutscene(Level level)
		{
			this.player = level.Tracker.GetEntity<Player>();
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.X = this.theo.X - 32f;
			this.playerMoveTo = new Vector2(this.theo.X - 32f, this.player.Y);
			this.player.Facing = Facings.Left;
			SpotlightWipe.FocusPoint = this.theo.TopCenter - Vector2.UnitX * 16f - level.Camera.Position;
			yield return 2f;
			this.player.Facing = Facings.Right;
			yield return 0.3f;
			yield return this.theo.MoveTo(new Vector2(this.theo.X + 48f, this.theo.Y), false, null, false);
			yield return Textbox.Say("ch5_entrance", new Func<IEnumerator>[]
			{
				new Func<IEnumerator>(this.MaddyTurnsRight),
				new Func<IEnumerator>(this.TheoTurns),
				new Func<IEnumerator>(this.TheoLeaves)
			});
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x00056F1A File Offset: 0x0005511A
		private IEnumerator MaddyTurnsRight()
		{
			this.player.Facing = Facings.Right;
			yield break;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00056F29 File Offset: 0x00055129
		private IEnumerator TheoTurns()
		{
			Sprite sprite = this.theo.Sprite;
			sprite.Scale.X = sprite.Scale.X * -1f;
			yield break;
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x00056F38 File Offset: 0x00055138
		private IEnumerator TheoLeaves()
		{
			yield return this.theo.MoveTo(new Vector2((float)(this.Level.Bounds.Right + 32), this.theo.Y), false, null, false);
			yield break;
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00056F48 File Offset: 0x00055148
		public override void OnEnd(Level level)
		{
			if (this.player != null)
			{
				this.player.StateMachine.Locked = false;
				this.player.StateMachine.State = 0;
				this.player.ForceCameraUpdate = false;
				this.player.Position = this.playerMoveTo;
				this.player.Facing = Facings.Right;
			}
			base.Scene.Remove(this.theo);
			level.Session.SetFlag("entrance", true);
		}

		// Token: 0x04000D2A RID: 3370
		public const string Flag = "entrance";

		// Token: 0x04000D2B RID: 3371
		private NPC theo;

		// Token: 0x04000D2C RID: 3372
		private Player player;

		// Token: 0x04000D2D RID: 3373
		private Vector2 playerMoveTo;
	}
}

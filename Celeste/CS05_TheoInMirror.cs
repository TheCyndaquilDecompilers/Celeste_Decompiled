using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000221 RID: 545
	public class CS05_TheoInMirror : CutsceneEntity
	{
		// Token: 0x06001188 RID: 4488 RVA: 0x00056DFE File Offset: 0x00054FFE
		public CS05_TheoInMirror(NPC theo, Player player) : base(true, false)
		{
			this.theo = theo;
			this.player = player;
			this.playerFinalX = (int)theo.Position.X + 24;
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x00056E2B File Offset: 0x0005502B
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x00056E40 File Offset: 0x00055040
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return this.player.DummyWalkTo(this.theo.X - 16f, false, 1f, false);
			yield return 0.5f;
			this.theo.Sprite.Scale.X = -1f;
			yield return 0.25f;
			yield return Textbox.Say("ch5_theo_mirror", new Func<IEnumerator>[0]);
			base.Add(new Coroutine(this.theo.MoveTo(this.theo.Position + new Vector2(64f, 0f), false, null, false), true));
			yield return 0.4f;
			yield return this.player.DummyWalkToExact(this.playerFinalX, false, 1f, false);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00056E58 File Offset: 0x00055058
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.player.X = (float)this.playerFinalX;
			this.player.MoveV(200f, null, null);
			this.player.Speed = Vector2.Zero;
			base.Scene.Remove(this.theo);
			level.Session.SetFlag("theoInMirror", true);
		}

		// Token: 0x04000D26 RID: 3366
		public const string Flag = "theoInMirror";

		// Token: 0x04000D27 RID: 3367
		private NPC theo;

		// Token: 0x04000D28 RID: 3368
		private Player player;

		// Token: 0x04000D29 RID: 3369
		private int playerFinalX;
	}
}

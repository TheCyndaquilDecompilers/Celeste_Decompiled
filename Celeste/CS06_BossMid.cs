using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000171 RID: 369
	public class CS06_BossMid : CutsceneEntity
	{
		// Token: 0x06000D1B RID: 3355 RVA: 0x0002A892 File Offset: 0x00028A92
		public CS06_BossMid() : base(true, false)
		{
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0002C660 File Offset: 0x0002A860
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0002C675 File Offset: 0x0002A875
		private IEnumerator Cutscene(Level level)
		{
			while (this.player == null)
			{
				this.player = base.Scene.Tracker.GetEntity<Player>();
				yield return null;
			}
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			while (!this.player.OnGround(1))
			{
				yield return null;
			}
			yield return this.player.DummyWalkToExact((int)this.player.X + 20, false, 1f, false);
			yield return level.ZoomTo(new Vector2(80f, 110f), 2f, 0.5f);
			yield return Textbox.Say("ch6_boss_middle", new Func<IEnumerator>[0]);
			yield return 0.1f;
			yield return level.ZoomBack(0.4f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x0002C68C File Offset: 0x0002A88C
		public override void OnEnd(Level level)
		{
			if (this.WasSkipped && this.player != null)
			{
				while (!this.player.OnGround(1) && this.player.Y < (float)level.Bounds.Bottom)
				{
					Player player = this.player;
					float y = player.Y;
					player.Y = y + 1f;
				}
			}
			if (this.player != null)
			{
				this.player.StateMachine.Locked = false;
				this.player.StateMachine.State = 0;
			}
			FinalBoss finalBoss = level.Entities.FindFirst<FinalBoss>();
			if (finalBoss != null)
			{
				finalBoss.OnPlayer(null);
			}
			level.Session.SetFlag("boss_mid", true);
		}

		// Token: 0x04000858 RID: 2136
		public const string Flag = "boss_mid";

		// Token: 0x04000859 RID: 2137
		private Player player;
	}
}

using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000170 RID: 368
	public class CS05_Reflection1 : CutsceneEntity
	{
		// Token: 0x06000D14 RID: 3348 RVA: 0x0002C59F File Offset: 0x0002A79F
		public CS05_Reflection1(Player player) : base(true, false)
		{
			this.player = player;
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0002C5B0 File Offset: 0x0002A7B0
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0002C5C5 File Offset: 0x0002A7C5
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.ForceCameraUpdate = true;
			TempleMirror templeMirror = base.Scene.Entities.FindFirst<TempleMirror>();
			yield return this.player.DummyWalkTo(templeMirror.Center.X + 8f, false, 1f, false);
			yield return 0.2f;
			this.player.Facing = Facings.Left;
			yield return 0.3f;
			if (!this.player.Dead)
			{
				yield return Textbox.Say("ch5_reflection", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.MadelineFallsToKnees),
					new Func<IEnumerator>(this.MadelineStopsPanicking),
					new Func<IEnumerator>(this.MadelineGetsUp)
				});
			}
			else
			{
				yield return 100f;
			}
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0002C5DB File Offset: 0x0002A7DB
		private IEnumerator MadelineFallsToKnees()
		{
			yield return 0.2f;
			this.player.DummyAutoAnimate = false;
			this.player.Sprite.Play("tired", false, false);
			yield return 0.2f;
			yield return this.Level.ZoomTo(new Vector2(90f, 116f), 2f, 0.5f);
			yield return 0.2f;
			yield break;
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0002C5EA File Offset: 0x0002A7EA
		private IEnumerator MadelineStopsPanicking()
		{
			yield return 0.8f;
			this.player.Sprite.Play("tiredStill", false, false);
			yield return 0.4f;
			yield break;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0002C5F9 File Offset: 0x0002A7F9
		private IEnumerator MadelineGetsUp()
		{
			this.player.DummyAutoAnimate = true;
			this.player.Sprite.Play("idle", false, false);
			yield break;
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0002C608 File Offset: 0x0002A808
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.player.ForceCameraUpdate = false;
			this.player.FlipInReflection = false;
			level.Session.SetFlag("reflection", true);
		}

		// Token: 0x04000856 RID: 2134
		public const string Flag = "reflection";

		// Token: 0x04000857 RID: 2135
		private Player player;
	}
}

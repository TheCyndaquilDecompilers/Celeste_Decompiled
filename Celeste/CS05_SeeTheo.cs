using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000220 RID: 544
	public class CS05_SeeTheo : CutsceneEntity
	{
		// Token: 0x0600117F RID: 4479 RVA: 0x00056CF8 File Offset: 0x00054EF8
		public CS05_SeeTheo(Player player, int index) : base(true, false)
		{
			this.player = player;
			this.index = index;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00056D10 File Offset: 0x00054F10
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00056D25 File Offset: 0x00054F25
		private IEnumerator Cutscene(Level level)
		{
			while (this.player.Scene == null || !this.player.OnGround(1))
			{
				yield return null;
			}
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return 0.25f;
			this.theo = base.Scene.Tracker.GetEntity<TheoCrystal>();
			if (this.theo != null && Math.Sign(this.player.X - this.theo.X) != 0)
			{
				this.player.Facing = (Facings)Math.Sign(this.theo.X - this.player.X);
			}
			yield return 0.25f;
			if (this.index == 0)
			{
				yield return Textbox.Say("ch5_see_theo", new Func<IEnumerator>[]
				{
					new Func<IEnumerator>(this.ZoomIn),
					new Func<IEnumerator>(this.MadelineTurnsAround),
					new Func<IEnumerator>(this.WaitABit),
					new Func<IEnumerator>(this.MadelineTurnsBackAndBrighten)
				});
			}
			else if (this.index == 1)
			{
				yield return Textbox.Say("ch5_see_theo_b", new Func<IEnumerator>[0]);
			}
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00056D3B File Offset: 0x00054F3B
		private IEnumerator ZoomIn()
		{
			yield return this.Level.ZoomTo(Vector2.Lerp(this.player.Position, this.theo.Position, 0.5f) - this.Level.Camera.Position + new Vector2(0f, -20f), 2f, 0.5f);
			yield break;
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00056D4A File Offset: 0x00054F4A
		private IEnumerator MadelineTurnsAround()
		{
			yield return 0.3f;
			this.player.Facing = Facings.Left;
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00056D59 File Offset: 0x00054F59
		private IEnumerator WaitABit()
		{
			yield return 1f;
			yield break;
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x00056D61 File Offset: 0x00054F61
		private IEnumerator MadelineTurnsBackAndBrighten()
		{
			yield return 0.1f;
			Coroutine coroutine = new Coroutine(this.Brighten(), true);
			base.Add(coroutine);
			yield return 0.2f;
			this.player.Facing = Facings.Right;
			yield return 0.1f;
			while (coroutine.Active)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00056D70 File Offset: 0x00054F70
		private IEnumerator Brighten()
		{
			yield return this.Level.ZoomBack(0.5f);
			yield return 0.3f;
			this.Level.Session.DarkRoomAlpha = 0.3f;
			float darkness = this.Level.Session.DarkRoomAlpha;
			while (this.Level.Lighting.Alpha != darkness)
			{
				this.Level.Lighting.Alpha = Calc.Approach(this.Level.Lighting.Alpha, darkness, Engine.DeltaTime * 0.5f);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00056D80 File Offset: 0x00054F80
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.player.ForceCameraUpdate = false;
			this.player.DummyAutoAnimate = true;
			level.Session.DarkRoomAlpha = 0.3f;
			level.Lighting.Alpha = level.Session.DarkRoomAlpha;
			level.Session.SetFlag("seeTheoInCrystal", true);
		}

		// Token: 0x04000D21 RID: 3361
		private const float NewDarknessAlpha = 0.3f;

		// Token: 0x04000D22 RID: 3362
		public const string Flag = "seeTheoInCrystal";

		// Token: 0x04000D23 RID: 3363
		private int index;

		// Token: 0x04000D24 RID: 3364
		private Player player;

		// Token: 0x04000D25 RID: 3365
		private TheoCrystal theo;
	}
}

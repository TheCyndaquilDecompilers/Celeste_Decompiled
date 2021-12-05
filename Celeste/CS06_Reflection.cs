using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000173 RID: 371
	public class CS06_Reflection : CutsceneEntity
	{
		// Token: 0x06000D31 RID: 3377 RVA: 0x0002CB96 File Offset: 0x0002AD96
		public CS06_Reflection(Player player, float targetX) : base(true, false)
		{
			this.player = player;
			this.targetX = targetX;
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x0002CBAE File Offset: 0x0002ADAE
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x0002CBC3 File Offset: 0x0002ADC3
		private IEnumerator Cutscene(Level level)
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			this.player.ForceCameraUpdate = true;
			yield return this.player.DummyWalkToExact((int)this.targetX, false, 1f, false);
			yield return 0.1f;
			this.player.Facing = Facings.Right;
			yield return 0.1f;
			yield return this.Level.ZoomTo(new Vector2(200f, 90f), 2f, 1f);
			yield return Textbox.Say("CH6_REFLECT_AFTER", new Func<IEnumerator>[0]);
			yield return this.Level.ZoomBack(0.5f);
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x0002CBDC File Offset: 0x0002ADDC
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
			this.player.ForceCameraUpdate = false;
			this.player.FlipInReflection = false;
			level.Session.SetFlag("reflection", true);
		}

		// Token: 0x04000872 RID: 2162
		public const string Flag = "reflection";

		// Token: 0x04000873 RID: 2163
		private Player player;

		// Token: 0x04000874 RID: 2164
		private float targetX;
	}
}

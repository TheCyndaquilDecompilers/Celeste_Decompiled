using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x02000164 RID: 356
	public class CS03_Guestbook : CutsceneEntity
	{
		// Token: 0x06000CA9 RID: 3241 RVA: 0x0002AA73 File Offset: 0x00028C73
		public CS03_Guestbook(Player player) : base(true, false)
		{
			this.player = player;
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x0002AA84 File Offset: 0x00028C84
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x0002AA98 File Offset: 0x00028C98
		private IEnumerator Routine()
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return Textbox.Say("ch3_guestbook", new Func<IEnumerator>[0]);
			yield return 0.1f;
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x0002AAA7 File Offset: 0x00028CA7
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
		}

		// Token: 0x0400080F RID: 2063
		private Player player;
	}
}

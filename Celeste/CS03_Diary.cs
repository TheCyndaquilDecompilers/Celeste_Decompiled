using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x02000163 RID: 355
	public class CS03_Diary : CutsceneEntity
	{
		// Token: 0x06000CA5 RID: 3237 RVA: 0x0002AA1B File Offset: 0x00028C1B
		public CS03_Diary(Player player) : base(true, false)
		{
			this.player = player;
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0002AA2C File Offset: 0x00028C2C
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Routine(), true));
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x0002AA40 File Offset: 0x00028C40
		private IEnumerator Routine()
		{
			this.player.StateMachine.State = 11;
			this.player.StateMachine.Locked = true;
			yield return Textbox.Say("CH3_DIARY", new Func<IEnumerator>[0]);
			yield return 0.1f;
			base.EndCutscene(this.Level, true);
			yield break;
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x0002AA4F File Offset: 0x00028C4F
		public override void OnEnd(Level level)
		{
			this.player.StateMachine.Locked = false;
			this.player.StateMachine.State = 0;
		}

		// Token: 0x0400080E RID: 2062
		private Player player;
	}
}

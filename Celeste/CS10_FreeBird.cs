using System;
using System.Collections;
using Monocle;

namespace Celeste
{
	// Token: 0x02000161 RID: 353
	public class CS10_FreeBird : CutsceneEntity
	{
		// Token: 0x06000C98 RID: 3224 RVA: 0x0002A892 File Offset: 0x00028A92
		public CS10_FreeBird() : base(true, false)
		{
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0002A89C File Offset: 0x00028A9C
		public override void OnBegin(Level level)
		{
			base.Add(new Coroutine(this.Cutscene(level), true));
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0002A8B1 File Offset: 0x00028AB1
		private IEnumerator Cutscene(Level level)
		{
			yield return Textbox.Say("CH9_FREE_BIRD", new Func<IEnumerator>[0]);
			yield return new FadeWipe(level, false, null)
			{
				Duration = 3f
			}.Duration;
			base.EndCutscene(level, true);
			yield break;
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0002A8C7 File Offset: 0x00028AC7
		public override void OnEnd(Level level)
		{
			level.CompleteArea(false, true, false);
		}
	}
}

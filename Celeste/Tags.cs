using System;
using Monocle;

namespace Celeste
{
	// Token: 0x0200038D RID: 909
	public static class Tags
	{
		// Token: 0x06001D7C RID: 7548 RVA: 0x000CD0D8 File Offset: 0x000CB2D8
		public static void Initialize()
		{
			Tags.PauseUpdate = new BitTag("pauseUpdate");
			Tags.FrozenUpdate = new BitTag("frozenUpdate");
			Tags.TransitionUpdate = new BitTag("transitionUpdate");
			Tags.HUD = new BitTag("hud");
			Tags.Persistent = new BitTag("persistent");
			Tags.Global = new BitTag("global");
		}

		// Token: 0x04001E4A RID: 7754
		public static BitTag PauseUpdate;

		// Token: 0x04001E4B RID: 7755
		public static BitTag FrozenUpdate;

		// Token: 0x04001E4C RID: 7756
		public static BitTag TransitionUpdate;

		// Token: 0x04001E4D RID: 7757
		public static BitTag HUD;

		// Token: 0x04001E4E RID: 7758
		public static BitTag Persistent;

		// Token: 0x04001E4F RID: 7759
		public static BitTag Global;
	}
}

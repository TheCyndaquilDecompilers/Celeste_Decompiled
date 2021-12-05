using System;
using System.Collections.Generic;

namespace Celeste
{
	// Token: 0x0200038C RID: 908
	public static class StatsForStadia
	{
		// Token: 0x06001D77 RID: 7543 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void MakeRequest()
		{
		}

		// Token: 0x06001D78 RID: 7544 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void Increment(StadiaStat stat, int increment = 1)
		{
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void SetIfLarger(StadiaStat stat, int value)
		{
		}

		// Token: 0x06001D7A RID: 7546 RVA: 0x000091E2 File Offset: 0x000073E2
		public static void BeginFrame(IntPtr handle)
		{
		}

		// Token: 0x04001E48 RID: 7752
		private static Dictionary<StadiaStat, string> statToString = new Dictionary<StadiaStat, string>();

		// Token: 0x04001E49 RID: 7753
		private static bool ready;
	}
}

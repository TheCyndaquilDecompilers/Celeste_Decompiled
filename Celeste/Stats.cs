using System;
using System.Collections.Generic;
using Steamworks;

namespace Celeste
{
	// Token: 0x0200038A RID: 906
	public static class Stats
	{
		// Token: 0x06001D6F RID: 7535 RVA: 0x000CCF89 File Offset: 0x000CB189
		public static void MakeRequest()
		{
			Stats.ready = SteamUserStats.RequestCurrentStats();
			SteamUserStats.RequestGlobalStats(0);
		}

		// Token: 0x06001D70 RID: 7536 RVA: 0x000CCF9C File Offset: 0x000CB19C
		public static bool Has()
		{
			return Stats.ready;
		}

		// Token: 0x06001D71 RID: 7537 RVA: 0x000CCFA4 File Offset: 0x000CB1A4
		public static void Increment(Stat stat, int increment = 1)
		{
			if (Stats.ready)
			{
				string text = null;
				if (!Stats.statToString.TryGetValue(stat, out text))
				{
					Stats.statToString.Add(stat, text = stat.ToString());
				}
				int num;
				if (SteamUserStats.GetStat(text, ref num))
				{
					SteamUserStats.SetStat(text, num + increment);
				}
			}
		}

		// Token: 0x06001D72 RID: 7538 RVA: 0x000CCFF8 File Offset: 0x000CB1F8
		public static int Local(Stat stat)
		{
			int result = 0;
			if (Stats.ready)
			{
				string text = null;
				if (!Stats.statToString.TryGetValue(stat, out text))
				{
					Stats.statToString.Add(stat, text = stat.ToString());
				}
				SteamUserStats.GetStat(text, ref result);
			}
			return result;
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x000CD044 File Offset: 0x000CB244
		public static long Global(Stat stat)
		{
			long result = 0L;
			if (Stats.ready)
			{
				string text = null;
				if (!Stats.statToString.TryGetValue(stat, out text))
				{
					Stats.statToString.Add(stat, text = stat.ToString());
				}
				SteamUserStats.GetGlobalStat(text, ref result);
			}
			return result;
		}

		// Token: 0x06001D74 RID: 7540 RVA: 0x000CD090 File Offset: 0x000CB290
		public static void Store()
		{
			if (Stats.ready)
			{
				SteamUserStats.StoreStats();
			}
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x000CD09F File Offset: 0x000CB29F
		public static string Name(Stat stat)
		{
			return Dialog.Clean("STAT_" + stat.ToString(), null);
		}

		// Token: 0x04001E42 RID: 7746
		private static Dictionary<Stat, string> statToString = new Dictionary<Stat, string>();

		// Token: 0x04001E43 RID: 7747
		private static bool ready;
	}
}

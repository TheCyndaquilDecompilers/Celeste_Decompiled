using System;
using Steamworks;

namespace Celeste
{
	// Token: 0x0200024D RID: 589
	public static class Achievements
	{
		// Token: 0x06001282 RID: 4738 RVA: 0x00061F2C File Offset: 0x0006012C
		public static string ID(Achievement achievement)
		{
			return achievement.ToString();
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x00061F3C File Offset: 0x0006013C
		public static bool Has(Achievement achievement)
		{
			bool flag;
			return SteamUserStats.GetAchievement(Achievements.ID(achievement), ref flag) && flag;
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00061F58 File Offset: 0x00060158
		public static void Register(Achievement achievement)
		{
			if (Achievements.Has(achievement))
			{
				return;
			}
			SteamUserStats.SetAchievement(Achievements.ID(achievement));
			Stats.Store();
		}
	}
}

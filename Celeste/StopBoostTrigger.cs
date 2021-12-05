using System;
using Microsoft.Xna.Framework;

namespace Celeste
{
	// Token: 0x020001EB RID: 491
	public class StopBoostTrigger : Trigger
	{
		// Token: 0x0600103F RID: 4159 RVA: 0x00042A43 File Offset: 0x00040C43
		public StopBoostTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x000467FA File Offset: 0x000449FA
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (player.StateMachine.State == 10)
			{
				player.StopSummitLaunch();
			}
		}
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002B8 RID: 696
	[Tracked(false)]
	public class LedgeBlocker : Component
	{
		// Token: 0x06001574 RID: 5492 RVA: 0x0007B636 File Offset: 0x00079836
		public LedgeBlocker(Func<Player, bool> blockChecker = null) : base(false, false)
		{
			this.BlockChecker = blockChecker;
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x0007B650 File Offset: 0x00079850
		public bool HopBlockCheck(Player player)
		{
			return this.Blocking && player.CollideCheck(base.Entity, player.Position + Vector2.UnitX * (float)player.Facing * 8f) && (this.BlockChecker == null || this.BlockChecker(player));
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x0007B6B4 File Offset: 0x000798B4
		public bool JumpThruBoostCheck(Player player)
		{
			return this.Blocking && player.CollideCheck(base.Entity, player.Position - Vector2.UnitY * 2f) && (this.BlockChecker == null || this.BlockChecker(player));
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x0007B709 File Offset: 0x00079909
		public bool DashCorrectCheck(Player player)
		{
			return this.Blocking && player.CollideCheck(base.Entity, player.Position) && (this.BlockChecker == null || this.BlockChecker(player));
		}

		// Token: 0x0400118B RID: 4491
		public bool Blocking = true;

		// Token: 0x0400118C RID: 4492
		public Func<Player, bool> BlockChecker;
	}
}

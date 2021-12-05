using System;

namespace Celeste
{
	// Token: 0x02000317 RID: 791
	[Serializable]
	public struct PlayerInventory
	{
		// Token: 0x060018FE RID: 6398 RVA: 0x000A0086 File Offset: 0x0009E286
		public PlayerInventory(int dashes = 1, bool dreamDash = true, bool backpack = true, bool noRefills = false)
		{
			this.Dashes = dashes;
			this.DreamDash = dreamDash;
			this.Backpack = backpack;
			this.NoRefills = noRefills;
		}

		// Token: 0x04001562 RID: 5474
		public static readonly PlayerInventory Prologue = new PlayerInventory(0, false, true, false);

		// Token: 0x04001563 RID: 5475
		public static readonly PlayerInventory Default = new PlayerInventory(1, true, true, false);

		// Token: 0x04001564 RID: 5476
		public static readonly PlayerInventory OldSite = new PlayerInventory(1, false, true, false);

		// Token: 0x04001565 RID: 5477
		public static readonly PlayerInventory CH6End = new PlayerInventory(2, true, true, false);

		// Token: 0x04001566 RID: 5478
		public static readonly PlayerInventory TheSummit = new PlayerInventory(2, true, false, false);

		// Token: 0x04001567 RID: 5479
		public static readonly PlayerInventory Core = new PlayerInventory(2, true, true, true);

		// Token: 0x04001568 RID: 5480
		public static readonly PlayerInventory Farewell = new PlayerInventory(1, true, false, false);

		// Token: 0x04001569 RID: 5481
		public int Dashes;

		// Token: 0x0400156A RID: 5482
		public bool DreamDash;

		// Token: 0x0400156B RID: 5483
		public bool Backpack;

		// Token: 0x0400156C RID: 5484
		public bool NoRefills;
	}
}

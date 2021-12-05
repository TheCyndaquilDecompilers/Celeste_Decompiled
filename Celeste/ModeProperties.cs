using System;

namespace Celeste
{
	// Token: 0x020002A8 RID: 680
	public class ModeProperties
	{
		// Token: 0x040010E7 RID: 4327
		public string PoemID;

		// Token: 0x040010E8 RID: 4328
		public string Path;

		// Token: 0x040010E9 RID: 4329
		public int TotalStrawberries;

		// Token: 0x040010EA RID: 4330
		public int StartStrawberries;

		// Token: 0x040010EB RID: 4331
		public EntityData[,] StrawberriesByCheckpoint;

		// Token: 0x040010EC RID: 4332
		public CheckpointData[] Checkpoints;

		// Token: 0x040010ED RID: 4333
		public MapData MapData;

		// Token: 0x040010EE RID: 4334
		public PlayerInventory Inventory;

		// Token: 0x040010EF RID: 4335
		public AudioState AudioState;

		// Token: 0x040010F0 RID: 4336
		public bool IgnoreLevelAudioLayerData;
	}
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x020002AB RID: 683
	[Serializable]
	public class AreaModeStats
	{
		// Token: 0x06001522 RID: 5410 RVA: 0x00078F18 File Offset: 0x00077118
		public AreaModeStats Clone()
		{
			return new AreaModeStats
			{
				TotalStrawberries = this.TotalStrawberries,
				Strawberries = new HashSet<EntityID>(this.Strawberries),
				Completed = this.Completed,
				SingleRunCompleted = this.SingleRunCompleted,
				FullClear = this.FullClear,
				Deaths = this.Deaths,
				TimePlayed = this.TimePlayed,
				BestTime = this.BestTime,
				BestFullClearTime = this.BestFullClearTime,
				BestDashes = this.BestDashes,
				BestDeaths = this.BestDeaths,
				HeartGem = this.HeartGem,
				Checkpoints = new HashSet<string>(this.Checkpoints)
			};
		}

		// Token: 0x0400111D RID: 4381
		[XmlAttribute]
		public int TotalStrawberries;

		// Token: 0x0400111E RID: 4382
		[XmlAttribute]
		public bool Completed;

		// Token: 0x0400111F RID: 4383
		[XmlAttribute]
		public bool SingleRunCompleted;

		// Token: 0x04001120 RID: 4384
		[XmlAttribute]
		public bool FullClear;

		// Token: 0x04001121 RID: 4385
		[XmlAttribute]
		public int Deaths;

		// Token: 0x04001122 RID: 4386
		[XmlAttribute]
		public long TimePlayed;

		// Token: 0x04001123 RID: 4387
		[XmlAttribute]
		public long BestTime;

		// Token: 0x04001124 RID: 4388
		[XmlAttribute]
		public long BestFullClearTime;

		// Token: 0x04001125 RID: 4389
		[XmlAttribute]
		public int BestDashes;

		// Token: 0x04001126 RID: 4390
		[XmlAttribute]
		public int BestDeaths;

		// Token: 0x04001127 RID: 4391
		[XmlAttribute]
		public bool HeartGem;

		// Token: 0x04001128 RID: 4392
		public HashSet<EntityID> Strawberries = new HashSet<EntityID>();

		// Token: 0x04001129 RID: 4393
		public HashSet<string> Checkpoints = new HashSet<string>();
	}
}

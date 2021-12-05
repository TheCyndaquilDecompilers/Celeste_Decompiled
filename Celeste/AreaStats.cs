using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x020002AC RID: 684
	[Serializable]
	public class AreaStats
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06001524 RID: 5412 RVA: 0x00078FF0 File Offset: 0x000771F0
		public int TotalStrawberries
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.Modes.Length; i++)
				{
					num += this.Modes[i].TotalStrawberries;
				}
				return num;
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06001525 RID: 5413 RVA: 0x00079024 File Offset: 0x00077224
		public int TotalDeaths
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.Modes.Length; i++)
				{
					num += this.Modes[i].Deaths;
				}
				return num;
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001526 RID: 5414 RVA: 0x00079058 File Offset: 0x00077258
		public long TotalTimePlayed
		{
			get
			{
				long num = 0L;
				for (int i = 0; i < this.Modes.Length; i++)
				{
					num += this.Modes[i].TimePlayed;
				}
				return num;
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x0007908C File Offset: 0x0007728C
		public int BestTotalDeaths
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.Modes.Length; i++)
				{
					num += this.Modes[i].BestDeaths;
				}
				return num;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06001528 RID: 5416 RVA: 0x000790C0 File Offset: 0x000772C0
		public int BestTotalDashes
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.Modes.Length; i++)
				{
					num += this.Modes[i].BestDashes;
				}
				return num;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06001529 RID: 5417 RVA: 0x000790F4 File Offset: 0x000772F4
		public long BestTotalTime
		{
			get
			{
				long num = 0L;
				for (int i = 0; i < this.Modes.Length; i++)
				{
					num += this.Modes[i].BestTime;
				}
				return num;
			}
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x00079128 File Offset: 0x00077328
		public AreaStats(int id)
		{
			this.ID = id;
			int length = Enum.GetValues(typeof(AreaMode)).Length;
			this.Modes = new AreaModeStats[length];
			for (int i = 0; i < this.Modes.Length; i++)
			{
				this.Modes[i] = new AreaModeStats();
			}
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x00079184 File Offset: 0x00077384
		private AreaStats()
		{
			int length = Enum.GetValues(typeof(AreaMode)).Length;
			this.Modes = new AreaModeStats[length];
			for (int i = 0; i < length; i++)
			{
				this.Modes[i] = new AreaModeStats();
			}
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x000791D4 File Offset: 0x000773D4
		public AreaStats Clone()
		{
			AreaStats areaStats = new AreaStats
			{
				ID = this.ID,
				Cassette = this.Cassette
			};
			for (int i = 0; i < this.Modes.Length; i++)
			{
				areaStats.Modes[i] = this.Modes[i].Clone();
			}
			return areaStats;
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00079228 File Offset: 0x00077428
		public void CleanCheckpoints()
		{
			foreach (object obj in Enum.GetValues(typeof(AreaMode)))
			{
				AreaMode areaMode = (AreaMode)obj;
				if ((AreaMode)AreaData.Get(this.ID).Mode.Length > areaMode)
				{
					AreaModeStats areaModeStats = this.Modes[(int)areaMode];
					ModeProperties modeProperties = AreaData.Get(this.ID).Mode[(int)areaMode];
					HashSet<string> hashSet = new HashSet<string>(areaModeStats.Checkpoints);
					areaModeStats.Checkpoints.Clear();
					if (modeProperties != null && modeProperties.Checkpoints != null)
					{
						foreach (CheckpointData checkpointData in modeProperties.Checkpoints)
						{
							if (hashSet.Contains(checkpointData.Level))
							{
								areaModeStats.Checkpoints.Add(checkpointData.Level);
							}
						}
					}
				}
			}
		}

		// Token: 0x0400112A RID: 4394
		[XmlAttribute]
		public int ID;

		// Token: 0x0400112B RID: 4395
		[XmlAttribute]
		public bool Cassette;

		// Token: 0x0400112C RID: 4396
		public AreaModeStats[] Modes;
	}
}

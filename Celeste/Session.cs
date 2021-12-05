using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000319 RID: 793
	[Serializable]
	public class Session
	{
		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06001924 RID: 6436 RVA: 0x000A0F93 File Offset: 0x0009F193
		public MapData MapData
		{
			get
			{
				return AreaData.Areas[this.Area.ID].Mode[(int)this.Area.Mode].MapData;
			}
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x000A0FC0 File Offset: 0x0009F1C0
		private Session()
		{
			this.JustStarted = true;
			this.InArea = true;
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x000A104C File Offset: 0x0009F24C
		public Session(AreaKey area, string checkpoint = null, AreaStats oldStats = null) : this()
		{
			this.Area = area;
			this.StartCheckpoint = checkpoint;
			this.ColorGrade = this.MapData.Data.ColorGrade;
			this.Dreaming = AreaData.Areas[area.ID].Dreaming;
			this.Inventory = AreaData.GetCheckpointInventory(area, checkpoint);
			this.CoreMode = AreaData.Areas[area.ID].CoreMode;
			this.FirstLevel = true;
			this.Audio = this.MapData.ModeData.AudioState.Clone();
			if (this.StartCheckpoint == null)
			{
				this.Level = this.MapData.StartLevel().Name;
				this.StartedFromBeginning = true;
			}
			else
			{
				this.Level = this.StartCheckpoint;
				this.StartedFromBeginning = false;
				this.Dreaming = AreaData.GetCheckpointDreaming(area, checkpoint);
				this.CoreMode = AreaData.GetCheckpointCoreMode(area, checkpoint);
				AudioState checkpointAudioState = AreaData.GetCheckpointAudioState(area, checkpoint);
				if (checkpointAudioState != null)
				{
					if (checkpointAudioState.Music != null)
					{
						this.Audio.Music = checkpointAudioState.Music.Clone();
					}
					if (checkpointAudioState.Ambience != null)
					{
						this.Audio.Ambience = checkpointAudioState.Ambience.Clone();
					}
				}
				string checkpointColorGrading = AreaData.GetCheckpointColorGrading(area, checkpoint);
				if (checkpointColorGrading != null)
				{
					this.ColorGrade = checkpointColorGrading;
				}
				CheckpointData checkpoint2 = AreaData.GetCheckpoint(area, checkpoint);
				if (checkpoint2 != null && checkpoint2.Flags != null)
				{
					foreach (string flag in checkpoint2.Flags)
					{
						this.SetFlag(flag, true);
					}
				}
			}
			if (oldStats != null)
			{
				this.OldStats = oldStats;
				return;
			}
			this.OldStats = SaveData.Instance.Areas[this.Area.ID].Clone();
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06001927 RID: 6439 RVA: 0x000A1224 File Offset: 0x0009F424
		public LevelData LevelData
		{
			get
			{
				return this.MapData.Get(this.Level);
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06001928 RID: 6440 RVA: 0x000A1238 File Offset: 0x0009F438
		public bool FullClear
		{
			get
			{
				return this.Area.Mode == AreaMode.Normal && this.Cassette && this.HeartGem && this.Strawberries.Count >= this.MapData.DetectedStrawberries && (this.Area.ID != 7 || this.HasAllSummitGems);
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06001929 RID: 6441 RVA: 0x000A1292 File Offset: 0x0009F492
		public bool ShouldAdvance
		{
			get
			{
				return this.Area.Mode == AreaMode.Normal && !this.OldStats.Modes[0].Completed && this.Area.ID < SaveData.Instance.MaxArea;
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x000A12D0 File Offset: 0x0009F4D0
		public Session Restart(string intoLevel = null)
		{
			Session session = new Session(this.Area, this.StartCheckpoint, this.OldStats)
			{
				UnlockedCSide = this.UnlockedCSide
			};
			if (intoLevel != null)
			{
				session.Level = intoLevel;
				if (intoLevel != this.MapData.StartLevel().Name)
				{
					session.StartedFromBeginning = false;
				}
			}
			return session;
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x000A132B File Offset: 0x0009F52B
		public void UpdateLevelStartDashes()
		{
			this.DashesAtLevelStart = this.Dashes;
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600192C RID: 6444 RVA: 0x000A133C File Offset: 0x0009F53C
		public bool HasAllSummitGems
		{
			get
			{
				for (int i = 0; i < this.SummitGems.Length; i++)
				{
					if (!this.SummitGems[i])
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x000A1369 File Offset: 0x0009F569
		public Vector2 GetSpawnPoint(Vector2 from)
		{
			return this.LevelData.Spawns.ClosestTo(from);
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x000A137C File Offset: 0x0009F57C
		public bool GetFlag(string flag)
		{
			return this.Flags.Contains(flag);
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x000A138A File Offset: 0x0009F58A
		public void SetFlag(string flag, bool setTo = true)
		{
			if (setTo)
			{
				this.Flags.Add(flag);
				return;
			}
			this.Flags.Remove(flag);
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x000A13AC File Offset: 0x0009F5AC
		public int GetCounter(string counter)
		{
			for (int i = 0; i < this.Counters.Count; i++)
			{
				if (this.Counters[i].Key.Equals(counter))
				{
					return this.Counters[i].Value;
				}
			}
			return 0;
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x000A13FC File Offset: 0x0009F5FC
		public void SetCounter(string counter, int value)
		{
			for (int i = 0; i < this.Counters.Count; i++)
			{
				if (this.Counters[i].Key.Equals(counter))
				{
					this.Counters[i].Value = value;
					return;
				}
			}
			this.Counters.Add(new Session.Counter
			{
				Key = counter,
				Value = value
			});
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x000A146C File Offset: 0x0009F66C
		public void IncrementCounter(string counter)
		{
			for (int i = 0; i < this.Counters.Count; i++)
			{
				if (this.Counters[i].Key.Equals(counter))
				{
					this.Counters[i].Value++;
					return;
				}
			}
			this.Counters.Add(new Session.Counter
			{
				Key = counter,
				Value = 1
			});
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x000A14E0 File Offset: 0x0009F6E0
		public bool GetLevelFlag(string level)
		{
			return this.LevelFlags.Contains(level);
		}

		// Token: 0x0400158E RID: 5518
		public AreaKey Area;

		// Token: 0x0400158F RID: 5519
		public Vector2? RespawnPoint;

		// Token: 0x04001590 RID: 5520
		public AudioState Audio = new AudioState();

		// Token: 0x04001591 RID: 5521
		public PlayerInventory Inventory;

		// Token: 0x04001592 RID: 5522
		public HashSet<string> Flags = new HashSet<string>();

		// Token: 0x04001593 RID: 5523
		public HashSet<string> LevelFlags = new HashSet<string>();

		// Token: 0x04001594 RID: 5524
		public HashSet<EntityID> Strawberries = new HashSet<EntityID>();

		// Token: 0x04001595 RID: 5525
		public HashSet<EntityID> DoNotLoad = new HashSet<EntityID>();

		// Token: 0x04001596 RID: 5526
		public HashSet<EntityID> Keys = new HashSet<EntityID>();

		// Token: 0x04001597 RID: 5527
		public List<Session.Counter> Counters = new List<Session.Counter>();

		// Token: 0x04001598 RID: 5528
		public bool[] SummitGems = new bool[6];

		// Token: 0x04001599 RID: 5529
		public AreaStats OldStats;

		// Token: 0x0400159A RID: 5530
		public bool UnlockedCSide;

		// Token: 0x0400159B RID: 5531
		public string FurthestSeenLevel;

		// Token: 0x0400159C RID: 5532
		public bool BeatBestTime;

		// Token: 0x0400159D RID: 5533
		[XmlAttribute]
		public string Level;

		// Token: 0x0400159E RID: 5534
		[XmlAttribute]
		public long Time;

		// Token: 0x0400159F RID: 5535
		[XmlAttribute]
		public bool StartedFromBeginning;

		// Token: 0x040015A0 RID: 5536
		[XmlAttribute]
		public int Deaths;

		// Token: 0x040015A1 RID: 5537
		[XmlAttribute]
		public int Dashes;

		// Token: 0x040015A2 RID: 5538
		[XmlAttribute]
		public int DashesAtLevelStart;

		// Token: 0x040015A3 RID: 5539
		[XmlAttribute]
		public int DeathsInCurrentLevel;

		// Token: 0x040015A4 RID: 5540
		[XmlAttribute]
		public bool InArea;

		// Token: 0x040015A5 RID: 5541
		[XmlAttribute]
		public string StartCheckpoint;

		// Token: 0x040015A6 RID: 5542
		[XmlAttribute]
		public bool FirstLevel = true;

		// Token: 0x040015A7 RID: 5543
		[XmlAttribute]
		public bool Cassette;

		// Token: 0x040015A8 RID: 5544
		[XmlAttribute]
		public bool HeartGem;

		// Token: 0x040015A9 RID: 5545
		[XmlAttribute]
		public bool Dreaming;

		// Token: 0x040015AA RID: 5546
		[XmlAttribute]
		public string ColorGrade;

		// Token: 0x040015AB RID: 5547
		[XmlAttribute]
		public float LightingAlphaAdd;

		// Token: 0x040015AC RID: 5548
		[XmlAttribute]
		public float BloomBaseAdd;

		// Token: 0x040015AD RID: 5549
		[XmlAttribute]
		public float DarkRoomAlpha = 0.75f;

		// Token: 0x040015AE RID: 5550
		[XmlAttribute]
		public Session.CoreModes CoreMode;

		// Token: 0x040015AF RID: 5551
		[XmlAttribute]
		public bool GrabbedGolden;

		// Token: 0x040015B0 RID: 5552
		[XmlAttribute]
		public bool HitCheckpoint;

		// Token: 0x040015B1 RID: 5553
		[XmlIgnore]
		[NonSerialized]
		public bool JustStarted;

		// Token: 0x020006DB RID: 1755
		[Serializable]
		public class Counter
		{
			// Token: 0x04002C84 RID: 11396
			[XmlAttribute("key")]
			public string Key;

			// Token: 0x04002C85 RID: 11397
			[XmlAttribute("value")]
			public int Value;
		}

		// Token: 0x020006DC RID: 1756
		public enum CoreModes
		{
			// Token: 0x04002C87 RID: 11399
			None,
			// Token: 0x04002C88 RID: 11400
			Hot,
			// Token: 0x04002C89 RID: 11401
			Cold
		}
	}
}

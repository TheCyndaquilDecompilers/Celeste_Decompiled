using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x02000318 RID: 792
	[Serializable]
	public class SaveData
	{
		// Token: 0x06001900 RID: 6400 RVA: 0x000A0117 File Offset: 0x0009E317
		public static void Start(SaveData data, int slot)
		{
			SaveData.Instance = data;
			SaveData.Instance.FileSlot = slot;
			SaveData.Instance.AfterInitialize();
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x000A0134 File Offset: 0x0009E334
		public static string GetFilename(int slot)
		{
			if (slot == 4)
			{
				return "debug";
			}
			return slot.ToString();
		}

		// Token: 0x06001902 RID: 6402 RVA: 0x000A0147 File Offset: 0x0009E347
		public static string GetFilename()
		{
			return SaveData.GetFilename(SaveData.Instance.FileSlot);
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x000A0158 File Offset: 0x0009E358
		public static void InitializeDebugMode(bool loadExisting = true)
		{
			SaveData saveData = null;
			if (loadExisting && UserIO.Open(UserIO.Mode.Read))
			{
				saveData = UserIO.Load<SaveData>(SaveData.GetFilename(4), false);
				UserIO.Close();
			}
			if (saveData == null)
			{
				saveData = new SaveData();
			}
			saveData.DebugMode = true;
			saveData.CurrentSession = null;
			SaveData.Start(saveData, 4);
		}

		// Token: 0x06001904 RID: 6404 RVA: 0x000A01A2 File Offset: 0x0009E3A2
		public static bool TryDelete(int slot)
		{
			return UserIO.Delete(SaveData.GetFilename(slot));
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x000A01B0 File Offset: 0x0009E3B0
		public void AfterInitialize()
		{
			while (this.Areas.Count < AreaData.Areas.Count)
			{
				this.Areas.Add(new AreaStats(this.Areas.Count));
			}
			while (this.Areas.Count > AreaData.Areas.Count)
			{
				this.Areas.RemoveAt(this.Areas.Count - 1);
			}
			int num = -1;
			for (int i = 0; i < this.Areas.Count; i++)
			{
				if (this.Areas[i].Modes[0].Completed || (this.Areas[i].Modes.Length > 1 && this.Areas[i].Modes[1].Completed))
				{
					num = i;
				}
			}
			if (this.UnlockedAreas < num + 1 && this.MaxArea >= num + 1)
			{
				this.UnlockedAreas = num + 1;
			}
			if (this.DebugMode)
			{
				this.CurrentSession = null;
				this.RevealedChapter9 = true;
				this.UnlockedAreas = this.MaxArea;
			}
			if (this.CheatMode)
			{
				this.UnlockedAreas = this.MaxArea;
			}
			if (string.IsNullOrEmpty(this.TheoSisterName))
			{
				this.TheoSisterName = Dialog.Clean("THEO_SISTER_NAME", null);
				if (this.Name.IndexOf(this.TheoSisterName, StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					this.TheoSisterName = Dialog.Clean("THEO_SISTER_ALT_NAME", null);
				}
			}
			this.AssistModeChecks();
			foreach (AreaStats areaStats in this.Areas)
			{
				areaStats.CleanCheckpoints();
			}
			if (this.Version != null && new Version(this.Version) < new Version(1, 2, 1, 1))
			{
				for (int j = 0; j < this.Areas.Count; j++)
				{
					if (this.Areas[j] != null)
					{
						for (int k = 0; k < this.Areas[j].Modes.Length; k++)
						{
							if (this.Areas[j].Modes[k] != null)
							{
								if (this.Areas[j].Modes[k].BestTime > 0L)
								{
									this.Areas[j].Modes[k].SingleRunCompleted = true;
								}
								this.Areas[j].Modes[k].BestTime = 0L;
								this.Areas[j].Modes[k].BestFullClearTime = 0L;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x000A046C File Offset: 0x0009E66C
		public void AssistModeChecks()
		{
			if (!this.VariantMode && !this.AssistMode)
			{
				this.Assists = default(Assists);
			}
			else if (!this.VariantMode)
			{
				this.Assists.EnfornceAssistMode();
			}
			if (this.Assists.GameSpeed < 5 || this.Assists.GameSpeed > 20)
			{
				this.Assists.GameSpeed = 10;
			}
			Input.MoveX.Inverted = (Input.Aim.InvertedX = (Input.Feather.InvertedX = this.Assists.MirrorMode));
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x000A0504 File Offset: 0x0009E704
		public static void NoFileAssistChecks()
		{
			Input.MoveX.Inverted = (Input.Aim.InvertedX = (Input.Feather.InvertedX = false));
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x000A0536 File Offset: 0x0009E736
		public void BeforeSave()
		{
			SaveData.Instance.Version = Celeste.Instance.Version.ToString();
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x000A0554 File Offset: 0x0009E754
		public void StartSession(Session session)
		{
			this.LastArea = session.Area;
			this.CurrentSession = session;
			if (this.DebugMode)
			{
				AreaModeStats areaModeStats = this.Areas[session.Area.ID].Modes[(int)session.Area.Mode];
				AreaModeStats areaModeStats2 = session.OldStats.Modes[(int)session.Area.Mode];
				SaveData.Instance.TotalStrawberries -= areaModeStats.TotalStrawberries;
				areaModeStats.Strawberries.Clear();
				areaModeStats.TotalStrawberries = 0;
				areaModeStats2.Strawberries.Clear();
				areaModeStats2.TotalStrawberries = 0;
			}
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x000A05F8 File Offset: 0x0009E7F8
		public void AddDeath(AreaKey area)
		{
			this.TotalDeaths++;
			this.Areas[area.ID].Modes[(int)area.Mode].Deaths++;
			Stats.Increment(Stat.DEATHS, 1);
			StatsForStadia.Increment(StadiaStat.DEATHS, 1);
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x000A064C File Offset: 0x0009E84C
		public void AddStrawberry(AreaKey area, EntityID strawberry, bool golden)
		{
			AreaModeStats areaModeStats = this.Areas[area.ID].Modes[(int)area.Mode];
			if (!areaModeStats.Strawberries.Contains(strawberry))
			{
				areaModeStats.Strawberries.Add(strawberry);
				areaModeStats.TotalStrawberries++;
				this.TotalStrawberries++;
				if (golden)
				{
					this.TotalGoldenStrawberries++;
				}
				if (this.TotalStrawberries >= 30)
				{
					Achievements.Register(Achievement.STRB1);
				}
				if (this.TotalStrawberries >= 80)
				{
					Achievements.Register(Achievement.STRB2);
				}
				if (this.TotalStrawberries >= 175)
				{
					Achievements.Register(Achievement.STRB3);
				}
				StatsForStadia.SetIfLarger(StadiaStat.BERRIES, this.TotalStrawberries);
			}
			Stats.Increment(golden ? Stat.GOLDBERRIES : Stat.BERRIES, 1);
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x000A070C File Offset: 0x0009E90C
		public void AddStrawberry(EntityID strawberry, bool golden)
		{
			this.AddStrawberry(this.CurrentSession.Area, strawberry, golden);
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x000A0721 File Offset: 0x0009E921
		public bool CheckStrawberry(AreaKey area, EntityID strawberry)
		{
			return this.Areas[area.ID].Modes[(int)area.Mode].Strawberries.Contains(strawberry);
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x000A074B File Offset: 0x0009E94B
		public bool CheckStrawberry(EntityID strawberry)
		{
			return this.CheckStrawberry(this.CurrentSession.Area, strawberry);
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x000A075F File Offset: 0x0009E95F
		public void AddTime(AreaKey area, long time)
		{
			this.Time += time;
			this.Areas[area.ID].Modes[(int)area.Mode].TimePlayed += time;
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x000A079C File Offset: 0x0009E99C
		public void RegisterHeartGem(AreaKey area)
		{
			this.Areas[area.ID].Modes[(int)area.Mode].HeartGem = true;
			if (area.Mode == AreaMode.Normal)
			{
				if (area.ID == 1)
				{
					Achievements.Register(Achievement.HEART1);
				}
				else if (area.ID == 2)
				{
					Achievements.Register(Achievement.HEART2);
				}
				else if (area.ID == 3)
				{
					Achievements.Register(Achievement.HEART3);
				}
				else if (area.ID == 4)
				{
					Achievements.Register(Achievement.HEART4);
				}
				else if (area.ID == 5)
				{
					Achievements.Register(Achievement.HEART5);
				}
				else if (area.ID == 6)
				{
					Achievements.Register(Achievement.HEART6);
				}
				else if (area.ID == 7)
				{
					Achievements.Register(Achievement.HEART7);
				}
				else if (area.ID == 9)
				{
					Achievements.Register(Achievement.HEART8);
				}
			}
			else if (area.Mode == AreaMode.BSide)
			{
				if (area.ID == 1)
				{
					Achievements.Register(Achievement.BSIDE1);
				}
				else if (area.ID == 2)
				{
					Achievements.Register(Achievement.BSIDE2);
				}
				else if (area.ID == 3)
				{
					Achievements.Register(Achievement.BSIDE3);
				}
				else if (area.ID == 4)
				{
					Achievements.Register(Achievement.BSIDE4);
				}
				else if (area.ID == 5)
				{
					Achievements.Register(Achievement.BSIDE5);
				}
				else if (area.ID == 6)
				{
					Achievements.Register(Achievement.BSIDE6);
				}
				else if (area.ID == 7)
				{
					Achievements.Register(Achievement.BSIDE7);
				}
				else if (area.ID == 9)
				{
					Achievements.Register(Achievement.BSIDE8);
				}
			}
			StatsForStadia.SetIfLarger(StadiaStat.HEARTS, this.TotalHeartGems);
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x000A092A File Offset: 0x0009EB2A
		public void RegisterCassette(AreaKey area)
		{
			this.Areas[area.ID].Cassette = true;
			Achievements.Register(Achievement.CASS);
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x000A094A File Offset: 0x0009EB4A
		public bool RegisterPoemEntry(string id)
		{
			id = id.ToLower();
			if (this.Poem.Contains(id))
			{
				return false;
			}
			this.Poem.Add(id);
			return true;
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x000A0971 File Offset: 0x0009EB71
		public void RegisterSummitGem(int id)
		{
			if (this.SummitGems == null)
			{
				this.SummitGems = new bool[6];
			}
			this.SummitGems[id] = true;
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x000A0990 File Offset: 0x0009EB90
		public void RegisterCompletion(Session session)
		{
			AreaKey area = session.Area;
			AreaModeStats areaModeStats = this.Areas[area.ID].Modes[(int)area.Mode];
			if (session.GrabbedGolden)
			{
				areaModeStats.BestDeaths = 0;
			}
			if (session.StartedFromBeginning)
			{
				areaModeStats.SingleRunCompleted = true;
				if (areaModeStats.BestTime <= 0L || session.Deaths < areaModeStats.BestDeaths)
				{
					areaModeStats.BestDeaths = session.Deaths;
				}
				if (areaModeStats.BestTime <= 0L || session.Dashes < areaModeStats.BestDashes)
				{
					areaModeStats.BestDashes = session.Dashes;
				}
				if (areaModeStats.BestTime <= 0L || session.Time < areaModeStats.BestTime)
				{
					if (areaModeStats.BestTime > 0L)
					{
						session.BeatBestTime = true;
					}
					areaModeStats.BestTime = session.Time;
				}
				if (area.Mode == AreaMode.Normal && session.FullClear)
				{
					areaModeStats.FullClear = true;
					if (session.StartedFromBeginning && (areaModeStats.BestFullClearTime <= 0L || session.Time < areaModeStats.BestFullClearTime))
					{
						areaModeStats.BestFullClearTime = session.Time;
					}
				}
			}
			if (area.ID + 1 > this.UnlockedAreas && area.ID < this.MaxArea)
			{
				this.UnlockedAreas = area.ID + 1;
			}
			areaModeStats.Completed = true;
			session.InArea = false;
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x000A0AE0 File Offset: 0x0009ECE0
		public bool SetCheckpoint(AreaKey area, string level)
		{
			AreaModeStats areaModeStats = this.Areas[area.ID].Modes[(int)area.Mode];
			if (!areaModeStats.Checkpoints.Contains(level))
			{
				areaModeStats.Checkpoints.Add(level);
				return true;
			}
			return false;
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x000A0B29 File Offset: 0x0009ED29
		public bool HasCheckpoint(AreaKey area, string level)
		{
			return this.Areas[area.ID].Modes[(int)area.Mode].Checkpoints.Contains(level);
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x000A0B54 File Offset: 0x0009ED54
		public bool FoundAnyCheckpoints(AreaKey area)
		{
			if (Celeste.PlayMode == Celeste.PlayModes.Event)
			{
				return false;
			}
			if (this.DebugMode || this.CheatMode)
			{
				ModeProperties modeProperties = AreaData.Areas[area.ID].Mode[(int)area.Mode];
				return modeProperties != null && modeProperties.Checkpoints != null && modeProperties.Checkpoints.Length != 0;
			}
			return this.Areas[area.ID].Modes[(int)area.Mode].Checkpoints.Count > 0;
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x000A0BDC File Offset: 0x0009EDDC
		public HashSet<string> GetCheckpoints(AreaKey area)
		{
			if (Celeste.PlayMode == Celeste.PlayModes.Event)
			{
				return new HashSet<string>();
			}
			if (this.DebugMode || this.CheatMode)
			{
				HashSet<string> hashSet = new HashSet<string>();
				ModeProperties modeProperties = AreaData.Areas[area.ID].Mode[(int)area.Mode];
				if (modeProperties.Checkpoints != null)
				{
					foreach (CheckpointData checkpointData in modeProperties.Checkpoints)
					{
						hashSet.Add(checkpointData.Level);
					}
				}
				return hashSet;
			}
			return this.Areas[area.ID].Modes[(int)area.Mode].Checkpoints;
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x000A0C7E File Offset: 0x0009EE7E
		public bool HasFlag(string flag)
		{
			return this.Flags.Contains(flag);
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x000A0C8C File Offset: 0x0009EE8C
		public void SetFlag(string flag)
		{
			if (!this.HasFlag(flag))
			{
				this.Flags.Add(flag);
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x0600191B RID: 6427 RVA: 0x000A0CA4 File Offset: 0x0009EEA4
		public int UnlockedModes
		{
			get
			{
				if (this.DebugMode || this.CheatMode)
				{
					return 3;
				}
				if (this.TotalHeartGems >= 16)
				{
					return 3;
				}
				for (int i = 1; i <= this.MaxArea; i++)
				{
					if (this.Areas[i].Cassette)
					{
						return 2;
					}
				}
				return 1;
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x0600191C RID: 6428 RVA: 0x000A0CF6 File Offset: 0x0009EEF6
		public int MaxArea
		{
			get
			{
				if (Celeste.PlayMode == Celeste.PlayModes.Event)
				{
					return 2;
				}
				return AreaData.Areas.Count - 1;
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x0600191D RID: 6429 RVA: 0x000A0D0E File Offset: 0x0009EF0E
		public int MaxAssistArea
		{
			get
			{
				return AreaData.Areas.Count - 1;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x0600191E RID: 6430 RVA: 0x000A0D1C File Offset: 0x0009EF1C
		public int TotalHeartGems
		{
			get
			{
				int num = 0;
				foreach (AreaStats areaStats in this.Areas)
				{
					for (int i = 0; i < areaStats.Modes.Length; i++)
					{
						if (areaStats.Modes[i] != null && areaStats.Modes[i].HeartGem)
						{
							num++;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600191F RID: 6431 RVA: 0x000A0D9C File Offset: 0x0009EF9C
		public int TotalCassettes
		{
			get
			{
				int num = 0;
				for (int i = 0; i <= this.MaxArea; i++)
				{
					if (!AreaData.Get(i).Interlude && this.Areas[i].Cassette)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06001920 RID: 6432 RVA: 0x000A0DE4 File Offset: 0x0009EFE4
		public int TotalCompletions
		{
			get
			{
				int num = 0;
				for (int i = 0; i <= this.MaxArea; i++)
				{
					if (!AreaData.Get(i).Interlude && this.Areas[i].Modes[0].Completed)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06001921 RID: 6433 RVA: 0x000A0E30 File Offset: 0x0009F030
		public bool HasAllFullClears
		{
			get
			{
				for (int i = 0; i <= this.MaxArea; i++)
				{
					if (AreaData.Get(i).CanFullClear && !this.Areas[i].Modes[0].FullClear)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06001922 RID: 6434 RVA: 0x000A0E78 File Offset: 0x0009F078
		public int CompletionPercent
		{
			get
			{
				float num = 0f;
				if (this.TotalHeartGems >= 24)
				{
					num += 24f;
				}
				else
				{
					num += (float)this.TotalHeartGems / 24f * 24f;
				}
				if (this.TotalStrawberries >= 175)
				{
					num += 55f;
				}
				else
				{
					num += (float)this.TotalStrawberries / 175f * 55f;
				}
				if (this.TotalCassettes >= 8)
				{
					num += 7f;
				}
				else
				{
					num += (float)this.TotalCassettes / 8f * 7f;
				}
				if (this.TotalCompletions >= 8)
				{
					num += 14f;
				}
				else
				{
					num += (float)this.TotalCompletions / 8f * 14f;
				}
				if (num < 0f)
				{
					num = 0f;
				}
				else if (num > 100f)
				{
					num = 100f;
				}
				return (int)num;
			}
		}

		// Token: 0x0400156D RID: 5485
		public const int MaxStrawberries = 175;

		// Token: 0x0400156E RID: 5486
		public const int MaxGoldenStrawberries = 25;

		// Token: 0x0400156F RID: 5487
		public const int MaxStrawberriesDLC = 202;

		// Token: 0x04001570 RID: 5488
		public const int MaxHeartGems = 24;

		// Token: 0x04001571 RID: 5489
		public const int MaxCassettes = 8;

		// Token: 0x04001572 RID: 5490
		public const int MaxCompletions = 8;

		// Token: 0x04001573 RID: 5491
		public static SaveData Instance;

		// Token: 0x04001574 RID: 5492
		public string Version;

		// Token: 0x04001575 RID: 5493
		public string Name = "Madeline";

		// Token: 0x04001576 RID: 5494
		public long Time;

		// Token: 0x04001577 RID: 5495
		public DateTime LastSave;

		// Token: 0x04001578 RID: 5496
		public bool CheatMode;

		// Token: 0x04001579 RID: 5497
		public bool AssistMode;

		// Token: 0x0400157A RID: 5498
		public bool VariantMode;

		// Token: 0x0400157B RID: 5499
		public Assists Assists = Assists.Default;

		// Token: 0x0400157C RID: 5500
		public string TheoSisterName;

		// Token: 0x0400157D RID: 5501
		public int UnlockedAreas;

		// Token: 0x0400157E RID: 5502
		public int TotalDeaths;

		// Token: 0x0400157F RID: 5503
		public int TotalStrawberries;

		// Token: 0x04001580 RID: 5504
		public int TotalGoldenStrawberries;

		// Token: 0x04001581 RID: 5505
		public int TotalJumps;

		// Token: 0x04001582 RID: 5506
		public int TotalWallJumps;

		// Token: 0x04001583 RID: 5507
		public int TotalDashes;

		// Token: 0x04001584 RID: 5508
		public HashSet<string> Flags = new HashSet<string>();

		// Token: 0x04001585 RID: 5509
		public List<string> Poem = new List<string>();

		// Token: 0x04001586 RID: 5510
		public bool[] SummitGems;

		// Token: 0x04001587 RID: 5511
		public bool RevealedChapter9;

		// Token: 0x04001588 RID: 5512
		public AreaKey LastArea;

		// Token: 0x04001589 RID: 5513
		public Session CurrentSession;

		// Token: 0x0400158A RID: 5514
		public List<AreaStats> Areas = new List<AreaStats>();

		// Token: 0x0400158B RID: 5515
		[XmlIgnore]
		[NonSerialized]
		public int FileSlot;

		// Token: 0x0400158C RID: 5516
		[XmlIgnore]
		[NonSerialized]
		public bool DoNotSave;

		// Token: 0x0400158D RID: 5517
		[XmlIgnore]
		[NonSerialized]
		public bool DebugMode;
	}
}

using System;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F0 RID: 496
	public class AutoSplitterInfo
	{
		// Token: 0x06001054 RID: 4180 RVA: 0x00047A14 File Offset: 0x00045C14
		public void Update()
		{
			Level level = Engine.Scene as Level;
			this.ChapterStarted = (level != null);
			this.ChapterComplete = (this.ChapterStarted && level.Completed);
			this.TimerActive = (this.ChapterStarted && !level.Completed);
			this.Chapter = (this.ChapterStarted ? level.Session.Area.ID : -1);
			this.Mode = (int)(this.ChapterStarted ? level.Session.Area.Mode : ((AreaMode)(-1)));
			this.Level = (this.ChapterStarted ? level.Session.Level : "");
			this.ChapterTime = (this.ChapterStarted ? level.Session.Time : 0L);
			this.FileTime = ((SaveData.Instance != null) ? SaveData.Instance.Time : 0L);
			this.ChapterStrawberries = (this.ChapterStarted ? level.Session.Strawberries.Count : 0);
			this.FileStrawberries = ((SaveData.Instance != null) ? SaveData.Instance.TotalStrawberries : 0);
			this.ChapterHeart = (this.ChapterStarted && level.Session.HeartGem);
			this.FileHearts = ((SaveData.Instance != null) ? SaveData.Instance.TotalHeartGems : 0);
			this.ChapterCassette = (this.ChapterStarted && level.Session.Cassette);
			this.FileCassettes = ((SaveData.Instance != null) ? SaveData.Instance.TotalCassettes : 0);
		}

		// Token: 0x04000BB2 RID: 2994
		public int Chapter;

		// Token: 0x04000BB3 RID: 2995
		public int Mode;

		// Token: 0x04000BB4 RID: 2996
		public string Level;

		// Token: 0x04000BB5 RID: 2997
		public bool TimerActive;

		// Token: 0x04000BB6 RID: 2998
		public bool ChapterStarted;

		// Token: 0x04000BB7 RID: 2999
		public bool ChapterComplete;

		// Token: 0x04000BB8 RID: 3000
		public long ChapterTime;

		// Token: 0x04000BB9 RID: 3001
		public int ChapterStrawberries;

		// Token: 0x04000BBA RID: 3002
		public bool ChapterCassette;

		// Token: 0x04000BBB RID: 3003
		public bool ChapterHeart;

		// Token: 0x04000BBC RID: 3004
		public long FileTime;

		// Token: 0x04000BBD RID: 3005
		public int FileStrawberries;

		// Token: 0x04000BBE RID: 3006
		public int FileCassettes;

		// Token: 0x04000BBF RID: 3007
		public int FileHearts;
	}
}

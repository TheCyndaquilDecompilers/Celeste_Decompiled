using System;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x020002A7 RID: 679
	[Serializable]
	public struct AreaKey
	{
		// Token: 0x06001502 RID: 5378 RVA: 0x00076AC0 File Offset: 0x00074CC0
		public AreaKey(int id, AreaMode mode = AreaMode.Normal)
		{
			this.ID = id;
			this.Mode = mode;
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x00076AD0 File Offset: 0x00074CD0
		public int ChapterIndex
		{
			get
			{
				if (AreaData.Areas[this.ID].Interlude)
				{
					return -1;
				}
				int num = 0;
				for (int i = 0; i <= this.ID; i++)
				{
					if (!AreaData.Areas[i].Interlude)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x00076B20 File Offset: 0x00074D20
		public static bool operator ==(AreaKey a, AreaKey b)
		{
			return a.ID == b.ID && a.Mode == b.Mode;
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x00076B40 File Offset: 0x00074D40
		public static bool operator !=(AreaKey a, AreaKey b)
		{
			return a.ID != b.ID || a.Mode != b.Mode;
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool Equals(object obj)
		{
			return false;
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x00076B63 File Offset: 0x00074D63
		public override int GetHashCode()
		{
			return (int)(this.ID * 3 + this.Mode);
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x00076B74 File Offset: 0x00074D74
		public override string ToString()
		{
			string text = this.ID.ToString();
			if (this.Mode == AreaMode.BSide)
			{
				text += "H";
			}
			else if (this.Mode == AreaMode.CSide)
			{
				text += "HH";
			}
			return text;
		}

		// Token: 0x040010E3 RID: 4323
		public static readonly AreaKey None = new AreaKey(-1, AreaMode.Normal);

		// Token: 0x040010E4 RID: 4324
		public static readonly AreaKey Default = new AreaKey(0, AreaMode.Normal);

		// Token: 0x040010E5 RID: 4325
		[XmlAttribute]
		public int ID;

		// Token: 0x040010E6 RID: 4326
		[XmlAttribute]
		public AreaMode Mode;
	}
}

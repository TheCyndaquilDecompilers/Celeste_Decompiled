using System;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x0200031A RID: 794
	[Serializable]
	public struct EntityID
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06001934 RID: 6452 RVA: 0x000A14EE File Offset: 0x0009F6EE
		// (set) Token: 0x06001935 RID: 6453 RVA: 0x000A150C File Offset: 0x0009F70C
		[XmlAttribute]
		public string Key
		{
			get
			{
				return this.Level + ":" + this.ID;
			}
			set
			{
				string[] array = value.Split(new char[]
				{
					':'
				});
				this.Level = array[0];
				this.ID = int.Parse(array[1]);
			}
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x000A1542 File Offset: 0x0009F742
		public EntityID(string level, int entityID)
		{
			this.Level = level;
			this.ID = entityID;
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x000A1552 File Offset: 0x0009F752
		public override string ToString()
		{
			return this.Key;
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x000A155A File Offset: 0x0009F75A
		public override int GetHashCode()
		{
			return this.Level.GetHashCode() ^ this.ID;
		}

		// Token: 0x040015B2 RID: 5554
		public static readonly EntityID None = new EntityID("null", -1);

		// Token: 0x040015B3 RID: 5555
		[XmlIgnore]
		public string Level;

		// Token: 0x040015B4 RID: 5556
		[XmlIgnore]
		public int ID;
	}
}

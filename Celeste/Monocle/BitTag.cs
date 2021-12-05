using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x0200011F RID: 287
	public class BitTag
	{
		// Token: 0x0600095A RID: 2394 RVA: 0x00016E40 File Offset: 0x00015040
		public static BitTag Get(string name)
		{
			return BitTag.byName[name];
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x00016E50 File Offset: 0x00015050
		public BitTag(string name)
		{
			this.ID = BitTag.TotalTags;
			this.Value = 1 << BitTag.TotalTags;
			this.Name = name;
			BitTag.byID[this.ID] = this;
			BitTag.byName[name] = this;
			BitTag.TotalTags++;
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x00016EAA File Offset: 0x000150AA
		public static implicit operator int(BitTag tag)
		{
			return tag.Value;
		}

		// Token: 0x04000617 RID: 1559
		internal static int TotalTags = 0;

		// Token: 0x04000618 RID: 1560
		internal static BitTag[] byID = new BitTag[32];

		// Token: 0x04000619 RID: 1561
		private static Dictionary<string, BitTag> byName = new Dictionary<string, BitTag>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0400061A RID: 1562
		public int ID;

		// Token: 0x0400061B RID: 1563
		public int Value;

		// Token: 0x0400061C RID: 1564
		public string Name;
	}
}

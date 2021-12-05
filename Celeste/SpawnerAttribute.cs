using System;

namespace Celeste
{
	// Token: 0x020001BB RID: 443
	public class SpawnerAttribute : Attribute
	{
		// Token: 0x06000F51 RID: 3921 RVA: 0x0003E457 File Offset: 0x0003C657
		public SpawnerAttribute(string name = null)
		{
			this.Name = name;
		}

		// Token: 0x04000ABA RID: 2746
		public string Name;
	}
}

using System;

namespace Monocle
{
	// Token: 0x02000128 RID: 296
	public class Command : Attribute
	{
		// Token: 0x06000AA7 RID: 2727 RVA: 0x0001C111 File Offset: 0x0001A311
		public Command(string name, string help)
		{
			this.Name = name;
			this.Help = help;
		}

		// Token: 0x0400065A RID: 1626
		public string Name;

		// Token: 0x0400065B RID: 1627
		public string Help;
	}
}

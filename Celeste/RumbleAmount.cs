using System;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x0200031B RID: 795
	public enum RumbleAmount
	{
		// Token: 0x040015B6 RID: 5558
		[XmlEnum("false")]
		Off,
		// Token: 0x040015B7 RID: 5559
		Half,
		// Token: 0x040015B8 RID: 5560
		[XmlEnum("true")]
		On
	}
}

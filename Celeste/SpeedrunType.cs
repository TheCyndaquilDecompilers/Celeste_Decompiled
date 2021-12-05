using System;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x0200031C RID: 796
	public enum SpeedrunType
	{
		// Token: 0x040015BA RID: 5562
		[XmlEnum("false")]
		Off,
		// Token: 0x040015BB RID: 5563
		[XmlEnum("true")]
		Chapter,
		// Token: 0x040015BC RID: 5564
		File
	}
}

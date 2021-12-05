using System;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x0200031D RID: 797
	public enum ScreenshakeAmount
	{
		// Token: 0x040015BE RID: 5566
		[XmlEnum("false")]
		Off,
		// Token: 0x040015BF RID: 5567
		[XmlEnum("true")]
		Half,
		// Token: 0x040015C0 RID: 5568
		On
	}
}

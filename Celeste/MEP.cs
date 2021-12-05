using System;
using System.Xml.Serialization;

namespace Celeste
{
	// Token: 0x0200025B RID: 603
	[Serializable]
	public class MEP
	{
		// Token: 0x060012C5 RID: 4805 RVA: 0x000026FC File Offset: 0x000008FC
		public MEP()
		{
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x00065B26 File Offset: 0x00063D26
		public MEP(string key, float value)
		{
			this.Key = key;
			this.Value = value;
		}

		// Token: 0x04000EC3 RID: 3779
		[XmlAttribute]
		public string Key;

		// Token: 0x04000EC4 RID: 3780
		[XmlAttribute]
		public float Value;
	}
}

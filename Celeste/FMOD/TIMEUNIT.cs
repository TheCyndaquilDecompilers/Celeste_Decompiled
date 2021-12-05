using System;

namespace FMOD
{
	// Token: 0x02000036 RID: 54
	[Flags]
	public enum TIMEUNIT : uint
	{
		// Token: 0x04000199 RID: 409
		MS = 1U,
		// Token: 0x0400019A RID: 410
		PCM = 2U,
		// Token: 0x0400019B RID: 411
		PCMBYTES = 4U,
		// Token: 0x0400019C RID: 412
		RAWBYTES = 8U,
		// Token: 0x0400019D RID: 413
		PCMFRACTION = 16U,
		// Token: 0x0400019E RID: 414
		MODORDER = 256U,
		// Token: 0x0400019F RID: 415
		MODROW = 512U,
		// Token: 0x040001A0 RID: 416
		MODPATTERN = 1024U
	}
}

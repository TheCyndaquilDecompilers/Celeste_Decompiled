using System;

namespace FMOD
{
	// Token: 0x02000088 RID: 136
	public struct DSP_STATE_FUNCTIONS
	{
		// Token: 0x0400027D RID: 637
		private DSP_ALLOC_FUNC alloc;

		// Token: 0x0400027E RID: 638
		private DSP_REALLOC_FUNC realloc;

		// Token: 0x0400027F RID: 639
		private DSP_FREE_FUNC free;

		// Token: 0x04000280 RID: 640
		private DSP_GETSAMPLERATE_FUNC getsamplerate;

		// Token: 0x04000281 RID: 641
		private DSP_GETBLOCKSIZE_FUNC getblocksize;

		// Token: 0x04000282 RID: 642
		private IntPtr dft;

		// Token: 0x04000283 RID: 643
		private IntPtr pan;

		// Token: 0x04000284 RID: 644
		private DSP_GETSPEAKERMODE_FUNC getspeakermode;

		// Token: 0x04000285 RID: 645
		private DSP_GETCLOCK_FUNC getclock;

		// Token: 0x04000286 RID: 646
		private DSP_GETLISTENERATTRIBUTES_FUNC getlistenerattributes;

		// Token: 0x04000287 RID: 647
		private DSP_LOG_FUNC log;

		// Token: 0x04000288 RID: 648
		private DSP_GETUSERDATA_FUNC getuserdata;
	}
}

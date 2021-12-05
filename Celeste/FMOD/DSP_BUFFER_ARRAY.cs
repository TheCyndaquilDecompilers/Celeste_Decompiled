using System;

namespace FMOD
{
	// Token: 0x0200004C RID: 76
	public struct DSP_BUFFER_ARRAY
	{
		// Token: 0x040001F2 RID: 498
		public int numbuffers;

		// Token: 0x040001F3 RID: 499
		public int[] buffernumchannels;

		// Token: 0x040001F4 RID: 500
		public CHANNELMASK[] bufferchannelmask;

		// Token: 0x040001F5 RID: 501
		public IntPtr[] buffers;

		// Token: 0x040001F6 RID: 502
		public SPEAKERMODE speakermode;
	}
}

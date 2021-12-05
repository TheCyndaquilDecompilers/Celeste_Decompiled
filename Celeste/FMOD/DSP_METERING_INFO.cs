using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200008A RID: 138
	[StructLayout(LayoutKind.Sequential)]
	public class DSP_METERING_INFO
	{
		// Token: 0x04000291 RID: 657
		public int numsamples;

		// Token: 0x04000292 RID: 658
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public float[] peaklevel;

		// Token: 0x04000293 RID: 659
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public float[] rmslevel;

		// Token: 0x04000294 RID: 660
		public short numchannels;
	}
}

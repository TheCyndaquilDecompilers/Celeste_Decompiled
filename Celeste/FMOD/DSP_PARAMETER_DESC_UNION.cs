using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200007D RID: 125
	[StructLayout(LayoutKind.Explicit)]
	public struct DSP_PARAMETER_DESC_UNION
	{
		// Token: 0x0400023F RID: 575
		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_FLOAT floatdesc;

		// Token: 0x04000240 RID: 576
		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_INT intdesc;

		// Token: 0x04000241 RID: 577
		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_BOOL booldesc;

		// Token: 0x04000242 RID: 578
		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_DATA datadesc;
	}
}

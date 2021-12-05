using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000082 RID: 130
	public struct DSP_PARAMETER_3DATTRIBUTES_MULTI
	{
		// Token: 0x04000253 RID: 595
		public int numlisteners;

		// Token: 0x04000254 RID: 596
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public _3D_ATTRIBUTES[] relative;

		// Token: 0x04000255 RID: 597
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public float[] weight;

		// Token: 0x04000256 RID: 598
		public _3D_ATTRIBUTES absolute;
	}
}

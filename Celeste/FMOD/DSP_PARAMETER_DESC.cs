using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200007E RID: 126
	public struct DSP_PARAMETER_DESC
	{
		// Token: 0x04000243 RID: 579
		public DSP_PARAMETER_TYPE type;

		// Token: 0x04000244 RID: 580
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public char[] name;

		// Token: 0x04000245 RID: 581
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public char[] label;

		// Token: 0x04000246 RID: 582
		public string description;

		// Token: 0x04000247 RID: 583
		public DSP_PARAMETER_DESC_UNION desc;
	}
}

using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	// Token: 0x020000CD RID: 205
	[StructLayout(LayoutKind.Explicit)]
	internal struct Union_IntBoolFloatString
	{
		// Token: 0x0400043F RID: 1087
		[FieldOffset(0)]
		public int intvalue;

		// Token: 0x04000440 RID: 1088
		[FieldOffset(0)]
		public bool boolvalue;

		// Token: 0x04000441 RID: 1089
		[FieldOffset(0)]
		public float floatvalue;

		// Token: 0x04000442 RID: 1090
		[FieldOffset(0)]
		public IntPtr stringvalue;
	}
}

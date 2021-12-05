using System;

namespace FMOD
{
	// Token: 0x02000009 RID: 9
	public struct ASYNCREADINFO
	{
		// Token: 0x04000065 RID: 101
		public IntPtr handle;

		// Token: 0x04000066 RID: 102
		public uint offset;

		// Token: 0x04000067 RID: 103
		public uint sizebytes;

		// Token: 0x04000068 RID: 104
		public int priority;

		// Token: 0x04000069 RID: 105
		public IntPtr userdata;

		// Token: 0x0400006A RID: 106
		public IntPtr buffer;

		// Token: 0x0400006B RID: 107
		public uint bytesread;

		// Token: 0x0400006C RID: 108
		public ASYNCREADINFO_DONE_CALLBACK done;
	}
}

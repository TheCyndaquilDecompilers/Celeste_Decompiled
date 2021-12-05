using System;

namespace FMOD.Studio
{
	// Token: 0x020000BF RID: 191
	public struct BANK_INFO
	{
		// Token: 0x040003FC RID: 1020
		public int size;

		// Token: 0x040003FD RID: 1021
		public IntPtr userdata;

		// Token: 0x040003FE RID: 1022
		public int userdatalength;

		// Token: 0x040003FF RID: 1023
		public FILE_OPENCALLBACK opencallback;

		// Token: 0x04000400 RID: 1024
		public FILE_CLOSECALLBACK closecallback;

		// Token: 0x04000401 RID: 1025
		public FILE_READCALLBACK readcallback;

		// Token: 0x04000402 RID: 1026
		public FILE_SEEKCALLBACK seekcallback;
	}
}

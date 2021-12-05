using System;

namespace FMOD
{
	// Token: 0x02000027 RID: 39
	// (Invoke) Token: 0x0600002E RID: 46
	public delegate RESULT FILE_OPENCALLBACK(StringWrapper name, ref uint filesize, ref IntPtr handle, IntPtr userdata);
}

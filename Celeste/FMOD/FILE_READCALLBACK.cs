using System;

namespace FMOD
{
	// Token: 0x02000029 RID: 41
	// (Invoke) Token: 0x06000036 RID: 54
	public delegate RESULT FILE_READCALLBACK(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata);
}

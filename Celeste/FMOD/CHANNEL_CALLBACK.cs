using System;

namespace FMOD
{
	// Token: 0x02000023 RID: 35
	// (Invoke) Token: 0x0600001E RID: 30
	public delegate RESULT CHANNEL_CALLBACK(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2);
}

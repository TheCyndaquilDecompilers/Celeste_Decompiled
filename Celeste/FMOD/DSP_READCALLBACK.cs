using System;

namespace FMOD
{
	// Token: 0x02000054 RID: 84
	// (Invoke) Token: 0x06000319 RID: 793
	public delegate RESULT DSP_READCALLBACK(ref DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels);
}

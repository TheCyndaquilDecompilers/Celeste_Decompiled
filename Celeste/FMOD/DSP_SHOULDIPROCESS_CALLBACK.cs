using System;

namespace FMOD
{
	// Token: 0x02000055 RID: 85
	// (Invoke) Token: 0x0600031D RID: 797
	public delegate RESULT DSP_SHOULDIPROCESS_CALLBACK(ref DSP_STATE dsp_state, bool inputsidle, uint length, CHANNELMASK inmask, int inchannels, SPEAKERMODE speakermode);
}

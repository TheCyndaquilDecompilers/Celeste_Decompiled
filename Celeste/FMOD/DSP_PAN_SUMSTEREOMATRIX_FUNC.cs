using System;

namespace FMOD
{
	// Token: 0x0200006F RID: 111
	// (Invoke) Token: 0x06000385 RID: 901
	public delegate RESULT DSP_PAN_SUMSTEREOMATRIX_FUNC(ref DSP_STATE dsp_state, int sourceSpeakerMode, float pan, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
}

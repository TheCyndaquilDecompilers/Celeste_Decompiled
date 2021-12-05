using System;

namespace FMOD
{
	// Token: 0x02000072 RID: 114
	// (Invoke) Token: 0x06000391 RID: 913
	public delegate RESULT DSP_PAN_SUMSTEREOTOSURROUNDMATRIX_FUNC(ref DSP_STATE dsp_state, int targetSpeakerMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
}

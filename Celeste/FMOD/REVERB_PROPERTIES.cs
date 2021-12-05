using System;

namespace FMOD
{
	// Token: 0x02000039 RID: 57
	public struct REVERB_PROPERTIES
	{
		// Token: 0x06000056 RID: 86 RVA: 0x000027B0 File Offset: 0x000009B0
		public REVERB_PROPERTIES(float decayTime, float earlyDelay, float lateDelay, float hfReference, float hfDecayRatio, float diffusion, float density, float lowShelfFrequency, float lowShelfGain, float highCut, float earlyLateMix, float wetLevel)
		{
			this.DecayTime = decayTime;
			this.EarlyDelay = earlyDelay;
			this.LateDelay = lateDelay;
			this.HFReference = hfReference;
			this.HFDecayRatio = hfDecayRatio;
			this.Diffusion = diffusion;
			this.Density = density;
			this.LowShelfFrequency = lowShelfFrequency;
			this.LowShelfGain = lowShelfGain;
			this.HighCut = highCut;
			this.EarlyLateMix = earlyLateMix;
			this.WetLevel = wetLevel;
		}

		// Token: 0x040001C7 RID: 455
		public float DecayTime;

		// Token: 0x040001C8 RID: 456
		public float EarlyDelay;

		// Token: 0x040001C9 RID: 457
		public float LateDelay;

		// Token: 0x040001CA RID: 458
		public float HFReference;

		// Token: 0x040001CB RID: 459
		public float HFDecayRatio;

		// Token: 0x040001CC RID: 460
		public float Diffusion;

		// Token: 0x040001CD RID: 461
		public float Density;

		// Token: 0x040001CE RID: 462
		public float LowShelfFrequency;

		// Token: 0x040001CF RID: 463
		public float LowShelfGain;

		// Token: 0x040001D0 RID: 464
		public float HighCut;

		// Token: 0x040001D1 RID: 465
		public float EarlyLateMix;

		// Token: 0x040001D2 RID: 466
		public float WetLevel;
	}
}

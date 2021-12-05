using System;

namespace FMOD
{
	// Token: 0x0200003B RID: 59
	public struct ADVANCEDSETTINGS
	{
		// Token: 0x040001D3 RID: 467
		public int cbSize;

		// Token: 0x040001D4 RID: 468
		public int maxMPEGCodecs;

		// Token: 0x040001D5 RID: 469
		public int maxADPCMCodecs;

		// Token: 0x040001D6 RID: 470
		public int maxXMACodecs;

		// Token: 0x040001D7 RID: 471
		public int maxVorbisCodecs;

		// Token: 0x040001D8 RID: 472
		public int maxAT9Codecs;

		// Token: 0x040001D9 RID: 473
		public int maxFADPCMCodecs;

		// Token: 0x040001DA RID: 474
		public int maxPCMCodecs;

		// Token: 0x040001DB RID: 475
		public int ASIONumChannels;

		// Token: 0x040001DC RID: 476
		public IntPtr ASIOChannelList;

		// Token: 0x040001DD RID: 477
		public IntPtr ASIOSpeakerList;

		// Token: 0x040001DE RID: 478
		public float HRTFMinAngle;

		// Token: 0x040001DF RID: 479
		public float HRTFMaxAngle;

		// Token: 0x040001E0 RID: 480
		public float HRTFFreq;

		// Token: 0x040001E1 RID: 481
		public float vol0virtualvol;

		// Token: 0x040001E2 RID: 482
		public uint defaultDecodeBufferSize;

		// Token: 0x040001E3 RID: 483
		public ushort profilePort;

		// Token: 0x040001E4 RID: 484
		public uint geometryMaxFadeTime;

		// Token: 0x040001E5 RID: 485
		public float distanceFilterCenterFreq;

		// Token: 0x040001E6 RID: 486
		public int reverb3Dinstance;

		// Token: 0x040001E7 RID: 487
		public int DSPBufferPoolSize;

		// Token: 0x040001E8 RID: 488
		public uint stackSizeStream;

		// Token: 0x040001E9 RID: 489
		public uint stackSizeNonBlocking;

		// Token: 0x040001EA RID: 490
		public uint stackSizeMixer;

		// Token: 0x040001EB RID: 491
		public DSP_RESAMPLER resamplerMethod;

		// Token: 0x040001EC RID: 492
		public uint commandQueueSize;

		// Token: 0x040001ED RID: 493
		public uint randomSeed;
	}
}

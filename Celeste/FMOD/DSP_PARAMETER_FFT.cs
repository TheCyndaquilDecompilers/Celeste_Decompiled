using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000084 RID: 132
	public struct DSP_PARAMETER_FFT
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00004B44 File Offset: 0x00002D44
		public float[][] spectrum
		{
			get
			{
				float[][] array = new float[this.numchannels][];
				for (int i = 0; i < this.numchannels; i++)
				{
					array[i] = new float[this.length];
					Marshal.Copy(this.spectrum_internal[i], array[i], 0, this.length);
				}
				return array;
			}
		}

		// Token: 0x04000258 RID: 600
		public int length;

		// Token: 0x04000259 RID: 601
		public int numchannels;

		// Token: 0x0400025A RID: 602
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private IntPtr[] spectrum_internal;
	}
}

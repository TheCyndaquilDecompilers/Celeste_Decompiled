using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000048 RID: 72
	public class DSPConnection : HandleBase
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x00004874 File Offset: 0x00002A74
		public RESULT getInput(out DSP input)
		{
			input = null;
			IntPtr raw;
			RESULT result = DSPConnection.FMOD_DSPConnection_GetInput(this.rawPtr, out raw);
			input = new DSP(raw);
			return result;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000489C File Offset: 0x00002A9C
		public RESULT getOutput(out DSP output)
		{
			output = null;
			IntPtr raw;
			RESULT result = DSPConnection.FMOD_DSPConnection_GetOutput(this.rawPtr, out raw);
			output = new DSP(raw);
			return result;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x000048C1 File Offset: 0x00002AC1
		public RESULT setMix(float volume)
		{
			return DSPConnection.FMOD_DSPConnection_SetMix(this.rawPtr, volume);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000048CF File Offset: 0x00002ACF
		public RESULT getMix(out float volume)
		{
			return DSPConnection.FMOD_DSPConnection_GetMix(this.rawPtr, out volume);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000048DD File Offset: 0x00002ADD
		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop = 0)
		{
			return DSPConnection.FMOD_DSPConnection_SetMixMatrix(this.rawPtr, matrix, outchannels, inchannels, inchannel_hop);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000048EF File Offset: 0x00002AEF
		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop = 0)
		{
			return DSPConnection.FMOD_DSPConnection_GetMixMatrix(this.rawPtr, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00004901 File Offset: 0x00002B01
		public RESULT getType(out DSPCONNECTION_TYPE type)
		{
			return DSPConnection.FMOD_DSPConnection_GetType(this.rawPtr, out type);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000490F File Offset: 0x00002B0F
		public RESULT setUserData(IntPtr userdata)
		{
			return DSPConnection.FMOD_DSPConnection_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000491D File Offset: 0x00002B1D
		public RESULT getUserData(out IntPtr userdata)
		{
			return DSPConnection.FMOD_DSPConnection_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060002C0 RID: 704
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_GetInput(IntPtr dspconnection, out IntPtr input);

		// Token: 0x060002C1 RID: 705
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_GetOutput(IntPtr dspconnection, out IntPtr output);

		// Token: 0x060002C2 RID: 706
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_SetMix(IntPtr dspconnection, float volume);

		// Token: 0x060002C3 RID: 707
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_GetMix(IntPtr dspconnection, out float volume);

		// Token: 0x060002C4 RID: 708
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_SetMixMatrix(IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		// Token: 0x060002C5 RID: 709
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_GetMixMatrix(IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		// Token: 0x060002C6 RID: 710
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_GetType(IntPtr dspconnection, out DSPCONNECTION_TYPE type);

		// Token: 0x060002C7 RID: 711
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata);

		// Token: 0x060002C8 RID: 712
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSPConnection_GetUserData(IntPtr dspconnection, out IntPtr userdata);

		// Token: 0x060002C9 RID: 713 RVA: 0x00003A27 File Offset: 0x00001C27
		public DSPConnection(IntPtr raw) : base(raw)
		{
		}
	}
}

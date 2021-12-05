using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x02000047 RID: 71
	public class DSP : HandleBase
	{
		// Token: 0x06000266 RID: 614 RVA: 0x000044E0 File Offset: 0x000026E0
		public RESULT release()
		{
			RESULT result = DSP.FMOD_DSP_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000044FC File Offset: 0x000026FC
		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = DSP.FMOD_DSP_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00004524 File Offset: 0x00002724
		public RESULT addInput(DSP target, out DSPConnection connection, DSPCONNECTION_TYPE type = DSPCONNECTION_TYPE.STANDARD)
		{
			connection = null;
			IntPtr raw;
			RESULT result = DSP.FMOD_DSP_AddInput(this.rawPtr, target.getRaw(), out raw, type);
			connection = new DSPConnection(raw);
			return result;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00004550 File Offset: 0x00002750
		public RESULT disconnectFrom(DSP target, DSPConnection connection = null)
		{
			return DSP.FMOD_DSP_DisconnectFrom(this.rawPtr, target.getRaw(), (connection != null) ? connection.getRaw() : ((IntPtr)0));
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00004576 File Offset: 0x00002776
		public RESULT disconnectAll(bool inputs, bool outputs)
		{
			return DSP.FMOD_DSP_DisconnectAll(this.rawPtr, inputs, outputs);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00004585 File Offset: 0x00002785
		public RESULT getNumInputs(out int numinputs)
		{
			return DSP.FMOD_DSP_GetNumInputs(this.rawPtr, out numinputs);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00004593 File Offset: 0x00002793
		public RESULT getNumOutputs(out int numoutputs)
		{
			return DSP.FMOD_DSP_GetNumOutputs(this.rawPtr, out numoutputs);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000045A4 File Offset: 0x000027A4
		public RESULT getInput(int index, out DSP input, out DSPConnection inputconnection)
		{
			input = null;
			inputconnection = null;
			IntPtr raw;
			IntPtr raw2;
			RESULT result = DSP.FMOD_DSP_GetInput(this.rawPtr, index, out raw, out raw2);
			input = new DSP(raw);
			inputconnection = new DSPConnection(raw2);
			return result;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x000045D8 File Offset: 0x000027D8
		public RESULT getOutput(int index, out DSP output, out DSPConnection outputconnection)
		{
			output = null;
			outputconnection = null;
			IntPtr raw;
			IntPtr raw2;
			RESULT result = DSP.FMOD_DSP_GetOutput(this.rawPtr, index, out raw, out raw2);
			output = new DSP(raw);
			outputconnection = new DSPConnection(raw2);
			return result;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000460B File Offset: 0x0000280B
		public RESULT setActive(bool active)
		{
			return DSP.FMOD_DSP_SetActive(this.rawPtr, active);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00004619 File Offset: 0x00002819
		public RESULT getActive(out bool active)
		{
			return DSP.FMOD_DSP_GetActive(this.rawPtr, out active);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00004627 File Offset: 0x00002827
		public RESULT setBypass(bool bypass)
		{
			return DSP.FMOD_DSP_SetBypass(this.rawPtr, bypass);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00004635 File Offset: 0x00002835
		public RESULT getBypass(out bool bypass)
		{
			return DSP.FMOD_DSP_GetBypass(this.rawPtr, out bypass);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00004643 File Offset: 0x00002843
		public RESULT setWetDryMix(float prewet, float postwet, float dry)
		{
			return DSP.FMOD_DSP_SetWetDryMix(this.rawPtr, prewet, postwet, dry);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00004653 File Offset: 0x00002853
		public RESULT getWetDryMix(out float prewet, out float postwet, out float dry)
		{
			return DSP.FMOD_DSP_GetWetDryMix(this.rawPtr, out prewet, out postwet, out dry);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00004663 File Offset: 0x00002863
		public RESULT setChannelFormat(CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode)
		{
			return DSP.FMOD_DSP_SetChannelFormat(this.rawPtr, channelmask, numchannels, source_speakermode);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00004673 File Offset: 0x00002873
		public RESULT getChannelFormat(out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode)
		{
			return DSP.FMOD_DSP_GetChannelFormat(this.rawPtr, out channelmask, out numchannels, out source_speakermode);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00004683 File Offset: 0x00002883
		public RESULT getOutputChannelFormat(CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode)
		{
			return DSP.FMOD_DSP_GetOutputChannelFormat(this.rawPtr, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00004699 File Offset: 0x00002899
		public RESULT reset()
		{
			return DSP.FMOD_DSP_Reset(this.rawPtr);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x000046A6 File Offset: 0x000028A6
		public RESULT setParameterFloat(int index, float value)
		{
			return DSP.FMOD_DSP_SetParameterFloat(this.rawPtr, index, value);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x000046B5 File Offset: 0x000028B5
		public RESULT setParameterInt(int index, int value)
		{
			return DSP.FMOD_DSP_SetParameterInt(this.rawPtr, index, value);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x000046C4 File Offset: 0x000028C4
		public RESULT setParameterBool(int index, bool value)
		{
			return DSP.FMOD_DSP_SetParameterBool(this.rawPtr, index, value);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x000046D3 File Offset: 0x000028D3
		public RESULT setParameterData(int index, byte[] data)
		{
			return DSP.FMOD_DSP_SetParameterData(this.rawPtr, index, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), (uint)data.Length);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000046EC File Offset: 0x000028EC
		public RESULT getParameterFloat(int index, out float value)
		{
			IntPtr zero = IntPtr.Zero;
			return DSP.FMOD_DSP_GetParameterFloat(this.rawPtr, index, out value, zero, 0);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00004710 File Offset: 0x00002910
		public RESULT getParameterInt(int index, out int value)
		{
			IntPtr zero = IntPtr.Zero;
			return DSP.FMOD_DSP_GetParameterInt(this.rawPtr, index, out value, zero, 0);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00004732 File Offset: 0x00002932
		public RESULT getParameterBool(int index, out bool value)
		{
			return DSP.FMOD_DSP_GetParameterBool(this.rawPtr, index, out value, IntPtr.Zero, 0);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00004747 File Offset: 0x00002947
		public RESULT getParameterData(int index, out IntPtr data, out uint length)
		{
			return DSP.FMOD_DSP_GetParameterData(this.rawPtr, index, out data, out length, IntPtr.Zero, 0);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000475D File Offset: 0x0000295D
		public RESULT getNumParameters(out int numparams)
		{
			return DSP.FMOD_DSP_GetNumParameters(this.rawPtr, out numparams);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000476C File Offset: 0x0000296C
		public RESULT getParameterInfo(int index, out DSP_PARAMETER_DESC desc)
		{
			IntPtr ptr;
			RESULT result = DSP.FMOD_DSP_GetParameterInfo(this.rawPtr, index, out ptr);
			if (result == RESULT.OK)
			{
				desc = (DSP_PARAMETER_DESC)Marshal.PtrToStructure(ptr, typeof(DSP_PARAMETER_DESC));
				return result;
			}
			desc = default(DSP_PARAMETER_DESC);
			return result;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000047AD File Offset: 0x000029AD
		public RESULT getDataParameterIndex(int datatype, out int index)
		{
			return DSP.FMOD_DSP_GetDataParameterIndex(this.rawPtr, datatype, out index);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x000047BC File Offset: 0x000029BC
		public RESULT showConfigDialog(IntPtr hwnd, bool show)
		{
			return DSP.FMOD_DSP_ShowConfigDialog(this.rawPtr, hwnd, show);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000047CC File Offset: 0x000029CC
		public RESULT getInfo(StringBuilder name, out uint version, out int channels, out int configwidth, out int configheight)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(32);
			RESULT result = DSP.FMOD_DSP_GetInfo(this.rawPtr, intPtr, out version, out channels, out configwidth, out configheight);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00004800 File Offset: 0x00002A00
		public RESULT getType(out DSP_TYPE type)
		{
			return DSP.FMOD_DSP_GetType(this.rawPtr, out type);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000480E File Offset: 0x00002A0E
		public RESULT getIdle(out bool idle)
		{
			return DSP.FMOD_DSP_GetIdle(this.rawPtr, out idle);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000481C File Offset: 0x00002A1C
		public RESULT setUserData(IntPtr userdata)
		{
			return DSP.FMOD_DSP_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000482A File Offset: 0x00002A2A
		public RESULT getUserData(out IntPtr userdata)
		{
			return DSP.FMOD_DSP_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00004838 File Offset: 0x00002A38
		public RESULT setMeteringEnabled(bool inputEnabled, bool outputEnabled)
		{
			return DSP.FMOD_DSP_SetMeteringEnabled(this.rawPtr, inputEnabled, outputEnabled);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00004847 File Offset: 0x00002A47
		public RESULT getMeteringEnabled(out bool inputEnabled, out bool outputEnabled)
		{
			return DSP.FMOD_DSP_GetMeteringEnabled(this.rawPtr, out inputEnabled, out outputEnabled);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00004856 File Offset: 0x00002A56
		public RESULT getMeteringInfo(DSP_METERING_INFO inputInfo, DSP_METERING_INFO outputInfo)
		{
			return DSP.FMOD_DSP_GetMeteringInfo(this.rawPtr, inputInfo, outputInfo);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00004865 File Offset: 0x00002A65
		public RESULT getCPUUsage(out uint exclusive, out uint inclusive)
		{
			return DSP.FMOD_DSP_GetCPUUsage(this.rawPtr, out exclusive, out inclusive);
		}

		// Token: 0x0600028E RID: 654
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_Release(IntPtr dsp);

		// Token: 0x0600028F RID: 655
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetSystemObject(IntPtr dsp, out IntPtr system);

		// Token: 0x06000290 RID: 656
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_AddInput(IntPtr dsp, IntPtr target, out IntPtr connection, DSPCONNECTION_TYPE type);

		// Token: 0x06000291 RID: 657
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection);

		// Token: 0x06000292 RID: 658
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs);

		// Token: 0x06000293 RID: 659
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetNumInputs(IntPtr dsp, out int numinputs);

		// Token: 0x06000294 RID: 660
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetNumOutputs(IntPtr dsp, out int numoutputs);

		// Token: 0x06000295 RID: 661
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetInput(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);

		// Token: 0x06000296 RID: 662
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetOutput(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);

		// Token: 0x06000297 RID: 663
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetActive(IntPtr dsp, bool active);

		// Token: 0x06000298 RID: 664
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetActive(IntPtr dsp, out bool active);

		// Token: 0x06000299 RID: 665
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetBypass(IntPtr dsp, bool bypass);

		// Token: 0x0600029A RID: 666
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetBypass(IntPtr dsp, out bool bypass);

		// Token: 0x0600029B RID: 667
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry);

		// Token: 0x0600029C RID: 668
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry);

		// Token: 0x0600029D RID: 669
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetChannelFormat(IntPtr dsp, CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode);

		// Token: 0x0600029E RID: 670
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetChannelFormat(IntPtr dsp, out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode);

		// Token: 0x0600029F RID: 671
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetOutputChannelFormat(IntPtr dsp, CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode);

		// Token: 0x060002A0 RID: 672
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_Reset(IntPtr dsp);

		// Token: 0x060002A1 RID: 673
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetParameterFloat(IntPtr dsp, int index, float value);

		// Token: 0x060002A2 RID: 674
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetParameterInt(IntPtr dsp, int index, int value);

		// Token: 0x060002A3 RID: 675
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetParameterBool(IntPtr dsp, int index, bool value);

		// Token: 0x060002A4 RID: 676
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetParameterData(IntPtr dsp, int index, IntPtr data, uint length);

		// Token: 0x060002A5 RID: 677
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetParameterFloat(IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen);

		// Token: 0x060002A6 RID: 678
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetParameterInt(IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen);

		// Token: 0x060002A7 RID: 679
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetParameterBool(IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen);

		// Token: 0x060002A8 RID: 680
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);

		// Token: 0x060002A9 RID: 681
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetNumParameters(IntPtr dsp, out int numparams);

		// Token: 0x060002AA RID: 682
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetParameterInfo(IntPtr dsp, int index, out IntPtr desc);

		// Token: 0x060002AB RID: 683
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetDataParameterIndex(IntPtr dsp, int datatype, out int index);

		// Token: 0x060002AC RID: 684
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show);

		// Token: 0x060002AD RID: 685
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetInfo(IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight);

		// Token: 0x060002AE RID: 686
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetType(IntPtr dsp, out DSP_TYPE type);

		// Token: 0x060002AF RID: 687
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetIdle(IntPtr dsp, out bool idle);

		// Token: 0x060002B0 RID: 688
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_SetUserData(IntPtr dsp, IntPtr userdata);

		// Token: 0x060002B1 RID: 689
		[DllImport("fmod")]
		private static extern RESULT FMOD_DSP_GetUserData(IntPtr dsp, out IntPtr userdata);

		// Token: 0x060002B2 RID: 690
		[DllImport("fmod")]
		public static extern RESULT FMOD_DSP_SetMeteringEnabled(IntPtr dsp, bool inputEnabled, bool outputEnabled);

		// Token: 0x060002B3 RID: 691
		[DllImport("fmod")]
		public static extern RESULT FMOD_DSP_GetMeteringEnabled(IntPtr dsp, out bool inputEnabled, out bool outputEnabled);

		// Token: 0x060002B4 RID: 692
		[DllImport("fmod")]
		public static extern RESULT FMOD_DSP_GetMeteringInfo(IntPtr dsp, [Out] DSP_METERING_INFO inputInfo, [Out] DSP_METERING_INFO outputInfo);

		// Token: 0x060002B5 RID: 693
		[DllImport("fmod")]
		public static extern RESULT FMOD_DSP_GetCPUUsage(IntPtr dsp, out uint exclusive, out uint inclusive);

		// Token: 0x060002B6 RID: 694 RVA: 0x00003A27 File Offset: 0x00001C27
		public DSP(IntPtr raw) : base(raw)
		{
		}
	}
}

using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000085 RID: 133
	public struct DSP_DESCRIPTION
	{
		// Token: 0x0400025B RID: 603
		public uint pluginsdkversion;

		// Token: 0x0400025C RID: 604
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public char[] name;

		// Token: 0x0400025D RID: 605
		public uint version;

		// Token: 0x0400025E RID: 606
		public int numinputbuffers;

		// Token: 0x0400025F RID: 607
		public int numoutputbuffers;

		// Token: 0x04000260 RID: 608
		public DSP_CREATECALLBACK create;

		// Token: 0x04000261 RID: 609
		public DSP_RELEASECALLBACK release;

		// Token: 0x04000262 RID: 610
		public DSP_RESETCALLBACK reset;

		// Token: 0x04000263 RID: 611
		public DSP_READCALLBACK read;

		// Token: 0x04000264 RID: 612
		public DSP_PROCESS_CALLBACK process;

		// Token: 0x04000265 RID: 613
		public DSP_SETPOSITIONCALLBACK setposition;

		// Token: 0x04000266 RID: 614
		public int numparameters;

		// Token: 0x04000267 RID: 615
		public IntPtr paramdesc;

		// Token: 0x04000268 RID: 616
		public DSP_SETPARAM_FLOAT_CALLBACK setparameterfloat;

		// Token: 0x04000269 RID: 617
		public DSP_SETPARAM_INT_CALLBACK setparameterint;

		// Token: 0x0400026A RID: 618
		public DSP_SETPARAM_BOOL_CALLBACK setparameterbool;

		// Token: 0x0400026B RID: 619
		public DSP_SETPARAM_DATA_CALLBACK setparameterdata;

		// Token: 0x0400026C RID: 620
		public DSP_GETPARAM_FLOAT_CALLBACK getparameterfloat;

		// Token: 0x0400026D RID: 621
		public DSP_GETPARAM_INT_CALLBACK getparameterint;

		// Token: 0x0400026E RID: 622
		public DSP_GETPARAM_BOOL_CALLBACK getparameterbool;

		// Token: 0x0400026F RID: 623
		public DSP_GETPARAM_DATA_CALLBACK getparameterdata;

		// Token: 0x04000270 RID: 624
		public DSP_SHOULDIPROCESS_CALLBACK shouldiprocess;

		// Token: 0x04000271 RID: 625
		public IntPtr userdata;

		// Token: 0x04000272 RID: 626
		public DSP_SYSTEM_REGISTER_CALLBACK sys_register;

		// Token: 0x04000273 RID: 627
		public DSP_SYSTEM_DEREGISTER_CALLBACK sys_deregister;

		// Token: 0x04000274 RID: 628
		public DSP_SYSTEM_MIX_CALLBACK sys_mix;
	}
}

using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200001D RID: 29
	public struct ERRORCALLBACK_INFO
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002720 File Offset: 0x00000920
		public string functionname
		{
			get
			{
				return Marshal.PtrToStringAnsi(this.functionname_internal);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000F RID: 15 RVA: 0x0000272D File Offset: 0x0000092D
		public string functionparams
		{
			get
			{
				return Marshal.PtrToStringAnsi(this.functionparams_internal);
			}
		}

		// Token: 0x04000159 RID: 345
		public RESULT result;

		// Token: 0x0400015A RID: 346
		public ERRORCALLBACK_INSTANCETYPE instancetype;

		// Token: 0x0400015B RID: 347
		public IntPtr instance;

		// Token: 0x0400015C RID: 348
		private IntPtr functionname_internal;

		// Token: 0x0400015D RID: 349
		private IntPtr functionparams_internal;
	}
}

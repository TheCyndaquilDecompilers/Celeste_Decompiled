using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x02000035 RID: 53
	public struct TAG
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000027A3 File Offset: 0x000009A3
		public string name
		{
			get
			{
				return Marshal.PtrToStringAnsi(this.name_internal);
			}
		}

		// Token: 0x04000192 RID: 402
		public TAGTYPE type;

		// Token: 0x04000193 RID: 403
		public TAGDATATYPE datatype;

		// Token: 0x04000194 RID: 404
		private IntPtr name_internal;

		// Token: 0x04000195 RID: 405
		public IntPtr data;

		// Token: 0x04000196 RID: 406
		public uint datalen;

		// Token: 0x04000197 RID: 407
		public bool updated;
	}
}

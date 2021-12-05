using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000C7 RID: 199
	public class SOUND_INFO
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x00004F44 File Offset: 0x00003144
		public string name
		{
			get
			{
				if ((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT || this.name_or_data == null)
				{
					return null;
				}
				int num = Array.IndexOf<byte>(this.name_or_data, 0);
				if (num > 0)
				{
					return Encoding.UTF8.GetString(this.name_or_data, 0, num);
				}
				return null;
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00004F90 File Offset: 0x00003190
		~SOUND_INFO()
		{
			if (this.exinfo.inclusionlist != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.exinfo.inclusionlist);
			}
		}

		// Token: 0x04000421 RID: 1057
		public byte[] name_or_data;

		// Token: 0x04000422 RID: 1058
		public MODE mode;

		// Token: 0x04000423 RID: 1059
		public CREATESOUNDEXINFO exinfo;

		// Token: 0x04000424 RID: 1060
		public int subsoundindex;
	}
}

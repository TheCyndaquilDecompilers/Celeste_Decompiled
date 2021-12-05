using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	// Token: 0x020000C8 RID: 200
	public struct SOUND_INFO_INTERNAL
	{
		// Token: 0x060003A5 RID: 933 RVA: 0x00004FE0 File Offset: 0x000031E0
		public void assign(out SOUND_INFO publicInfo)
		{
			publicInfo = new SOUND_INFO();
			publicInfo.mode = this.mode;
			publicInfo.exinfo = this.exinfo;
			publicInfo.exinfo.inclusionlist = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
			Marshal.WriteInt32(publicInfo.exinfo.inclusionlist, this.subsoundindex);
			publicInfo.exinfo.inclusionlistnum = 1;
			publicInfo.subsoundindex = this.subsoundindex;
			if (this.name_or_data != IntPtr.Zero)
			{
				int num;
				int num2;
				if ((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT)
				{
					publicInfo.mode = ((publicInfo.mode & ~MODE.OPENMEMORY_POINT) | MODE.OPENMEMORY);
					num = (int)this.exinfo.fileoffset;
					publicInfo.exinfo.fileoffset = 0U;
					num2 = (int)this.exinfo.length;
				}
				else
				{
					num = 0;
					num2 = MarshallingHelper.stringLengthUtf8(this.name_or_data) + 1;
				}
				publicInfo.name_or_data = new byte[num2];
				Marshal.Copy(new IntPtr(this.name_or_data.ToInt64() + (long)num), publicInfo.name_or_data, 0, num2);
				return;
			}
			publicInfo.name_or_data = null;
		}

		// Token: 0x04000425 RID: 1061
		private IntPtr name_or_data;

		// Token: 0x04000426 RID: 1062
		private MODE mode;

		// Token: 0x04000427 RID: 1063
		private CREATESOUNDEXINFO exinfo;

		// Token: 0x04000428 RID: 1064
		private int subsoundindex;
	}
}

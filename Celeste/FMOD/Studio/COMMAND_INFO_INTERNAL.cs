using System;

namespace FMOD.Studio
{
	// Token: 0x020000CC RID: 204
	internal struct COMMAND_INFO_INTERNAL
	{
		// Token: 0x060003A7 RID: 935 RVA: 0x000051B4 File Offset: 0x000033B4
		public COMMAND_INFO createPublic()
		{
			return new COMMAND_INFO
			{
				commandname = MarshallingHelper.stringFromNativeUtf8(this.commandname),
				parentcommandindex = this.parentcommandindex,
				framenumber = this.framenumber,
				frametime = this.frametime,
				instancetype = this.instancetype,
				outputtype = this.outputtype,
				instancehandle = this.instancehandle,
				outputhandle = this.outputhandle
			};
		}

		// Token: 0x04000437 RID: 1079
		public IntPtr commandname;

		// Token: 0x04000438 RID: 1080
		public int parentcommandindex;

		// Token: 0x04000439 RID: 1081
		public int framenumber;

		// Token: 0x0400043A RID: 1082
		public float frametime;

		// Token: 0x0400043B RID: 1083
		public INSTANCETYPE instancetype;

		// Token: 0x0400043C RID: 1084
		public INSTANCETYPE outputtype;

		// Token: 0x0400043D RID: 1085
		public uint instancehandle;

		// Token: 0x0400043E RID: 1086
		public uint outputhandle;
	}
}

using System;

namespace FMOD.Studio
{
	// Token: 0x020000DB RID: 219
	public struct COMMAND_INFO
	{
		// Token: 0x04000482 RID: 1154
		public string commandname;

		// Token: 0x04000483 RID: 1155
		public int parentcommandindex;

		// Token: 0x04000484 RID: 1156
		public int framenumber;

		// Token: 0x04000485 RID: 1157
		public float frametime;

		// Token: 0x04000486 RID: 1158
		public INSTANCETYPE instancetype;

		// Token: 0x04000487 RID: 1159
		public INSTANCETYPE outputtype;

		// Token: 0x04000488 RID: 1160
		public uint instancehandle;

		// Token: 0x04000489 RID: 1161
		public uint outputhandle;
	}
}

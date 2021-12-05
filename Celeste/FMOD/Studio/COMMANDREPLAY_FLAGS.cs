using System;

namespace FMOD.Studio
{
	// Token: 0x020000D1 RID: 209
	[Flags]
	public enum COMMANDREPLAY_FLAGS : uint
	{
		// Token: 0x04000453 RID: 1107
		NORMAL = 0U,
		// Token: 0x04000454 RID: 1108
		SKIP_CLEANUP = 1U,
		// Token: 0x04000455 RID: 1109
		FAST_FORWARD = 2U,
		// Token: 0x04000456 RID: 1110
		SKIP_BANK_LOAD = 4U
	}
}

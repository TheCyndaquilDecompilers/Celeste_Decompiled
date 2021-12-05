using System;

namespace FMOD.Studio
{
	// Token: 0x020000C0 RID: 192
	[Flags]
	public enum SYSTEM_CALLBACK_TYPE : uint
	{
		// Token: 0x04000404 RID: 1028
		PREUPDATE = 1U,
		// Token: 0x04000405 RID: 1029
		POSTUPDATE = 2U,
		// Token: 0x04000406 RID: 1030
		BANK_UNLOAD = 4U,
		// Token: 0x04000407 RID: 1031
		ALL = 4294967295U
	}
}

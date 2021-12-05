using System;

namespace FMOD.Studio
{
	// Token: 0x020000D8 RID: 216
	// (Invoke) Token: 0x060003B1 RID: 945
	public delegate RESULT COMMANDREPLAY_LOAD_BANK_CALLBACK(IntPtr replay, ref Guid guid, StringWrapper bankFilename, LOAD_BANK_FLAGS flags, out IntPtr bank, IntPtr userdata);
}

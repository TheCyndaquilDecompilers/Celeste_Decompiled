using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200003F RID: 63
	public class Debug
	{
		// Token: 0x06000079 RID: 121 RVA: 0x00002FEF File Offset: 0x000011EF
		public static RESULT Initialize(DEBUG_FLAGS flags, DEBUG_MODE mode = DEBUG_MODE.TTY, DEBUG_CALLBACK callback = null, string filename = null)
		{
			return Debug.FMOD_Debug_Initialize(flags, mode, callback, filename);
		}

		// Token: 0x0600007A RID: 122
		[DllImport("fmod")]
		private static extern RESULT FMOD_Debug_Initialize(DEBUG_FLAGS flags, DEBUG_MODE mode, DEBUG_CALLBACK callback, string filename);
	}
}

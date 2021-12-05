using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200003D RID: 61
	public class Factory
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00002F9C File Offset: 0x0000119C
		public static RESULT System_Create(out System system)
		{
			system = null;
			IntPtr raw = 0;
			RESULT result = Factory.FMOD_System_Create(out raw);
			if (result != RESULT.OK)
			{
				return result;
			}
			system = new System(raw);
			return result;
		}

		// Token: 0x06000071 RID: 113
		[DllImport("fmod")]
		private static extern RESULT FMOD_System_Create(out IntPtr system);
	}
}

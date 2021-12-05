using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200003E RID: 62
	public class Memory
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00002FCC File Offset: 0x000011CC
		public static RESULT Initialize(IntPtr poolmem, int poollen, MEMORY_ALLOC_CALLBACK useralloc, MEMORY_REALLOC_CALLBACK userrealloc, MEMORY_FREE_CALLBACK userfree, MEMORY_TYPE memtypeflags = MEMORY_TYPE.ALL)
		{
			return Memory.FMOD_Memory_Initialize(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002FDB File Offset: 0x000011DB
		public static RESULT GetStats(out int currentalloced, out int maxalloced)
		{
			return Memory.GetStats(out currentalloced, out maxalloced, false);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002FE5 File Offset: 0x000011E5
		public static RESULT GetStats(out int currentalloced, out int maxalloced, bool blocking)
		{
			return Memory.FMOD_Memory_GetStats(out currentalloced, out maxalloced, blocking);
		}

		// Token: 0x06000076 RID: 118
		[DllImport("fmod")]
		private static extern RESULT FMOD_Memory_Initialize(IntPtr poolmem, int poollen, MEMORY_ALLOC_CALLBACK useralloc, MEMORY_REALLOC_CALLBACK userrealloc, MEMORY_FREE_CALLBACK userfree, MEMORY_TYPE memtypeflags);

		// Token: 0x06000077 RID: 119
		[DllImport("fmod")]
		private static extern RESULT FMOD_Memory_GetStats(out int currentalloced, out int maxalloced, bool blocking);
	}
}

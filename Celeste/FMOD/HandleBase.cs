using System;

namespace FMOD
{
	// Token: 0x02000040 RID: 64
	public class HandleBase
	{
		// Token: 0x0600007C RID: 124 RVA: 0x00002FFA File Offset: 0x000011FA
		public HandleBase(IntPtr newPtr)
		{
			this.rawPtr = newPtr;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003009 File Offset: 0x00001209
		public bool isValid()
		{
			return this.rawPtr != IntPtr.Zero;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000301B File Offset: 0x0000121B
		public IntPtr getRaw()
		{
			return this.rawPtr;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003023 File Offset: 0x00001223
		public override bool Equals(object obj)
		{
			return this.Equals(obj as HandleBase);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003031 File Offset: 0x00001231
		public bool Equals(HandleBase p)
		{
			return p != null && this.rawPtr == p.rawPtr;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003049 File Offset: 0x00001249
		public override int GetHashCode()
		{
			return this.rawPtr.ToInt32();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003056 File Offset: 0x00001256
		public static bool operator ==(HandleBase a, HandleBase b)
		{
			return a == b || (a != null && b != null && a.rawPtr == b.rawPtr);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00003077 File Offset: 0x00001277
		public static bool operator !=(HandleBase a, HandleBase b)
		{
			return !(a == b);
		}

		// Token: 0x040001F1 RID: 497
		protected IntPtr rawPtr;
	}
}

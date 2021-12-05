using System;

namespace FMOD.Studio
{
	// Token: 0x020000DD RID: 221
	public abstract class HandleBase
	{
		// Token: 0x060003BB RID: 955 RVA: 0x00005254 File Offset: 0x00003454
		public HandleBase(IntPtr newPtr)
		{
			this.rawPtr = newPtr;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00005263 File Offset: 0x00003463
		public bool isValid()
		{
			return this.rawPtr != IntPtr.Zero && this.isValidInternal();
		}

		// Token: 0x060003BD RID: 957
		protected abstract bool isValidInternal();

		// Token: 0x060003BE RID: 958 RVA: 0x0000527F File Offset: 0x0000347F
		public IntPtr getRaw()
		{
			return this.rawPtr;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00005287 File Offset: 0x00003487
		public override bool Equals(object obj)
		{
			return this.Equals(obj as HandleBase);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00005295 File Offset: 0x00003495
		public bool Equals(HandleBase p)
		{
			return p != null && this.rawPtr == p.rawPtr;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x000052AD File Offset: 0x000034AD
		public override int GetHashCode()
		{
			return this.rawPtr.ToInt32();
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x000052BA File Offset: 0x000034BA
		public static bool operator ==(HandleBase a, HandleBase b)
		{
			return a == b || (a != null && b != null && a.rawPtr == b.rawPtr);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x000052DB File Offset: 0x000034DB
		public static bool operator !=(HandleBase a, HandleBase b)
		{
			return !(a == b);
		}

		// Token: 0x0400048A RID: 1162
		protected IntPtr rawPtr;
	}
}

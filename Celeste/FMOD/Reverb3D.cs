using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	// Token: 0x0200004A RID: 74
	public class Reverb3D : HandleBase
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x00004A69 File Offset: 0x00002C69
		public RESULT release()
		{
			RESULT result = Reverb3D.FMOD_Reverb3D_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00004A84 File Offset: 0x00002C84
		public RESULT set3DAttributes(ref VECTOR position, float mindistance, float maxdistance)
		{
			return Reverb3D.FMOD_Reverb3D_Set3DAttributes(this.rawPtr, ref position, mindistance, maxdistance);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00004A94 File Offset: 0x00002C94
		public RESULT get3DAttributes(ref VECTOR position, ref float mindistance, ref float maxdistance)
		{
			return Reverb3D.FMOD_Reverb3D_Get3DAttributes(this.rawPtr, ref position, ref mindistance, ref maxdistance);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00004AA4 File Offset: 0x00002CA4
		public RESULT setProperties(ref REVERB_PROPERTIES properties)
		{
			return Reverb3D.FMOD_Reverb3D_SetProperties(this.rawPtr, ref properties);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00004AB2 File Offset: 0x00002CB2
		public RESULT getProperties(ref REVERB_PROPERTIES properties)
		{
			return Reverb3D.FMOD_Reverb3D_GetProperties(this.rawPtr, ref properties);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00004AC0 File Offset: 0x00002CC0
		public RESULT setActive(bool active)
		{
			return Reverb3D.FMOD_Reverb3D_SetActive(this.rawPtr, active);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00004ACE File Offset: 0x00002CCE
		public RESULT getActive(out bool active)
		{
			return Reverb3D.FMOD_Reverb3D_GetActive(this.rawPtr, out active);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00004ADC File Offset: 0x00002CDC
		public RESULT setUserData(IntPtr userdata)
		{
			return Reverb3D.FMOD_Reverb3D_SetUserData(this.rawPtr, userdata);
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00004AEA File Offset: 0x00002CEA
		public RESULT getUserData(out IntPtr userdata)
		{
			return Reverb3D.FMOD_Reverb3D_GetUserData(this.rawPtr, out userdata);
		}

		// Token: 0x060002FC RID: 764
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_Release(IntPtr reverb);

		// Token: 0x060002FD RID: 765
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_Set3DAttributes(IntPtr reverb, ref VECTOR position, float mindistance, float maxdistance);

		// Token: 0x060002FE RID: 766
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_Get3DAttributes(IntPtr reverb, ref VECTOR position, ref float mindistance, ref float maxdistance);

		// Token: 0x060002FF RID: 767
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_SetProperties(IntPtr reverb, ref REVERB_PROPERTIES properties);

		// Token: 0x06000300 RID: 768
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_GetProperties(IntPtr reverb, ref REVERB_PROPERTIES properties);

		// Token: 0x06000301 RID: 769
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_SetActive(IntPtr reverb, bool active);

		// Token: 0x06000302 RID: 770
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_GetActive(IntPtr reverb, out bool active);

		// Token: 0x06000303 RID: 771
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_SetUserData(IntPtr reverb, IntPtr userdata);

		// Token: 0x06000304 RID: 772
		[DllImport("fmod")]
		private static extern RESULT FMOD_Reverb3D_GetUserData(IntPtr reverb, out IntPtr userdata);

		// Token: 0x06000305 RID: 773 RVA: 0x00003A27 File Offset: 0x00001C27
		public Reverb3D(IntPtr raw) : base(raw)
		{
		}
	}
}

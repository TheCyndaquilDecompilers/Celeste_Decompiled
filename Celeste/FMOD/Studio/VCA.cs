using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000E3 RID: 227
	public class VCA : HandleBase
	{
		// Token: 0x060004BF RID: 1215 RVA: 0x000060E9 File Offset: 0x000042E9
		public RESULT getID(out Guid id)
		{
			return VCA.FMOD_Studio_VCA_GetID(this.rawPtr, out id);
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x000060F8 File Offset: 0x000042F8
		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT result = VCA.FMOD_Studio_VCA_GetPath(this.rawPtr, array, array.Length, out num);
			if (result == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				result = VCA.FMOD_Studio_VCA_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (result == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return result;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0000615A File Offset: 0x0000435A
		public RESULT getVolume(out float volume, out float finalvolume)
		{
			return VCA.FMOD_Studio_VCA_GetVolume(this.rawPtr, out volume, out finalvolume);
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x00006169 File Offset: 0x00004369
		public RESULT setVolume(float volume)
		{
			return VCA.FMOD_Studio_VCA_SetVolume(this.rawPtr, volume);
		}

		// Token: 0x060004C3 RID: 1219
		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_VCA_IsValid(IntPtr vca);

		// Token: 0x060004C4 RID: 1220
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetID(IntPtr vca, out Guid id);

		// Token: 0x060004C5 RID: 1221
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetPath(IntPtr vca, [Out] byte[] path, int size, out int retrieved);

		// Token: 0x060004C6 RID: 1222
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetVolume(IntPtr vca, out float volume, out float finalvolume);

		// Token: 0x060004C7 RID: 1223
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_SetVolume(IntPtr vca, float value);

		// Token: 0x060004C8 RID: 1224 RVA: 0x00005941 File Offset: 0x00003B41
		public VCA(IntPtr raw) : base(raw)
		{
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00006177 File Offset: 0x00004377
		protected override bool isValidInternal()
		{
			return VCA.FMOD_Studio_VCA_IsValid(this.rawPtr);
		}
	}
}

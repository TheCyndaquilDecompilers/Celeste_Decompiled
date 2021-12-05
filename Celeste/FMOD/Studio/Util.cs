using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000DC RID: 220
	public class Util
	{
		// Token: 0x060003B8 RID: 952 RVA: 0x00005237 File Offset: 0x00003437
		public static RESULT ParseID(string idString, out Guid id)
		{
			return Util.FMOD_Studio_ParseID(Encoding.UTF8.GetBytes(idString + "\0"), out id);
		}

		// Token: 0x060003B9 RID: 953
		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParseID(byte[] idString, out Guid id);
	}
}

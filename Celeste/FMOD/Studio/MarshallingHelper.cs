using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	// Token: 0x020000E6 RID: 230
	internal class MarshallingHelper
	{
		// Token: 0x0600051C RID: 1308 RVA: 0x00006678 File Offset: 0x00004878
		public static int stringLengthUtf8(IntPtr nativeUtf8)
		{
			int num = 0;
			while (Marshal.ReadByte(nativeUtf8, num) != 0)
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00006698 File Offset: 0x00004898
		public static string stringFromNativeUtf8(IntPtr nativeUtf8)
		{
			int num = MarshallingHelper.stringLengthUtf8(nativeUtf8);
			if (num == 0)
			{
				return string.Empty;
			}
			byte[] array = new byte[num];
			Marshal.Copy(nativeUtf8, array, 0, array.Length);
			return Encoding.UTF8.GetString(array, 0, num);
		}
	}
}

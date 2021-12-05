using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x0200001F RID: 31
	public struct StringWrapper
	{
		// Token: 0x06000010 RID: 16 RVA: 0x0000273C File Offset: 0x0000093C
		public static implicit operator string(StringWrapper fstring)
		{
			if (fstring.nativeUtf8Ptr == IntPtr.Zero)
			{
				return "";
			}
			int num = 0;
			while (Marshal.ReadByte(fstring.nativeUtf8Ptr, num) != 0)
			{
				num++;
			}
			if (num > 0)
			{
				byte[] array = new byte[num];
				Marshal.Copy(fstring.nativeUtf8Ptr, array, 0, num);
				return Encoding.UTF8.GetString(array, 0, num);
			}
			return "";
		}

		// Token: 0x0400016D RID: 365
		private IntPtr nativeUtf8Ptr;
	}
}

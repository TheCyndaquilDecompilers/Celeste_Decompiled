using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	// Token: 0x0200004B RID: 75
	internal class StringMarshalHelper
	{
		// Token: 0x06000306 RID: 774 RVA: 0x00004AF8 File Offset: 0x00002CF8
		internal static void NativeToBuilder(StringBuilder builder, IntPtr nativeMem)
		{
			byte[] array = new byte[builder.Capacity];
			Marshal.Copy(nativeMem, array, 0, builder.Capacity);
			int num = Array.IndexOf<byte>(array, 0);
			if (num > 0)
			{
				string @string = Encoding.UTF8.GetString(array, 0, num);
				builder.Append(@string);
			}
		}
	}
}

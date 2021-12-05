using System;
using System.Collections.Generic;
using System.Text;

namespace Celeste
{
	// Token: 0x02000381 RID: 897
	public static class RunLengthEncoding
	{
		// Token: 0x06001D62 RID: 7522 RVA: 0x000CC9C4 File Offset: 0x000CABC4
		public static byte[] Encode(string str)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < str.Length; i++)
			{
				byte b = 1;
				char c = str[i];
				while (i + 1 < str.Length && str[i + 1] == c && b < 255)
				{
					b += 1;
					i++;
				}
				list.Add(b);
				list.Add((byte)c);
			}
			return list.ToArray();
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x000CCA34 File Offset: 0x000CAC34
		public static string Decode(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i += 2)
			{
				stringBuilder.Append((char)bytes[i + 1], (int)bytes[i]);
			}
			return stringBuilder.ToString();
		}
	}
}

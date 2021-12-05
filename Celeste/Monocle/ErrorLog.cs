using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Monocle
{
	// Token: 0x0200012B RID: 299
	public static class ErrorLog
	{
		// Token: 0x06000AD8 RID: 2776 RVA: 0x0001D0B6 File Offset: 0x0001B2B6
		public static void Write(Exception e)
		{
			ErrorLog.Write(e.ToString());
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0001D0C4 File Offset: 0x0001B2C4
		public static void Write(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			if (Path.IsPathRooted("error_log.txt"))
			{
				string directoryName = Path.GetDirectoryName("error_log.txt");
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
			}
			if (File.Exists("error_log.txt"))
			{
				StreamReader streamReader = new StreamReader("error_log.txt");
				text = streamReader.ReadToEnd();
				streamReader.Close();
				if (!text.Contains("=========================================="))
				{
					text = "";
				}
			}
			if (Engine.Instance != null)
			{
				stringBuilder.Append(Engine.Instance.Title);
			}
			else
			{
				stringBuilder.Append("Monocle Engine");
			}
			stringBuilder.AppendLine(" Error Log");
			stringBuilder.AppendLine("==========================================");
			stringBuilder.AppendLine();
			if (Engine.Instance != null && Engine.Instance.Version != null)
			{
				stringBuilder.Append("Ver ");
				stringBuilder.AppendLine(Engine.Instance.Version.ToString());
			}
			stringBuilder.AppendLine(DateTime.Now.ToString());
			stringBuilder.AppendLine(str);
			if (text != "")
			{
				int startIndex = text.IndexOf("==========================================") + "==========================================".Length;
				string value = text.Substring(startIndex);
				stringBuilder.AppendLine(value);
			}
			StreamWriter streamWriter = new StreamWriter("error_log.txt", false);
			streamWriter.Write(stringBuilder.ToString());
			streamWriter.Close();
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0001D228 File Offset: 0x0001B428
		public static void Open()
		{
			if (File.Exists("error_log.txt"))
			{
				Process.Start("error_log.txt");
			}
		}

		// Token: 0x04000684 RID: 1668
		public const string Filename = "error_log.txt";

		// Token: 0x04000685 RID: 1669
		public const string Marker = "==========================================";
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Monocle;

namespace Celeste
{
	// Token: 0x0200030F RID: 783
	public static class Fonts
	{
		// Token: 0x060018CB RID: 6347 RVA: 0x0009B964 File Offset: 0x00099B64
		public static PixelFont Load(string face)
		{
			PixelFont pixelFont;
			List<string> list;
			if (!Fonts.loadedFonts.TryGetValue(face, out pixelFont) && Fonts.paths.TryGetValue(face, out list))
			{
				Fonts.loadedFonts.Add(face, pixelFont = new PixelFont(face));
				foreach (string path in list)
				{
					pixelFont.AddFontSize(path, GFX.Gui, false);
				}
			}
			return pixelFont;
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x0009B9EC File Offset: 0x00099BEC
		public static PixelFont Get(string face)
		{
			PixelFont result;
			if (Fonts.loadedFonts.TryGetValue(face, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0009BA0C File Offset: 0x00099C0C
		public static void Unload(string face)
		{
			PixelFont pixelFont;
			if (Fonts.loadedFonts.TryGetValue(face, out pixelFont))
			{
				pixelFont.Dispose();
				Fonts.loadedFonts.Remove(face);
			}
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x0009BA3C File Offset: 0x00099C3C
		public static void Reload()
		{
			List<string> list = new List<string>();
			foreach (string item in Fonts.loadedFonts.Keys)
			{
				list.Add(item);
			}
			foreach (string text in list)
			{
				Fonts.loadedFonts[text].Dispose();
				Fonts.Load(text);
			}
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x0009BAEC File Offset: 0x00099CEC
		public static void Prepare()
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.CloseInput = true;
			foreach (string text in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Dialog"), "*.fnt", SearchOption.AllDirectories))
			{
				string text2 = null;
				using (XmlReader xmlReader = XmlReader.Create(File.OpenRead(text), xmlReaderSettings))
				{
					while (xmlReader.Read())
					{
						if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "info")
						{
							text2 = xmlReader.GetAttribute("face");
						}
					}
				}
				if (text2 != null)
				{
					List<string> list;
					if (!Fonts.paths.TryGetValue(text2, out list))
					{
						Fonts.paths.Add(text2, list = new List<string>());
					}
					list.Add(text);
				}
			}
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x0009BBD0 File Offset: 0x00099DD0
		public static void Log()
		{
			Engine.Commands.Log("EXISTING FONTS:");
			foreach (KeyValuePair<string, List<string>> keyValuePair in Fonts.paths)
			{
				Engine.Commands.Log(" - " + keyValuePair.Key);
				foreach (string str in keyValuePair.Value)
				{
					Engine.Commands.Log(" - > " + str);
				}
			}
			Engine.Commands.Log("LOADED:");
			foreach (KeyValuePair<string, PixelFont> keyValuePair2 in Fonts.loadedFonts)
			{
				Engine.Commands.Log(" - " + keyValuePair2.Key);
			}
		}

		// Token: 0x0400154F RID: 5455
		private static Dictionary<string, List<string>> paths = new Dictionary<string, List<string>>();

		// Token: 0x04001550 RID: 5456
		private static Dictionary<string, PixelFont> loadedFonts = new Dictionary<string, PixelFont>();
	}
}

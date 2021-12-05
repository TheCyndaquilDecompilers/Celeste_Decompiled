using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Monocle;

namespace Celeste
{
	// Token: 0x0200013F RID: 319
	public class Language
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x00021268 File Offset: 0x0001F468
		public PixelFont Font
		{
			get
			{
				return Fonts.Get(this.FontFace);
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000B97 RID: 2967 RVA: 0x00021275 File Offset: 0x0001F475
		public PixelFontSize FontSize
		{
			get
			{
				return this.Font.Get(this.FontFaceSize);
			}
		}

		// Token: 0x1700010C RID: 268
		public string this[string name]
		{
			get
			{
				return this.Dialog[name];
			}
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x00021298 File Offset: 0x0001F498
		public bool CanDisplay(string text)
		{
			PixelFontSize fontSize = this.FontSize;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != ' ' && fontSize.Get((int)text[i]) == null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x000212DC File Offset: 0x0001F4DC
		public void Export(string path)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(path)))
			{
				binaryWriter.Write(this.Id);
				binaryWriter.Write(this.Label);
				binaryWriter.Write(this.IconPath);
				binaryWriter.Write(this.Order);
				binaryWriter.Write(this.FontFace);
				binaryWriter.Write(this.FontFaceSize);
				binaryWriter.Write(this.SplitRegex);
				binaryWriter.Write(this.CommaCharacters);
				binaryWriter.Write(this.PeriodCharacters);
				binaryWriter.Write(this.Lines);
				binaryWriter.Write(this.Words);
				binaryWriter.Write(this.Dialog.Count);
				foreach (KeyValuePair<string, string> keyValuePair in this.Dialog)
				{
					binaryWriter.Write(keyValuePair.Key);
					binaryWriter.Write(keyValuePair.Value);
					binaryWriter.Write(this.Cleaned[keyValuePair.Key]);
				}
			}
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x00021414 File Offset: 0x0001F614
		public static Language FromExport(string path)
		{
			Language language = new Language();
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
			{
				language.Id = binaryReader.ReadString();
				language.Label = binaryReader.ReadString();
				language.IconPath = binaryReader.ReadString();
				language.Icon = new MTexture(VirtualContent.CreateTexture(Path.Combine("Dialog", language.IconPath)));
				language.Order = binaryReader.ReadInt32();
				language.FontFace = binaryReader.ReadString();
				language.FontFaceSize = binaryReader.ReadSingle();
				language.SplitRegex = binaryReader.ReadString();
				language.CommaCharacters = binaryReader.ReadString();
				language.PeriodCharacters = binaryReader.ReadString();
				language.Lines = binaryReader.ReadInt32();
				language.Words = binaryReader.ReadInt32();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string key = binaryReader.ReadString();
					language.Dialog[key] = binaryReader.ReadString();
					language.Cleaned[key] = binaryReader.ReadString();
				}
			}
			return language;
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x00021538 File Offset: 0x0001F738
		public static Language FromTxt(string path)
		{
			Language language = null;
			string text = "";
			StringBuilder stringBuilder = new StringBuilder();
			string input = "";
			foreach (string text2 in File.ReadLines(path, Encoding.UTF8))
			{
				string text3 = text2.Trim();
				if (text3.Length > 0 && text3[0] != '#')
				{
					if (text3.IndexOf('[') >= 0)
					{
						text3 = Language.portrait.Replace(text3, "{portrait ${content}}");
					}
					text3 = text3.Replace("\\#", "#");
					if (text3.Length > 0)
					{
						if (Language.variable.IsMatch(text3))
						{
							if (!string.IsNullOrEmpty(text))
							{
								language.Dialog[text] = stringBuilder.ToString();
							}
							string[] array = text3.Split(new char[]
							{
								'='
							});
							string text4 = array[0].Trim();
							string text5 = (array.Length > 1) ? array[1].Trim() : "";
							if (text4.Equals("language", StringComparison.OrdinalIgnoreCase))
							{
								string[] array2 = text5.Split(new char[]
								{
									','
								});
								language = new Language();
								language.FontFace = null;
								language.Id = array2[0];
								language.FilePath = Path.GetFileName(path);
								if (array2.Length > 1)
								{
									language.Label = array2[1];
								}
							}
							else if (text4.Equals("icon", StringComparison.OrdinalIgnoreCase))
							{
								VirtualTexture texture = VirtualContent.CreateTexture(Path.Combine("Dialog", text5));
								language.IconPath = text5;
								language.Icon = new MTexture(texture);
							}
							else if (text4.Equals("order", StringComparison.OrdinalIgnoreCase))
							{
								language.Order = int.Parse(text5);
							}
							else if (text4.Equals("font", StringComparison.OrdinalIgnoreCase))
							{
								string[] array3 = text5.Split(new char[]
								{
									','
								});
								language.FontFace = array3[0];
								language.FontFaceSize = float.Parse(array3[1], CultureInfo.InvariantCulture);
							}
							else if (text4.Equals("SPLIT_REGEX", StringComparison.OrdinalIgnoreCase))
							{
								language.SplitRegex = text5;
							}
							else if (text4.Equals("commas", StringComparison.OrdinalIgnoreCase))
							{
								language.CommaCharacters = text5;
							}
							else if (text4.Equals("periods", StringComparison.OrdinalIgnoreCase))
							{
								language.PeriodCharacters = text5;
							}
							else
							{
								text = text4;
								stringBuilder.Clear();
								stringBuilder.Append(text5);
							}
						}
						else
						{
							if (stringBuilder.Length > 0)
							{
								string text6 = stringBuilder.ToString();
								if (!text6.EndsWith("{break}") && !text6.EndsWith("{n}") && Language.command.Replace(input, "").Length > 0)
								{
									stringBuilder.Append("{break}");
								}
							}
							stringBuilder.Append(text3);
						}
						input = text3;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				language.Dialog[text] = stringBuilder.ToString();
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in language.Dialog)
			{
				list.Add(keyValuePair.Key);
			}
			foreach (string key in list)
			{
				string text7 = language.Dialog[key];
				MatchCollection matchCollection = null;
				while (matchCollection == null || matchCollection.Count > 0)
				{
					matchCollection = Language.insert.Matches(text7);
					for (int i = 0; i < matchCollection.Count; i++)
					{
						Match match = matchCollection[i];
						string value = match.Groups[1].Value;
						string newValue;
						if (language.Dialog.TryGetValue(value, out newValue))
						{
							text7 = text7.Replace(match.Value, newValue);
						}
						else
						{
							text7 = text7.Replace(match.Value, "[XXX]");
						}
					}
				}
				language.Dialog[key] = text7;
			}
			language.Lines = 0;
			language.Words = 0;
			foreach (string key2 in list)
			{
				string text8 = language.Dialog[key2];
				if (text8.IndexOf('{') >= 0)
				{
					text8 = text8.Replace("{n}", "\n");
					text8 = text8.Replace("{break}", "\n");
					text8 = Language.command.Replace(text8, "");
				}
				language.Cleaned.Add(key2, text8);
			}
			return language;
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x00021A68 File Offset: 0x0001FC68
		public void Dispose()
		{
			if (this.Icon.Texture != null && !this.Icon.Texture.IsDisposed)
			{
				this.Icon.Texture.Dispose();
			}
		}

		// Token: 0x040006DD RID: 1757
		public string FilePath;

		// Token: 0x040006DE RID: 1758
		public string Id;

		// Token: 0x040006DF RID: 1759
		public string Label;

		// Token: 0x040006E0 RID: 1760
		public string IconPath;

		// Token: 0x040006E1 RID: 1761
		public MTexture Icon;

		// Token: 0x040006E2 RID: 1762
		public int Order = 100;

		// Token: 0x040006E3 RID: 1763
		public string FontFace;

		// Token: 0x040006E4 RID: 1764
		public float FontFaceSize;

		// Token: 0x040006E5 RID: 1765
		public string SplitRegex = "(\\s|\\{|\\})";

		// Token: 0x040006E6 RID: 1766
		public string CommaCharacters = ",";

		// Token: 0x040006E7 RID: 1767
		public string PeriodCharacters = ".!?";

		// Token: 0x040006E8 RID: 1768
		public int Lines;

		// Token: 0x040006E9 RID: 1769
		public int Words;

		// Token: 0x040006EA RID: 1770
		public Dictionary<string, string> Dialog = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040006EB RID: 1771
		public Dictionary<string, string> Cleaned = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040006EC RID: 1772
		private static readonly Regex command = new Regex("\\{(.*?)\\}", RegexOptions.RightToLeft);

		// Token: 0x040006ED RID: 1773
		private static readonly Regex insert = new Regex("\\{\\+\\s*(.*?)\\}");

		// Token: 0x040006EE RID: 1774
		private static readonly Regex variable = new Regex("^\\w+\\=.*");

		// Token: 0x040006EF RID: 1775
		private static readonly Regex portrait = new Regex("\\[(?<content>[^\\[\\\\]*(?:\\\\.[^\\]\\\\]*)*)\\]", RegexOptions.IgnoreCase);
	}
}

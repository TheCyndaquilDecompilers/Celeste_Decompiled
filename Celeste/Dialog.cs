using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Monocle;

namespace Celeste
{
	// Token: 0x0200030D RID: 781
	public static class Dialog
	{
		// Token: 0x06001887 RID: 6279 RVA: 0x00099FCC File Offset: 0x000981CC
		public static void Load()
		{
			Dialog.Language = null;
			Dialog.Languages = new Dictionary<string, Language>();
			string[] files = Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Dialog"), "*.txt", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				Dialog.LoadLanguage(files[i]);
			}
			if (Settings.Instance != null && Settings.Instance.Language != null && Dialog.Languages.ContainsKey(Settings.Instance.Language))
			{
				Dialog.Language = Dialog.Languages[Settings.Instance.Language];
			}
			else if (Dialog.Languages.ContainsKey("english"))
			{
				Dialog.Language = Dialog.Languages["english"];
			}
			else
			{
				if (Dialog.Languages.Count <= 0)
				{
					throw new Exception("Missing Language Files");
				}
				Dialog.Language = Dialog.Languages.ElementAt(0).Value;
			}
			Settings.Instance.Language = Dialog.Language.Id;
			Dialog.OrderedLanguages = new List<Language>();
			foreach (KeyValuePair<string, Language> keyValuePair in Dialog.Languages)
			{
				Dialog.OrderedLanguages.Add(keyValuePair.Value);
			}
			Dialog.OrderedLanguages.Sort(delegate(Language a, Language b)
			{
				if (a.Order != b.Order)
				{
					return a.Order - b.Order;
				}
				return a.Id.CompareTo(b.Id);
			});
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0009A150 File Offset: 0x00098350
		public static Language LoadLanguage(string filename)
		{
			Language language;
			if (File.Exists(filename + ".export"))
			{
				language = Language.FromExport(filename + ".export");
			}
			else
			{
				language = Language.FromTxt(filename);
			}
			if (language != null)
			{
				Dialog.Languages[language.Id] = language;
			}
			return language;
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0009A1A0 File Offset: 0x000983A0
		public static void Unload()
		{
			foreach (KeyValuePair<string, Language> keyValuePair in Dialog.Languages)
			{
				keyValuePair.Value.Dispose();
			}
			Dialog.Languages.Clear();
			Dialog.Language = null;
			Dialog.OrderedLanguages.Clear();
			Dialog.OrderedLanguages = null;
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x0009A218 File Offset: 0x00098418
		public static bool Has(string name, Language language = null)
		{
			if (language == null)
			{
				language = Dialog.Language;
			}
			return language.Dialog.ContainsKey(name);
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x0009A230 File Offset: 0x00098430
		public static string Get(string name, Language language = null)
		{
			if (language == null)
			{
				language = Dialog.Language;
			}
			string result = "";
			if (language.Dialog.TryGetValue(name, out result))
			{
				return result;
			}
			return "XXX";
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x0009A264 File Offset: 0x00098464
		public static string Clean(string name, Language language = null)
		{
			if (language == null)
			{
				language = Dialog.Language;
			}
			string result = "";
			if (language.Cleaned.TryGetValue(name, out result))
			{
				return result;
			}
			return "XXX";
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0009A298 File Offset: 0x00098498
		public static string Time(long ticks)
		{
			TimeSpan timeSpan = TimeSpan.FromTicks(ticks);
			if ((int)timeSpan.TotalHours > 0)
			{
				return ((int)timeSpan.TotalHours).ToString() + timeSpan.ToString("\\:mm\\:ss\\.fff");
			}
			return timeSpan.Minutes.ToString() + timeSpan.ToString("\\:ss\\.fff");
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x0009A2FC File Offset: 0x000984FC
		public static string FileTime(long ticks)
		{
			TimeSpan timeSpan = TimeSpan.FromTicks(ticks);
			if (timeSpan.TotalHours >= 1.0)
			{
				return ((int)timeSpan.TotalHours).ToString() + timeSpan.ToString("\\:mm\\:ss\\.fff");
			}
			return timeSpan.ToString("mm\\:ss\\.fff");
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0009A350 File Offset: 0x00098550
		public static string Deaths(int deaths)
		{
			if (deaths > 999999)
			{
				return ((float)deaths / 1000000f).ToString("0.00") + "m";
			}
			if (deaths > 9999)
			{
				return ((float)deaths / 1000f).ToString("0.0") + "k";
			}
			return deaths.ToString();
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0009A3B4 File Offset: 0x000985B4
		public static void CheckCharacters()
		{
			foreach (string str in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Dialog"), "*.txt", SearchOption.AllDirectories))
			{
				HashSet<int> hashSet = new HashSet<int>();
				foreach (string text in File.ReadLines(str, Encoding.UTF8))
				{
					for (int j = 0; j < text.Length; j++)
					{
						if (!hashSet.Contains((int)text[j]))
						{
							hashSet.Add((int)text[j]);
						}
					}
				}
				List<int> list = new List<int>();
				foreach (int item in hashSet)
				{
					list.Add(item);
				}
				list.Sort();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("chars=");
				int num = 0;
				Console.WriteLine("Characters of : " + str);
				for (int k = 0; k < list.Count; k++)
				{
					bool flag = false;
					int num2 = k + 1;
					while (num2 < list.Count && list[num2] == list[num2 - 1] + 1)
					{
						flag = true;
						num2++;
					}
					if (flag)
					{
						stringBuilder.Append(string.Concat(new object[]
						{
							list[k],
							"-",
							list[num2 - 1],
							","
						}));
					}
					else
					{
						stringBuilder.Append(list[k] + ",");
					}
					k = num2 - 1;
					num++;
					if (num >= 10)
					{
						num = 0;
						stringBuilder.Remove(stringBuilder.Length - 1, 1);
						Console.WriteLine(stringBuilder.ToString());
						stringBuilder.Clear();
						stringBuilder.Append("chars=");
					}
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				Console.WriteLine(stringBuilder.ToString());
				Console.WriteLine();
			}
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0009A618 File Offset: 0x00098818
		public static bool CheckLanguageFontCharacters(string a)
		{
			Language language = Dialog.Languages[a];
			bool result = true;
			HashSet<int> hashSet = new HashSet<int>();
			foreach (KeyValuePair<string, string> keyValuePair in language.Dialog)
			{
				for (int i = 0; i < keyValuePair.Value.Length; i++)
				{
					int num = (int)keyValuePair.Value[i];
					if (!hashSet.Contains(num) && !language.FontSize.Characters.ContainsKey(num))
					{
						hashSet.Add(num);
						result = false;
					}
				}
			}
			Console.WriteLine("FONT: " + a);
			if (hashSet.Count > 0)
			{
				Console.WriteLine(" - Missing Characters: " + string.Join<int>(",", hashSet));
			}
			Console.WriteLine(" - OK: " + result.ToString());
			Console.WriteLine();
			if (hashSet.Count > 0)
			{
				string text = "";
				foreach (int num2 in hashSet)
				{
					text += ((char)num2).ToString();
				}
				File.WriteAllText(a + "-missing-debug.txt", text);
			}
			return result;
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0009A78C File Offset: 0x0009898C
		public static bool CompareLanguages(string a, string b, bool compareContent)
		{
			Console.WriteLine("COMPARE: " + a + " -> " + b);
			Language language = Dialog.Languages[a];
			Language language2 = Dialog.Languages[b];
			bool result = true;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in language.Dialog)
			{
				if (!language2.Dialog.ContainsKey(keyValuePair.Key))
				{
					list2.Add(keyValuePair.Key);
					result = false;
				}
				else if (compareContent && language2.Dialog[keyValuePair.Key] != language.Dialog[keyValuePair.Key])
				{
					list3.Add(keyValuePair.Key);
					result = false;
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in language2.Dialog)
			{
				if (!language.Dialog.ContainsKey(keyValuePair2.Key))
				{
					list.Add(keyValuePair2.Key);
					result = false;
				}
			}
			if (list.Count > 0)
			{
				Console.WriteLine(" - Missing from " + a + ": " + string.Join(", ", list));
			}
			if (list2.Count > 0)
			{
				Console.WriteLine(" - Missing from " + b + ": " + string.Join(", ", list2));
			}
			if (list3.Count > 0)
			{
				Console.WriteLine(" - Diff. Content: " + string.Join(", ", list3));
			}
			Func<string, List<List<string>>> func = delegate(string text)
			{
				List<List<string>> list6 = new List<List<string>>();
				foreach (object obj in Regex.Matches(text, "\\{([^}]*)\\}"))
				{
					string[] array = Regex.Split(((Match)obj).Value, "(\\{|\\}|\\s)");
					List<string> list7 = new List<string>();
					foreach (string text3 in array)
					{
						if (!string.IsNullOrWhiteSpace(text3) && text3.Length > 0 && text3 != "{" && text3 != "}")
						{
							list7.Add(text3);
						}
					}
					list6.Add(list7);
				}
				return list6;
			};
			foreach (KeyValuePair<string, string> keyValuePair3 in language.Dialog)
			{
				if (language2.Dialog.ContainsKey(keyValuePair3.Key))
				{
					List<List<string>> list4 = func(keyValuePair3.Value);
					List<List<string>> list5 = func(language2.Dialog[keyValuePair3.Key]);
					int i = 0;
					int num = 0;
					while (i < list4.Count)
					{
						string text2 = list4[i][0];
						if (!(text2 == "portrait"))
						{
							if (!(text2 == "trigger"))
							{
								goto IL_43F;
							}
						}
						while (num < list5.Count && list5[num][0] != text2)
						{
							num++;
						}
						if (num >= list5.Count)
						{
							Console.WriteLine(" - Command number mismatch in " + keyValuePair3.Key + " in " + b);
							result = false;
							i = list4.Count;
						}
						else
						{
							if (text2 == "portrait")
							{
								for (int j = 0; j < list4[i].Count; j++)
								{
									if (list4[i][j] != list5[num][j])
									{
										Console.WriteLine(string.Concat(new string[]
										{
											" - Portrait in ",
											keyValuePair3.Key,
											" is incorrect in ",
											b,
											" ({",
											string.Join(" ", list4[i]),
											"} vs {",
											string.Join(" ", list5[num]),
											"})"
										}));
										result = false;
									}
								}
							}
							else if (text2 == "trigger" && list4[i][1] != list5[num][1])
							{
								Console.WriteLine(string.Concat(new string[]
								{
									" - Trigger in ",
									keyValuePair3.Key,
									" is incorrect in ",
									b,
									" (",
									list4[i][1],
									" vs ",
									list5[num][1],
									")"
								}));
								result = false;
							}
							num++;
						}
						IL_43F:
						i++;
					}
				}
			}
			Console.WriteLine(" - OK: " + result.ToString());
			Console.WriteLine();
			return result;
		}

		// Token: 0x0400153E RID: 5438
		public static Language Language = null;

		// Token: 0x0400153F RID: 5439
		public static Dictionary<string, Language> Languages;

		// Token: 0x04001540 RID: 5440
		public static List<Language> OrderedLanguages;

		// Token: 0x04001541 RID: 5441
		private static string[] LanguageDataVariables = new string[]
		{
			"language",
			"icon",
			"order",
			"split_regex",
			"commas",
			"periods",
			"font"
		};

		// Token: 0x04001542 RID: 5442
		private const string path = "Dialog";
	}
}

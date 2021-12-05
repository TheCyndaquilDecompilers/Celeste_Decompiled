using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000107 RID: 263
	public class Atlas
	{
		// Token: 0x060007D6 RID: 2006 RVA: 0x0000ECA8 File Offset: 0x0000CEA8
		public static Atlas FromAtlas(string path, Atlas.AtlasDataFormat format)
		{
			Atlas atlas = new Atlas();
			atlas.Sources = new List<VirtualTexture>();
			Atlas.ReadAtlasData(atlas, path, format);
			return atlas;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0000ECC4 File Offset: 0x0000CEC4
		private static void ReadAtlasData(Atlas atlas, string path, Atlas.AtlasDataFormat format)
		{
			switch (format)
			{
			case Atlas.AtlasDataFormat.TexturePacker_Sparrow:
			{
				XmlElement xmlElement = Calc.LoadContentXML(path)["TextureAtlas"];
				string path2 = xmlElement.Attr("imagePath", "");
				VirtualTexture virtualTexture = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), path2));
				MTexture parent = new MTexture(virtualTexture);
				atlas.Sources.Add(virtualTexture);
				using (IEnumerator enumerator = xmlElement.GetElementsByTagName("SubTexture").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xml = (XmlElement)obj;
						string text = xml.Attr("name");
						Rectangle clipRect = xml.Rect();
						if (xml.HasAttr("frameX"))
						{
							atlas.textures[text] = new MTexture(parent, text, clipRect, new Vector2((float)(-(float)xml.AttrInt("frameX")), (float)(-(float)xml.AttrInt("frameY"))), xml.AttrInt("frameWidth"), xml.AttrInt("frameHeight"));
						}
						else
						{
							atlas.textures[text] = new MTexture(parent, text, clipRect);
						}
					}
					return;
				}
				break;
			}
			case Atlas.AtlasDataFormat.CrunchXml:
				break;
			case Atlas.AtlasDataFormat.CrunchBinary:
				goto IL_2CC;
			case Atlas.AtlasDataFormat.CrunchXmlOrBinary:
				goto IL_88F;
			case Atlas.AtlasDataFormat.CrunchBinaryNoAtlas:
				goto IL_3FA;
			case Atlas.AtlasDataFormat.Packer:
				goto IL_521;
			case Atlas.AtlasDataFormat.PackerNoAtlas:
				goto IL_6D8;
			default:
				throw new NotImplementedException();
			}
			using (IEnumerator enumerator = Calc.LoadContentXML(path)["atlas"].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj2 = enumerator.Current;
					XmlElement xmlElement2 = (XmlElement)obj2;
					string str = xmlElement2.Attr("n", "");
					VirtualTexture virtualTexture2 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str + ".png"));
					MTexture parent2 = new MTexture(virtualTexture2);
					atlas.Sources.Add(virtualTexture2);
					foreach (object obj3 in xmlElement2)
					{
						XmlElement xml2 = (XmlElement)obj3;
						string text2 = xml2.Attr("n");
						Rectangle clipRect2 = new Rectangle(xml2.AttrInt("x"), xml2.AttrInt("y"), xml2.AttrInt("w"), xml2.AttrInt("h"));
						if (xml2.HasAttr("fx"))
						{
							atlas.textures[text2] = new MTexture(parent2, text2, clipRect2, new Vector2((float)(-(float)xml2.AttrInt("fx")), (float)(-(float)xml2.AttrInt("fy"))), xml2.AttrInt("fw"), xml2.AttrInt("fh"));
						}
						else
						{
							atlas.textures[text2] = new MTexture(parent2, text2, clipRect2);
						}
					}
				}
				return;
			}
			IL_2CC:
			using (FileStream fileStream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path)))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				short num = binaryReader.ReadInt16();
				for (int i = 0; i < (int)num; i++)
				{
					string str2 = binaryReader.ReadNullTerminatedString();
					VirtualTexture virtualTexture3 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str2 + ".png"));
					atlas.Sources.Add(virtualTexture3);
					MTexture parent3 = new MTexture(virtualTexture3);
					short num2 = binaryReader.ReadInt16();
					for (int j = 0; j < (int)num2; j++)
					{
						string text3 = binaryReader.ReadNullTerminatedString();
						short x = binaryReader.ReadInt16();
						short y = binaryReader.ReadInt16();
						short width = binaryReader.ReadInt16();
						short height = binaryReader.ReadInt16();
						short num3 = binaryReader.ReadInt16();
						short num4 = binaryReader.ReadInt16();
						short width2 = binaryReader.ReadInt16();
						short height2 = binaryReader.ReadInt16();
						atlas.textures[text3] = new MTexture(parent3, text3, new Rectangle((int)x, (int)y, (int)width, (int)height), new Vector2((float)(-(float)num3), (float)(-(float)num4)), (int)width2, (int)height2);
					}
				}
				return;
			}
			IL_3FA:
			using (FileStream fileStream2 = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".bin")))
			{
				BinaryReader binaryReader2 = new BinaryReader(fileStream2);
				short num5 = binaryReader2.ReadInt16();
				for (int k = 0; k < (int)num5; k++)
				{
					string path3 = binaryReader2.ReadNullTerminatedString();
					string path4 = Path.Combine(Path.GetDirectoryName(path), path3);
					short num6 = binaryReader2.ReadInt16();
					for (int l = 0; l < (int)num6; l++)
					{
						string text4 = binaryReader2.ReadNullTerminatedString();
						binaryReader2.ReadInt16();
						binaryReader2.ReadInt16();
						binaryReader2.ReadInt16();
						binaryReader2.ReadInt16();
						short num7 = binaryReader2.ReadInt16();
						short num8 = binaryReader2.ReadInt16();
						short frameWidth = binaryReader2.ReadInt16();
						short frameHeight = binaryReader2.ReadInt16();
						VirtualTexture virtualTexture4 = VirtualContent.CreateTexture(Path.Combine(path4, text4 + ".png"));
						atlas.Sources.Add(virtualTexture4);
						atlas.textures[text4] = new MTexture(virtualTexture4, new Vector2((float)(-(float)num7), (float)(-(float)num8)), (int)frameWidth, (int)frameHeight);
					}
				}
				return;
			}
			IL_521:
			using (FileStream fileStream3 = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
			{
				BinaryReader binaryReader3 = new BinaryReader(fileStream3);
				binaryReader3.ReadInt32();
				binaryReader3.ReadString();
				binaryReader3.ReadInt32();
				short num9 = binaryReader3.ReadInt16();
				for (int m = 0; m < (int)num9; m++)
				{
					string str3 = binaryReader3.ReadString();
					VirtualTexture virtualTexture5 = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), str3 + ".data"));
					atlas.Sources.Add(virtualTexture5);
					MTexture parent4 = new MTexture(virtualTexture5);
					short num10 = binaryReader3.ReadInt16();
					for (int n = 0; n < (int)num10; n++)
					{
						string text5 = binaryReader3.ReadString().Replace('\\', '/');
						short x2 = binaryReader3.ReadInt16();
						short y2 = binaryReader3.ReadInt16();
						short width3 = binaryReader3.ReadInt16();
						short height3 = binaryReader3.ReadInt16();
						short num11 = binaryReader3.ReadInt16();
						short num12 = binaryReader3.ReadInt16();
						short width4 = binaryReader3.ReadInt16();
						short height4 = binaryReader3.ReadInt16();
						atlas.textures[text5] = new MTexture(parent4, text5, new Rectangle((int)x2, (int)y2, (int)width3, (int)height3), new Vector2((float)(-(float)num11), (float)(-(float)num12)), (int)width4, (int)height4);
					}
				}
				if (fileStream3.Position < fileStream3.Length && binaryReader3.ReadString() == "LINKS")
				{
					short num13 = binaryReader3.ReadInt16();
					for (int num14 = 0; num14 < (int)num13; num14++)
					{
						string key = binaryReader3.ReadString();
						string value = binaryReader3.ReadString();
						atlas.links.Add(key, value);
					}
				}
				return;
			}
			IL_6D8:
			using (FileStream fileStream4 = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
			{
				BinaryReader binaryReader4 = new BinaryReader(fileStream4);
				binaryReader4.ReadInt32();
				binaryReader4.ReadString();
				binaryReader4.ReadInt32();
				short num15 = binaryReader4.ReadInt16();
				for (int num16 = 0; num16 < (int)num15; num16++)
				{
					string path5 = binaryReader4.ReadString();
					string path6 = Path.Combine(Path.GetDirectoryName(path), path5);
					short num17 = binaryReader4.ReadInt16();
					for (int num18 = 0; num18 < (int)num17; num18++)
					{
						string text6 = binaryReader4.ReadString().Replace('\\', '/');
						binaryReader4.ReadInt16();
						binaryReader4.ReadInt16();
						binaryReader4.ReadInt16();
						binaryReader4.ReadInt16();
						short num19 = binaryReader4.ReadInt16();
						short num20 = binaryReader4.ReadInt16();
						short frameWidth2 = binaryReader4.ReadInt16();
						short frameHeight2 = binaryReader4.ReadInt16();
						VirtualTexture virtualTexture6 = VirtualContent.CreateTexture(Path.Combine(path6, text6 + ".data"));
						atlas.Sources.Add(virtualTexture6);
						atlas.textures[text6] = new MTexture(virtualTexture6, new Vector2((float)(-(float)num19), (float)(-(float)num20)), (int)frameWidth2, (int)frameHeight2);
						atlas.textures[text6].AtlasPath = text6;
					}
				}
				if (fileStream4.Position < fileStream4.Length && binaryReader4.ReadString() == "LINKS")
				{
					short num21 = binaryReader4.ReadInt16();
					for (int num22 = 0; num22 < (int)num21; num22++)
					{
						string key2 = binaryReader4.ReadString();
						string value2 = binaryReader4.ReadString();
						atlas.links.Add(key2, value2);
					}
				}
				return;
			}
			IL_88F:
			if (File.Exists(Path.Combine(Engine.ContentDirectory, path + ".bin")))
			{
				Atlas.ReadAtlasData(atlas, path + ".bin", Atlas.AtlasDataFormat.CrunchBinary);
				return;
			}
			Atlas.ReadAtlasData(atlas, path + ".xml", Atlas.AtlasDataFormat.CrunchXml);
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0000F654 File Offset: 0x0000D854
		public static Atlas FromMultiAtlas(string rootPath, string[] dataPath, Atlas.AtlasDataFormat format)
		{
			Atlas atlas = new Atlas();
			atlas.Sources = new List<VirtualTexture>();
			for (int i = 0; i < dataPath.Length; i++)
			{
				Atlas.ReadAtlasData(atlas, Path.Combine(rootPath, dataPath[i]), format);
			}
			return atlas;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0000F694 File Offset: 0x0000D894
		public static Atlas FromMultiAtlas(string rootPath, string filename, Atlas.AtlasDataFormat format)
		{
			Atlas atlas = new Atlas();
			atlas.Sources = new List<VirtualTexture>();
			int num = 0;
			for (;;)
			{
				string text = Path.Combine(rootPath, filename + num.ToString() + ".xml");
				if (!File.Exists(Path.Combine(Engine.ContentDirectory, text)))
				{
					break;
				}
				Atlas.ReadAtlasData(atlas, text, format);
				num++;
			}
			return atlas;
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0000F6F0 File Offset: 0x0000D8F0
		public static Atlas FromDirectory(string path)
		{
			Atlas atlas = new Atlas();
			atlas.Sources = new List<VirtualTexture>();
			string contentDirectory = Engine.ContentDirectory;
			int length = contentDirectory.Length;
			string text = Path.Combine(contentDirectory, path);
			int length2 = text.Length;
			foreach (string text2 in Directory.GetFiles(text, "*", SearchOption.AllDirectories))
			{
				string extension = Path.GetExtension(text2);
				if (!(extension != ".png") || !(extension != ".xnb"))
				{
					VirtualTexture virtualTexture = VirtualContent.CreateTexture(text2.Substring(length + 1));
					atlas.Sources.Add(virtualTexture);
					string text3 = text2.Substring(length2 + 1);
					text3 = text3.Substring(0, text3.Length - 4);
					text3 = text3.Replace('\\', '/');
					atlas.textures.Add(text3, new MTexture(virtualTexture));
				}
			}
			return atlas;
		}

		// Token: 0x170000AE RID: 174
		public MTexture this[string id]
		{
			get
			{
				return this.textures[id];
			}
			set
			{
				this.textures[id] = value;
			}
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0000F7F2 File Offset: 0x0000D9F2
		public bool Has(string id)
		{
			return this.textures.ContainsKey(id);
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x0000F800 File Offset: 0x0000DA00
		public MTexture GetOrDefault(string id, MTexture defaultTexture)
		{
			if (string.IsNullOrEmpty(id) || !this.Has(id))
			{
				return defaultTexture;
			}
			return this.textures[id];
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x0000F824 File Offset: 0x0000DA24
		public List<MTexture> GetAtlasSubtextures(string key)
		{
			List<MTexture> list;
			if (!this.orderedTexturesCache.TryGetValue(key, out list))
			{
				list = new List<MTexture>();
				int num = 0;
				for (;;)
				{
					MTexture atlasSubtextureFromAtlasAt = this.GetAtlasSubtextureFromAtlasAt(key, num);
					if (atlasSubtextureFromAtlasAt == null)
					{
						break;
					}
					list.Add(atlasSubtextureFromAtlasAt);
					num++;
				}
				this.orderedTexturesCache.Add(key, list);
			}
			return list;
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0000F870 File Offset: 0x0000DA70
		private MTexture GetAtlasSubtextureFromCacheAt(string key, int index)
		{
			return this.orderedTexturesCache[key][index];
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x0000F884 File Offset: 0x0000DA84
		private MTexture GetAtlasSubtextureFromAtlasAt(string key, int index)
		{
			if (index == 0 && this.textures.ContainsKey(key))
			{
				return this.textures[key];
			}
			string text = index.ToString();
			int length = text.Length;
			while (text.Length < length + 6)
			{
				MTexture result;
				if (this.textures.TryGetValue(key + text, out result))
				{
					return result;
				}
				text = "0" + text;
			}
			return null;
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
		public MTexture GetAtlasSubtexturesAt(string key, int index)
		{
			List<MTexture> list;
			if (this.orderedTexturesCache.TryGetValue(key, out list))
			{
				return list[index];
			}
			return this.GetAtlasSubtextureFromAtlasAt(key, index);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x0000F920 File Offset: 0x0000DB20
		public MTexture GetLinkedTexture(string key)
		{
			string key2;
			MTexture result;
			if (key != null && this.links.TryGetValue(key, out key2) && this.textures.TryGetValue(key2, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0000F954 File Offset: 0x0000DB54
		public void Dispose()
		{
			foreach (VirtualTexture virtualTexture in this.Sources)
			{
				virtualTexture.Dispose();
			}
			this.Sources.Clear();
			this.textures.Clear();
		}

		// Token: 0x0400056D RID: 1389
		public List<VirtualTexture> Sources;

		// Token: 0x0400056E RID: 1390
		private Dictionary<string, MTexture> textures = new Dictionary<string, MTexture>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0400056F RID: 1391
		private Dictionary<string, List<MTexture>> orderedTexturesCache = new Dictionary<string, List<MTexture>>();

		// Token: 0x04000570 RID: 1392
		private Dictionary<string, string> links = new Dictionary<string, string>();

		// Token: 0x020003A6 RID: 934
		public enum AtlasDataFormat
		{
			// Token: 0x04001F15 RID: 7957
			TexturePacker_Sparrow,
			// Token: 0x04001F16 RID: 7958
			CrunchXml,
			// Token: 0x04001F17 RID: 7959
			CrunchBinary,
			// Token: 0x04001F18 RID: 7960
			CrunchXmlOrBinary,
			// Token: 0x04001F19 RID: 7961
			CrunchBinaryNoAtlas,
			// Token: 0x04001F1A RID: 7962
			Packer,
			// Token: 0x04001F1B RID: 7963
			PackerNoAtlas
		}
	}
}

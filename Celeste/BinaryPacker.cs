using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Celeste
{
	// Token: 0x0200024E RID: 590
	public static class BinaryPacker
	{
		// Token: 0x06001285 RID: 4741 RVA: 0x00061F74 File Offset: 0x00060174
		public static void ToBinary(string filename, string outdir = null)
		{
			string extension = Path.GetExtension(filename);
			if (outdir != null)
			{
				Path.Combine(new string[]
				{
					outdir + Path.GetFileName(filename)
				});
			}
			filename.Replace(extension, BinaryPacker.OutputFileExtension);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			XmlElement rootElement = null;
			foreach (object obj in xmlDocument.ChildNodes)
			{
				if (obj is XmlElement)
				{
					rootElement = (obj as XmlElement);
					break;
				}
			}
			BinaryPacker.ToBinary(rootElement, outdir);
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x0006201C File Offset: 0x0006021C
		public static void ToBinary(XmlElement rootElement, string outfilename)
		{
			BinaryPacker.stringValue.Clear();
			BinaryPacker.stringCounter = 0;
			BinaryPacker.CreateLookupTable(rootElement);
			BinaryPacker.AddLookupValue(BinaryPacker.InnerTextAttributeName);
			using (FileStream fileStream = new FileStream(outfilename, FileMode.Create))
			{
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write("CELESTE MAP");
				binaryWriter.Write(Path.GetFileNameWithoutExtension(outfilename));
				binaryWriter.Write((short)BinaryPacker.stringValue.Count);
				foreach (KeyValuePair<string, short> keyValuePair in BinaryPacker.stringValue)
				{
					binaryWriter.Write(keyValuePair.Key);
				}
				BinaryPacker.WriteElement(binaryWriter, rootElement);
				binaryWriter.Flush();
			}
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x000620F0 File Offset: 0x000602F0
		private static void CreateLookupTable(XmlElement element)
		{
			BinaryPacker.AddLookupValue(element.Name);
			foreach (object obj in element.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!BinaryPacker.IgnoreAttributes.Contains(xmlAttribute.Name))
				{
					BinaryPacker.AddLookupValue(xmlAttribute.Name);
					byte b;
					object obj2;
					if (BinaryPacker.ParseValue(xmlAttribute.Value, out b, out obj2) && b == 5)
					{
						BinaryPacker.AddLookupValue(xmlAttribute.Value);
					}
				}
			}
			foreach (object obj3 in element.ChildNodes)
			{
				if (obj3 is XmlElement)
				{
					BinaryPacker.CreateLookupTable(obj3 as XmlElement);
				}
			}
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x000621E4 File Offset: 0x000603E4
		private static void AddLookupValue(string name)
		{
			if (!BinaryPacker.stringValue.ContainsKey(name))
			{
				BinaryPacker.stringValue.Add(name, BinaryPacker.stringCounter);
				BinaryPacker.stringCounter += 1;
			}
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x00062210 File Offset: 0x00060410
		private static void WriteElement(BinaryWriter writer, XmlElement element)
		{
			int num = 0;
			using (IEnumerator enumerator = element.ChildNodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is XmlElement)
					{
						num++;
					}
				}
			}
			int num2 = 0;
			foreach (object obj in element.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (!BinaryPacker.IgnoreAttributes.Contains(xmlAttribute.Name))
				{
					num2++;
				}
			}
			if (element.InnerText.Length > 0 && num == 0)
			{
				num2++;
			}
			writer.Write(BinaryPacker.stringValue[element.Name]);
			writer.Write((byte)num2);
			foreach (object obj2 in element.Attributes)
			{
				XmlAttribute xmlAttribute2 = (XmlAttribute)obj2;
				if (!BinaryPacker.IgnoreAttributes.Contains(xmlAttribute2.Name))
				{
					byte value;
					object obj3;
					BinaryPacker.ParseValue(xmlAttribute2.Value, out value, out obj3);
					writer.Write(BinaryPacker.stringValue[xmlAttribute2.Name]);
					writer.Write(value);
					switch (value)
					{
					case 0:
						writer.Write((bool)obj3);
						break;
					case 1:
						writer.Write((byte)obj3);
						break;
					case 2:
						writer.Write((short)obj3);
						break;
					case 3:
						writer.Write((int)obj3);
						break;
					case 4:
						writer.Write((float)obj3);
						break;
					case 5:
						writer.Write(BinaryPacker.stringValue[(string)obj3]);
						break;
					}
				}
			}
			if (element.InnerText.Length > 0 && num == 0)
			{
				writer.Write(BinaryPacker.stringValue[BinaryPacker.InnerTextAttributeName]);
				if (element.Name == "solids" || element.Name == "bg")
				{
					byte[] array = RunLengthEncoding.Encode(element.InnerText);
					writer.Write(7);
					writer.Write((short)array.Length);
					writer.Write(array);
				}
				else
				{
					writer.Write(6);
					writer.Write(element.InnerText);
				}
			}
			writer.Write((short)num);
			foreach (object obj4 in element.ChildNodes)
			{
				if (obj4 is XmlElement)
				{
					BinaryPacker.WriteElement(writer, obj4 as XmlElement);
				}
			}
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x000624EC File Offset: 0x000606EC
		private static bool ParseValue(string value, out byte type, out object result)
		{
			bool flag;
			byte b;
			short num;
			int num2;
			float num3;
			if (bool.TryParse(value, out flag))
			{
				type = 0;
				result = flag;
			}
			else if (byte.TryParse(value, out b))
			{
				type = 1;
				result = b;
			}
			else if (short.TryParse(value, out num))
			{
				type = 2;
				result = num;
			}
			else if (int.TryParse(value, out num2))
			{
				type = 3;
				result = num2;
			}
			else if (float.TryParse(value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num3))
			{
				type = 4;
				result = num3;
			}
			else
			{
				type = 5;
				result = value;
			}
			return true;
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x0006257C File Offset: 0x0006077C
		public static BinaryPacker.Element FromBinary(string filename)
		{
			BinaryPacker.Element element;
			using (FileStream fileStream = File.OpenRead(filename))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				binaryReader.ReadString();
				string package = binaryReader.ReadString();
				short num = binaryReader.ReadInt16();
				BinaryPacker.stringLookup = new string[(int)num];
				for (int i = 0; i < (int)num; i++)
				{
					BinaryPacker.stringLookup[i] = binaryReader.ReadString();
				}
				element = BinaryPacker.ReadElement(binaryReader);
				element.Package = package;
			}
			return element;
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x00062604 File Offset: 0x00060804
		private static BinaryPacker.Element ReadElement(BinaryReader reader)
		{
			BinaryPacker.Element element = new BinaryPacker.Element();
			element.Name = BinaryPacker.stringLookup[(int)reader.ReadInt16()];
			byte b = reader.ReadByte();
			if (b > 0)
			{
				element.Attributes = new Dictionary<string, object>();
			}
			for (int i = 0; i < (int)b; i++)
			{
				string key = BinaryPacker.stringLookup[(int)reader.ReadInt16()];
				byte b2 = reader.ReadByte();
				object value = null;
				if (b2 == 0)
				{
					value = reader.ReadBoolean();
				}
				else if (b2 == 1)
				{
					value = Convert.ToInt32(reader.ReadByte());
				}
				else if (b2 == 2)
				{
					value = Convert.ToInt32(reader.ReadInt16());
				}
				else if (b2 == 3)
				{
					value = reader.ReadInt32();
				}
				else if (b2 == 4)
				{
					value = reader.ReadSingle();
				}
				else if (b2 == 5)
				{
					value = BinaryPacker.stringLookup[(int)reader.ReadInt16()];
				}
				else if (b2 == 6)
				{
					value = reader.ReadString();
				}
				else if (b2 == 7)
				{
					short count = reader.ReadInt16();
					value = RunLengthEncoding.Decode(reader.ReadBytes((int)count));
				}
				element.Attributes.Add(key, value);
			}
			short num = reader.ReadInt16();
			if (num > 0)
			{
				element.Children = new List<BinaryPacker.Element>();
			}
			for (int j = 0; j < (int)num; j++)
			{
				element.Children.Add(BinaryPacker.ReadElement(reader));
			}
			return element;
		}

		// Token: 0x04000E65 RID: 3685
		public static readonly HashSet<string> IgnoreAttributes = new HashSet<string>
		{
			"_eid"
		};

		// Token: 0x04000E66 RID: 3686
		public static string InnerTextAttributeName = "innerText";

		// Token: 0x04000E67 RID: 3687
		public static string OutputFileExtension = ".bin";

		// Token: 0x04000E68 RID: 3688
		private static Dictionary<string, short> stringValue = new Dictionary<string, short>();

		// Token: 0x04000E69 RID: 3689
		private static string[] stringLookup;

		// Token: 0x04000E6A RID: 3690
		private static short stringCounter;

		// Token: 0x02000572 RID: 1394
		public class Element
		{
			// Token: 0x060026A9 RID: 9897 RVA: 0x000FE5AE File Offset: 0x000FC7AE
			public bool HasAttr(string name)
			{
				return this.Attributes != null && this.Attributes.ContainsKey(name);
			}

			// Token: 0x060026AA RID: 9898 RVA: 0x000FE5C8 File Offset: 0x000FC7C8
			public string Attr(string name, string defaultValue = "")
			{
				object obj;
				if (this.Attributes == null || !this.Attributes.TryGetValue(name, out obj))
				{
					obj = defaultValue;
				}
				return obj.ToString();
			}

			// Token: 0x060026AB RID: 9899 RVA: 0x000FE5F8 File Offset: 0x000FC7F8
			public bool AttrBool(string name, bool defaultValue = false)
			{
				object obj;
				if (this.Attributes == null || !this.Attributes.TryGetValue(name, out obj))
				{
					obj = defaultValue;
				}
				if (obj is bool)
				{
					return (bool)obj;
				}
				return bool.Parse(obj.ToString());
			}

			// Token: 0x060026AC RID: 9900 RVA: 0x000FE640 File Offset: 0x000FC840
			public float AttrFloat(string name, float defaultValue = 0f)
			{
				object obj;
				if (this.Attributes == null || !this.Attributes.TryGetValue(name, out obj))
				{
					obj = defaultValue;
				}
				if (obj is float)
				{
					return (float)obj;
				}
				return float.Parse(obj.ToString(), CultureInfo.InvariantCulture);
			}

			// Token: 0x0400268B RID: 9867
			public string Package;

			// Token: 0x0400268C RID: 9868
			public string Name;

			// Token: 0x0400268D RID: 9869
			public Dictionary<string, object> Attributes;

			// Token: 0x0400268E RID: 9870
			public List<BinaryPacker.Element> Children;
		}
	}
}

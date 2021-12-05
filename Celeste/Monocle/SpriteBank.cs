using System;
using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
	// Token: 0x02000109 RID: 265
	public class SpriteBank
	{
		// Token: 0x0600083C RID: 2108 RVA: 0x00011C2C File Offset: 0x0000FE2C
		public SpriteBank(Atlas atlas, XmlDocument xml)
		{
			this.Atlas = atlas;
			this.XML = xml;
			this.SpriteData = new Dictionary<string, SpriteData>(StringComparer.OrdinalIgnoreCase);
			Dictionary<string, XmlElement> dictionary = new Dictionary<string, XmlElement>();
			foreach (object obj in this.XML["Sprites"].ChildNodes)
			{
				if (obj is XmlElement)
				{
					XmlElement xmlElement = obj as XmlElement;
					dictionary.Add(xmlElement.Name, xmlElement);
					if (this.SpriteData.ContainsKey(xmlElement.Name))
					{
						throw new Exception("Duplicate sprite name in SpriteData: '" + xmlElement.Name + "'!");
					}
					SpriteData spriteData = this.SpriteData[xmlElement.Name] = new SpriteData(this.Atlas);
					if (xmlElement.HasAttr("copy"))
					{
						spriteData.Add(dictionary[xmlElement.Attr("copy")], xmlElement.Attr("path"));
					}
					spriteData.Add(xmlElement, null);
				}
			}
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x00011D64 File Offset: 0x0000FF64
		public SpriteBank(Atlas atlas, string xmlPath) : this(atlas, Calc.LoadContentXML(xmlPath))
		{
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00011D73 File Offset: 0x0000FF73
		public bool Has(string id)
		{
			return this.SpriteData.ContainsKey(id);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x00011D81 File Offset: 0x0000FF81
		public Sprite Create(string id)
		{
			if (this.SpriteData.ContainsKey(id))
			{
				return this.SpriteData[id].Create();
			}
			throw new Exception("Missing animation name in SpriteData: '" + id + "'!");
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x00011DB8 File Offset: 0x0000FFB8
		public Sprite CreateOn(Sprite sprite, string id)
		{
			if (this.SpriteData.ContainsKey(id))
			{
				return this.SpriteData[id].CreateOn(sprite);
			}
			throw new Exception("Missing animation name in SpriteData: '" + id + "'!");
		}

		// Token: 0x0400057C RID: 1404
		public Atlas Atlas;

		// Token: 0x0400057D RID: 1405
		public XmlDocument XML;

		// Token: 0x0400057E RID: 1406
		public Dictionary<string, SpriteData> SpriteData;
	}
}

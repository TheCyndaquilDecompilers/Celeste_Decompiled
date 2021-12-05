using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x0200010B RID: 267
	public class SpriteData
	{
		// Token: 0x06000842 RID: 2114 RVA: 0x00011DF0 File Offset: 0x0000FFF0
		public SpriteData(Atlas atlas)
		{
			this.Sprite = new Sprite(atlas, "");
			this.Atlas = atlas;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00011E1C File Offset: 0x0001001C
		public void Add(XmlElement xml, string overridePath = null)
		{
			SpriteDataSource spriteDataSource = new SpriteDataSource();
			spriteDataSource.XML = xml;
			spriteDataSource.Path = spriteDataSource.XML.Attr("path");
			spriteDataSource.OverridePath = overridePath;
			string text = "Sprite '" + spriteDataSource.XML.Name + "': ";
			if (!spriteDataSource.XML.HasAttr("path") && string.IsNullOrEmpty(overridePath))
			{
				throw new Exception(text + "'path' is missing!");
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (object obj in spriteDataSource.XML.GetElementsByTagName("Anim"))
			{
				XmlElement xml2 = (XmlElement)obj;
				this.CheckAnimXML(xml2, text, hashSet);
			}
			foreach (object obj2 in spriteDataSource.XML.GetElementsByTagName("Loop"))
			{
				XmlElement xml3 = (XmlElement)obj2;
				this.CheckAnimXML(xml3, text, hashSet);
			}
			if (spriteDataSource.XML.HasAttr("start") && !hashSet.Contains(spriteDataSource.XML.Attr("start")))
			{
				throw new Exception(text + "starting animation '" + spriteDataSource.XML.Attr("start") + "' is missing!");
			}
			if (spriteDataSource.XML.HasChild("Justify") && spriteDataSource.XML.HasChild("Origin"))
			{
				throw new Exception(text + "has both Origin and Justify tags!");
			}
			string str = spriteDataSource.XML.Attr("path", "");
			float defaultValue = spriteDataSource.XML.AttrFloat("delay", 0f);
			foreach (object obj3 in spriteDataSource.XML.GetElementsByTagName("Anim"))
			{
				XmlElement xml4 = (XmlElement)obj3;
				Chooser<string> into;
				if (xml4.HasAttr("goto"))
				{
					into = Chooser<string>.FromString<string>(xml4.Attr("goto"));
				}
				else
				{
					into = null;
				}
				string id = xml4.Attr("id");
				string text2 = xml4.Attr("path", "");
				int[] frames = Calc.ReadCSVIntWithTricks(xml4.Attr("frames", ""));
				if (!string.IsNullOrEmpty(overridePath) && this.HasFrames(this.Atlas, overridePath + text2, frames))
				{
					text2 = overridePath + text2;
				}
				else
				{
					text2 = str + text2;
				}
				this.Sprite.Add(id, text2, xml4.AttrFloat("delay", defaultValue), into, frames);
			}
			foreach (object obj4 in spriteDataSource.XML.GetElementsByTagName("Loop"))
			{
				XmlElement xml5 = (XmlElement)obj4;
				string id2 = xml5.Attr("id");
				string text3 = xml5.Attr("path", "");
				int[] frames2 = Calc.ReadCSVIntWithTricks(xml5.Attr("frames", ""));
				if (!string.IsNullOrEmpty(overridePath) && this.HasFrames(this.Atlas, overridePath + text3, frames2))
				{
					text3 = overridePath + text3;
				}
				else
				{
					text3 = str + text3;
				}
				this.Sprite.AddLoop(id2, text3, xml5.AttrFloat("delay", defaultValue), frames2);
			}
			if (spriteDataSource.XML.HasChild("Center"))
			{
				this.Sprite.CenterOrigin();
				this.Sprite.Justify = new Vector2?(new Vector2(0.5f, 0.5f));
			}
			else if (spriteDataSource.XML.HasChild("Justify"))
			{
				this.Sprite.JustifyOrigin(spriteDataSource.XML.ChildPosition("Justify"));
				this.Sprite.Justify = new Vector2?(spriteDataSource.XML.ChildPosition("Justify"));
			}
			else if (spriteDataSource.XML.HasChild("Origin"))
			{
				this.Sprite.Origin = spriteDataSource.XML.ChildPosition("Origin");
			}
			if (spriteDataSource.XML.HasChild("Position"))
			{
				this.Sprite.Position = spriteDataSource.XML.ChildPosition("Position");
			}
			if (spriteDataSource.XML.HasAttr("start"))
			{
				this.Sprite.Play(spriteDataSource.XML.Attr("start"), false, false);
			}
			this.Sources.Add(spriteDataSource);
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x0001231C File Offset: 0x0001051C
		private bool HasFrames(Atlas atlas, string path, int[] frames = null)
		{
			if (frames == null || frames.Length == 0)
			{
				return atlas.GetAtlasSubtexturesAt(path, 0) != null;
			}
			for (int i = 0; i < frames.Length; i++)
			{
				if (atlas.GetAtlasSubtexturesAt(path, frames[i]) == null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0001235C File Offset: 0x0001055C
		private void CheckAnimXML(XmlElement xml, string prefix, HashSet<string> ids)
		{
			if (!xml.HasAttr("id"))
			{
				throw new Exception(prefix + "'id' is missing on " + xml.Name + "!");
			}
			if (ids.Contains(xml.Attr("id")))
			{
				throw new Exception(prefix + "multiple animations with id '" + xml.Attr("id") + "'!");
			}
			ids.Add(xml.Attr("id"));
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x000123D8 File Offset: 0x000105D8
		public Sprite Create()
		{
			return this.Sprite.CreateClone();
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x000123E5 File Offset: 0x000105E5
		public Sprite CreateOn(Sprite sprite)
		{
			return this.Sprite.CloneInto(sprite);
		}

		// Token: 0x04000582 RID: 1410
		public List<SpriteDataSource> Sources = new List<SpriteDataSource>();

		// Token: 0x04000583 RID: 1411
		public Sprite Sprite;

		// Token: 0x04000584 RID: 1412
		public Atlas Atlas;
	}
}

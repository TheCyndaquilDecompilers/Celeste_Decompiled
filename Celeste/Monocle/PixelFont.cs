using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x0200012F RID: 303
	public class PixelFont
	{
		// Token: 0x06000AEB RID: 2795 RVA: 0x0001DC25 File Offset: 0x0001BE25
		public PixelFont(string face)
		{
			this.Face = face;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x0001DC4C File Offset: 0x0001BE4C
		public PixelFontSize AddFontSize(string path, Atlas atlas = null, bool outline = false)
		{
			XmlElement data = Calc.LoadXML(path)["font"];
			return this.AddFontSize(path, data, atlas, outline);
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x0001DC74 File Offset: 0x0001BE74
		public PixelFontSize AddFontSize(string path, XmlElement data, Atlas atlas = null, bool outline = false)
		{
			float num = data["info"].AttrFloat("size");
			foreach (PixelFontSize pixelFontSize in this.Sizes)
			{
				if (pixelFontSize.Size == num)
				{
					return pixelFontSize;
				}
			}
			List<MTexture> list = new List<MTexture>();
			foreach (object obj in data["pages"])
			{
				string text = ((XmlElement)obj).Attr("file");
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (atlas != null && atlas.Has(fileNameWithoutExtension))
				{
					list.Add(atlas[fileNameWithoutExtension]);
				}
				else
				{
					VirtualTexture virtualTexture = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path).Substring(Engine.ContentDirectory.Length + 1), text));
					list.Add(new MTexture(virtualTexture));
					this.managedTextures.Add(virtualTexture);
				}
			}
			PixelFontSize pixelFontSize2 = new PixelFontSize
			{
				Textures = list,
				Characters = new Dictionary<int, PixelFontCharacter>(),
				LineHeight = data["common"].AttrInt("lineHeight"),
				Size = num,
				Outline = outline
			};
			foreach (object obj2 in data["chars"])
			{
				XmlElement xml = (XmlElement)obj2;
				int num2 = xml.AttrInt("id");
				int index = xml.AttrInt("page", 0);
				pixelFontSize2.Characters.Add(num2, new PixelFontCharacter(num2, list[index], xml));
			}
			if (data["kernings"] != null)
			{
				foreach (object obj3 in data["kernings"])
				{
					XmlElement xml2 = (XmlElement)obj3;
					int key = xml2.AttrInt("first");
					int key2 = xml2.AttrInt("second");
					int value = xml2.AttrInt("amount");
					PixelFontCharacter pixelFontCharacter = null;
					if (pixelFontSize2.Characters.TryGetValue(key, out pixelFontCharacter))
					{
						pixelFontCharacter.Kerning.Add(key2, value);
					}
				}
			}
			this.Sizes.Add(pixelFontSize2);
			this.Sizes.Sort((PixelFontSize a, PixelFontSize b) => Math.Sign(a.Size - b.Size));
			return pixelFontSize2;
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x0001DF5C File Offset: 0x0001C15C
		public PixelFontSize Get(float size)
		{
			int i = 0;
			int num = this.Sizes.Count - 1;
			while (i < num)
			{
				if (this.Sizes[i].Size >= size)
				{
					return this.Sizes[i];
				}
				i++;
			}
			return this.Sizes[this.Sizes.Count - 1];
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x0001DFBC File Offset: 0x0001C1BC
		public bool Has(float size)
		{
			int i = 0;
			int num = this.Sizes.Count - 1;
			while (i < num)
			{
				if (this.Sizes[i].Size == size)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x0001DFFC File Offset: 0x0001C1FC
		public void Draw(float baseSize, char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
		{
			PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
			scale *= baseSize / pixelFontSize.Size;
			pixelFontSize.Draw(character, position, justify, scale, color);
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x0001E044 File Offset: 0x0001C244
		public void Draw(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke, Color strokeColor)
		{
			PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
			scale *= baseSize / pixelFontSize.Size;
			pixelFontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x0001E094 File Offset: 0x0001C294
		public void Draw(float baseSize, string text, Vector2 position, Color color)
		{
			Vector2 vector = Vector2.One;
			PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(vector.X, vector.Y));
			vector *= baseSize / pixelFontSize.Size;
			pixelFontSize.Draw(text, position, Vector2.Zero, Vector2.One, color, 0f, Color.Transparent, 0f, Color.Transparent);
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x0001E0F8 File Offset: 0x0001C2F8
		public void Draw(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
		{
			PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
			scale *= baseSize / pixelFontSize.Size;
			pixelFontSize.Draw(text, position, justify, scale, color, 0f, Color.Transparent, 0f, Color.Transparent);
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x0001E154 File Offset: 0x0001C354
		public void DrawOutline(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float stroke, Color strokeColor)
		{
			PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
			scale *= baseSize / pixelFontSize.Size;
			pixelFontSize.Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x0001E1AC File Offset: 0x0001C3AC
		public void DrawEdgeOutline(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke = 0f, Color strokeColor = default(Color))
		{
			PixelFontSize pixelFontSize = this.Get(baseSize * Math.Max(scale.X, scale.Y));
			scale *= baseSize / pixelFontSize.Size;
			pixelFontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x0001E1FC File Offset: 0x0001C3FC
		public void Dispose()
		{
			foreach (VirtualTexture virtualTexture in this.managedTextures)
			{
				virtualTexture.Dispose();
			}
			this.Sizes.Clear();
		}

		// Token: 0x04000693 RID: 1683
		public string Face;

		// Token: 0x04000694 RID: 1684
		public List<PixelFontSize> Sizes = new List<PixelFontSize>();

		// Token: 0x04000695 RID: 1685
		private List<VirtualTexture> managedTextures = new List<VirtualTexture>();
	}
}

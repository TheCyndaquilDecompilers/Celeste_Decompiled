using System;
using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
	// Token: 0x0200012D RID: 301
	public class PixelFontCharacter
	{
		// Token: 0x06000ADD RID: 2781 RVA: 0x0001D278 File Offset: 0x0001B478
		public PixelFontCharacter(int character, MTexture texture, XmlElement xml)
		{
			this.Character = character;
			this.Texture = texture.GetSubtexture(xml.AttrInt("x"), xml.AttrInt("y"), xml.AttrInt("width"), xml.AttrInt("height"), null);
			this.XOffset = xml.AttrInt("xoffset");
			this.YOffset = xml.AttrInt("yoffset");
			this.XAdvance = xml.AttrInt("xadvance");
		}

		// Token: 0x04000687 RID: 1671
		public int Character;

		// Token: 0x04000688 RID: 1672
		public MTexture Texture;

		// Token: 0x04000689 RID: 1673
		public int XOffset;

		// Token: 0x0400068A RID: 1674
		public int YOffset;

		// Token: 0x0400068B RID: 1675
		public int XAdvance;

		// Token: 0x0400068C RID: 1676
		public Dictionary<int, int> Kerning = new Dictionary<int, int>();
	}
}

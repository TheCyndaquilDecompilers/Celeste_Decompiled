using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001C0 RID: 448
	public class BridgeFixed : Solid
	{
		// Token: 0x06000F5D RID: 3933 RVA: 0x0003E6CC File Offset: 0x0003C8CC
		public BridgeFixed(EntityData data, Vector2 offset) : base(data.Position + offset, (float)data.Width, 8f, true)
		{
			MTexture mtexture = GFX.Game["scenery/bridge_fixed"];
			int num = 0;
			while ((float)num < base.Width)
			{
				Rectangle rectangle = new Rectangle(0, 0, mtexture.Width, mtexture.Height);
				if ((float)(num + rectangle.Width) > base.Width)
				{
					rectangle.Width = (int)base.Width - num;
				}
				base.Add(new Image(mtexture)
				{
					Position = new Vector2((float)num, -8f)
				});
				num += mtexture.Width;
			}
		}
	}
}

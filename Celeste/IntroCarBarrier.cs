using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000343 RID: 835
	public class IntroCarBarrier : Entity
	{
		// Token: 0x06001A55 RID: 6741 RVA: 0x000A94F0 File Offset: 0x000A76F0
		public IntroCarBarrier(Vector2 position, int depth, Color color)
		{
			this.Position = position;
			base.Depth = depth;
			Image image = new Image(GFX.Game["scenery/car/barrier"]);
			image.Origin = new Vector2(0f, image.Height);
			image.Color = color;
			base.Add(image);
		}
	}
}

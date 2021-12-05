using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000345 RID: 837
	public class IntroPavement : Solid
	{
		// Token: 0x06001A5D RID: 6749 RVA: 0x000A9686 File Offset: 0x000A7886
		public IntroPavement(Vector2 position, int width) : base(position, (float)width, 8f, true)
		{
			this.columns = width / 8;
			base.Depth = -10;
			this.SurfaceSoundIndex = 1;
			this.SurfaceSoundPriority = 10;
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x000A96B8 File Offset: 0x000A78B8
		public override void Awake(Scene scene)
		{
			for (int i = 0; i < this.columns; i++)
			{
				int num;
				if (i < this.columns - 2)
				{
					num = Calc.Random.Next(0, 2);
				}
				else if (i == this.columns - 2)
				{
					num = 2;
				}
				else
				{
					num = 3;
				}
				base.Add(new Image(GFX.Game["scenery/car/pavement"].GetSubtexture(num * 8, 0, 8, 8, null))
				{
					Position = new Vector2((float)(i * 8), 0f)
				});
			}
		}

		// Token: 0x040016F3 RID: 5875
		private int columns;
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200028F RID: 655
	public class Clothesline : Entity
	{
		// Token: 0x06001455 RID: 5205 RVA: 0x0006E974 File Offset: 0x0006CB74
		public Clothesline(Vector2 from, Vector2 to)
		{
			base.Depth = 8999;
			this.Position = from;
			base.Add(new Flagline(to, Clothesline.lineColor, Clothesline.pinColor, Clothesline.colors, 8, 20, 8, 16, 2, 8));
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0006E9BC File Offset: 0x0006CBBC
		public Clothesline(EntityData data, Vector2 offset) : this(data.Position + offset, data.Nodes[0] + offset)
		{
		}

		// Token: 0x04001008 RID: 4104
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("0d2e6b"),
			Calc.HexToColor("3d2688"),
			Calc.HexToColor("4f6e9d"),
			Calc.HexToColor("47194a")
		};

		// Token: 0x04001009 RID: 4105
		private static readonly Color lineColor = Color.Lerp(Color.Gray, Color.DarkBlue, 0.25f);

		// Token: 0x0400100A RID: 4106
		private static readonly Color pinColor = Color.Gray;
	}
}

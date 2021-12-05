using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200028E RID: 654
	public class CliffFlags : Entity
	{
		// Token: 0x06001452 RID: 5202 RVA: 0x0006E874 File Offset: 0x0006CA74
		public CliffFlags(Vector2 from, Vector2 to)
		{
			base.Depth = 8999;
			this.Position = from;
			Flagline flagline;
			base.Add(flagline = new Flagline(to, CliffFlags.lineColor, CliffFlags.pinColor, CliffFlags.colors, 10, 10, 10, 10, 2, 8));
			flagline.ClothDroopAmount = 0.2f;
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0006E8CB File Offset: 0x0006CACB
		public CliffFlags(EntityData data, Vector2 offset) : this(data.Position + offset, data.Nodes[0] + offset)
		{
		}

		// Token: 0x04001005 RID: 4101
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("d85f2f"),
			Calc.HexToColor("d82f63"),
			Calc.HexToColor("2fd8a2"),
			Calc.HexToColor("d8d62f")
		};

		// Token: 0x04001006 RID: 4102
		private static readonly Color lineColor = Color.Lerp(Color.Gray, Color.DarkBlue, 0.25f);

		// Token: 0x04001007 RID: 4103
		private static readonly Color pinColor = Color.Gray;
	}
}

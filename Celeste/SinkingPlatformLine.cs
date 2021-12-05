using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000334 RID: 820
	public class SinkingPlatformLine : Entity
	{
		// Token: 0x060019B7 RID: 6583 RVA: 0x000A5DAA File Offset: 0x000A3FAA
		public SinkingPlatformLine(Vector2 position)
		{
			this.Position = position;
			base.Depth = 9001;
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x000A5DE4 File Offset: 0x000A3FE4
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.height = (float)base.SceneAs<Level>().Bounds.Height - (base.Y - (float)base.SceneAs<Level>().Bounds.Y);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x000A5E20 File Offset: 0x000A4020
		public override void Render()
		{
			Draw.Rect(base.X - 1f, base.Y, 3f, this.height, this.lineEdgeColor);
			Draw.Rect(base.X, base.Y + 1f, 1f, this.height, this.lineInnerColor);
		}

		// Token: 0x0400167A RID: 5754
		private Color lineEdgeColor = Calc.HexToColor("2a1923");

		// Token: 0x0400167B RID: 5755
		private Color lineInnerColor = Calc.HexToColor("160b12");

		// Token: 0x0400167C RID: 5756
		private float height;
	}
}

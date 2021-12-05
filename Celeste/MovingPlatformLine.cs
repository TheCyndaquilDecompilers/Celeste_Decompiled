using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000325 RID: 805
	public class MovingPlatformLine : Entity
	{
		// Token: 0x06001960 RID: 6496 RVA: 0x000A2F1C File Offset: 0x000A111C
		public MovingPlatformLine(Vector2 position, Vector2 end)
		{
			this.Position = position;
			base.Depth = 9001;
			this.end = end;
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x000A2F40 File Offset: 0x000A1140
		public override void Added(Scene scene)
		{
			if ((scene as Level).Session.Area.ID == 4)
			{
				this.lineEdgeColor = Calc.HexToColor("a4464a");
				this.lineInnerColor = Calc.HexToColor("86354e");
			}
			else
			{
				this.lineEdgeColor = Calc.HexToColor("2a1923");
				this.lineInnerColor = Calc.HexToColor("160b12");
			}
			base.Added(scene);
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x000A2FB0 File Offset: 0x000A11B0
		public override void Render()
		{
			Vector2 vector = (this.end - this.Position).SafeNormalize();
			Vector2 value = new Vector2(-vector.Y, vector.X);
			Draw.Line(this.Position - vector - value, this.end + vector - value, this.lineEdgeColor);
			Draw.Line(this.Position - vector, this.end + vector, this.lineEdgeColor);
			Draw.Line(this.Position - vector + value, this.end + vector + value, this.lineEdgeColor);
			Draw.Line(this.Position, this.end, this.lineInnerColor);
		}

		// Token: 0x04001619 RID: 5657
		private Color lineEdgeColor;

		// Token: 0x0400161A RID: 5658
		private Color lineInnerColor;

		// Token: 0x0400161B RID: 5659
		private Vector2 end;
	}
}

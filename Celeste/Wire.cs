using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000349 RID: 841
	public class Wire : Entity
	{
		// Token: 0x06001A68 RID: 6760 RVA: 0x000AA004 File Offset: 0x000A8204
		public Wire(Vector2 from, Vector2 to, bool above)
		{
			this.Curve = new SimpleCurve(from, to, Vector2.Zero);
			base.Depth = (above ? -8500 : 2000);
			Random random = new Random((int)Math.Min(from.X, to.X));
			this.sineX = random.NextFloat(4f);
			this.sineY = random.NextFloat(4f);
		}

		// Token: 0x06001A69 RID: 6761 RVA: 0x000AA088 File Offset: 0x000A8288
		public Wire(EntityData data, Vector2 offset) : this(data.Position + offset, data.Nodes[0] + offset, data.Bool("above", false))
		{
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x000AA0BC File Offset: 0x000A82BC
		public override void Render()
		{
			Level level = base.SceneAs<Level>();
			Vector2 value = new Vector2((float)Math.Sin((double)(this.sineX + level.WindSineTimer * 2f)), (float)Math.Sin((double)(this.sineY + level.WindSineTimer * 2.8f))) * 8f * level.VisualWind;
			this.Curve.Control = (this.Curve.Begin + this.Curve.End) / 2f + new Vector2(0f, 24f) + value;
			Vector2 start = this.Curve.Begin;
			for (int i = 1; i <= 16; i++)
			{
				float percent = (float)i / 16f;
				Vector2 point = this.Curve.GetPoint(percent);
				Draw.Line(start, point, this.Color);
				start = point;
			}
		}

		// Token: 0x04001705 RID: 5893
		public Color Color = Calc.HexToColor("595866");

		// Token: 0x04001706 RID: 5894
		public SimpleCurve Curve;

		// Token: 0x04001707 RID: 5895
		private float sineX;

		// Token: 0x04001708 RID: 5896
		private float sineY;
	}
}

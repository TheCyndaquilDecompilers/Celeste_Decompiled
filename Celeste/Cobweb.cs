using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002C5 RID: 709
	public class Cobweb : Entity
	{
		// Token: 0x060015FB RID: 5627 RVA: 0x0007FEB0 File Offset: 0x0007E0B0
		public Cobweb(EntityData data, Vector2 offset)
		{
			base.Depth = -1;
			this.anchorA = (this.Position = data.Position + offset);
			this.anchorB = data.Nodes[0] + offset;
			foreach (Vector2 value in data.Nodes)
			{
				if (this.offshoots == null)
				{
					this.offshoots = new List<Vector2>();
					this.offshootEndings = new List<float>();
				}
				else
				{
					this.offshoots.Add(value + offset);
					this.offshootEndings.Add(0.3f + Calc.Random.NextFloat(0.4f));
				}
			}
			this.waveTimer = Calc.Random.NextFloat();
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x0007FF7C File Offset: 0x0007E17C
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.color = Calc.Random.Choose(AreaData.Get(scene).CobwebColor);
			this.edge = Color.Lerp(this.color, Calc.HexToColor("0f0e17"), 0.2f);
			if (!base.Scene.CollideCheck<Solid>(new Rectangle((int)this.anchorA.X - 2, (int)this.anchorA.Y - 2, 4, 4)) || !base.Scene.CollideCheck<Solid>(new Rectangle((int)this.anchorB.X - 2, (int)this.anchorB.Y - 2, 4, 4)))
			{
				base.RemoveSelf();
			}
			for (int i = 0; i < this.offshoots.Count; i++)
			{
				Vector2 vector = this.offshoots[i];
				if (!base.Scene.CollideCheck<Solid>(new Rectangle((int)vector.X - 2, (int)vector.Y - 2, 4, 4)))
				{
					this.offshoots.RemoveAt(i);
					this.offshootEndings.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00080096 File Offset: 0x0007E296
		public override void Update()
		{
			this.waveTimer += Engine.DeltaTime;
			base.Update();
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x000800B0 File Offset: 0x0007E2B0
		public override void Render()
		{
			this.DrawCobweb(this.anchorA, this.anchorB, 12, true);
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x000800C8 File Offset: 0x0007E2C8
		private void DrawCobweb(Vector2 a, Vector2 b, int steps, bool drawOffshoots)
		{
			SimpleCurve simpleCurve = new SimpleCurve(a, b, (a + b) / 2f + Vector2.UnitY * (8f + (float)Math.Sin((double)this.waveTimer) * 4f));
			if (drawOffshoots && this.offshoots != null)
			{
				for (int i = 0; i < this.offshoots.Count; i++)
				{
					this.DrawCobweb(this.offshoots[i], simpleCurve.GetPoint(this.offshootEndings[i]), 4, false);
				}
			}
			Vector2 vector = simpleCurve.Begin;
			for (int j = 1; j <= steps; j++)
			{
				float percent = (float)j / (float)steps;
				Vector2 point = simpleCurve.GetPoint(percent);
				Draw.Line(vector, point, (j <= 2 || j >= steps - 1) ? this.edge : this.color);
				vector = point + (vector - point).SafeNormalize();
			}
		}

		// Token: 0x04001241 RID: 4673
		private Color color;

		// Token: 0x04001242 RID: 4674
		private Color edge;

		// Token: 0x04001243 RID: 4675
		private Vector2 anchorA;

		// Token: 0x04001244 RID: 4676
		private Vector2 anchorB;

		// Token: 0x04001245 RID: 4677
		private List<Vector2> offshoots;

		// Token: 0x04001246 RID: 4678
		private List<float> offshootEndings;

		// Token: 0x04001247 RID: 4679
		private float waveTimer;
	}
}

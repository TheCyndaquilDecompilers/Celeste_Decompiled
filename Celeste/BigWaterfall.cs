using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001B1 RID: 433
	public class BigWaterfall : Entity
	{
		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0003C342 File Offset: 0x0003A542
		private Vector2 RenderPosition
		{
			get
			{
				return this.RenderPositionAtCamera((base.Scene as Level).Camera.Position + new Vector2(160f, 90f));
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0003C374 File Offset: 0x0003A574
		public BigWaterfall(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			base.Tag = Tags.TransitionUpdate;
			this.layer = data.Enum<BigWaterfall.Layers>("layer", BigWaterfall.Layers.BG);
			this.width = (float)data.Width;
			this.height = (float)data.Height;
			if (this.layer == BigWaterfall.Layers.FG)
			{
				base.Depth = -49900;
				this.parallax = 0.1f + Calc.Random.NextFloat() * 0.2f;
				this.surfaceColor = Water.SurfaceColor;
				this.fillColor = Water.FillColor;
				base.Add(new DisplacementRenderHook(new Action(this.RenderDisplacement)));
				this.lines.Add(3f);
				this.lines.Add(this.width - 4f);
				base.Add(this.loopingSfx = new SoundSource());
				this.loopingSfx.Play("event:/env/local/waterfall_big_main", null, 0f);
			}
			else
			{
				base.Depth = 10010;
				this.parallax = -(0.7f + Calc.Random.NextFloat() * 0.2f);
				this.surfaceColor = Calc.HexToColor("89dbf0") * 0.5f;
				this.fillColor = Calc.HexToColor("29a7ea") * 0.3f;
				this.lines.Add(6f);
				this.lines.Add(this.width - 7f);
			}
			this.fade = 1f;
			base.Add(new TransitionListener
			{
				OnIn = delegate(float f)
				{
					this.fade = f;
				},
				OnOut = delegate(float f)
				{
					this.fade = 1f - f;
				}
			});
			if (this.width > 16f)
			{
				int num = Calc.Random.Next((int)(this.width / 16f));
				for (int i = 0; i < num; i++)
				{
					this.lines.Add(8f + Calc.Random.NextFloat(this.width - 16f));
				}
			}
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0003C5A7 File Offset: 0x0003A7A7
		public override void Added(Scene scene)
		{
			base.Added(scene);
			if ((base.Scene as Level).Transitioning)
			{
				this.fade = 0f;
			}
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0003C5D0 File Offset: 0x0003A7D0
		public Vector2 RenderPositionAtCamera(Vector2 camera)
		{
			Vector2 value = this.Position + new Vector2(this.width, this.height) / 2f - camera;
			Vector2 vector = Vector2.Zero;
			if (this.layer == BigWaterfall.Layers.BG)
			{
				vector -= value * 0.6f;
			}
			else if (this.layer == BigWaterfall.Layers.FG)
			{
				vector += value * 0.2f;
			}
			return this.Position + vector;
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0003C653 File Offset: 0x0003A853
		public void RenderDisplacement()
		{
			Draw.Rect(this.RenderPosition.X, base.Y, this.width, this.height, new Color(0.5f, 0.5f, 1f, 1f));
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0003C690 File Offset: 0x0003A890
		public override void Update()
		{
			this.sine += Engine.DeltaTime;
			if (this.loopingSfx != null)
			{
				Vector2 position = (base.Scene as Level).Camera.Position;
				this.loopingSfx.Position = new Vector2(this.RenderPosition.X - base.X, Calc.Clamp(position.Y + 90f, base.Y, this.height) - base.Y);
			}
			base.Update();
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0003C71C File Offset: 0x0003A91C
		public override void Render()
		{
			float x = this.RenderPosition.X;
			Color color = this.fillColor * this.fade;
			Color color2 = this.surfaceColor * this.fade;
			Draw.Rect(x, base.Y, this.width, this.height, color);
			if (this.layer == BigWaterfall.Layers.FG)
			{
				Draw.Rect(x - 1f, base.Y, 3f, this.height, color2);
				Draw.Rect(x + this.width - 2f, base.Y, 3f, this.height, color2);
				using (List<float>.Enumerator enumerator = this.lines.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						float num = enumerator.Current;
						Draw.Rect(x + num, base.Y, 1f, this.height, color2);
					}
					return;
				}
			}
			Vector2 position = (base.Scene as Level).Camera.Position;
			int num2 = 3;
			float num3 = Math.Max(base.Y, (float)Math.Floor((double)(position.Y / (float)num2)) * (float)num2);
			float num4 = Math.Min(base.Y + this.height, position.Y + 180f);
			for (float num5 = num3; num5 < num4; num5 += (float)num2)
			{
				int num6 = (int)(Math.Sin((double)(num5 / 6f - this.sine * 8f)) * 2.0);
				Draw.Rect(x, num5, (float)(4 + num6), (float)num2, color2);
				Draw.Rect(x + this.width - 4f + (float)num6, num5, (float)(4 - num6), (float)num2, color2);
				foreach (float num7 in this.lines)
				{
					Draw.Rect(x + (float)num6 + num7, num5, 1f, (float)num2, color2);
				}
			}
		}

		// Token: 0x04000A7A RID: 2682
		private BigWaterfall.Layers layer;

		// Token: 0x04000A7B RID: 2683
		private float width;

		// Token: 0x04000A7C RID: 2684
		private float height;

		// Token: 0x04000A7D RID: 2685
		private float parallax;

		// Token: 0x04000A7E RID: 2686
		private List<float> lines = new List<float>();

		// Token: 0x04000A7F RID: 2687
		private Color surfaceColor;

		// Token: 0x04000A80 RID: 2688
		private Color fillColor;

		// Token: 0x04000A81 RID: 2689
		private float sine;

		// Token: 0x04000A82 RID: 2690
		private SoundSource loopingSfx;

		// Token: 0x04000A83 RID: 2691
		private float fade;

		// Token: 0x020004BF RID: 1215
		private enum Layers
		{
			// Token: 0x0400236A RID: 9066
			FG,
			// Token: 0x0400236B RID: 9067
			BG
		}
	}
}

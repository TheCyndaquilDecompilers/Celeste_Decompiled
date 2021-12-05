using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200014C RID: 332
	[Tracked(false)]
	public class GlassBlock : Solid
	{
		// Token: 0x06000C1C RID: 3100 RVA: 0x0002673C File Offset: 0x0002493C
		public GlassBlock(Vector2 position, float width, float height, bool sinks) : base(position, width, height, false)
		{
			this.sinks = sinks;
			this.startY = base.Y;
			base.Depth = -10000;
			base.Add(new LightOcclude(1f));
			base.Add(new MirrorSurface(null));
			this.SurfaceSoundIndex = 32;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x000267AC File Offset: 0x000249AC
		public GlassBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Bool("sinks", false))
		{
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x000267DC File Offset: 0x000249DC
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			int num = (int)base.Width / 8;
			int num2 = (int)base.Height / 8;
			this.AddSide(new Vector2(0f, 0f), new Vector2(0f, -1f), num);
			this.AddSide(new Vector2((float)(num - 1), 0f), new Vector2(1f, 0f), num2);
			this.AddSide(new Vector2((float)(num - 1), (float)(num2 - 1)), new Vector2(0f, 1f), num);
			this.AddSide(new Vector2(0f, (float)(num2 - 1)), new Vector2(-1f, 0f), num2);
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x000268A0 File Offset: 0x00024AA0
		private void AddSide(Vector2 start, Vector2 normal, int tiles)
		{
			Vector2 vector = new Vector2(-normal.Y, normal.X);
			for (int i = 0; i < tiles; i++)
			{
				if (this.Open(start + vector * (float)i + normal))
				{
					Vector2 value = (start + vector * (float)i) * 8f + new Vector2(4f) - vector * 4f + normal * 4f;
					if (!this.Open(start + vector * (float)(i - 1)))
					{
						value -= vector;
					}
					while (i < tiles && this.Open(start + vector * (float)i + normal))
					{
						i++;
					}
					Vector2 value2 = (start + vector * (float)i) * 8f + new Vector2(4f) - vector * 4f + normal * 4f;
					if (!this.Open(start + vector * (float)i))
					{
						value2 += vector;
					}
					this.lines.Add(new GlassBlock.Line(value + normal, value2 + normal));
				}
			}
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x00026A04 File Offset: 0x00024C04
		private bool Open(Vector2 tile)
		{
			Vector2 point = new Vector2(base.X + tile.X * 8f + 4f, base.Y + tile.Y * 8f + 4f);
			return !base.Scene.CollideCheck<SolidTiles>(point) && !base.Scene.CollideCheck<GlassBlock>(point);
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00026A6C File Offset: 0x00024C6C
		public override void Render()
		{
			foreach (GlassBlock.Line line in this.lines)
			{
				Draw.Line(this.Position + line.A, this.Position + line.B, this.lineColor);
			}
		}

		// Token: 0x04000774 RID: 1908
		private bool sinks;

		// Token: 0x04000775 RID: 1909
		private float startY;

		// Token: 0x04000776 RID: 1910
		private List<GlassBlock.Line> lines = new List<GlassBlock.Line>();

		// Token: 0x04000777 RID: 1911
		private Color lineColor = Color.White;

		// Token: 0x020003DC RID: 988
		private struct Line
		{
			// Token: 0x06001F43 RID: 8003 RVA: 0x000D857A File Offset: 0x000D677A
			public Line(Vector2 a, Vector2 b)
			{
				this.A = a;
				this.B = b;
			}

			// Token: 0x04001FD2 RID: 8146
			public Vector2 A;

			// Token: 0x04001FD3 RID: 8147
			public Vector2 B;
		}
	}
}

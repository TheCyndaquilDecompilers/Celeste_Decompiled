using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x020000F1 RID: 241
	public abstract class GraphicsComponent : Component
	{
		// Token: 0x0600061E RID: 1566 RVA: 0x00009235 File Offset: 0x00007435
		public GraphicsComponent(bool active) : base(active, true)
		{
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x00009255 File Offset: 0x00007455
		// (set) Token: 0x06000620 RID: 1568 RVA: 0x00009262 File Offset: 0x00007462
		public float X
		{
			get
			{
				return this.Position.X;
			}
			set
			{
				this.Position.X = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x00009270 File Offset: 0x00007470
		// (set) Token: 0x06000622 RID: 1570 RVA: 0x0000927D File Offset: 0x0000747D
		public float Y
		{
			get
			{
				return this.Position.Y;
			}
			set
			{
				this.Position.Y = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x0000928B File Offset: 0x0000748B
		// (set) Token: 0x06000624 RID: 1572 RVA: 0x00009298 File Offset: 0x00007498
		public bool FlipX
		{
			get
			{
				return (this.Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
			}
			set
			{
				this.Effects = (value ? (this.Effects | SpriteEffects.FlipHorizontally) : (this.Effects & ~SpriteEffects.FlipHorizontally));
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000625 RID: 1573 RVA: 0x000092B6 File Offset: 0x000074B6
		// (set) Token: 0x06000626 RID: 1574 RVA: 0x000092C3 File Offset: 0x000074C3
		public bool FlipY
		{
			get
			{
				return (this.Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
			}
			set
			{
				this.Effects = (value ? (this.Effects | SpriteEffects.FlipVertically) : (this.Effects & ~SpriteEffects.FlipVertically));
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x000092E1 File Offset: 0x000074E1
		// (set) Token: 0x06000628 RID: 1576 RVA: 0x00009308 File Offset: 0x00007508
		public Vector2 RenderPosition
		{
			get
			{
				return ((base.Entity == null) ? Vector2.Zero : base.Entity.Position) + this.Position;
			}
			set
			{
				this.Position = value - ((base.Entity == null) ? Vector2.Zero : base.Entity.Position);
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00009330 File Offset: 0x00007530
		public void DrawOutline(int offset = 1)
		{
			this.DrawOutline(Color.Black, offset);
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00009340 File Offset: 0x00007540
		public void DrawOutline(Color color, int offset = 1)
		{
			Vector2 position = this.Position;
			Color color2 = this.Color;
			this.Color = color;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						this.Position = position + new Vector2((float)(i * offset), (float)(j * offset));
						this.Render();
					}
				}
			}
			this.Position = position;
			this.Color = color2;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x000093B0 File Offset: 0x000075B0
		public void DrawSimpleOutline()
		{
			Vector2 position = this.Position;
			Color color = this.Color;
			this.Color = Color.Black;
			this.Position = position + new Vector2(-1f, 0f);
			this.Render();
			this.Position = position + new Vector2(0f, -1f);
			this.Render();
			this.Position = position + new Vector2(1f, 0f);
			this.Render();
			this.Position = position + new Vector2(0f, 1f);
			this.Render();
			this.Position = position;
			this.Color = color;
		}

		// Token: 0x040004AD RID: 1197
		public Vector2 Position;

		// Token: 0x040004AE RID: 1198
		public Vector2 Origin;

		// Token: 0x040004AF RID: 1199
		public Vector2 Scale = Vector2.One;

		// Token: 0x040004B0 RID: 1200
		public float Rotation;

		// Token: 0x040004B1 RID: 1201
		public Color Color = Color.White;

		// Token: 0x040004B2 RID: 1202
		public SpriteEffects Effects;
	}
}

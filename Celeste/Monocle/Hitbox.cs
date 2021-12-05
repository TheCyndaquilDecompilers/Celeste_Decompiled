using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000EF RID: 239
	public class Hitbox : Collider
	{
		// Token: 0x060005EE RID: 1518 RVA: 0x00008E38 File Offset: 0x00007038
		public Hitbox(float width, float height, float x = 0f, float y = 0f)
		{
			this.width = width;
			this.height = height;
			this.Position.X = x;
			this.Position.Y = y;
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x00008E67 File Offset: 0x00007067
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x00008E6F File Offset: 0x0000706F
		public override float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x00008E78 File Offset: 0x00007078
		// (set) Token: 0x060005F2 RID: 1522 RVA: 0x00008E80 File Offset: 0x00007080
		public override float Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x00008951 File Offset: 0x00006B51
		// (set) Token: 0x060005F4 RID: 1524 RVA: 0x0000895E File Offset: 0x00006B5E
		public override float Left
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x0000896C File Offset: 0x00006B6C
		// (set) Token: 0x060005F6 RID: 1526 RVA: 0x00008979 File Offset: 0x00006B79
		public override float Top
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x00008987 File Offset: 0x00006B87
		// (set) Token: 0x060005F8 RID: 1528 RVA: 0x0000899B File Offset: 0x00006B9B
		public override float Right
		{
			get
			{
				return this.Position.X + this.Width;
			}
			set
			{
				this.Position.X = value - this.Width;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x000089B0 File Offset: 0x00006BB0
		// (set) Token: 0x060005FA RID: 1530 RVA: 0x000089C4 File Offset: 0x00006BC4
		public override float Bottom
		{
			get
			{
				return this.Position.Y + this.Height;
			}
			set
			{
				this.Position.Y = value - this.Height;
			}
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00008E89 File Offset: 0x00007089
		public bool Intersects(Hitbox hitbox)
		{
			return base.AbsoluteLeft < hitbox.AbsoluteRight && base.AbsoluteRight > hitbox.AbsoluteLeft && base.AbsoluteBottom > hitbox.AbsoluteTop && base.AbsoluteTop < hitbox.AbsoluteBottom;
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00008EC5 File Offset: 0x000070C5
		public bool Intersects(float x, float y, float width, float height)
		{
			return base.AbsoluteRight > x && base.AbsoluteBottom > y && base.AbsoluteLeft < x + width && base.AbsoluteTop < y + height;
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00008EF2 File Offset: 0x000070F2
		public override Collider Clone()
		{
			return new Hitbox(this.width, this.height, this.Position.X, this.Position.Y);
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00008F1B File Offset: 0x0000711B
		public override void Render(Camera camera, Color color)
		{
			Draw.HollowRect(base.AbsoluteX, base.AbsoluteY, this.Width, this.Height, color);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00008F3B File Offset: 0x0000713B
		public void SetFromRectangle(Rectangle rect)
		{
			this.Position = new Vector2((float)rect.X, (float)rect.Y);
			this.Width = (float)rect.Width;
			this.Height = (float)rect.Height;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00008F70 File Offset: 0x00007170
		public void Set(float x, float y, float w, float h)
		{
			this.Position = new Vector2(x, y);
			this.Width = w;
			this.Height = h;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00008F90 File Offset: 0x00007190
		public void GetTopEdge(out Vector2 from, out Vector2 to)
		{
			from.X = base.AbsoluteLeft;
			to.X = base.AbsoluteRight;
			from.Y = (to.Y = base.AbsoluteTop);
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00008FCC File Offset: 0x000071CC
		public void GetBottomEdge(out Vector2 from, out Vector2 to)
		{
			from.X = base.AbsoluteLeft;
			to.X = base.AbsoluteRight;
			from.Y = (to.Y = base.AbsoluteBottom);
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00009008 File Offset: 0x00007208
		public void GetLeftEdge(out Vector2 from, out Vector2 to)
		{
			from.Y = base.AbsoluteTop;
			to.Y = base.AbsoluteBottom;
			from.X = (to.X = base.AbsoluteLeft);
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00009044 File Offset: 0x00007244
		public void GetRightEdge(out Vector2 from, out Vector2 to)
		{
			from.Y = base.AbsoluteTop;
			to.Y = base.AbsoluteBottom;
			from.X = (to.X = base.AbsoluteRight);
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0000907E File Offset: 0x0000727E
		public override bool Collide(Vector2 point)
		{
			return Monocle.Collide.RectToPoint(base.AbsoluteLeft, base.AbsoluteTop, this.Width, this.Height, point);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x000090A0 File Offset: 0x000072A0
		public override bool Collide(Rectangle rect)
		{
			return base.AbsoluteRight > (float)rect.Left && base.AbsoluteBottom > (float)rect.Top && base.AbsoluteLeft < (float)rect.Right && base.AbsoluteTop < (float)rect.Bottom;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x000090EF File Offset: 0x000072EF
		public override bool Collide(Vector2 from, Vector2 to)
		{
			return Monocle.Collide.RectToLine(base.AbsoluteLeft, base.AbsoluteTop, this.Width, this.Height, from, to);
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00009110 File Offset: 0x00007310
		public override bool Collide(Hitbox hitbox)
		{
			return this.Intersects(hitbox);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00009119 File Offset: 0x00007319
		public override bool Collide(Grid grid)
		{
			return grid.Collide(base.Bounds);
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00009127 File Offset: 0x00007327
		public override bool Collide(Circle circle)
		{
			return Monocle.Collide.RectToCircle(base.AbsoluteLeft, base.AbsoluteTop, this.Width, this.Height, circle.AbsolutePosition, circle.Radius);
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00009152 File Offset: 0x00007352
		public override bool Collide(ColliderList list)
		{
			return list.Collide(this);
		}

		// Token: 0x040004A8 RID: 1192
		private float width;

		// Token: 0x040004A9 RID: 1193
		private float height;
	}
}

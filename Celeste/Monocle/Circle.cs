using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000E9 RID: 233
	public class Circle : Collider
	{
		// Token: 0x06000535 RID: 1333 RVA: 0x00006FF7 File Offset: 0x000051F7
		public Circle(float radius, float x = 0f, float y = 0f)
		{
			this.Radius = radius;
			this.Position.X = x;
			this.Position.Y = y;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0000701E File Offset: 0x0000521E
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x0000702C File Offset: 0x0000522C
		public override float Width
		{
			get
			{
				return this.Radius * 2f;
			}
			set
			{
				this.Radius = value / 2f;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x0000701E File Offset: 0x0000521E
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x0000702C File Offset: 0x0000522C
		public override float Height
		{
			get
			{
				return this.Radius * 2f;
			}
			set
			{
				this.Radius = value / 2f;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0000703B File Offset: 0x0000523B
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x0000704F File Offset: 0x0000524F
		public override float Left
		{
			get
			{
				return this.Position.X - this.Radius;
			}
			set
			{
				this.Position.X = value + this.Radius;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x00007064 File Offset: 0x00005264
		// (set) Token: 0x0600053D RID: 1341 RVA: 0x00007078 File Offset: 0x00005278
		public override float Top
		{
			get
			{
				return this.Position.Y - this.Radius;
			}
			set
			{
				this.Position.Y = value + this.Radius;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x0000708D File Offset: 0x0000528D
		// (set) Token: 0x0600053F RID: 1343 RVA: 0x000070A1 File Offset: 0x000052A1
		public override float Right
		{
			get
			{
				return this.Position.X + this.Radius;
			}
			set
			{
				this.Position.X = value - this.Radius;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000540 RID: 1344 RVA: 0x000070B6 File Offset: 0x000052B6
		// (set) Token: 0x06000541 RID: 1345 RVA: 0x000070CA File Offset: 0x000052CA
		public override float Bottom
		{
			get
			{
				return this.Position.Y + this.Radius;
			}
			set
			{
				this.Position.Y = value - this.Radius;
			}
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x000070DF File Offset: 0x000052DF
		public override Collider Clone()
		{
			return new Circle(this.Radius, this.Position.X, this.Position.Y);
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00007102 File Offset: 0x00005302
		public override void Render(Camera camera, Color color)
		{
			Draw.Circle(base.AbsolutePosition, this.Radius, color, 4);
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00007117 File Offset: 0x00005317
		public override bool Collide(Vector2 point)
		{
			return Monocle.Collide.CircleToPoint(base.AbsolutePosition, this.Radius, point);
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0000712B File Offset: 0x0000532B
		public override bool Collide(Rectangle rect)
		{
			return Monocle.Collide.RectToCircle(rect, base.AbsolutePosition, this.Radius);
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0000713F File Offset: 0x0000533F
		public override bool Collide(Vector2 from, Vector2 to)
		{
			return Monocle.Collide.CircleToLine(base.AbsolutePosition, this.Radius, from, to);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00007154 File Offset: 0x00005354
		public override bool Collide(Circle circle)
		{
			return Vector2.DistanceSquared(base.AbsolutePosition, circle.AbsolutePosition) < (this.Radius + circle.Radius) * (this.Radius + circle.Radius);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00007184 File Offset: 0x00005384
		public override bool Collide(Hitbox hitbox)
		{
			return hitbox.Collide(this);
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x00007184 File Offset: 0x00005384
		public override bool Collide(Grid grid)
		{
			return grid.Collide(this);
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00007184 File Offset: 0x00005384
		public override bool Collide(ColliderList list)
		{
			return list.Collide(this);
		}

		// Token: 0x04000497 RID: 1175
		public float Radius;
	}
}

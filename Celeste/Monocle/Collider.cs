using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000EC RID: 236
	public abstract class Collider
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x00007910 File Offset: 0x00005B10
		// (set) Token: 0x0600056A RID: 1386 RVA: 0x00007918 File Offset: 0x00005B18
		public Entity Entity { get; private set; }

		// Token: 0x0600056B RID: 1387 RVA: 0x00007921 File Offset: 0x00005B21
		internal virtual void Added(Entity entity)
		{
			this.Entity = entity;
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0000792A File Offset: 0x00005B2A
		internal virtual void Removed()
		{
			this.Entity = null;
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00007933 File Offset: 0x00005B33
		public bool Collide(Entity entity)
		{
			return this.Collide(entity.Collider);
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00007944 File Offset: 0x00005B44
		public bool Collide(Collider collider)
		{
			if (collider is Hitbox)
			{
				return this.Collide(collider as Hitbox);
			}
			if (collider is Grid)
			{
				return this.Collide(collider as Grid);
			}
			if (collider is ColliderList)
			{
				return this.Collide(collider as ColliderList);
			}
			if (collider is Circle)
			{
				return this.Collide(collider as Circle);
			}
			throw new Exception("Collisions against the collider type are not implemented!");
		}

		// Token: 0x0600056F RID: 1391
		public abstract bool Collide(Vector2 point);

		// Token: 0x06000570 RID: 1392
		public abstract bool Collide(Rectangle rect);

		// Token: 0x06000571 RID: 1393
		public abstract bool Collide(Vector2 from, Vector2 to);

		// Token: 0x06000572 RID: 1394
		public abstract bool Collide(Hitbox hitbox);

		// Token: 0x06000573 RID: 1395
		public abstract bool Collide(Grid grid);

		// Token: 0x06000574 RID: 1396
		public abstract bool Collide(Circle circle);

		// Token: 0x06000575 RID: 1397
		public abstract bool Collide(ColliderList list);

		// Token: 0x06000576 RID: 1398
		public abstract Collider Clone();

		// Token: 0x06000577 RID: 1399
		public abstract void Render(Camera camera, Color color);

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000578 RID: 1400
		// (set) Token: 0x06000579 RID: 1401
		public abstract float Width { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600057A RID: 1402
		// (set) Token: 0x0600057B RID: 1403
		public abstract float Height { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600057C RID: 1404
		// (set) Token: 0x0600057D RID: 1405
		public abstract float Top { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600057E RID: 1406
		// (set) Token: 0x0600057F RID: 1407
		public abstract float Bottom { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000580 RID: 1408
		// (set) Token: 0x06000581 RID: 1409
		public abstract float Left { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000582 RID: 1410
		// (set) Token: 0x06000583 RID: 1411
		public abstract float Right { get; set; }

		// Token: 0x06000584 RID: 1412 RVA: 0x000079AF File Offset: 0x00005BAF
		public void CenterOrigin()
		{
			this.Position.X = -this.Width / 2f;
			this.Position.Y = -this.Height / 2f;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x000079E1 File Offset: 0x00005BE1
		// (set) Token: 0x06000586 RID: 1414 RVA: 0x000079F6 File Offset: 0x00005BF6
		public float CenterX
		{
			get
			{
				return this.Left + this.Width / 2f;
			}
			set
			{
				this.Left = value - this.Width / 2f;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x00007A0C File Offset: 0x00005C0C
		// (set) Token: 0x06000588 RID: 1416 RVA: 0x00007A21 File Offset: 0x00005C21
		public float CenterY
		{
			get
			{
				return this.Top + this.Height / 2f;
			}
			set
			{
				this.Top = value - this.Height / 2f;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x00007A37 File Offset: 0x00005C37
		// (set) Token: 0x0600058A RID: 1418 RVA: 0x00007A4A File Offset: 0x00005C4A
		public Vector2 TopLeft
		{
			get
			{
				return new Vector2(this.Left, this.Top);
			}
			set
			{
				this.Left = value.X;
				this.Top = value.Y;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x00007A64 File Offset: 0x00005C64
		// (set) Token: 0x0600058C RID: 1420 RVA: 0x00007A77 File Offset: 0x00005C77
		public Vector2 TopCenter
		{
			get
			{
				return new Vector2(this.CenterX, this.Top);
			}
			set
			{
				this.CenterX = value.X;
				this.Top = value.Y;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x00007A91 File Offset: 0x00005C91
		// (set) Token: 0x0600058E RID: 1422 RVA: 0x00007AA4 File Offset: 0x00005CA4
		public Vector2 TopRight
		{
			get
			{
				return new Vector2(this.Right, this.Top);
			}
			set
			{
				this.Right = value.X;
				this.Top = value.Y;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x00007ABE File Offset: 0x00005CBE
		// (set) Token: 0x06000590 RID: 1424 RVA: 0x00007AD1 File Offset: 0x00005CD1
		public Vector2 CenterLeft
		{
			get
			{
				return new Vector2(this.Left, this.CenterY);
			}
			set
			{
				this.Left = value.X;
				this.CenterY = value.Y;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x00007AEB File Offset: 0x00005CEB
		// (set) Token: 0x06000592 RID: 1426 RVA: 0x00007AFE File Offset: 0x00005CFE
		public Vector2 Center
		{
			get
			{
				return new Vector2(this.CenterX, this.CenterY);
			}
			set
			{
				this.CenterX = value.X;
				this.CenterY = value.Y;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x00007B18 File Offset: 0x00005D18
		public Vector2 Size
		{
			get
			{
				return new Vector2(this.Width, this.Height);
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x00007B2B File Offset: 0x00005D2B
		public Vector2 HalfSize
		{
			get
			{
				return this.Size * 0.5f;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000595 RID: 1429 RVA: 0x00007B3D File Offset: 0x00005D3D
		// (set) Token: 0x06000596 RID: 1430 RVA: 0x00007B50 File Offset: 0x00005D50
		public Vector2 CenterRight
		{
			get
			{
				return new Vector2(this.Right, this.CenterY);
			}
			set
			{
				this.Right = value.X;
				this.CenterY = value.Y;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000597 RID: 1431 RVA: 0x00007B6A File Offset: 0x00005D6A
		// (set) Token: 0x06000598 RID: 1432 RVA: 0x00007B7D File Offset: 0x00005D7D
		public Vector2 BottomLeft
		{
			get
			{
				return new Vector2(this.Left, this.Bottom);
			}
			set
			{
				this.Left = value.X;
				this.Bottom = value.Y;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x00007B97 File Offset: 0x00005D97
		// (set) Token: 0x0600059A RID: 1434 RVA: 0x00007BAA File Offset: 0x00005DAA
		public Vector2 BottomCenter
		{
			get
			{
				return new Vector2(this.CenterX, this.Bottom);
			}
			set
			{
				this.CenterX = value.X;
				this.Bottom = value.Y;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x00007BC4 File Offset: 0x00005DC4
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x00007BD7 File Offset: 0x00005DD7
		public Vector2 BottomRight
		{
			get
			{
				return new Vector2(this.Right, this.Bottom);
			}
			set
			{
				this.Right = value.X;
				this.Bottom = value.Y;
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00007BF1 File Offset: 0x00005DF1
		public void Render(Camera camera)
		{
			this.Render(camera, Color.Red);
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x00007BFF File Offset: 0x00005DFF
		public Vector2 AbsolutePosition
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Entity.Position + this.Position;
				}
				return this.Position;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00007C26 File Offset: 0x00005E26
		public float AbsoluteX
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Entity.Position.X + this.Position.X;
				}
				return this.Position.X;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060005A0 RID: 1440 RVA: 0x00007C58 File Offset: 0x00005E58
		public float AbsoluteY
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Entity.Position.Y + this.Position.Y;
				}
				return this.Position.Y;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x00007C8A File Offset: 0x00005E8A
		public float AbsoluteTop
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Top + this.Entity.Position.Y;
				}
				return this.Top;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x00007CB2 File Offset: 0x00005EB2
		public float AbsoluteBottom
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Bottom + this.Entity.Position.Y;
				}
				return this.Bottom;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00007CDA File Offset: 0x00005EDA
		public float AbsoluteLeft
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Left + this.Entity.Position.X;
				}
				return this.Left;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x00007D02 File Offset: 0x00005F02
		public float AbsoluteRight
		{
			get
			{
				if (this.Entity != null)
				{
					return this.Right + this.Entity.Position.X;
				}
				return this.Right;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00007D2A File Offset: 0x00005F2A
		public Rectangle Bounds
		{
			get
			{
				return new Rectangle((int)this.AbsoluteLeft, (int)this.AbsoluteTop, (int)this.Width, (int)this.Height);
			}
		}

		// Token: 0x040004A3 RID: 1187
		public Vector2 Position;
	}
}

using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000ED RID: 237
	public class ColliderList : Collider
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00007D4D File Offset: 0x00005F4D
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x00007D55 File Offset: 0x00005F55
		public Collider[] colliders { get; private set; }

		// Token: 0x060005A9 RID: 1449 RVA: 0x00007D5E File Offset: 0x00005F5E
		public ColliderList(params Collider[] colliders)
		{
			this.colliders = colliders;
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00007D70 File Offset: 0x00005F70
		public void Add(params Collider[] toAdd)
		{
			Collider[] array = new Collider[this.colliders.Length + toAdd.Length];
			for (int i = 0; i < this.colliders.Length; i++)
			{
				array[i] = this.colliders[i];
			}
			for (int j = 0; j < toAdd.Length; j++)
			{
				array[j + this.colliders.Length] = toAdd[j];
				toAdd[j].Added(base.Entity);
			}
			this.colliders = array;
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x00007DE0 File Offset: 0x00005FE0
		public void Remove(params Collider[] toRemove)
		{
			Collider[] array = new Collider[this.colliders.Length - toRemove.Length];
			int num = 0;
			foreach (Collider collider in this.colliders)
			{
				if (!toRemove.Contains(collider))
				{
					array[num] = collider;
					num++;
				}
			}
			this.colliders = array;
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00007E38 File Offset: 0x00006038
		internal override void Added(Entity entity)
		{
			base.Added(entity);
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].Added(entity);
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00007E6C File Offset: 0x0000606C
		internal override void Removed()
		{
			base.Removed();
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].Removed();
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x00007E9C File Offset: 0x0000609C
		// (set) Token: 0x060005AF RID: 1455 RVA: 0x00007EAB File Offset: 0x000060AB
		public override float Width
		{
			get
			{
				return this.Right - this.Left;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x00007EB2 File Offset: 0x000060B2
		// (set) Token: 0x060005B1 RID: 1457 RVA: 0x00007EAB File Offset: 0x000060AB
		public override float Height
		{
			get
			{
				return this.Bottom - this.Top;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x00007EC4 File Offset: 0x000060C4
		// (set) Token: 0x060005B3 RID: 1459 RVA: 0x00007F14 File Offset: 0x00006114
		public override float Left
		{
			get
			{
				float left = this.colliders[0].Left;
				for (int i = 1; i < this.colliders.Length; i++)
				{
					if (this.colliders[i].Left < left)
					{
						left = this.colliders[i].Left;
					}
				}
				return left;
			}
			set
			{
				float num = value - this.Left;
				foreach (Collider collider in this.colliders)
				{
					this.Position.X = this.Position.X + num;
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x00007F54 File Offset: 0x00006154
		// (set) Token: 0x060005B5 RID: 1461 RVA: 0x00007FA4 File Offset: 0x000061A4
		public override float Right
		{
			get
			{
				float right = this.colliders[0].Right;
				for (int i = 1; i < this.colliders.Length; i++)
				{
					if (this.colliders[i].Right > right)
					{
						right = this.colliders[i].Right;
					}
				}
				return right;
			}
			set
			{
				float num = value - this.Right;
				foreach (Collider collider in this.colliders)
				{
					this.Position.X = this.Position.X + num;
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x00007FE4 File Offset: 0x000061E4
		// (set) Token: 0x060005B7 RID: 1463 RVA: 0x00008034 File Offset: 0x00006234
		public override float Top
		{
			get
			{
				float top = this.colliders[0].Top;
				for (int i = 1; i < this.colliders.Length; i++)
				{
					if (this.colliders[i].Top < top)
					{
						top = this.colliders[i].Top;
					}
				}
				return top;
			}
			set
			{
				float num = value - this.Top;
				foreach (Collider collider in this.colliders)
				{
					this.Position.Y = this.Position.Y + num;
				}
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00008074 File Offset: 0x00006274
		// (set) Token: 0x060005B9 RID: 1465 RVA: 0x000080C4 File Offset: 0x000062C4
		public override float Bottom
		{
			get
			{
				float bottom = this.colliders[0].Bottom;
				for (int i = 1; i < this.colliders.Length; i++)
				{
					if (this.colliders[i].Bottom > bottom)
					{
						bottom = this.colliders[i].Bottom;
					}
				}
				return bottom;
			}
			set
			{
				float num = value - this.Bottom;
				foreach (Collider collider in this.colliders)
				{
					this.Position.Y = this.Position.Y + num;
				}
			}
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00008104 File Offset: 0x00006304
		public override Collider Clone()
		{
			Collider[] array = new Collider[this.colliders.Length];
			for (int i = 0; i < this.colliders.Length; i++)
			{
				array[i] = this.colliders[i].Clone();
			}
			return new ColliderList(array);
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x00008148 File Offset: 0x00006348
		public override void Render(Camera camera, Color color)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].Render(camera, color);
			}
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00008174 File Offset: 0x00006374
		public override bool Collide(Vector2 point)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(point))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x000081A4 File Offset: 0x000063A4
		public override bool Collide(Rectangle rect)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(rect))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x000081D4 File Offset: 0x000063D4
		public override bool Collide(Vector2 from, Vector2 to)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(from, to))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00008208 File Offset: 0x00006408
		public override bool Collide(Hitbox hitbox)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(hitbox))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x00008238 File Offset: 0x00006438
		public override bool Collide(Grid grid)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(grid))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00008268 File Offset: 0x00006468
		public override bool Collide(Circle circle)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(circle))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00008298 File Offset: 0x00006498
		public override bool Collide(ColliderList list)
		{
			Collider[] colliders = this.colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].Collide(list))
				{
					return true;
				}
			}
			return false;
		}
	}
}

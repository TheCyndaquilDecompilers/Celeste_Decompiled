using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000EB RID: 235
	public static class Collide
	{
		// Token: 0x0600054B RID: 1355 RVA: 0x0000718D File Offset: 0x0000538D
		public static bool Check(Entity a, Entity b)
		{
			return a.Collider != null && b.Collider != null && (a != b && b.Collidable) && a.Collider.Collide(b);
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x000071BC File Offset: 0x000053BC
		public static bool Check(Entity a, Entity b, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			bool result = Collide.Check(a, b);
			a.Position = position;
			return result;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x000071E8 File Offset: 0x000053E8
		public static bool Check(Entity a, IEnumerable<Entity> b)
		{
			foreach (Entity b2 in b)
			{
				if (Collide.Check(a, b2))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0000723C File Offset: 0x0000543C
		public static bool Check(Entity a, IEnumerable<Entity> b, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			bool result = Collide.Check(a, b);
			a.Position = position;
			return result;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x00007268 File Offset: 0x00005468
		public static Entity First(Entity a, IEnumerable<Entity> b)
		{
			foreach (Entity entity in b)
			{
				if (Collide.Check(a, entity))
				{
					return entity;
				}
			}
			return null;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x000072BC File Offset: 0x000054BC
		public static Entity First(Entity a, IEnumerable<Entity> b, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			Entity result = Collide.First(a, b);
			a.Position = position;
			return result;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x000072E8 File Offset: 0x000054E8
		public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into)
		{
			foreach (Entity entity in b)
			{
				if (Collide.Check(a, entity))
				{
					into.Add(entity);
				}
			}
			return into;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0000733C File Offset: 0x0000553C
		public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			List<Entity> result = Collide.All(a, b, into);
			a.Position = position;
			return result;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00007366 File Offset: 0x00005566
		public static List<Entity> All(Entity a, IEnumerable<Entity> b)
		{
			return Collide.All(a, b, new List<Entity>());
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x00007374 File Offset: 0x00005574
		public static List<Entity> All(Entity a, IEnumerable<Entity> b, Vector2 at)
		{
			return Collide.All(a, b, new List<Entity>(), at);
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x00007383 File Offset: 0x00005583
		public static bool CheckPoint(Entity a, Vector2 point)
		{
			return a.Collider != null && a.Collider.Collide(point);
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0000739C File Offset: 0x0000559C
		public static bool CheckPoint(Entity a, Vector2 point, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			bool result = Collide.CheckPoint(a, point);
			a.Position = position;
			return result;
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x000073C5 File Offset: 0x000055C5
		public static bool CheckLine(Entity a, Vector2 from, Vector2 to)
		{
			return a.Collider != null && a.Collider.Collide(from, to);
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000073E0 File Offset: 0x000055E0
		public static bool CheckLine(Entity a, Vector2 from, Vector2 to, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			bool result = Collide.CheckLine(a, from, to);
			a.Position = position;
			return result;
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0000740A File Offset: 0x0000560A
		public static bool CheckRect(Entity a, Rectangle rect)
		{
			return a.Collider != null && a.Collider.Collide(rect);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00007424 File Offset: 0x00005624
		public static bool CheckRect(Entity a, Rectangle rect, Vector2 at)
		{
			Vector2 position = a.Position;
			a.Position = at;
			bool result = Collide.CheckRect(a, rect);
			a.Position = position;
			return result;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00007450 File Offset: 0x00005650
		public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			Vector2 vector = a2 - a1;
			Vector2 vector2 = b2 - b1;
			float num = vector.X * vector2.Y - vector.Y * vector2.X;
			if (num == 0f)
			{
				return false;
			}
			Vector2 vector3 = b1 - a1;
			float num2 = (vector3.X * vector2.Y - vector3.Y * vector2.X) / num;
			if (num2 < 0f || num2 > 1f)
			{
				return false;
			}
			float num3 = (vector3.X * vector.Y - vector3.Y * vector.X) / num;
			return num3 >= 0f && num3 <= 1f;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00007504 File Offset: 0x00005704
		public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
		{
			intersection = Vector2.Zero;
			Vector2 vector = a2 - a1;
			Vector2 vector2 = b2 - b1;
			float num = vector.X * vector2.Y - vector.Y * vector2.X;
			if (num == 0f)
			{
				return false;
			}
			Vector2 vector3 = b1 - a1;
			float num2 = (vector3.X * vector2.Y - vector3.Y * vector2.X) / num;
			if (num2 < 0f || num2 > 1f)
			{
				return false;
			}
			float num3 = (vector3.X * vector.Y - vector3.Y * vector.X) / num;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			intersection = a1 + num2 * vector;
			return true;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x000075D7 File Offset: 0x000057D7
		public static bool CircleToLine(Vector2 cPosiition, float cRadius, Vector2 lineFrom, Vector2 lineTo)
		{
			return Vector2.DistanceSquared(cPosiition, Calc.ClosestPointOnLine(lineFrom, lineTo, cPosiition)) < cRadius * cRadius;
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x000075EC File Offset: 0x000057EC
		public static bool CircleToPoint(Vector2 cPosition, float cRadius, Vector2 point)
		{
			return Vector2.DistanceSquared(cPosition, point) < cRadius * cRadius;
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x000075FA File Offset: 0x000057FA
		public static bool CircleToRect(Vector2 cPosition, float cRadius, float rX, float rY, float rW, float rH)
		{
			return Collide.RectToCircle(rX, rY, rW, rH, cPosition, cRadius);
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00007609 File Offset: 0x00005809
		public static bool CircleToRect(Vector2 cPosition, float cRadius, Rectangle rect)
		{
			return Collide.RectToCircle(rect, cPosition, cRadius);
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00007614 File Offset: 0x00005814
		public static bool RectToCircle(float rX, float rY, float rW, float rH, Vector2 cPosition, float cRadius)
		{
			if (Collide.RectToPoint(rX, rY, rW, rH, cPosition))
			{
				return true;
			}
			PointSectors sector = Collide.GetSector(rX, rY, rW, rH, cPosition);
			if ((sector & PointSectors.Top) != PointSectors.Center)
			{
				Vector2 lineFrom = new Vector2(rX, rY);
				Vector2 lineTo = new Vector2(rX + rW, rY);
				if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
				{
					return true;
				}
			}
			if ((sector & PointSectors.Bottom) != PointSectors.Center)
			{
				Vector2 lineFrom = new Vector2(rX, rY + rH);
				Vector2 lineTo = new Vector2(rX + rW, rY + rH);
				if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
				{
					return true;
				}
			}
			if ((sector & PointSectors.Left) != PointSectors.Center)
			{
				Vector2 lineFrom = new Vector2(rX, rY);
				Vector2 lineTo = new Vector2(rX, rY + rH);
				if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
				{
					return true;
				}
			}
			if ((sector & PointSectors.Right) != PointSectors.Center)
			{
				Vector2 lineFrom = new Vector2(rX + rW, rY);
				Vector2 lineTo = new Vector2(rX + rW, rY + rH);
				if (Collide.CircleToLine(cPosition, cRadius, lineFrom, lineTo))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x000076E5 File Offset: 0x000058E5
		public static bool RectToCircle(Rectangle rect, Vector2 cPosition, float cRadius)
		{
			return Collide.RectToCircle((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, cPosition, cRadius);
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0000770C File Offset: 0x0000590C
		public static bool RectToLine(float rX, float rY, float rW, float rH, Vector2 lineFrom, Vector2 lineTo)
		{
			PointSectors sector = Collide.GetSector(rX, rY, rW, rH, lineFrom);
			PointSectors sector2 = Collide.GetSector(rX, rY, rW, rH, lineTo);
			if (sector == PointSectors.Center || sector2 == PointSectors.Center)
			{
				return true;
			}
			if ((sector & sector2) != PointSectors.Center)
			{
				return false;
			}
			PointSectors pointSectors = sector | sector2;
			if ((pointSectors & PointSectors.Top) != PointSectors.Center)
			{
				Vector2 a = new Vector2(rX, rY);
				Vector2 a2 = new Vector2(rX + rW, rY);
				if (Collide.LineCheck(a, a2, lineFrom, lineTo))
				{
					return true;
				}
			}
			if ((pointSectors & PointSectors.Bottom) != PointSectors.Center)
			{
				Vector2 a3 = new Vector2(rX, rY + rH);
				Vector2 a2 = new Vector2(rX + rW, rY + rH);
				if (Collide.LineCheck(a3, a2, lineFrom, lineTo))
				{
					return true;
				}
			}
			if ((pointSectors & PointSectors.Left) != PointSectors.Center)
			{
				Vector2 a4 = new Vector2(rX, rY);
				Vector2 a2 = new Vector2(rX, rY + rH);
				if (Collide.LineCheck(a4, a2, lineFrom, lineTo))
				{
					return true;
				}
			}
			if ((pointSectors & PointSectors.Right) != PointSectors.Center)
			{
				Vector2 a5 = new Vector2(rX + rW, rY);
				Vector2 a2 = new Vector2(rX + rW, rY + rH);
				if (Collide.LineCheck(a5, a2, lineFrom, lineTo))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x000077E1 File Offset: 0x000059E1
		public static bool RectToLine(Rectangle rect, Vector2 lineFrom, Vector2 lineTo)
		{
			return Collide.RectToLine((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, lineFrom, lineTo);
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00007806 File Offset: 0x00005A06
		public static bool RectToPoint(float rX, float rY, float rW, float rH, Vector2 point)
		{
			return point.X >= rX && point.Y >= rY && point.X < rX + rW && point.Y < rY + rH;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00007836 File Offset: 0x00005A36
		public static bool RectToPoint(Rectangle rect, Vector2 point)
		{
			return Collide.RectToPoint((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, point);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0000785C File Offset: 0x00005A5C
		public static PointSectors GetSector(Rectangle rect, Vector2 point)
		{
			PointSectors pointSectors = PointSectors.Center;
			if (point.X < (float)rect.Left)
			{
				pointSectors |= PointSectors.Left;
			}
			else if (point.X >= (float)rect.Right)
			{
				pointSectors |= PointSectors.Right;
			}
			if (point.Y < (float)rect.Top)
			{
				pointSectors |= PointSectors.Top;
			}
			else if (point.Y >= (float)rect.Bottom)
			{
				pointSectors |= PointSectors.Bottom;
			}
			return pointSectors;
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x000078C0 File Offset: 0x00005AC0
		public static PointSectors GetSector(float rX, float rY, float rW, float rH, Vector2 point)
		{
			PointSectors pointSectors = PointSectors.Center;
			if (point.X < rX)
			{
				pointSectors |= PointSectors.Left;
			}
			else if (point.X >= rX + rW)
			{
				pointSectors |= PointSectors.Right;
			}
			if (point.Y < rY)
			{
				pointSectors |= PointSectors.Top;
			}
			else if (point.Y >= rY + rH)
			{
				pointSectors |= PointSectors.Bottom;
			}
			return pointSectors;
		}
	}
}

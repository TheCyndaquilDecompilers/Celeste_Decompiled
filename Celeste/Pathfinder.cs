using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200028D RID: 653
	public class Pathfinder
	{
		// Token: 0x0600144E RID: 5198 RVA: 0x0006DD80 File Offset: 0x0006BF80
		public Pathfinder(Level level)
		{
			this.level = level;
			this.map = new Pathfinder.Tile[200, 200];
			this.comparer = new Pathfinder.PointMapComparer(this.map);
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0006DDC0 File Offset: 0x0006BFC0
		public bool Find(ref List<Vector2> path, Vector2 from, Vector2 to, bool fewerTurns = true, bool logging = false)
		{
			this.lastPath = null;
			int num = this.level.Bounds.Left / 8;
			int num2 = this.level.Bounds.Top / 8;
			int num3 = this.level.Bounds.Width / 8;
			int num4 = this.level.Bounds.Height / 8;
			Point levelSolidOffset = this.level.LevelSolidOffset;
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < num4; j++)
				{
					this.map[i, j].Solid = (this.level.SolidsData[i + levelSolidOffset.X, j + levelSolidOffset.Y] != '0');
					this.map[i, j].Cost = int.MaxValue;
					this.map[i, j].Parent = null;
				}
			}
			foreach (Entity entity in this.level.Tracker.GetEntities<Solid>())
			{
				if (entity.Collidable && entity.Collider is Hitbox)
				{
					int k = (int)Math.Floor((double)(entity.Left / 8f));
					int num5 = (int)Math.Ceiling((double)(entity.Right / 8f));
					while (k < num5)
					{
						int l = (int)Math.Floor((double)(entity.Top / 8f));
						int num6 = (int)Math.Ceiling((double)(entity.Bottom / 8f));
						while (l < num6)
						{
							int num7 = k - num;
							int num8 = l - num2;
							if (num7 >= 0 && num8 >= 0 && num7 < num3 && num8 < num4)
							{
								this.map[num7, num8].Solid = true;
							}
							l++;
						}
						k++;
					}
				}
			}
			Point point = this.debugLastStart = new Point((int)Math.Floor((double)(from.X / 8f)) - num, (int)Math.Floor((double)(from.Y / 8f)) - num2);
			Point point2 = this.debugLastEnd = new Point((int)Math.Floor((double)(to.X / 8f)) - num, (int)Math.Floor((double)(to.Y / 8f)) - num2);
			if (point.X < 0 || point.Y < 0 || point.X >= num3 || point.Y >= num4 || point2.X < 0 || point2.Y < 0 || point2.X >= num3 || point2.Y >= num4)
			{
				if (logging)
				{
					Calc.Log(new object[]
					{
						"PF: FAILED - Start or End outside the level bounds"
					});
				}
				return false;
			}
			if (this.map[point.X, point.Y].Solid)
			{
				if (logging)
				{
					Calc.Log(new object[]
					{
						"PF: FAILED - Start inside a solid"
					});
				}
				return false;
			}
			if (this.map[point2.X, point2.Y].Solid)
			{
				if (logging)
				{
					Calc.Log(new object[]
					{
						"PF: FAILED - End inside a solid"
					});
				}
				return false;
			}
			this.active.Clear();
			this.active.Add(point);
			this.map[point.X, point.Y].Cost = 0;
			bool flag = false;
			while (this.active.Count > 0 && !flag)
			{
				Point point3 = this.active[this.active.Count - 1];
				this.active.RemoveAt(this.active.Count - 1);
				for (int m = 0; m < 4; m++)
				{
					Point point4 = new Point(Pathfinder.directions[m].X, Pathfinder.directions[m].Y);
					Point point5 = new Point(point3.X + point4.X, point3.Y + point4.Y);
					int num9 = 1;
					if (point5.X >= 0 && point5.Y >= 0 && point5.X < num3 && point5.Y < num4 && !this.map[point5.X, point5.Y].Solid)
					{
						for (int n = 0; n < 4; n++)
						{
							Point point6 = new Point(point5.X + Pathfinder.directions[n].X, point5.Y + Pathfinder.directions[n].Y);
							if (point6.X >= 0 && point6.Y >= 0 && point6.X < num3 && point6.Y < num4 && this.map[point6.X, point6.Y].Solid)
							{
								num9 = 7;
								break;
							}
						}
						if (fewerTurns && this.map[point3.X, point3.Y].Parent != null && point5.X != this.map[point3.X, point3.Y].Parent.Value.X && point5.Y != this.map[point3.X, point3.Y].Parent.Value.Y)
						{
							num9 += 4;
						}
						int cost = this.map[point3.X, point3.Y].Cost;
						if (point4.Y != 0)
						{
							num9 += (int)((float)cost * 0.5f);
						}
						int num10 = cost + num9;
						if (this.map[point5.X, point5.Y].Cost > num10)
						{
							this.map[point5.X, point5.Y].Cost = num10;
							this.map[point5.X, point5.Y].Parent = new Point?(point3);
							int num11 = this.active.BinarySearch(point5, this.comparer);
							if (num11 < 0)
							{
								num11 = ~num11;
							}
							this.active.Insert(num11, point5);
							if (point5 == point2)
							{
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (!flag)
			{
				if (logging)
				{
					Calc.Log(new object[]
					{
						"PF: FAILED - ran out of active nodes, can't find ending"
					});
				}
				return false;
			}
			path.Clear();
			Point point7 = point2;
			int num12 = 0;
			while (point7 != point && num12++ < 1000)
			{
				path.Add(new Vector2((float)point7.X + 0.5f, (float)point7.Y + 0.5f) * 8f + this.level.LevelOffset);
				point7 = this.map[point7.X, point7.Y].Parent.Value;
			}
			if (num12 >= 1000)
			{
				Console.WriteLine("WARNING: Pathfinder 'succeeded' but then was unable to work out its path?");
				return false;
			}
			int num13 = 1;
			while (num13 < path.Count - 1 && path.Count > 2)
			{
				if ((path[num13].X == path[num13 - 1].X && path[num13].X == path[num13 + 1].X) || (path[num13].Y == path[num13 - 1].Y && path[num13].Y == path[num13 + 1].Y))
				{
					path.RemoveAt(num13);
					num13--;
				}
				num13++;
			}
			path.Reverse();
			this.lastPath = path;
			if (logging)
			{
				Calc.Log(new object[]
				{
					"PF: SUCCESS"
				});
			}
			return true;
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0006E634 File Offset: 0x0006C834
		public void Render()
		{
			for (int i = 0; i < 200; i++)
			{
				for (int j = 0; j < 200; j++)
				{
					if (this.map[i, j].Solid)
					{
						Draw.Rect((float)(this.level.Bounds.Left + i * 8), (float)(this.level.Bounds.Top + j * 8), 8f, 8f, Color.Red * 0.25f);
					}
				}
			}
			if (this.lastPath != null)
			{
				Vector2 vector = this.lastPath[0];
				for (int k = 1; k < this.lastPath.Count; k++)
				{
					Vector2 vector2 = this.lastPath[k];
					Draw.Line(vector, vector2, Color.Red);
					Draw.Rect(vector.X - 2f, vector.Y - 2f, 4f, 4f, Color.Red);
					vector = vector2;
				}
				Draw.Rect(vector.X - 2f, vector.Y - 2f, 4f, 4f, Color.Red);
			}
			Draw.Rect((float)(this.level.Bounds.Left + this.debugLastStart.X * 8 + 2), (float)(this.level.Bounds.Top + this.debugLastStart.Y * 8 + 2), 4f, 4f, Color.Green);
			Draw.Rect((float)(this.level.Bounds.Left + this.debugLastEnd.X * 8 + 2), (float)(this.level.Bounds.Top + this.debugLastEnd.Y * 8 + 2), 4f, 4f, Color.Green);
		}

		// Token: 0x04000FFB RID: 4091
		private static readonly Point[] directions = new Point[]
		{
			new Point(1, 0),
			new Point(0, 1),
			new Point(-1, 0),
			new Point(0, -1)
		};

		// Token: 0x04000FFC RID: 4092
		private const int MapSize = 200;

		// Token: 0x04000FFD RID: 4093
		private Level level;

		// Token: 0x04000FFE RID: 4094
		private Pathfinder.Tile[,] map;

		// Token: 0x04000FFF RID: 4095
		private List<Point> active = new List<Point>();

		// Token: 0x04001000 RID: 4096
		private Pathfinder.PointMapComparer comparer;

		// Token: 0x04001001 RID: 4097
		public bool DebugRenderEnabled;

		// Token: 0x04001002 RID: 4098
		private List<Vector2> lastPath;

		// Token: 0x04001003 RID: 4099
		private Point debugLastStart;

		// Token: 0x04001004 RID: 4100
		private Point debugLastEnd;

		// Token: 0x0200060A RID: 1546
		private struct Tile
		{
			// Token: 0x040028F5 RID: 10485
			public bool Solid;

			// Token: 0x040028F6 RID: 10486
			public int Cost;

			// Token: 0x040028F7 RID: 10487
			public Point? Parent;
		}

		// Token: 0x0200060B RID: 1547
		private class PointMapComparer : IComparer<Point>
		{
			// Token: 0x060029F7 RID: 10743 RVA: 0x0010DB29 File Offset: 0x0010BD29
			public PointMapComparer(Pathfinder.Tile[,] map)
			{
				this.map = map;
			}

			// Token: 0x060029F8 RID: 10744 RVA: 0x0010DB38 File Offset: 0x0010BD38
			public int Compare(Point a, Point b)
			{
				return this.map[b.X, b.Y].Cost - this.map[a.X, a.Y].Cost;
			}

			// Token: 0x040028F8 RID: 10488
			private Pathfinder.Tile[,] map;
		}
	}
}

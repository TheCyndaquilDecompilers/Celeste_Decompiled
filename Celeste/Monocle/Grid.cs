using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000EE RID: 238
	public class Grid : Collider
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x000082C8 File Offset: 0x000064C8
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x000082D0 File Offset: 0x000064D0
		public float CellWidth { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x000082D9 File Offset: 0x000064D9
		// (set) Token: 0x060005C6 RID: 1478 RVA: 0x000082E1 File Offset: 0x000064E1
		public float CellHeight { get; private set; }

		// Token: 0x060005C7 RID: 1479 RVA: 0x000082EA File Offset: 0x000064EA
		public Grid(int cellsX, int cellsY, float cellWidth, float cellHeight)
		{
			this.Data = new VirtualMap<bool>(cellsX, cellsY, false);
			this.CellWidth = cellWidth;
			this.CellHeight = cellHeight;
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00008310 File Offset: 0x00006510
		public Grid(float cellWidth, float cellHeight, string bitstring)
		{
			this.CellWidth = cellWidth;
			this.CellHeight = cellHeight;
			int num = 0;
			int num2 = 0;
			int num3 = 1;
			for (int i = 0; i < bitstring.Length; i++)
			{
				if (bitstring[i] == '\n')
				{
					num3++;
					num = Math.Max(num2, num);
					num2 = 0;
				}
				else
				{
					num2++;
				}
			}
			this.Data = new VirtualMap<bool>(num, num3, false);
			this.LoadBitstring(bitstring);
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0000837C File Offset: 0x0000657C
		public Grid(float cellWidth, float cellHeight, bool[,] data)
		{
			this.CellWidth = cellWidth;
			this.CellHeight = cellHeight;
			this.Data = new VirtualMap<bool>(data, false);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x0000839F File Offset: 0x0000659F
		public Grid(float cellWidth, float cellHeight, VirtualMap<bool> data)
		{
			this.CellWidth = cellWidth;
			this.CellHeight = cellHeight;
			this.Data = data;
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x000083BC File Offset: 0x000065BC
		public void Extend(int left, int right, int up, int down)
		{
			this.Position -= new Vector2((float)left * this.CellWidth, (float)up * this.CellHeight);
			int num = this.Data.Columns + left + right;
			int num2 = this.Data.Rows + up + down;
			if (num <= 0 || num2 <= 0)
			{
				this.Data = new VirtualMap<bool>(0, 0, false);
				return;
			}
			VirtualMap<bool> virtualMap = new VirtualMap<bool>(num, num2, false);
			for (int i = 0; i < this.Data.Columns; i++)
			{
				for (int j = 0; j < this.Data.Rows; j++)
				{
					int num3 = i + left;
					int num4 = j + up;
					if (num3 >= 0 && num3 < num && num4 >= 0 && num4 < num2)
					{
						virtualMap[num3, num4] = this.Data[i, j];
					}
				}
			}
			for (int k = 0; k < left; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					virtualMap[k, l] = this.Data[0, Calc.Clamp(l - up, 0, this.Data.Rows - 1)];
				}
			}
			for (int m = num - right; m < num; m++)
			{
				for (int n = 0; n < num2; n++)
				{
					virtualMap[m, n] = this.Data[this.Data.Columns - 1, Calc.Clamp(n - up, 0, this.Data.Rows - 1)];
				}
			}
			for (int num5 = 0; num5 < up; num5++)
			{
				for (int num6 = 0; num6 < num; num6++)
				{
					virtualMap[num6, num5] = this.Data[Calc.Clamp(num6 - left, 0, this.Data.Columns - 1), 0];
				}
			}
			for (int num7 = num2 - down; num7 < num2; num7++)
			{
				for (int num8 = 0; num8 < num; num8++)
				{
					virtualMap[num8, num7] = this.Data[Calc.Clamp(num8 - left, 0, this.Data.Columns - 1), this.Data.Rows - 1];
				}
			}
			this.Data = virtualMap;
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x000085F4 File Offset: 0x000067F4
		public void LoadBitstring(string bitstring)
		{
			int i = 0;
			int num = 0;
			for (int j = 0; j < bitstring.Length; j++)
			{
				if (bitstring[j] == '\n')
				{
					while (i < this.CellsX)
					{
						this.Data[i, num] = false;
						i++;
					}
					i = 0;
					num++;
					if (num >= this.CellsY)
					{
						return;
					}
				}
				else if (i < this.CellsX)
				{
					if (bitstring[j] == '0')
					{
						this.Data[i, num] = false;
						i++;
					}
					else
					{
						this.Data[i, num] = true;
						i++;
					}
				}
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00008688 File Offset: 0x00006888
		public string GetBitstring()
		{
			string text = "";
			for (int i = 0; i < this.CellsY; i++)
			{
				if (i != 0)
				{
					text += "\n";
				}
				for (int j = 0; j < this.CellsX; j++)
				{
					if (this.Data[j, i])
					{
						text += "1";
					}
					else
					{
						text += "0";
					}
				}
			}
			return text;
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x000086F8 File Offset: 0x000068F8
		public void Clear(bool to = false)
		{
			for (int i = 0; i < this.CellsX; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					this.Data[i, j] = to;
				}
			}
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00008738 File Offset: 0x00006938
		public void SetRect(int x, int y, int width, int height, bool to = true)
		{
			if (x < 0)
			{
				width += x;
				x = 0;
			}
			if (y < 0)
			{
				height += y;
				y = 0;
			}
			if (x + width > this.CellsX)
			{
				width = this.CellsX - x;
			}
			if (y + height > this.CellsY)
			{
				height = this.CellsY - y;
			}
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					this.Data[x + i, y + j] = to;
				}
			}
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x000087B8 File Offset: 0x000069B8
		public bool CheckRect(int x, int y, int width, int height)
		{
			if (x < 0)
			{
				width += x;
				x = 0;
			}
			if (y < 0)
			{
				height += y;
				y = 0;
			}
			if (x + width > this.CellsX)
			{
				width = this.CellsX - x;
			}
			if (y + height > this.CellsY)
			{
				height = this.CellsY - y;
			}
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (this.Data[x + i, y + j])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00008838 File Offset: 0x00006A38
		public bool CheckColumn(int x)
		{
			for (int i = 0; i < this.CellsY; i++)
			{
				if (!this.Data[x, i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00008868 File Offset: 0x00006A68
		public bool CheckRow(int y)
		{
			for (int i = 0; i < this.CellsX; i++)
			{
				if (!this.Data[i, y])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x17000032 RID: 50
		public bool this[int x, int y]
		{
			get
			{
				return x >= 0 && y >= 0 && x < this.CellsX && y < this.CellsY && this.Data[x, y];
			}
			set
			{
				this.Data[x, y] = value;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x000088D3 File Offset: 0x00006AD3
		public int CellsX
		{
			get
			{
				return this.Data.Columns;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x000088E0 File Offset: 0x00006AE0
		public int CellsY
		{
			get
			{
				return this.Data.Rows;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x000088ED File Offset: 0x00006AED
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x00007EAB File Offset: 0x000060AB
		public override float Width
		{
			get
			{
				return this.CellWidth * (float)this.CellsX;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x000088FD File Offset: 0x00006AFD
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x00007EAB File Offset: 0x000060AB
		public override float Height
		{
			get
			{
				return this.CellHeight * (float)this.CellsY;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x00008910 File Offset: 0x00006B10
		public bool IsEmpty
		{
			get
			{
				for (int i = 0; i < this.CellsX; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						if (this.Data[i, j])
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x00008951 File Offset: 0x00006B51
		// (set) Token: 0x060005DD RID: 1501 RVA: 0x0000895E File Offset: 0x00006B5E
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x0000896C File Offset: 0x00006B6C
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x00008979 File Offset: 0x00006B79
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00008987 File Offset: 0x00006B87
		// (set) Token: 0x060005E1 RID: 1505 RVA: 0x0000899B File Offset: 0x00006B9B
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x000089B0 File Offset: 0x00006BB0
		// (set) Token: 0x060005E3 RID: 1507 RVA: 0x000089C4 File Offset: 0x00006BC4
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

		// Token: 0x060005E4 RID: 1508 RVA: 0x000089D9 File Offset: 0x00006BD9
		public override Collider Clone()
		{
			return new Grid(this.CellWidth, this.CellHeight, this.Data.Clone());
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x000089F8 File Offset: 0x00006BF8
		public override void Render(Camera camera, Color color)
		{
			if (camera == null)
			{
				for (int i = 0; i < this.CellsX; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						if (this.Data[i, j])
						{
							Draw.HollowRect(base.AbsoluteLeft + (float)i * this.CellWidth, base.AbsoluteTop + (float)j * this.CellHeight, this.CellWidth, this.CellHeight, color);
						}
					}
				}
				return;
			}
			int num = (int)Math.Max(0f, (camera.Left - base.AbsoluteLeft) / this.CellWidth);
			int num2 = (int)Math.Min((double)(this.CellsX - 1), Math.Ceiling((double)((camera.Right - base.AbsoluteLeft) / this.CellWidth)));
			int num3 = (int)Math.Max(0f, (camera.Top - base.AbsoluteTop) / this.CellHeight);
			int num4 = (int)Math.Min((double)(this.CellsY - 1), Math.Ceiling((double)((camera.Bottom - base.AbsoluteTop) / this.CellHeight)));
			for (int k = num; k <= num2; k++)
			{
				for (int l = num3; l <= num4; l++)
				{
					if (this.Data[k, l])
					{
						Draw.HollowRect(base.AbsoluteLeft + (float)k * this.CellWidth, base.AbsoluteTop + (float)l * this.CellHeight, this.CellWidth, this.CellHeight, color);
					}
				}
			}
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00008B68 File Offset: 0x00006D68
		public override bool Collide(Vector2 point)
		{
			return point.X >= base.AbsoluteLeft && point.Y >= base.AbsoluteTop && point.X < base.AbsoluteRight && point.Y < base.AbsoluteBottom && this.Data[(int)((point.X - base.AbsoluteLeft) / this.CellWidth), (int)((point.Y - base.AbsoluteTop) / this.CellHeight)];
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00008BE4 File Offset: 0x00006DE4
		public override bool Collide(Rectangle rect)
		{
			if (rect.Intersects(base.Bounds))
			{
				int num = (int)(((float)rect.Left - base.AbsoluteLeft) / this.CellWidth);
				int num2 = (int)(((float)rect.Top - base.AbsoluteTop) / this.CellHeight);
				int width = (int)(((float)rect.Right - base.AbsoluteLeft - 1f) / this.CellWidth) - num + 1;
				int height = (int)(((float)rect.Bottom - base.AbsoluteTop - 1f) / this.CellHeight) - num2 + 1;
				return this.CheckRect(num, num2, width, height);
			}
			return false;
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00008C80 File Offset: 0x00006E80
		public override bool Collide(Vector2 from, Vector2 to)
		{
			from -= base.AbsolutePosition;
			to -= base.AbsolutePosition;
			from /= new Vector2(this.CellWidth, this.CellHeight);
			to /= new Vector2(this.CellWidth, this.CellHeight);
			bool flag = Math.Abs(to.Y - from.Y) > Math.Abs(to.X - from.X);
			if (flag)
			{
				float x = from.X;
				from.X = from.Y;
				from.Y = x;
				x = to.X;
				to.X = to.Y;
				to.Y = x;
			}
			if (from.X > to.X)
			{
				Vector2 vector = from;
				from = to;
				to = vector;
			}
			float num = 0f;
			float num2 = Math.Abs(to.Y - from.Y) / (to.X - from.X);
			int num3 = (from.Y < to.Y) ? 1 : -1;
			int num4 = (int)from.Y;
			int num5 = (int)to.X;
			for (int i = (int)from.X; i <= num5; i++)
			{
				if (flag)
				{
					if (this[num4, i])
					{
						return true;
					}
				}
				else if (this[i, num4])
				{
					return true;
				}
				num += num2;
				if (num >= 0.5f)
				{
					num4 += num3;
					num -= 1f;
				}
			}
			return false;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00008DF2 File Offset: 0x00006FF2
		public override bool Collide(Hitbox hitbox)
		{
			return this.Collide(hitbox.Bounds);
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00007EAB File Offset: 0x000060AB
		public override bool Collide(Grid grid)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00008E00 File Offset: 0x00007000
		public override bool Collide(Circle circle)
		{
			return false;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00008E03 File Offset: 0x00007003
		public override bool Collide(ColliderList list)
		{
			return list.Collide(this);
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00008E0C File Offset: 0x0000700C
		public static bool IsBitstringEmpty(string bitstring)
		{
			for (int i = 0; i < bitstring.Length; i++)
			{
				if (bitstring[i] == '1')
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040004A5 RID: 1189
		public VirtualMap<bool> Data;
	}
}

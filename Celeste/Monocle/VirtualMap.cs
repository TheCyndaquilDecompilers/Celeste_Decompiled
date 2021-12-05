using System;

namespace Monocle
{
	// Token: 0x0200013E RID: 318
	public class VirtualMap<T>
	{
		// Token: 0x06000B8B RID: 2955 RVA: 0x00020F70 File Offset: 0x0001F170
		public VirtualMap(int columns, int rows, T emptyValue = default(T))
		{
			this.Columns = columns;
			this.Rows = rows;
			this.SegmentColumns = columns / 50 + 1;
			this.SegmentRows = rows / 50 + 1;
			this.segments = new T[this.SegmentColumns, this.SegmentRows][,];
			this.EmptyValue = emptyValue;
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x00020FC8 File Offset: 0x0001F1C8
		public VirtualMap(T[,] map, T emptyValue = default(T)) : this(map.GetLength(0), map.GetLength(1), emptyValue)
		{
			for (int i = 0; i < this.Columns; i++)
			{
				for (int j = 0; j < this.Rows; j++)
				{
					this[i, j] = map[i, j];
				}
			}
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x0002101C File Offset: 0x0001F21C
		public bool AnyInSegmentAtTile(int x, int y)
		{
			int num = x / 50;
			int num2 = y / 50;
			return this.segments[num, num2] != null;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x00021043 File Offset: 0x0001F243
		public bool AnyInSegment(int segmentX, int segmentY)
		{
			return this.segments[segmentX, segmentY] != null;
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x00021055 File Offset: 0x0001F255
		public T InSegment(int segmentX, int segmentY, int x, int y)
		{
			return this.segments[segmentX, segmentY][x, y];
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x0002106C File Offset: 0x0001F26C
		public T[,] GetSegment(int segmentX, int segmentY)
		{
			return this.segments[segmentX, segmentY];
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0002107B File Offset: 0x0001F27B
		public T SafeCheck(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < this.Columns && y < this.Rows)
			{
				return this[x, y];
			}
			return this.EmptyValue;
		}

		// Token: 0x17000109 RID: 265
		public T this[int x, int y]
		{
			get
			{
				int num = x / 50;
				int num2 = y / 50;
				T[,] array = this.segments[num, num2];
				if (array == null)
				{
					return this.EmptyValue;
				}
				return array[x - num * 50, y - num2 * 50];
			}
			set
			{
				int num = x / 50;
				int num2 = y / 50;
				if (this.segments[num, num2] == null)
				{
					this.segments[num, num2] = new T[50, 50];
					if (this.EmptyValue != null)
					{
						T emptyValue = this.EmptyValue;
						if (!emptyValue.Equals(default(T)))
						{
							for (int i = 0; i < 50; i++)
							{
								for (int j = 0; j < 50; j++)
								{
									this.segments[num, num2][i, j] = this.EmptyValue;
								}
							}
						}
					}
				}
				this.segments[num, num2][x - num * 50, y - num2 * 50] = value;
			}
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x000211BC File Offset: 0x0001F3BC
		public T[,] ToArray()
		{
			T[,] array = new T[this.Columns, this.Rows];
			for (int i = 0; i < this.Columns; i++)
			{
				for (int j = 0; j < this.Rows; j++)
				{
					array[i, j] = this[i, j];
				}
			}
			return array;
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00021210 File Offset: 0x0001F410
		public VirtualMap<T> Clone()
		{
			VirtualMap<T> virtualMap = new VirtualMap<T>(this.Columns, this.Rows, this.EmptyValue);
			for (int i = 0; i < this.Columns; i++)
			{
				for (int j = 0; j < this.Rows; j++)
				{
					virtualMap[i, j] = this[i, j];
				}
			}
			return virtualMap;
		}

		// Token: 0x040006D6 RID: 1750
		public const int SegmentSize = 50;

		// Token: 0x040006D7 RID: 1751
		public readonly int Columns;

		// Token: 0x040006D8 RID: 1752
		public readonly int Rows;

		// Token: 0x040006D9 RID: 1753
		public readonly int SegmentColumns;

		// Token: 0x040006DA RID: 1754
		public readonly int SegmentRows;

		// Token: 0x040006DB RID: 1755
		public readonly T EmptyValue;

		// Token: 0x040006DC RID: 1756
		private T[,][,] segments;
	}
}

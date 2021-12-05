using System;
using System.Xml;

namespace Monocle
{
	// Token: 0x02000137 RID: 311
	public class AutotileData
	{
		// Token: 0x06000B3C RID: 2876 RVA: 0x0001F5D8 File Offset: 0x0001D7D8
		public AutotileData(XmlElement xml)
		{
			this.Center = Calc.ReadCSVInt(xml.ChildText("Center", ""));
			this.Single = Calc.ReadCSVInt(xml.ChildText("Single", ""));
			this.SingleHorizontalLeft = Calc.ReadCSVInt(xml.ChildText("SingleHorizontalLeft", ""));
			this.SingleHorizontalCenter = Calc.ReadCSVInt(xml.ChildText("SingleHorizontalCenter", ""));
			this.SingleHorizontalRight = Calc.ReadCSVInt(xml.ChildText("SingleHorizontalRight", ""));
			this.SingleVerticalTop = Calc.ReadCSVInt(xml.ChildText("SingleVerticalTop", ""));
			this.SingleVerticalCenter = Calc.ReadCSVInt(xml.ChildText("SingleVerticalCenter", ""));
			this.SingleVerticalBottom = Calc.ReadCSVInt(xml.ChildText("SingleVerticalBottom", ""));
			this.Top = Calc.ReadCSVInt(xml.ChildText("Top", ""));
			this.Bottom = Calc.ReadCSVInt(xml.ChildText("Bottom", ""));
			this.Left = Calc.ReadCSVInt(xml.ChildText("Left", ""));
			this.Right = Calc.ReadCSVInt(xml.ChildText("Right", ""));
			this.TopLeft = Calc.ReadCSVInt(xml.ChildText("TopLeft", ""));
			this.TopRight = Calc.ReadCSVInt(xml.ChildText("TopRight", ""));
			this.BottomLeft = Calc.ReadCSVInt(xml.ChildText("BottomLeft", ""));
			this.BottomRight = Calc.ReadCSVInt(xml.ChildText("BottomRight", ""));
			this.InsideTopLeft = Calc.ReadCSVInt(xml.ChildText("InsideTopLeft", ""));
			this.InsideTopRight = Calc.ReadCSVInt(xml.ChildText("InsideTopRight", ""));
			this.InsideBottomLeft = Calc.ReadCSVInt(xml.ChildText("InsideBottomLeft", ""));
			this.InsideBottomRight = Calc.ReadCSVInt(xml.ChildText("InsideBottomRight", ""));
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x0001F808 File Offset: 0x0001DA08
		public int TileHandler()
		{
			if (Tiler.Left && Tiler.Right && Tiler.Up && Tiler.Down && Tiler.UpLeft && Tiler.UpRight && Tiler.DownLeft && Tiler.DownRight)
			{
				return this.GetTileID(this.Center);
			}
			if (!Tiler.Up && !Tiler.Down)
			{
				if (Tiler.Left && Tiler.Right)
				{
					return this.GetTileID(this.SingleHorizontalCenter);
				}
				if (!Tiler.Left && !Tiler.Right)
				{
					return this.GetTileID(this.Single);
				}
				if (Tiler.Left)
				{
					return this.GetTileID(this.SingleHorizontalRight);
				}
				return this.GetTileID(this.SingleHorizontalLeft);
			}
			else if (!Tiler.Left && !Tiler.Right)
			{
				if (Tiler.Up && Tiler.Down)
				{
					return this.GetTileID(this.SingleVerticalCenter);
				}
				if (Tiler.Down)
				{
					return this.GetTileID(this.SingleVerticalTop);
				}
				return this.GetTileID(this.SingleVerticalBottom);
			}
			else
			{
				if (Tiler.Up && Tiler.Down && Tiler.Left && !Tiler.Right)
				{
					return this.GetTileID(this.Right);
				}
				if (Tiler.Up && Tiler.Down && !Tiler.Left && Tiler.Right)
				{
					return this.GetTileID(this.Left);
				}
				if (Tiler.Up && !Tiler.Left && Tiler.Right && !Tiler.Down)
				{
					return this.GetTileID(this.BottomLeft);
				}
				if (Tiler.Up && Tiler.Left && !Tiler.Right && !Tiler.Down)
				{
					return this.GetTileID(this.BottomRight);
				}
				if (Tiler.Down && Tiler.Right && !Tiler.Left && !Tiler.Up)
				{
					return this.GetTileID(this.TopLeft);
				}
				if (Tiler.Down && !Tiler.Right && Tiler.Left && !Tiler.Up)
				{
					return this.GetTileID(this.TopRight);
				}
				if (Tiler.Up && Tiler.Down && !Tiler.DownRight && Tiler.DownLeft)
				{
					return this.GetTileID(this.InsideTopLeft);
				}
				if (Tiler.Up && Tiler.Down && Tiler.DownRight && !Tiler.DownLeft)
				{
					return this.GetTileID(this.InsideTopRight);
				}
				if (Tiler.Up && Tiler.Down && Tiler.UpLeft && !Tiler.UpRight)
				{
					return this.GetTileID(this.InsideBottomLeft);
				}
				if (Tiler.Up && Tiler.Down && !Tiler.UpLeft && Tiler.UpRight)
				{
					return this.GetTileID(this.InsideBottomRight);
				}
				if (!Tiler.Down)
				{
					return this.GetTileID(this.Bottom);
				}
				return this.GetTileID(this.Top);
			}
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x0001FAC3 File Offset: 0x0001DCC3
		private int GetTileID(int[] choices)
		{
			if (choices.Length == 0)
			{
				return -1;
			}
			if (choices.Length == 1)
			{
				return choices[0];
			}
			return Calc.Random.Choose(choices);
		}

		// Token: 0x040006AB RID: 1707
		public int[] Center;

		// Token: 0x040006AC RID: 1708
		public int[] Single;

		// Token: 0x040006AD RID: 1709
		public int[] SingleHorizontalLeft;

		// Token: 0x040006AE RID: 1710
		public int[] SingleHorizontalCenter;

		// Token: 0x040006AF RID: 1711
		public int[] SingleHorizontalRight;

		// Token: 0x040006B0 RID: 1712
		public int[] SingleVerticalTop;

		// Token: 0x040006B1 RID: 1713
		public int[] SingleVerticalCenter;

		// Token: 0x040006B2 RID: 1714
		public int[] SingleVerticalBottom;

		// Token: 0x040006B3 RID: 1715
		public int[] Top;

		// Token: 0x040006B4 RID: 1716
		public int[] Bottom;

		// Token: 0x040006B5 RID: 1717
		public int[] Left;

		// Token: 0x040006B6 RID: 1718
		public int[] Right;

		// Token: 0x040006B7 RID: 1719
		public int[] TopLeft;

		// Token: 0x040006B8 RID: 1720
		public int[] TopRight;

		// Token: 0x040006B9 RID: 1721
		public int[] BottomLeft;

		// Token: 0x040006BA RID: 1722
		public int[] BottomRight;

		// Token: 0x040006BB RID: 1723
		public int[] InsideTopLeft;

		// Token: 0x040006BC RID: 1724
		public int[] InsideTopRight;

		// Token: 0x040006BD RID: 1725
		public int[] InsideBottomLeft;

		// Token: 0x040006BE RID: 1726
		public int[] InsideBottomRight;
	}
}

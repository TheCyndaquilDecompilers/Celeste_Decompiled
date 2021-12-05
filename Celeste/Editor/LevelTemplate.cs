using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Editor
{
	// Token: 0x02000393 RID: 915
	public class LevelTemplate
	{
		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06001D9F RID: 7583 RVA: 0x000CE3FC File Offset: 0x000CC5FC
		private Vector2 resizeHoldSize
		{
			get
			{
				return new Vector2((float)Math.Min(this.Width, 4), (float)Math.Min(this.Height, 4));
			}
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x000CE420 File Offset: 0x000CC620
		public LevelTemplate(LevelData data)
		{
			this.Name = data.Name;
			this.EditorColorIndex = data.EditorColorIndex;
			this.X = data.Bounds.X / 8;
			this.Y = data.Bounds.Y / 8;
			this.ActualWidth = data.Bounds.Width;
			this.ActualHeight = data.Bounds.Height;
			this.Width = (int)Math.Ceiling((double)((float)this.ActualWidth / 8f));
			this.Height = (int)Math.Ceiling((double)((float)this.ActualHeight / 8f));
			this.Grid = new Grid(8f, 8f, data.Solids);
			this.Back = new Grid(8f, 8f, data.Bg);
			for (int i = 0; i < this.Height; i++)
			{
				for (int j = 0; j < this.Width; j++)
				{
					int num = 0;
					while (j + num < this.Width && this.Back[j + num, i] && !this.Grid[j + num, i])
					{
						num++;
					}
					if (num > 0)
					{
						this.backs.Add(new Rectangle(j, i, num, 1));
						j += num - 1;
					}
				}
				for (int k = 0; k < this.Width; k++)
				{
					int num2 = 0;
					while (k + num2 < this.Width && this.Grid[k + num2, i])
					{
						num2++;
					}
					if (num2 > 0)
					{
						this.solids.Add(new Rectangle(k, i, num2, 1));
						k += num2 - 1;
					}
				}
			}
			this.Spawns = new List<Vector2>();
			foreach (Vector2 value in data.Spawns)
			{
				this.Spawns.Add(value / 8f - new Vector2((float)this.X, (float)this.Y));
			}
			this.Strawberries = new List<Vector2>();
			this.StrawberryMetadata = new List<string>();
			this.Checkpoints = new List<Vector2>();
			this.Jumpthrus = new List<Rectangle>();
			foreach (EntityData entityData in data.Entities)
			{
				if (entityData.Name.Equals("strawberry") || entityData.Name.Equals("snowberry"))
				{
					this.Strawberries.Add(entityData.Position / 8f);
					this.StrawberryMetadata.Add(entityData.Int("checkpointID", 0) + ":" + entityData.Int("order", 0));
				}
				else if (entityData.Name.Equals("checkpoint"))
				{
					this.Checkpoints.Add(entityData.Position / 8f);
				}
				else if (entityData.Name.Equals("jumpThru"))
				{
					this.Jumpthrus.Add(new Rectangle((int)(entityData.Position.X / 8f), (int)(entityData.Position.Y / 8f), entityData.Width / 8, 1));
				}
			}
			this.Dummy = data.Dummy;
			this.Type = LevelTemplateType.Level;
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x000CE804 File Offset: 0x000CCA04
		public LevelTemplate(int x, int y, int w, int h)
		{
			this.Name = "FILLER";
			this.X = x;
			this.Y = y;
			this.Width = w;
			this.Height = h;
			this.ActualWidth = w * 8;
			this.ActualHeight = h * 8;
			this.Type = LevelTemplateType.Filler;
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x000CE870 File Offset: 0x000CCA70
		public void RenderContents(Camera camera, List<LevelTemplate> allLevels)
		{
			if (this.Type == LevelTemplateType.Level)
			{
				bool flag = false;
				if (Engine.Scene.BetweenInterval(0.1f))
				{
					foreach (LevelTemplate levelTemplate in allLevels)
					{
						if (levelTemplate != this && levelTemplate.Rect.Intersects(this.Rect))
						{
							flag = true;
							break;
						}
					}
				}
				Draw.Rect((float)this.X, (float)this.Y, (float)this.Width, (float)this.Height, (flag ? Color.Red : Color.Black) * 0.5f);
				foreach (Rectangle rectangle in this.backs)
				{
					Draw.Rect((float)(this.X + rectangle.X), (float)(this.Y + rectangle.Y), (float)rectangle.Width, (float)rectangle.Height, this.Dummy ? LevelTemplate.dummyBgTilesColor : LevelTemplate.bgTilesColor);
				}
				foreach (Rectangle rectangle2 in this.solids)
				{
					Draw.Rect((float)(this.X + rectangle2.X), (float)(this.Y + rectangle2.Y), (float)rectangle2.Width, (float)rectangle2.Height, this.Dummy ? LevelTemplate.dummyFgTilesColor : LevelTemplate.fgTilesColor[this.EditorColorIndex]);
				}
				foreach (Vector2 vector in this.Spawns)
				{
					Draw.Rect((float)this.X + vector.X, (float)this.Y + vector.Y - 1f, 1f, 1f, Color.Red);
				}
				foreach (Vector2 vector2 in this.Strawberries)
				{
					Draw.HollowRect((float)this.X + vector2.X - 1f, (float)this.Y + vector2.Y - 2f, 3f, 3f, Color.LightPink);
				}
				foreach (Vector2 vector3 in this.Checkpoints)
				{
					Draw.HollowRect((float)this.X + vector3.X - 1f, (float)this.Y + vector3.Y - 2f, 3f, 3f, Color.Lime);
				}
				using (List<Rectangle>.Enumerator enumerator2 = this.Jumpthrus.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Rectangle rectangle3 = enumerator2.Current;
						Draw.Rect((float)(this.X + rectangle3.X), (float)(this.Y + rectangle3.Y), (float)rectangle3.Width, 1f, Color.Yellow);
					}
					return;
				}
			}
			Draw.Rect((float)this.X, (float)this.Y, (float)this.Width, (float)this.Height, LevelTemplate.dummyFgTilesColor);
			Draw.Rect((float)(this.X + this.Width) - this.resizeHoldSize.X, (float)(this.Y + this.Height) - this.resizeHoldSize.Y, this.resizeHoldSize.X, this.resizeHoldSize.Y, Color.Orange);
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x000CEC9C File Offset: 0x000CCE9C
		public void RenderOutline(Camera camera)
		{
			float t = 1f / camera.Zoom * 2f;
			if (this.Check(Vector2.Zero))
			{
				this.Outline((float)(this.X + 1), (float)(this.Y + 1), (float)(this.Width - 2), (float)(this.Height - 2), t, LevelTemplate.firstBorderColor);
			}
			this.Outline((float)this.X, (float)this.Y, (float)this.Width, (float)this.Height, t, this.Dummy ? LevelTemplate.dummyInactiveBorderColor : LevelTemplate.inactiveBorderColor);
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x000CED30 File Offset: 0x000CCF30
		public void RenderHighlight(Camera camera, bool hovered, bool selected)
		{
			if (selected || hovered)
			{
				float t = 1f / camera.Zoom * 2f;
				this.Outline((float)this.X, (float)this.Y, (float)this.Width, (float)this.Height, t, hovered ? LevelTemplate.hoveredBorderColor : LevelTemplate.selectedBorderColor);
			}
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x000CED88 File Offset: 0x000CCF88
		private void Outline(float x, float y, float w, float h, float t, Color color)
		{
			Draw.Line(x, y, x + w, y, color, t);
			Draw.Line(x + w, y, x + w, y + h, color, t);
			Draw.Line(x, y + h, x + w, y + h, color, t);
			Draw.Line(x, y, x, y + h, color, t);
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x000CEDDD File Offset: 0x000CCFDD
		public bool Check(Vector2 point)
		{
			return point.X >= (float)this.Left && point.Y >= (float)this.Top && point.X < (float)this.Right && point.Y < (float)this.Bottom;
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x000CEE20 File Offset: 0x000CD020
		public bool Check(Rectangle rect)
		{
			return this.Rect.Intersects(rect);
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x000CEE3C File Offset: 0x000CD03C
		public void StartMoving()
		{
			this.moveAnchor = new Vector2((float)this.X, (float)this.Y);
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x000CEE58 File Offset: 0x000CD058
		public void Move(Vector2 relativeMove, List<LevelTemplate> allLevels, bool snap)
		{
			this.X = (int)(this.moveAnchor.X + relativeMove.X);
			this.Y = (int)(this.moveAnchor.Y + relativeMove.Y);
			if (snap)
			{
				foreach (LevelTemplate levelTemplate in allLevels)
				{
					if (levelTemplate != this)
					{
						if (this.Bottom >= levelTemplate.Top && this.Top <= levelTemplate.Bottom)
						{
							bool flag = Math.Abs(this.Left - levelTemplate.Right) < 3;
							bool flag2 = Math.Abs(this.Right - levelTemplate.Left) < 3;
							if (flag)
							{
								this.Left = levelTemplate.Right;
							}
							else if (flag2)
							{
								this.Right = levelTemplate.Left;
							}
							if (flag || flag2)
							{
								if (Math.Abs(this.Top - levelTemplate.Top) < 3)
								{
									this.Top = levelTemplate.Top;
								}
								else if (Math.Abs(this.Bottom - levelTemplate.Bottom) < 3)
								{
									this.Bottom = levelTemplate.Bottom;
								}
							}
						}
						if (this.Right >= levelTemplate.Left && this.Left <= levelTemplate.Right)
						{
							bool flag3 = Math.Abs(this.Top - levelTemplate.Bottom) < 5;
							bool flag4 = Math.Abs(this.Bottom - levelTemplate.Top) < 5;
							if (flag3)
							{
								this.Top = levelTemplate.Bottom;
							}
							else if (flag4)
							{
								this.Bottom = levelTemplate.Top;
							}
							if (flag3 || flag4)
							{
								if (Math.Abs(this.Left - levelTemplate.Left) < 3)
								{
									this.Left = levelTemplate.Left;
								}
								else if (Math.Abs(this.Right - levelTemplate.Right) < 3)
								{
									this.Right = levelTemplate.Right;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x000CF05C File Offset: 0x000CD25C
		public void StartResizing()
		{
			this.resizeAnchor = new Vector2((float)this.Width, (float)this.Height);
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x000CF078 File Offset: 0x000CD278
		public void Resize(Vector2 relativeMove)
		{
			this.Width = Math.Max(1, (int)(this.resizeAnchor.X + relativeMove.X));
			this.Height = Math.Max(1, (int)(this.resizeAnchor.Y + relativeMove.Y));
			this.ActualWidth = this.Width * 8;
			this.ActualHeight = this.Height * 8;
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x000CF0E0 File Offset: 0x000CD2E0
		public bool ResizePosition(Vector2 mouse)
		{
			return mouse.X > (float)(this.X + this.Width) - this.resizeHoldSize.X && mouse.Y > (float)(this.Y + this.Height) - this.resizeHoldSize.Y && mouse.X < (float)(this.X + this.Width) && mouse.Y < (float)(this.Y + this.Height);
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06001DAD RID: 7597 RVA: 0x000CF15F File Offset: 0x000CD35F
		public Rectangle Rect
		{
			get
			{
				return new Rectangle(this.X, this.Y, this.Width, this.Height);
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06001DAE RID: 7598 RVA: 0x000CF17E File Offset: 0x000CD37E
		// (set) Token: 0x06001DAF RID: 7599 RVA: 0x000CF186 File Offset: 0x000CD386
		public int Left
		{
			get
			{
				return this.X;
			}
			set
			{
				this.X = value;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06001DB0 RID: 7600 RVA: 0x000CF18F File Offset: 0x000CD38F
		// (set) Token: 0x06001DB1 RID: 7601 RVA: 0x000CF197 File Offset: 0x000CD397
		public int Top
		{
			get
			{
				return this.Y;
			}
			set
			{
				this.Y = value;
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x000CF1A0 File Offset: 0x000CD3A0
		// (set) Token: 0x06001DB3 RID: 7603 RVA: 0x000CF1AF File Offset: 0x000CD3AF
		public int Right
		{
			get
			{
				return this.X + this.Width;
			}
			set
			{
				this.X = value - this.Width;
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x000CF1BF File Offset: 0x000CD3BF
		// (set) Token: 0x06001DB5 RID: 7605 RVA: 0x000CF1CE File Offset: 0x000CD3CE
		public int Bottom
		{
			get
			{
				return this.Y + this.Height;
			}
			set
			{
				this.Y = value - this.Height;
			}
		}

		// Token: 0x04001E6A RID: 7786
		public string Name;

		// Token: 0x04001E6B RID: 7787
		public LevelTemplateType Type;

		// Token: 0x04001E6C RID: 7788
		public int X;

		// Token: 0x04001E6D RID: 7789
		public int Y;

		// Token: 0x04001E6E RID: 7790
		public int Width;

		// Token: 0x04001E6F RID: 7791
		public int Height;

		// Token: 0x04001E70 RID: 7792
		public int ActualWidth;

		// Token: 0x04001E71 RID: 7793
		public int ActualHeight;

		// Token: 0x04001E72 RID: 7794
		public Grid Grid;

		// Token: 0x04001E73 RID: 7795
		public Grid Back;

		// Token: 0x04001E74 RID: 7796
		public List<Vector2> Spawns;

		// Token: 0x04001E75 RID: 7797
		public List<Vector2> Strawberries;

		// Token: 0x04001E76 RID: 7798
		public List<string> StrawberryMetadata;

		// Token: 0x04001E77 RID: 7799
		public List<Vector2> Checkpoints;

		// Token: 0x04001E78 RID: 7800
		public List<Rectangle> Jumpthrus;

		// Token: 0x04001E79 RID: 7801
		public bool Dummy;

		// Token: 0x04001E7A RID: 7802
		public int EditorColorIndex;

		// Token: 0x04001E7B RID: 7803
		private Vector2 moveAnchor;

		// Token: 0x04001E7C RID: 7804
		private Vector2 resizeAnchor;

		// Token: 0x04001E7D RID: 7805
		private List<Rectangle> solids = new List<Rectangle>();

		// Token: 0x04001E7E RID: 7806
		private List<Rectangle> backs = new List<Rectangle>();

		// Token: 0x04001E7F RID: 7807
		private static readonly Color bgTilesColor = Color.DarkSlateGray * 0.5f;

		// Token: 0x04001E80 RID: 7808
		private static readonly Color[] fgTilesColor = new Color[]
		{
			Color.White,
			Calc.HexToColor("f6735e"),
			Calc.HexToColor("85f65e"),
			Calc.HexToColor("37d7e3"),
			Calc.HexToColor("376be3"),
			Calc.HexToColor("c337e3"),
			Calc.HexToColor("e33773")
		};

		// Token: 0x04001E81 RID: 7809
		private static readonly Color inactiveBorderColor = Color.DarkSlateGray;

		// Token: 0x04001E82 RID: 7810
		private static readonly Color selectedBorderColor = Color.Red;

		// Token: 0x04001E83 RID: 7811
		private static readonly Color hoveredBorderColor = Color.Yellow;

		// Token: 0x04001E84 RID: 7812
		private static readonly Color dummyBgTilesColor = Color.DarkSlateGray * 0.5f;

		// Token: 0x04001E85 RID: 7813
		private static readonly Color dummyFgTilesColor = Color.LightGray;

		// Token: 0x04001E86 RID: 7814
		private static readonly Color dummyInactiveBorderColor = Color.DarkOrange;

		// Token: 0x04001E87 RID: 7815
		private static readonly Color firstBorderColor = Color.Aqua;
	}
}

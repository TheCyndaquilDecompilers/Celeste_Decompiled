using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200029B RID: 667
	public abstract class OuiJournalPage
	{
		// Token: 0x060014BA RID: 5306 RVA: 0x00072FA9 File Offset: 0x000711A9
		public OuiJournalPage(OuiJournal journal)
		{
			this.Journal = journal;
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00072FE2 File Offset: 0x000711E2
		public virtual void Redraw(VirtualRenderTarget buffer)
		{
			Engine.Graphics.GraphicsDevice.SetRenderTarget(buffer);
			Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Update()
		{
		}

		// Token: 0x04001085 RID: 4229
		public const int PageWidth = 1610;

		// Token: 0x04001086 RID: 4230
		public const int PageHeight = 1000;

		// Token: 0x04001087 RID: 4231
		public readonly Vector2 TextJustify = new Vector2(0.5f, 0.5f);

		// Token: 0x04001088 RID: 4232
		public const float TextScale = 0.5f;

		// Token: 0x04001089 RID: 4233
		public readonly Color TextColor = Color.Black * 0.6f;

		// Token: 0x0400108A RID: 4234
		public int PageIndex;

		// Token: 0x0400108B RID: 4235
		public string PageTexture;

		// Token: 0x0400108C RID: 4236
		public OuiJournal Journal;

		// Token: 0x02000626 RID: 1574
		public class Table
		{
			// Token: 0x170005B5 RID: 1461
			// (get) Token: 0x06002A54 RID: 10836 RVA: 0x0010FF11 File Offset: 0x0010E111
			public int Rows
			{
				get
				{
					return this.rows.Count;
				}
			}

			// Token: 0x170005B6 RID: 1462
			// (get) Token: 0x06002A55 RID: 10837 RVA: 0x0010FF1E File Offset: 0x0010E11E
			public OuiJournalPage.Row Header
			{
				get
				{
					if (this.rows.Count <= 0)
					{
						return null;
					}
					return this.rows[0];
				}
			}

			// Token: 0x06002A56 RID: 10838 RVA: 0x0010FF3C File Offset: 0x0010E13C
			public OuiJournalPage.Table AddColumn(OuiJournalPage.Cell label)
			{
				if (this.rows.Count == 0)
				{
					this.AddRow();
				}
				this.rows[0].Add(label);
				return this;
			}

			// Token: 0x06002A57 RID: 10839 RVA: 0x0010FF68 File Offset: 0x0010E168
			public OuiJournalPage.Row AddRow()
			{
				OuiJournalPage.Row row = new OuiJournalPage.Row();
				this.rows.Add(row);
				return row;
			}

			// Token: 0x06002A58 RID: 10840 RVA: 0x0010FF88 File Offset: 0x0010E188
			public float Height()
			{
				return 100f + 60f * (float)(this.rows.Count - 1);
			}

			// Token: 0x06002A59 RID: 10841 RVA: 0x0010FFA4 File Offset: 0x0010E1A4
			public void Render(Vector2 position)
			{
				if (this.Header == null)
				{
					return;
				}
				float num = 0f;
				float num2 = 0f;
				for (int i = 0; i < this.Header.Count; i++)
				{
					num2 += this.Header[i].Width() + 20f;
				}
				for (int j = 0; j < this.Header.Count; j++)
				{
					float num3 = this.Header[j].Width();
					this.Header[j].Render(position + new Vector2(num + num3 * 0.5f, 40f), num3);
					int num4 = 1;
					float num5 = 130f;
					for (int k = 1; k < this.rows.Count; k++)
					{
						Vector2 vector = position + new Vector2(num + num3 * 0.5f, num5);
						if (this.rows[k].Count > 0)
						{
							if (num4 % 2 == 0)
							{
								Draw.Rect(vector.X - num3 * 0.5f, vector.Y - 27f, num3 + 20f, 54f, Color.Black * 0.08f);
							}
							if (j < this.rows[k].Count && this.rows[k][j] != null)
							{
								OuiJournalPage.Cell cell = this.rows[k][j];
								if (cell.SpreadOverColumns > 1)
								{
									for (int l = j + 1; l < j + cell.SpreadOverColumns; l++)
									{
										vector.X += this.Header[l].Width() * 0.5f;
									}
									vector.X += (float)(cell.SpreadOverColumns - 1) * 20f * 0.5f;
								}
								this.rows[k][j].Render(vector, num3);
							}
							num4++;
							num5 += 60f;
						}
						else
						{
							Draw.Rect(vector.X - num3 * 0.5f, vector.Y - 25.5f, num3 + 20f, 6f, Color.Black * 0.3f);
							num5 += 15f;
						}
					}
					num += num3 + 20f;
				}
			}

			// Token: 0x04002965 RID: 10597
			private const float headerHeight = 80f;

			// Token: 0x04002966 RID: 10598
			private const float headerBottomMargin = 20f;

			// Token: 0x04002967 RID: 10599
			private const float rowHeight = 60f;

			// Token: 0x04002968 RID: 10600
			private List<OuiJournalPage.Row> rows = new List<OuiJournalPage.Row>();
		}

		// Token: 0x02000627 RID: 1575
		public class Row
		{
			// Token: 0x06002A5B RID: 10843 RVA: 0x00110232 File Offset: 0x0010E432
			public OuiJournalPage.Row Add(OuiJournalPage.Cell entry)
			{
				this.Entries.Add(entry);
				return this;
			}

			// Token: 0x170005B7 RID: 1463
			// (get) Token: 0x06002A5C RID: 10844 RVA: 0x00110241 File Offset: 0x0010E441
			public int Count
			{
				get
				{
					return this.Entries.Count;
				}
			}

			// Token: 0x170005B8 RID: 1464
			public OuiJournalPage.Cell this[int index]
			{
				get
				{
					return this.Entries[index];
				}
			}

			// Token: 0x04002969 RID: 10601
			public List<OuiJournalPage.Cell> Entries = new List<OuiJournalPage.Cell>();
		}

		// Token: 0x02000628 RID: 1576
		public abstract class Cell
		{
			// Token: 0x06002A5F RID: 10847 RVA: 0x000FBF17 File Offset: 0x000FA117
			public virtual float Width()
			{
				return 0f;
			}

			// Token: 0x06002A60 RID: 10848 RVA: 0x000091E2 File Offset: 0x000073E2
			public virtual void Render(Vector2 center, float columnWidth)
			{
			}

			// Token: 0x0400296A RID: 10602
			public int SpreadOverColumns = 1;
		}

		// Token: 0x02000629 RID: 1577
		public class EmptyCell : OuiJournalPage.Cell
		{
			// Token: 0x06002A62 RID: 10850 RVA: 0x0011027E File Offset: 0x0010E47E
			public EmptyCell(float width)
			{
				this.width = width;
			}

			// Token: 0x06002A63 RID: 10851 RVA: 0x0011028D File Offset: 0x0010E48D
			public override float Width()
			{
				return this.width;
			}

			// Token: 0x0400296B RID: 10603
			private float width;
		}

		// Token: 0x0200062A RID: 1578
		public class TextCell : OuiJournalPage.Cell
		{
			// Token: 0x06002A64 RID: 10852 RVA: 0x00110295 File Offset: 0x0010E495
			public TextCell(string text, Vector2 justify, float scale, Color color, float width = 0f, bool forceWidth = false)
			{
				this.text = text;
				this.justify = justify;
				this.scale = scale;
				this.color = color;
				this.width = width;
				this.forceWidth = forceWidth;
			}

			// Token: 0x06002A65 RID: 10853 RVA: 0x001102CA File Offset: 0x0010E4CA
			public override float Width()
			{
				if (this.forceWidth)
				{
					return this.width;
				}
				return Math.Max(this.width, ActiveFont.Measure(this.text).X * this.scale);
			}

			// Token: 0x06002A66 RID: 10854 RVA: 0x00110300 File Offset: 0x0010E500
			public override void Render(Vector2 center, float columnWidth)
			{
				float num = ActiveFont.Measure(this.text).X * this.scale;
				float scaleFactor = 1f;
				if (!this.forceWidth && num > columnWidth)
				{
					scaleFactor = columnWidth / num;
				}
				ActiveFont.Draw(this.text, center + new Vector2(-columnWidth / 2f + columnWidth * this.justify.X, 0f), this.justify, Vector2.One * this.scale * scaleFactor, this.color);
			}

			// Token: 0x0400296C RID: 10604
			private string text;

			// Token: 0x0400296D RID: 10605
			private Vector2 justify;

			// Token: 0x0400296E RID: 10606
			private float scale;

			// Token: 0x0400296F RID: 10607
			private Color color;

			// Token: 0x04002970 RID: 10608
			private float width;

			// Token: 0x04002971 RID: 10609
			private bool forceWidth;
		}

		// Token: 0x0200062B RID: 1579
		public class IconCell : OuiJournalPage.Cell
		{
			// Token: 0x06002A67 RID: 10855 RVA: 0x0011038E File Offset: 0x0010E58E
			public IconCell(string icon, float width = 0f)
			{
				this.icon = icon;
				this.width = width;
			}

			// Token: 0x06002A68 RID: 10856 RVA: 0x001103A4 File Offset: 0x0010E5A4
			public override float Width()
			{
				return Math.Max((float)MTN.Journal[this.icon].Width, this.width);
			}

			// Token: 0x06002A69 RID: 10857 RVA: 0x001103C7 File Offset: 0x0010E5C7
			public override void Render(Vector2 center, float columnWidth)
			{
				MTN.Journal[this.icon].DrawCentered(center);
			}

			// Token: 0x04002972 RID: 10610
			private string icon;

			// Token: 0x04002973 RID: 10611
			private float width;
		}

		// Token: 0x0200062C RID: 1580
		public class IconsCell : OuiJournalPage.Cell
		{
			// Token: 0x06002A6A RID: 10858 RVA: 0x001103DF File Offset: 0x0010E5DF
			public IconsCell(float iconSpacing, params string[] icons)
			{
				this.iconSpacing = iconSpacing;
				this.icons = icons;
			}

			// Token: 0x06002A6B RID: 10859 RVA: 0x00110400 File Offset: 0x0010E600
			public IconsCell(params string[] icons)
			{
				this.icons = icons;
			}

			// Token: 0x06002A6C RID: 10860 RVA: 0x0011041C File Offset: 0x0010E61C
			public override float Width()
			{
				float num = 0f;
				for (int i = 0; i < this.icons.Length; i++)
				{
					num += (float)MTN.Journal[this.icons[i]].Width;
				}
				return num + (float)(this.icons.Length - 1) * this.iconSpacing;
			}

			// Token: 0x06002A6D RID: 10861 RVA: 0x00110474 File Offset: 0x0010E674
			public override void Render(Vector2 center, float columnWidth)
			{
				float num = this.Width();
				Vector2 position = center + new Vector2(-num * 0.5f, 0f);
				for (int i = 0; i < this.icons.Length; i++)
				{
					MTexture mtexture = MTN.Journal[this.icons[i]];
					mtexture.DrawJustified(position, new Vector2(0f, 0.5f));
					position.X += (float)mtexture.Width + this.iconSpacing;
				}
			}

			// Token: 0x04002974 RID: 10612
			private float iconSpacing = 4f;

			// Token: 0x04002975 RID: 10613
			private string[] icons;
		}
	}
}

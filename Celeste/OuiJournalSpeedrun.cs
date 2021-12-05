using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200029D RID: 669
	public class OuiJournalSpeedrun : OuiJournalPage
	{
		// Token: 0x060014C2 RID: 5314 RVA: 0x000734FC File Offset: 0x000716FC
		public OuiJournalSpeedrun(OuiJournal journal) : base(journal)
		{
			this.PageTexture = "page";
			Vector2 justify = new Vector2(0.5f, 0.5f);
			float num = 0.5f;
			Color color = Color.Black * 0.6f;
			this.table = new OuiJournalPage.Table().AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_speedruns", null), new Vector2(1f, 0.5f), 0.7f, Color.Black * 0.7f, 0f, false)).AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_normal", null), justify, num + 0.1f, color, 240f, false)).AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_normal_fullclear", null), justify, num + 0.1f, color, 240f, false));
			if (SaveData.Instance.UnlockedModes >= 2)
			{
				this.table.AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_bside", null), justify, num + 0.1f, color, 240f, false));
			}
			if (SaveData.Instance.UnlockedModes >= 3)
			{
				this.table.AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_cside", null), justify, num + 0.1f, color, 240f, false));
			}
			foreach (AreaStats areaStats in SaveData.Instance.Areas)
			{
				AreaData areaData = AreaData.Get(areaStats.ID);
				if (!areaData.Interlude && !areaData.IsFinal)
				{
					if (areaData.ID > SaveData.Instance.UnlockedAreas)
					{
						break;
					}
					OuiJournalPage.Row row = this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name, null), new Vector2(1f, 0.5f), num + 0.1f, color, 0f, false));
					if (areaStats.Modes[0].BestTime > 0L)
					{
						row.Add(new OuiJournalPage.TextCell(Dialog.Time(areaStats.Modes[0].BestTime), justify, num, color, 0f, false));
					}
					else
					{
						row.Add(new OuiJournalPage.IconCell("dot", 0f));
					}
					if (areaData.CanFullClear)
					{
						if (areaStats.Modes[0].BestFullClearTime > 0L)
						{
							row.Add(new OuiJournalPage.TextCell(Dialog.Time(areaStats.Modes[0].BestFullClearTime), justify, num, color, 0f, false));
						}
						else
						{
							row.Add(new OuiJournalPage.IconCell("dot", 0f));
						}
					}
					else
					{
						row.Add(new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor, 0f, false));
					}
					if (SaveData.Instance.UnlockedModes >= 2)
					{
						if (areaData.HasMode(AreaMode.BSide))
						{
							if (areaStats.Modes[1].BestTime > 0L)
							{
								row.Add(new OuiJournalPage.TextCell(Dialog.Time(areaStats.Modes[1].BestTime), justify, num, color, 0f, false));
							}
							else
							{
								row.Add(new OuiJournalPage.IconCell("dot", 0f));
							}
						}
						else
						{
							row.Add(new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor, 0f, false));
						}
					}
					if (SaveData.Instance.UnlockedModes >= 3)
					{
						if (areaData.HasMode(AreaMode.CSide))
						{
							if (areaStats.Modes[2].BestTime > 0L)
							{
								row.Add(new OuiJournalPage.TextCell(Dialog.Time(areaStats.Modes[2].BestTime), justify, num, color, 0f, false));
							}
							else
							{
								row.Add(new OuiJournalPage.IconCell("dot", 0f));
							}
						}
						else
						{
							row.Add(new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor, 0f, false));
						}
					}
				}
			}
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			bool flag4 = true;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			long num5 = 0L;
			foreach (AreaStats areaStats2 in SaveData.Instance.Areas)
			{
				AreaData areaData2 = AreaData.Get(areaStats2.ID);
				if (!areaData2.Interlude && !areaData2.IsFinal)
				{
					if (areaStats2.ID > SaveData.Instance.UnlockedAreas)
					{
						flag2 = (flag = (flag3 = (flag4 = false)));
						break;
					}
					num2 += areaStats2.Modes[0].BestTime;
					num3 += areaStats2.Modes[0].BestFullClearTime;
					num4 += areaStats2.Modes[1].BestTime;
					num5 += areaStats2.Modes[2].BestTime;
					if (areaStats2.Modes[0].BestTime <= 0L)
					{
						flag = false;
					}
					if (areaData2.CanFullClear && areaStats2.Modes[0].BestFullClearTime <= 0L)
					{
						flag2 = false;
					}
					if (areaData2.HasMode(AreaMode.BSide) && areaStats2.Modes[1].BestTime <= 0L)
					{
						flag3 = false;
					}
					if (areaData2.HasMode(AreaMode.CSide) && areaStats2.Modes[2].BestTime <= 0L)
					{
						flag4 = false;
					}
				}
			}
			if (flag || flag2 || flag3 || flag4)
			{
				this.table.AddRow();
				OuiJournalPage.Row row2 = this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean("journal_totals", null), new Vector2(1f, 0.5f), num + 0.2f, color, 0f, false));
				if (flag)
				{
					row2.Add(new OuiJournalPage.TextCell(Dialog.Time(num2), justify, num + 0.1f, color, 0f, false));
				}
				else
				{
					row2.Add(new OuiJournalPage.IconCell("dot", 0f));
				}
				if (flag2)
				{
					row2.Add(new OuiJournalPage.TextCell(Dialog.Time(num3), justify, num + 0.1f, color, 0f, false));
				}
				else
				{
					row2.Add(new OuiJournalPage.IconCell("dot", 0f));
				}
				if (SaveData.Instance.UnlockedModes >= 2)
				{
					if (flag3)
					{
						row2.Add(new OuiJournalPage.TextCell(Dialog.Time(num4), justify, num + 0.1f, color, 0f, false));
					}
					else
					{
						row2.Add(new OuiJournalPage.IconCell("dot", 0f));
					}
				}
				if (SaveData.Instance.UnlockedModes >= 3)
				{
					if (flag4)
					{
						row2.Add(new OuiJournalPage.TextCell(Dialog.Time(num5), justify, num + 0.1f, color, 0f, false));
					}
					else
					{
						row2.Add(new OuiJournalPage.IconCell("dot", 0f));
					}
				}
				this.table.AddRow();
			}
			long num6 = 0L;
			foreach (AreaStats areaStats3 in SaveData.Instance.Areas)
			{
				AreaData areaData3 = AreaData.Get(areaStats3.ID);
				if (areaData3.IsFinal)
				{
					if (areaData3.ID > SaveData.Instance.UnlockedAreas)
					{
						break;
					}
					num6 += areaStats3.Modes[0].BestTime;
					OuiJournalPage.Row row3 = this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean(areaData3.Name, null), new Vector2(1f, 0.5f), num + 0.1f, color, 0f, false));
					row3.Add(null);
					if (areaStats3.Modes[0].BestTime > 0L)
					{
						row3.Add(new OuiJournalPage.TextCell(Dialog.Time(areaStats3.Modes[0].BestTime), justify, num, color, 0f, false));
					}
					else
					{
						row3.Add(new OuiJournalPage.IconCell("dot", 0f));
					}
					this.table.AddRow();
				}
			}
			if (flag && flag2 && flag3 && flag4)
			{
				OuiJournalPage.TextCell textCell = new OuiJournalPage.TextCell(Dialog.Time(num2 + num3 + num4 + num5 + num6), justify, num + 0.2f, color, 0f, false);
				textCell.SpreadOverColumns = 1 + SaveData.Instance.UnlockedModes;
				this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean("journal_grandtotal", null), new Vector2(1f, 0.5f), num + 0.3f, color, 0f, false)).Add(textCell);
			}
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x00073DF8 File Offset: 0x00071FF8
		public override void Redraw(VirtualRenderTarget buffer)
		{
			base.Redraw(buffer);
			Draw.SpriteBatch.Begin();
			this.table.Render(new Vector2(60f, 20f));
			Draw.SpriteBatch.End();
		}

		// Token: 0x0400109A RID: 4250
		private OuiJournalPage.Table table;
	}
}

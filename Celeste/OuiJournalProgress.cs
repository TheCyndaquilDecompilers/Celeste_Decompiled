using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200029E RID: 670
	public class OuiJournalProgress : OuiJournalPage
	{
		// Token: 0x060014C4 RID: 5316 RVA: 0x00073E30 File Offset: 0x00072030
		public OuiJournalProgress(OuiJournal journal) : base(journal)
		{
			this.PageTexture = "page";
			this.table = new OuiJournalPage.Table().AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_progress", null), new Vector2(0f, 0.5f), 1f, Color.Black * 0.7f, 0f, false)).AddColumn(new OuiJournalPage.EmptyCell(20f)).AddColumn(new OuiJournalPage.EmptyCell(64f)).AddColumn(new OuiJournalPage.EmptyCell(64f)).AddColumn(new OuiJournalPage.EmptyCell(100f)).AddColumn(new OuiJournalPage.IconCell("strawberry", 150f)).AddColumn(new OuiJournalPage.IconCell("skullblue", 100f));
			if (SaveData.Instance.UnlockedModes >= 2)
			{
				this.table.AddColumn(new OuiJournalPage.IconCell("skullred", 100f));
			}
			if (SaveData.Instance.UnlockedModes >= 3)
			{
				this.table.AddColumn(new OuiJournalPage.IconCell("skullgold", 100f));
			}
			this.table.AddColumn(new OuiJournalPage.IconCell("time", 220f));
			foreach (AreaStats areaStats in SaveData.Instance.Areas)
			{
				AreaData areaData = AreaData.Get(areaStats.ID);
				if (!areaData.Interlude)
				{
					if (areaData.ID > SaveData.Instance.UnlockedAreas)
					{
						break;
					}
					string text;
					if (areaData.Mode[0].TotalStrawberries > 0 || areaStats.TotalStrawberries > 0)
					{
						text = areaStats.TotalStrawberries.ToString();
						if (areaStats.Modes[0].Completed)
						{
							text = text + "/" + areaData.Mode[0].TotalStrawberries;
						}
					}
					else
					{
						text = "-";
					}
					List<string> list = new List<string>();
					for (int i = 0; i < areaStats.Modes.Length; i++)
					{
						if (areaStats.Modes[i].HeartGem)
						{
							list.Add("heartgem" + i);
						}
					}
					if (list.Count <= 0)
					{
						list.Add("dot");
					}
					OuiJournalPage.IconsCell iconsCell;
					OuiJournalPage.Row row = this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name, null), new Vector2(1f, 0.5f), 0.6f, this.TextColor, 0f, false)).Add(null).Add(iconsCell = new OuiJournalPage.IconsCell(new string[]
					{
						this.CompletionIcon(areaStats)
					}));
					if (areaData.CanFullClear)
					{
						row.Add(new OuiJournalPage.IconsCell(new string[]
						{
							areaStats.Cassette ? "cassette" : "dot"
						}));
						row.Add(new OuiJournalPage.IconsCell(-32f, list.ToArray()));
					}
					else
					{
						iconsCell.SpreadOverColumns = 3;
						row.Add(null).Add(null);
					}
					row.Add(new OuiJournalPage.TextCell(text, this.TextJustify, 0.5f, this.TextColor, 0f, false));
					if (areaData.IsFinal)
					{
						row.Add(new OuiJournalPage.TextCell(Dialog.Deaths(areaStats.Modes[0].Deaths), this.TextJustify, 0.5f, this.TextColor, 0f, false)
						{
							SpreadOverColumns = SaveData.Instance.UnlockedModes
						});
						for (int j = 0; j < SaveData.Instance.UnlockedModes - 1; j++)
						{
							row.Add(null);
						}
					}
					else
					{
						for (int k = 0; k < SaveData.Instance.UnlockedModes; k++)
						{
							if (areaData.HasMode((AreaMode)k))
							{
								row.Add(new OuiJournalPage.TextCell(Dialog.Deaths(areaStats.Modes[k].Deaths), this.TextJustify, 0.5f, this.TextColor, 0f, false));
							}
							else
							{
								row.Add(new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor, 0f, false));
							}
						}
					}
					if (areaStats.TotalTimePlayed > 0L)
					{
						row.Add(new OuiJournalPage.TextCell(Dialog.Time(areaStats.TotalTimePlayed), this.TextJustify, 0.5f, this.TextColor, 0f, false));
					}
					else
					{
						row.Add(new OuiJournalPage.IconCell("dot", 0f));
					}
				}
			}
			if (this.table.Rows > 1)
			{
				this.table.AddRow();
				OuiJournalPage.Row row2 = this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean("journal_totals", null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0f, false)).Add(null).Add(null).Add(null).Add(null).Add(new OuiJournalPage.TextCell(SaveData.Instance.TotalStrawberries.ToString(), this.TextJustify, 0.6f, this.TextColor, 0f, false));
				row2.Add(new OuiJournalPage.TextCell(Dialog.Deaths(SaveData.Instance.TotalDeaths), this.TextJustify, 0.6f, this.TextColor, 0f, false)
				{
					SpreadOverColumns = SaveData.Instance.UnlockedModes
				});
				for (int l = 1; l < SaveData.Instance.UnlockedModes; l++)
				{
					row2.Add(null);
				}
				row2.Add(new OuiJournalPage.TextCell(Dialog.Time(SaveData.Instance.Time), this.TextJustify, 0.6f, this.TextColor, 0f, false));
				this.table.AddRow();
			}
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x00074438 File Offset: 0x00072638
		private string CompletionIcon(AreaStats data)
		{
			if (!AreaData.Get(data.ID).CanFullClear && data.Modes[0].Completed)
			{
				return "beat";
			}
			if (data.Modes[0].FullClear)
			{
				return "fullclear";
			}
			if (data.Modes[0].Completed)
			{
				return "clear";
			}
			return "dot";
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x0007449B File Offset: 0x0007269B
		public override void Redraw(VirtualRenderTarget buffer)
		{
			base.Redraw(buffer);
			Draw.SpriteBatch.Begin();
			this.table.Render(new Vector2(60f, 20f));
			Draw.SpriteBatch.End();
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x000744D2 File Offset: 0x000726D2
		private void DrawIcon(Vector2 pos, bool obtained, string icon)
		{
			if (obtained)
			{
				MTN.Journal[icon].DrawCentered(pos);
				return;
			}
			MTN.Journal["dot"].DrawCentered(pos);
		}

		// Token: 0x0400109B RID: 4251
		private OuiJournalPage.Table table;
	}
}

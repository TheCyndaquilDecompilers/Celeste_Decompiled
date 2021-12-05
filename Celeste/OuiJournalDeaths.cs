using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000257 RID: 599
	public class OuiJournalDeaths : OuiJournalPage
	{
		// Token: 0x060012B5 RID: 4789 RVA: 0x00064ED0 File Offset: 0x000630D0
		public OuiJournalDeaths(OuiJournal journal) : base(journal)
		{
			this.PageTexture = "page";
			this.table = new OuiJournalPage.Table().AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_deaths", null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 300f, false));
			for (int i = 0; i < SaveData.Instance.UnlockedModes; i++)
			{
				this.table.AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("journal_mode_" + (AreaMode)i, null), this.TextJustify, 0.6f, this.TextColor, 240f, false));
			}
			bool[] array = new bool[]
			{
				true,
				SaveData.Instance.UnlockedModes >= 2,
				SaveData.Instance.UnlockedModes >= 3
			};
			int[] array2 = new int[3];
			foreach (AreaStats areaStats in SaveData.Instance.Areas)
			{
				AreaData areaData = AreaData.Get(areaStats.ID);
				if (!areaData.Interlude && !areaData.IsFinal)
				{
					if (areaData.ID > SaveData.Instance.UnlockedAreas)
					{
						array[0] = (array[1] = (array[2] = false));
						break;
					}
					OuiJournalPage.Row row = this.table.AddRow();
					row.Add(new OuiJournalPage.TextCell(Dialog.Clean(areaData.Name, null), new Vector2(1f, 0.5f), 0.6f, this.TextColor, 0f, false));
					for (int j = 0; j < SaveData.Instance.UnlockedModes; j++)
					{
						if (areaData.HasMode((AreaMode)j))
						{
							if (areaStats.Modes[j].SingleRunCompleted)
							{
								int num = areaStats.Modes[j].BestDeaths;
								if (num > 0)
								{
									foreach (EntityData entityData in AreaData.Areas[areaStats.ID].Mode[j].MapData.Goldenberries)
									{
										EntityID item = new EntityID(entityData.Level.Name, entityData.ID);
										if (areaStats.Modes[j].Strawberries.Contains(item))
										{
											num = 0;
										}
									}
								}
								row.Add(new OuiJournalPage.TextCell(Dialog.Deaths(num), this.TextJustify, 0.5f, this.TextColor, 0f, false));
								array2[j] += num;
							}
							else
							{
								row.Add(new OuiJournalPage.IconCell("dot", 0f));
								array[j] = false;
							}
						}
						else
						{
							row.Add(new OuiJournalPage.TextCell("-", this.TextJustify, 0.5f, this.TextColor, 0f, false));
						}
					}
				}
			}
			if (array[0] || array[1] || array[2])
			{
				this.table.AddRow();
				OuiJournalPage.Row row2 = this.table.AddRow();
				row2.Add(new OuiJournalPage.TextCell(Dialog.Clean("journal_totals", null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0f, false));
				for (int k = 0; k < SaveData.Instance.UnlockedModes; k++)
				{
					row2.Add(new OuiJournalPage.TextCell(Dialog.Deaths(array2[k]), this.TextJustify, 0.6f, this.TextColor, 0f, false));
				}
				this.table.AddRow();
			}
			int num2 = 0;
			foreach (AreaStats areaStats2 in SaveData.Instance.Areas)
			{
				AreaData areaData2 = AreaData.Get(areaStats2.ID);
				if (areaData2.IsFinal)
				{
					if (areaData2.ID > SaveData.Instance.UnlockedAreas)
					{
						break;
					}
					OuiJournalPage.Row row3 = this.table.AddRow();
					row3.Add(new OuiJournalPage.TextCell(Dialog.Clean(areaData2.Name, null), new Vector2(1f, 0.5f), 0.6f, this.TextColor, 0f, false));
					if (areaStats2.Modes[0].SingleRunCompleted)
					{
						int num3 = areaStats2.Modes[0].BestDeaths;
						if (num3 > 0)
						{
							foreach (EntityData entityData2 in AreaData.Areas[areaStats2.ID].Mode[0].MapData.Goldenberries)
							{
								EntityID item2 = new EntityID(entityData2.Level.Name, entityData2.ID);
								if (areaStats2.Modes[0].Strawberries.Contains(item2))
								{
									num3 = 0;
								}
							}
						}
						OuiJournalPage.TextCell entry = new OuiJournalPage.TextCell(Dialog.Deaths(num3), this.TextJustify, 0.5f, this.TextColor, 0f, false);
						row3.Add(entry);
						num2 += num3;
					}
					else
					{
						row3.Add(new OuiJournalPage.IconCell("dot", 0f));
					}
					this.table.AddRow();
				}
			}
			if (array[0] && array[1] && array[2])
			{
				OuiJournalPage.TextCell textCell = new OuiJournalPage.TextCell(Dialog.Deaths(array2[0] + array2[1] + array2[2] + num2), this.TextJustify, 0.6f, this.TextColor, 0f, false);
				textCell.SpreadOverColumns = 3;
				this.table.AddRow().Add(new OuiJournalPage.TextCell(Dialog.Clean("journal_grandtotal", null), new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0f, false)).Add(textCell);
			}
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x0006555C File Offset: 0x0006375C
		public override void Redraw(VirtualRenderTarget buffer)
		{
			base.Redraw(buffer);
			Draw.SpriteBatch.Begin();
			this.table.Render(new Vector2(60f, 20f));
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000EB2 RID: 3762
		private OuiJournalPage.Table table;
	}
}

using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000249 RID: 585
	public class OuiJournalGlobal : OuiJournalPage
	{
		// Token: 0x0600126E RID: 4718 RVA: 0x00061148 File Offset: 0x0005F348
		public OuiJournalGlobal(OuiJournal journal) : base(journal)
		{
			this.PageTexture = "page";
			this.table = new OuiJournalPage.Table().AddColumn(new OuiJournalPage.TextCell("", new Vector2(1f, 0.5f), 1f, this.TextColor, 700f, false)).AddColumn(new OuiJournalPage.TextCell(Dialog.Clean("STATS_TITLE", null), new Vector2(0.5f, 0.5f), 1f, this.TextColor, 48f, true)).AddColumn(new OuiJournalPage.TextCell("", new Vector2(1f, 0.5f), 0.7f, this.TextColor, 700f, false));
			foreach (object obj in Enum.GetValues(typeof(Stat)))
			{
				Stat stat = (Stat)obj;
				if (SaveData.Instance.CheatMode || SaveData.Instance.DebugMode || ((stat != Stat.GOLDBERRIES || SaveData.Instance.TotalHeartGems >= 16) && ((stat != Stat.PICO_BERRIES && stat != Stat.PICO_COMPLETES && stat != Stat.PICO_DEATHS) || Settings.Instance.Pico8OnMainMenu)))
				{
					string text = Stats.Global(stat).ToString();
					string text2 = Stats.Name(stat);
					string text3 = "";
					int i = text.Length - 1;
					int num = 0;
					while (i >= 0)
					{
						text3 = text[i].ToString() + ((num > 0 && num % 3 == 0) ? "," : "") + text3;
						i--;
						num++;
					}
					OuiJournalPage.Row row = this.table.AddRow();
					row.Add(new OuiJournalPage.TextCell(text2, new Vector2(1f, 0.5f), 0.7f, this.TextColor, 0f, false));
					row.Add(null);
					row.Add(new OuiJournalPage.TextCell(text3, new Vector2(0f, 0.5f), 0.8f, this.TextColor, 0f, false));
				}
			}
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0006138C File Offset: 0x0005F58C
		public override void Redraw(VirtualRenderTarget buffer)
		{
			base.Redraw(buffer);
			Draw.SpriteBatch.Begin();
			this.table.Render(new Vector2(60f, 20f));
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000E29 RID: 3625
		private OuiJournalPage.Table table;
	}
}

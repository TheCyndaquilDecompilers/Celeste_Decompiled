using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200029A RID: 666
	public class OuiJournalCover : OuiJournalPage
	{
		// Token: 0x060014B8 RID: 5304 RVA: 0x00072EC5 File Offset: 0x000710C5
		public OuiJournalCover(OuiJournal journal) : base(journal)
		{
			this.PageTexture = "cover";
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x00072EDC File Offset: 0x000710DC
		public override void Redraw(VirtualRenderTarget buffer)
		{
			base.Redraw(buffer);
			Draw.SpriteBatch.Begin();
			string text = Dialog.Clean("journal_of", null);
			if (text.Length > 0)
			{
				text += "\n";
			}
			if (SaveData.Instance != null && Dialog.Language.CanDisplay(SaveData.Instance.Name))
			{
				text += SaveData.Instance.Name;
			}
			else
			{
				text += Dialog.Clean("FILE_DEFAULT", null);
			}
			ActiveFont.Draw(text, new Vector2(805f, 400f), new Vector2(0.5f, 0.5f), Vector2.One * 2f, Color.Black * 0.5f);
			Draw.SpriteBatch.End();
		}
	}
}

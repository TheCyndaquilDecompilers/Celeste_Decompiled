using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001FE RID: 510
	public class PrologueEndingText : Entity
	{
		// Token: 0x060010B7 RID: 4279 RVA: 0x0004E494 File Offset: 0x0004C694
		public PrologueEndingText(bool instant)
		{
			base.Tag = Tags.HUD;
			this.text = FancyText.Parse(Dialog.Clean("CH0_END", null), 960, 4, 0f, null, null);
			base.Add(new Coroutine(this.Routine(instant), true));
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0004E4F5 File Offset: 0x0004C6F5
		private IEnumerator Routine(bool instant)
		{
			if (!instant)
			{
				yield return 4f;
			}
			int num;
			for (int i = 0; i < this.text.Count; i = num + 1)
			{
				FancyText.Char c = this.text[i] as FancyText.Char;
				if (c != null)
				{
					while ((c.Fade += Engine.DeltaTime * 20f) < 1f)
					{
						yield return null;
					}
					c.Fade = 1f;
					c = null;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x0004E50B File Offset: 0x0004C70B
		public override void Render()
		{
			this.text.Draw(this.Position, new Vector2(0.5f, 0.5f), Vector2.One, 1f, 0, int.MaxValue);
		}

		// Token: 0x04000C31 RID: 3121
		private FancyText.Text text;
	}
}

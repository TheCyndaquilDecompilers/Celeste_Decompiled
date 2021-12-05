using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D9 RID: 473
	public class WaveDashPage02 : WaveDashPage
	{
		// Token: 0x06000FDF RID: 4063 RVA: 0x000437C9 File Offset: 0x000419C9
		public WaveDashPage02()
		{
			this.Transition = WaveDashPage.Transitions.Rotate3D;
			this.ClearColor = Calc.HexToColor("fff2cc");
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x000437F3 File Offset: 0x000419F3
		public override void Added(WaveDashPresentation presentation)
		{
			base.Added(presentation);
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x000437FC File Offset: 0x000419FC
		public override IEnumerator Routine()
		{
			string[] text = Dialog.Clean("WAVEDASH_PAGE2_TITLE", null).Split(new char[]
			{
				'|'
			});
			Vector2 pos = new Vector2(128f, 128f);
			int num;
			for (int i = 0; i < text.Length; i = num + 1)
			{
				WaveDashPage02.TitleText item = new WaveDashPage02.TitleText(pos, text[i]);
				this.title.Add(item);
				yield return item.Stamp();
				pos.X += item.Width + ActiveFont.Measure(' ').X * 1.5f;
				item = null;
				num = i;
			}
			text = null;
			pos = default(Vector2);
			yield return base.PressButton();
			this.list = FancyText.Parse(Dialog.Get("WAVEDASH_PAGE2_LIST", null), base.Width, 32, 1f, new Color?(Color.Black * 0.7f), null);
			float delay = 0f;
			while (this.listIndex < this.list.Nodes.Count)
			{
				if (this.list.Nodes[this.listIndex] is FancyText.NewLine)
				{
					yield return base.PressButton();
				}
				else
				{
					delay += 0.008f;
					if (delay >= 0.016f)
					{
						delay -= 0.016f;
						yield return 0.016f;
					}
				}
				this.listIndex++;
			}
			yield return base.PressButton();
			Audio.Play("event:/new_content/game/10_farewell/ppt_impossible");
			while (this.impossibleEase < 1f)
			{
				this.impossibleEase = Calc.Approach(this.impossibleEase, 1f, Engine.DeltaTime);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void Update()
		{
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0004380C File Offset: 0x00041A0C
		public override void Render()
		{
			foreach (WaveDashPage02.TitleText titleText in this.title)
			{
				titleText.Render();
			}
			if (this.list != null)
			{
				this.list.Draw(new Vector2(160f, 260f), new Vector2(0f, 0f), Vector2.One, 1f, 0, this.listIndex);
			}
			if (this.impossibleEase > 0f)
			{
				MTexture mtexture = this.Presentation.Gfx["Guy Clip Art"];
				float num = 0.75f;
				mtexture.Draw(new Vector2((float)base.Width - (float)mtexture.Width * num, (float)base.Height - 640f * this.impossibleEase), Vector2.Zero, Color.White, num);
				Matrix transformMatrix = Matrix.CreateRotationZ(-0.5f + Ease.CubeIn(1f - this.impossibleEase) * 8f) * Matrix.CreateTranslation((float)(base.Width - 500), (float)(base.Height - 600), 0f);
				Draw.SpriteBatch.End();
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, transformMatrix);
				ActiveFont.Draw(Dialog.Clean("WAVEDASH_PAGE2_IMPOSSIBLE", null), Vector2.Zero, new Vector2(0.5f, 0.5f), Vector2.One * (2f + (1f - this.impossibleEase) * 0.5f), Color.Black * this.impossibleEase);
				Draw.SpriteBatch.End();
				Draw.SpriteBatch.Begin();
			}
		}

		// Token: 0x04000B3D RID: 2877
		private List<WaveDashPage02.TitleText> title = new List<WaveDashPage02.TitleText>();

		// Token: 0x04000B3E RID: 2878
		private FancyText.Text list;

		// Token: 0x04000B3F RID: 2879
		private int listIndex;

		// Token: 0x04000B40 RID: 2880
		private float impossibleEase;

		// Token: 0x020004DE RID: 1246
		private class TitleText
		{
			// Token: 0x0600245D RID: 9309 RVA: 0x000F3575 File Offset: 0x000F1775
			public TitleText(Vector2 pos, string text)
			{
				this.Position = pos;
				this.Text = text;
				this.Width = ActiveFont.Measure(text).X * 1.5f;
			}

			// Token: 0x0600245E RID: 9310 RVA: 0x000F35A2 File Offset: 0x000F17A2
			public IEnumerator Stamp()
			{
				while (this.ease < 1f)
				{
					this.ease = Calc.Approach(this.ease, 1f, Engine.DeltaTime * 4f);
					yield return null;
				}
				yield return 0.2f;
				yield break;
			}

			// Token: 0x0600245F RID: 9311 RVA: 0x000F35B4 File Offset: 0x000F17B4
			public void Render()
			{
				if (this.ease <= 0f)
				{
					return;
				}
				Vector2 scale = Vector2.One * (1f + (1f - Ease.CubeOut(this.ease))) * 1.5f;
				ActiveFont.DrawOutline(this.Text, this.Position + new Vector2(this.Width / 2f, ActiveFont.LineHeight * 0.5f * 1.5f), new Vector2(0.5f, 0.5f), scale, Color.White, 2f, Color.Black);
			}

			// Token: 0x040023F6 RID: 9206
			public const float Scale = 1.5f;

			// Token: 0x040023F7 RID: 9207
			public string Text;

			// Token: 0x040023F8 RID: 9208
			public Vector2 Position;

			// Token: 0x040023F9 RID: 9209
			public float Width;

			// Token: 0x040023FA RID: 9210
			private float ease;
		}
	}
}

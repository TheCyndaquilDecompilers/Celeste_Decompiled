using System;
using System.Collections;
using System.Globalization;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001DB RID: 475
	public class WaveDashPage00 : WaveDashPage
	{
		// Token: 0x06000FE9 RID: 4073 RVA: 0x00043AF8 File Offset: 0x00041CF8
		public WaveDashPage00()
		{
			this.AutoProgress = true;
			this.ClearColor = Calc.HexToColor("118475");
			this.time = DateTime.Now.ToString("h:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
			this.pptIcon = new Vector2(600f, 500f);
			this.cursor = new Vector2(1000f, 700f);
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x00043B7E File Offset: 0x00041D7E
		public override IEnumerator Routine()
		{
			yield return 1f;
			yield return this.MoveCursor(this.cursor + new Vector2(0f, -80f), 0.3f);
			yield return 0.2f;
			yield return this.MoveCursor(this.pptIcon, 0.8f);
			yield return 0.7f;
			this.selected = true;
			Audio.Play("event:/new_content/game/10_farewell/ppt_doubleclick");
			yield return 0.1f;
			this.selected = false;
			yield return 0.1f;
			this.selected = true;
			yield return 0.08f;
			this.selected = false;
			yield return 0.5f;
			this.Presentation.ScaleInPoint = this.pptIcon;
			yield break;
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x00043B8D File Offset: 0x00041D8D
		private IEnumerator MoveCursor(Vector2 to, float time)
		{
			Vector2 from = this.cursor;
			for (float t = 0f; t < 1f; t += Engine.DeltaTime / time)
			{
				this.cursor = from + (to - from) * Ease.SineOut(t);
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000FEC RID: 4076 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void Update()
		{
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x00043BAC File Offset: 0x00041DAC
		public override void Render()
		{
			this.DrawIcon(new Vector2(160f, 120f), "desktop/mymountain_icon", Dialog.Clean("WAVEDASH_DESKTOP_MYPC", null));
			this.DrawIcon(new Vector2(160f, 320f), "desktop/recyclebin_icon", Dialog.Clean("WAVEDASH_DESKTOP_RECYCLEBIN", null));
			this.DrawIcon(this.pptIcon, "desktop/wavedashing_icon", Dialog.Clean("WAVEDASH_DESKTOP_POWERPOINT", null));
			this.DrawTaskbar();
			this.Presentation.Gfx["desktop/cursor"].DrawCentered(this.cursor);
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00043C48 File Offset: 0x00041E48
		public void DrawTaskbar()
		{
			Draw.Rect(0f, (float)base.Height - 80f, (float)base.Width, 80f, this.taskbarColor);
			Draw.Rect(0f, (float)base.Height - 80f, (float)base.Width, 4f, Color.White * 0.5f);
			MTexture mtexture = this.Presentation.Gfx["desktop/startberry"];
			float num = 64f;
			float num2 = num / (float)mtexture.Height * 0.7f;
			string text = Dialog.Clean("WAVEDASH_DESKTOP_STARTBUTTON", null);
			float num3 = 0.6f;
			float width = (float)mtexture.Width * num2 + ActiveFont.Measure(text).X * num3 + 32f;
			Vector2 vector = new Vector2(8f, (float)base.Height - 80f + 8f);
			Draw.Rect(vector.X, vector.Y, width, num, Color.White * 0.5f);
			mtexture.DrawJustified(vector + new Vector2(8f, num / 2f), new Vector2(0f, 0.5f), Color.White, Vector2.One * num2);
			ActiveFont.Draw(text, vector + new Vector2((float)mtexture.Width * num2 + 16f, num / 2f), new Vector2(0f, 0.5f), Vector2.One * num3, Color.Black * 0.8f);
			ActiveFont.Draw(this.time, new Vector2((float)base.Width - 24f, (float)base.Height - 40f), new Vector2(1f, 0.5f), Vector2.One * 0.6f, Color.Black * 0.8f);
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00043E3C File Offset: 0x0004203C
		private void DrawIcon(Vector2 position, string icon, string text)
		{
			bool flag = this.cursor.X > position.X - 64f && this.cursor.Y > position.Y - 64f && this.cursor.X < position.X + 64f && this.cursor.Y < position.Y + 80f;
			if (this.selected && flag)
			{
				Draw.Rect(position.X - 80f, position.Y - 80f, 160f, 200f, Color.White * 0.25f);
			}
			if (flag)
			{
				this.DrawDottedRect(position.X - 80f, position.Y - 80f, 160f, 200f);
			}
			MTexture mtexture = this.Presentation.Gfx[icon];
			float scale = 128f / (float)mtexture.Height;
			mtexture.DrawCentered(position, Color.White, scale);
			ActiveFont.Draw(text, position + new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One * 0.6f, (this.selected && flag) ? Color.Black : Color.White);
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x00043F98 File Offset: 0x00042198
		private void DrawDottedRect(float x, float y, float w, float h)
		{
			float num = 4f;
			Draw.Rect(x, y, w, num, Color.White);
			Draw.Rect(x + w - num, y, num, h, Color.White);
			Draw.Rect(x, y, num, h, Color.White);
			Draw.Rect(x, y + h - num, w, num, Color.White);
			if (!this.selected)
			{
				for (float num2 = 4f; num2 < w; num2 += num * 2f)
				{
					Draw.Rect(x + num2, y, num, num, this.ClearColor);
					Draw.Rect(x + w - num2, y + h - num, num, num, this.ClearColor);
				}
				for (float num3 = 4f; num3 < h; num3 += num * 2f)
				{
					Draw.Rect(x, y + num3, num, num, this.ClearColor);
					Draw.Rect(x + w - num, y + h - num3, num, num, this.ClearColor);
				}
			}
		}

		// Token: 0x04000B43 RID: 2883
		private Color taskbarColor = Calc.HexToColor("d9d3b1");

		// Token: 0x04000B44 RID: 2884
		private string time;

		// Token: 0x04000B45 RID: 2885
		private Vector2 pptIcon;

		// Token: 0x04000B46 RID: 2886
		private Vector2 cursor;

		// Token: 0x04000B47 RID: 2887
		private bool selected;
	}
}

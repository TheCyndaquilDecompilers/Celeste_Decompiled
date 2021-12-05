using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200021A RID: 538
	public static class ButtonUI
	{
		// Token: 0x0600116D RID: 4461 RVA: 0x00055C74 File Offset: 0x00053E74
		public static float Width(string label, VirtualButton button)
		{
			MTexture mtexture = Input.GuiButton(button, Input.PrefixMode.Latest, "controls/keyboard/oemquestion");
			return ActiveFont.Measure(label).X + 8f + (float)mtexture.Width;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x00055CA8 File Offset: 0x00053EA8
		public static void Render(Vector2 position, string label, VirtualButton button, float scale, float justifyX = 0.5f, float wiggle = 0f, float alpha = 1f)
		{
			MTexture mtexture = Input.GuiButton(button, Input.PrefixMode.Latest, "controls/keyboard/oemquestion");
			float num = ButtonUI.Width(label, button);
			position.X -= scale * num * (justifyX - 0.5f);
			mtexture.Draw(position, new Vector2((float)mtexture.Width - num / 2f, (float)mtexture.Height / 2f), Color.White * alpha, scale + wiggle);
			ButtonUI.DrawText(label, position, num / 2f, scale + wiggle, alpha);
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x00055D30 File Offset: 0x00053F30
		private static void DrawText(string text, Vector2 position, float justify, float scale, float alpha)
		{
			float x = ActiveFont.Measure(text).X;
			ActiveFont.DrawOutline(text, position, new Vector2(justify / x, 0.5f), Vector2.One * scale, Color.White * alpha, 2f, Color.Black * alpha);
		}
	}
}

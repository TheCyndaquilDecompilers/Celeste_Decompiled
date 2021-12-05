using System;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200030A RID: 778
	public static class ColorGrade
	{
		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600183D RID: 6205 RVA: 0x00098437 File Offset: 0x00096637
		public static Effect Effect
		{
			get
			{
				return GFX.FxColorGrading;
			}
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x0009843E File Offset: 0x0009663E
		public static void Set(MTexture grade)
		{
			ColorGrade.Set(grade, grade, 0f);
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0009844C File Offset: 0x0009664C
		public static void Set(MTexture fromTex, MTexture toTex, float p)
		{
			if (!ColorGrade.Enabled || fromTex == null || toTex == null)
			{
				ColorGrade.from = GFX.ColorGrades["none"];
				ColorGrade.to = GFX.ColorGrades["none"];
			}
			else
			{
				ColorGrade.from = fromTex;
				ColorGrade.to = toTex;
			}
			ColorGrade.percent = Calc.Clamp(p, 0f, 1f);
			if (ColorGrade.from == ColorGrade.to || ColorGrade.percent <= 0f)
			{
				ColorGrade.Effect.CurrentTechnique = ColorGrade.Effect.Techniques["ColorGradeSingle"];
				Engine.Graphics.GraphicsDevice.Textures[1] = ColorGrade.from.Texture.Texture;
				return;
			}
			if (ColorGrade.percent >= 1f)
			{
				ColorGrade.Effect.CurrentTechnique = ColorGrade.Effect.Techniques["ColorGradeSingle"];
				Engine.Graphics.GraphicsDevice.Textures[1] = ColorGrade.to.Texture.Texture;
				return;
			}
			ColorGrade.Effect.CurrentTechnique = ColorGrade.Effect.Techniques["ColorGrade"];
			ColorGrade.Effect.Parameters["percent"].SetValue(ColorGrade.percent);
			Engine.Graphics.GraphicsDevice.Textures[1] = ColorGrade.from.Texture.Texture;
			Engine.Graphics.GraphicsDevice.Textures[2] = ColorGrade.to.Texture.Texture;
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06001840 RID: 6208 RVA: 0x000985DF File Offset: 0x000967DF
		// (set) Token: 0x06001841 RID: 6209 RVA: 0x000985E6 File Offset: 0x000967E6
		public static float Percent
		{
			get
			{
				return ColorGrade.percent;
			}
			set
			{
				ColorGrade.Set(ColorGrade.from, ColorGrade.to, value);
			}
		}

		// Token: 0x04001521 RID: 5409
		public static bool Enabled = true;

		// Token: 0x04001522 RID: 5410
		private static MTexture from;

		// Token: 0x04001523 RID: 5411
		private static MTexture to;

		// Token: 0x04001524 RID: 5412
		private static float percent = 0f;
	}
}

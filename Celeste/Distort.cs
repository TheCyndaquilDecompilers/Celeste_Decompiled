using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200035F RID: 863
	public static class Distort
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06001B2F RID: 6959 RVA: 0x000B191D File Offset: 0x000AFB1D
		// (set) Token: 0x06001B30 RID: 6960 RVA: 0x000B1924 File Offset: 0x000AFB24
		public static Vector2 AnxietyOrigin
		{
			get
			{
				return Distort.anxietyOrigin;
			}
			set
			{
				EffectParameter effectParameter = GFX.FxDistort.Parameters["anxietyOrigin"];
				Distort.anxietyOrigin = value;
				effectParameter.SetValue(value);
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001B31 RID: 6961 RVA: 0x000B1946 File Offset: 0x000AFB46
		// (set) Token: 0x06001B32 RID: 6962 RVA: 0x000B194D File Offset: 0x000AFB4D
		public static float Anxiety
		{
			get
			{
				return Distort.anxiety;
			}
			set
			{
				Distort.anxiety = value;
				GFX.FxDistort.Parameters["anxiety"].SetValue((!Settings.Instance.DisableFlashes) ? Distort.anxiety : 0f);
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06001B33 RID: 6963 RVA: 0x000B1986 File Offset: 0x000AFB86
		// (set) Token: 0x06001B34 RID: 6964 RVA: 0x000B198D File Offset: 0x000AFB8D
		public static float GameRate
		{
			get
			{
				return Distort.gamerate;
			}
			set
			{
				EffectParameter effectParameter = GFX.FxDistort.Parameters["gamerate"];
				Distort.gamerate = value;
				effectParameter.SetValue(value);
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06001B35 RID: 6965 RVA: 0x000B19AF File Offset: 0x000AFBAF
		// (set) Token: 0x06001B36 RID: 6966 RVA: 0x000B19B6 File Offset: 0x000AFBB6
		public static float WaterSine
		{
			get
			{
				return Distort.waterSine;
			}
			set
			{
				GFX.FxDistort.Parameters["waterSine"].SetValue(Distort.waterSine = Distort.WaterSineDirection * value);
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06001B37 RID: 6967 RVA: 0x000B19DE File Offset: 0x000AFBDE
		// (set) Token: 0x06001B38 RID: 6968 RVA: 0x000B19E5 File Offset: 0x000AFBE5
		public static float WaterCameraY
		{
			get
			{
				return Distort.waterCameraY;
			}
			set
			{
				EffectParameter effectParameter = GFX.FxDistort.Parameters["waterCameraY"];
				Distort.waterCameraY = value;
				effectParameter.SetValue(value);
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000B1A07 File Offset: 0x000AFC07
		// (set) Token: 0x06001B3A RID: 6970 RVA: 0x000B1A0E File Offset: 0x000AFC0E
		public static float WaterAlpha
		{
			get
			{
				return Distort.waterAlpha;
			}
			set
			{
				EffectParameter effectParameter = GFX.FxDistort.Parameters["waterAlpha"];
				Distort.waterAlpha = value;
				effectParameter.SetValue(value);
			}
		}

		// Token: 0x06001B3B RID: 6971 RVA: 0x000B1A30 File Offset: 0x000AFC30
		public static void Render(Texture2D source, Texture2D map, bool hasDistortion)
		{
			Effect fxDistort = GFX.FxDistort;
			if (fxDistort != null && (Distort.anxiety > 0f || Distort.gamerate < 1f || hasDistortion))
			{
				if (Distort.anxiety > 0f || Distort.gamerate < 1f)
				{
					fxDistort.CurrentTechnique = fxDistort.Techniques["Distort"];
				}
				else
				{
					fxDistort.CurrentTechnique = fxDistort.Techniques["Displace"];
				}
				Engine.Graphics.GraphicsDevice.Textures[1] = map;
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, fxDistort);
				Draw.SpriteBatch.Draw(source, Vector2.Zero, Color.White);
				Draw.SpriteBatch.End();
				return;
			}
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
			Draw.SpriteBatch.Draw(source, Vector2.Zero, Color.White);
			Draw.SpriteBatch.End();
		}

		// Token: 0x0400180E RID: 6158
		private static Vector2 anxietyOrigin;

		// Token: 0x0400180F RID: 6159
		private static float anxiety = 0f;

		// Token: 0x04001810 RID: 6160
		private static float gamerate = 1f;

		// Token: 0x04001811 RID: 6161
		private static float waterSine = 0f;

		// Token: 0x04001812 RID: 6162
		public static float WaterSineDirection = 1f;

		// Token: 0x04001813 RID: 6163
		private static float waterCameraY = 0f;

		// Token: 0x04001814 RID: 6164
		private static float waterAlpha = 1f;
	}
}

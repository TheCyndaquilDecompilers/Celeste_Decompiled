using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000360 RID: 864
	public static class GaussianBlur
	{
		// Token: 0x06001B3D RID: 6973 RVA: 0x000B1B84 File Offset: 0x000AFD84
		public static Texture2D Blur(Texture2D texture, VirtualRenderTarget temp, VirtualRenderTarget output, float fade = 0f, bool clear = true, GaussianBlur.Samples samples = GaussianBlur.Samples.Nine, float sampleScale = 1f, GaussianBlur.Direction direction = GaussianBlur.Direction.Both, float alpha = 1f)
		{
			Effect fxGaussianBlur = GFX.FxGaussianBlur;
			string name = GaussianBlur.techniques[(int)samples];
			if (fxGaussianBlur != null)
			{
				fxGaussianBlur.CurrentTechnique = fxGaussianBlur.Techniques[name];
				fxGaussianBlur.Parameters["fade"].SetValue(fade);
				fxGaussianBlur.Parameters["pixel"].SetValue(new Vector2(1f / (float)temp.Width, 0f) * sampleScale);
				Engine.Instance.GraphicsDevice.SetRenderTarget(temp);
				if (clear)
				{
					Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				}
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (direction != GaussianBlur.Direction.Vertical) ? fxGaussianBlur : null);
				Draw.SpriteBatch.Draw(texture, new Rectangle(0, 0, temp.Width, temp.Height), Color.White);
				Draw.SpriteBatch.End();
				fxGaussianBlur.Parameters["pixel"].SetValue(new Vector2(0f, 1f / (float)output.Height) * sampleScale);
				Engine.Instance.GraphicsDevice.SetRenderTarget(output);
				if (clear)
				{
					Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				}
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (direction != GaussianBlur.Direction.Horizontal) ? fxGaussianBlur : null);
				Draw.SpriteBatch.Draw(temp, new Rectangle(0, 0, output.Width, output.Height), Color.White);
				Draw.SpriteBatch.End();
				return output;
			}
			return texture;
		}

		// Token: 0x04001815 RID: 6165
		private static string[] techniques = new string[]
		{
			"GaussianBlur3",
			"GaussianBlur5",
			"GaussianBlur9"
		};

		// Token: 0x02000718 RID: 1816
		public enum Samples
		{
			// Token: 0x04002DAB RID: 11691
			Three,
			// Token: 0x04002DAC RID: 11692
			Five,
			// Token: 0x04002DAD RID: 11693
			Nine
		}

		// Token: 0x02000719 RID: 1817
		public enum Direction
		{
			// Token: 0x04002DAF RID: 11695
			Both,
			// Token: 0x04002DB0 RID: 11696
			Horizontal,
			// Token: 0x04002DB1 RID: 11697
			Vertical
		}
	}
}

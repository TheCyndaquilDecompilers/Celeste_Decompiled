using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000313 RID: 787
	public static class Glitch
	{
		// Token: 0x060018E6 RID: 6374 RVA: 0x0009BFD0 File Offset: 0x0009A1D0
		public static void Apply(VirtualRenderTarget source, float timer, float seed, float amplitude)
		{
			if (Glitch.Value > 0f && !Settings.Instance.DisableFlashes)
			{
				Effect fxGlitch = GFX.FxGlitch;
				Vector2 value = new Vector2((float)Engine.Graphics.GraphicsDevice.Viewport.Width, (float)Engine.Graphics.GraphicsDevice.Viewport.Height);
				fxGlitch.Parameters["dimensions"].SetValue(value);
				fxGlitch.Parameters["amplitude"].SetValue(amplitude);
				fxGlitch.Parameters["minimum"].SetValue(-1f);
				fxGlitch.Parameters["glitch"].SetValue(Glitch.Value);
				fxGlitch.Parameters["timer"].SetValue(timer);
				fxGlitch.Parameters["seed"].SetValue(seed);
				VirtualRenderTarget tempA = GameplayBuffers.TempA;
				Engine.Instance.GraphicsDevice.SetRenderTarget(tempA);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, fxGlitch);
				Draw.SpriteBatch.Draw(source, Vector2.Zero, Color.White);
				Draw.SpriteBatch.End();
				Engine.Instance.GraphicsDevice.SetRenderTarget(source);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, fxGlitch);
				Draw.SpriteBatch.Draw(tempA, Vector2.Zero, Color.White);
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x04001554 RID: 5460
		public static float Value;
	}
}

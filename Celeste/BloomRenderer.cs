using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000366 RID: 870
	public class BloomRenderer : Renderer
	{
		// Token: 0x06001B5B RID: 7003 RVA: 0x000B2C11 File Offset: 0x000B0E11
		public BloomRenderer()
		{
			this.gradient = GFX.Game["util/bloomgradient"];
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x000B2C3C File Offset: 0x000B0E3C
		public void Apply(VirtualRenderTarget target, Scene scene)
		{
			if (this.Strength > 0f)
			{
				VirtualRenderTarget tempA = GameplayBuffers.TempA;
				Texture2D texture = GaussianBlur.Blur(target, GameplayBuffers.TempA, GameplayBuffers.TempB, 0f, true, GaussianBlur.Samples.Nine, 1f, GaussianBlur.Direction.Both, 1f);
				List<Component> components = scene.Tracker.GetComponents<BloomPoint>();
				List<Component> components2 = scene.Tracker.GetComponents<EffectCutout>();
				Engine.Instance.GraphicsDevice.SetRenderTarget(tempA);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				if (this.Base < 1f)
				{
					Camera camera = (scene as Level).Camera;
					Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera.Matrix);
					float num = 1f / (float)this.gradient.Width;
					foreach (Component component in components)
					{
						BloomPoint bloomPoint = component as BloomPoint;
						if (bloomPoint.Visible && bloomPoint.Radius > 0f && bloomPoint.Alpha > 0f)
						{
							this.gradient.DrawCentered(bloomPoint.Entity.Position + bloomPoint.Position, Color.White * bloomPoint.Alpha, bloomPoint.Radius * 2f * num);
						}
					}
					foreach (Component component2 in scene.Tracker.GetComponents<CustomBloom>())
					{
						CustomBloom customBloom = (CustomBloom)component2;
						if (customBloom.Visible && customBloom.OnRenderBloom != null)
						{
							customBloom.OnRenderBloom();
						}
					}
					foreach (Entity entity in scene.Tracker.GetEntities<SeekerBarrier>())
					{
						Draw.Rect(entity.Collider, Color.White);
					}
					Draw.SpriteBatch.End();
					if (components2.Count > 0)
					{
						Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BloomRenderer.CutoutBlendstate, SamplerState.PointClamp, null, null, null, camera.Matrix);
						foreach (Component component3 in components2)
						{
							EffectCutout effectCutout = component3 as EffectCutout;
							if (effectCutout.Visible)
							{
								Draw.Rect((float)effectCutout.Left, (float)effectCutout.Top, (float)(effectCutout.Right - effectCutout.Left), (float)(effectCutout.Bottom - effectCutout.Top), Color.White * (1f - effectCutout.Alpha));
							}
						}
						Draw.SpriteBatch.End();
					}
				}
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				Draw.Rect(-10f, -10f, 340f, 200f, Color.White * this.Base);
				Draw.SpriteBatch.End();
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BloomRenderer.BlurredScreenToMask);
				Draw.SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
				Draw.SpriteBatch.End();
				Engine.Instance.GraphicsDevice.SetRenderTarget(target);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BloomRenderer.AdditiveMaskToScreen);
				int num2 = 0;
				while ((float)num2 < this.Strength)
				{
					float scale = ((float)num2 < this.Strength - 1f) ? 1f : (this.Strength - (float)num2);
					Draw.SpriteBatch.Draw(tempA, Vector2.Zero, Color.White * scale);
					num2++;
				}
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x0400186D RID: 6253
		public float Strength = 1f;

		// Token: 0x0400186E RID: 6254
		public float Base;

		// Token: 0x0400186F RID: 6255
		private MTexture gradient;

		// Token: 0x04001870 RID: 6256
		public static readonly BlendState BlurredScreenToMask = new BlendState
		{
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.Zero,
			ColorBlendFunction = BlendFunction.Add,
			AlphaSourceBlend = Blend.Zero,
			AlphaDestinationBlend = Blend.One,
			AlphaBlendFunction = BlendFunction.Add
		};

		// Token: 0x04001871 RID: 6257
		public static readonly BlendState AdditiveMaskToScreen = new BlendState
		{
			ColorSourceBlend = Blend.SourceAlpha,
			ColorDestinationBlend = Blend.One,
			ColorBlendFunction = BlendFunction.Add,
			AlphaSourceBlend = Blend.Zero,
			AlphaDestinationBlend = Blend.One,
			AlphaBlendFunction = BlendFunction.Add
		};

		// Token: 0x04001872 RID: 6258
		public static readonly BlendState CutoutBlendstate = new BlendState
		{
			ColorSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			AlphaDestinationBlend = Blend.One,
			ColorBlendFunction = BlendFunction.Min,
			AlphaBlendFunction = BlendFunction.Min
		};
	}
}

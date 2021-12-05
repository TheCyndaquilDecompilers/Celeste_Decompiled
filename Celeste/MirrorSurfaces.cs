using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200022F RID: 559
	[Tracked(false)]
	public class MirrorSurfaces : Entity
	{
		// Token: 0x060011CC RID: 4556 RVA: 0x00058DBC File Offset: 0x00056FBC
		public MirrorSurfaces()
		{
			base.Depth = 9490;
			base.Tag = Tags.Global;
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x00058DF8 File Offset: 0x00056FF8
		public void BeforeRender()
		{
			Level level = base.Scene as Level;
			List<Component> components = base.Scene.Tracker.GetComponents<MirrorReflection>();
			List<Component> components2 = base.Scene.Tracker.GetComponents<MirrorSurface>();
			if (this.hasReflections = (components2.Count > 0 && components.Count > 0))
			{
				if (this.target == null)
				{
					this.target = VirtualContent.CreateRenderTarget("mirror-surfaces", 320, 180, false, true, 0);
				}
				Matrix transformMatrix = Matrix.CreateTranslation(32f, 32f, 0f) * level.Camera.Matrix;
				components.Sort((Component a, Component b) => b.Entity.Depth - a.Entity.Depth);
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.MirrorSources);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, transformMatrix);
				foreach (Component component in components)
				{
					MirrorReflection mirrorReflection = (MirrorReflection)component;
					if ((mirrorReflection.Entity.Visible || mirrorReflection.IgnoreEntityVisible) && mirrorReflection.Visible)
					{
						mirrorReflection.IsRendering = true;
						mirrorReflection.Entity.Render();
						mirrorReflection.IsRendering = false;
					}
				}
				Draw.SpriteBatch.End();
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.MirrorMasks);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, null, transformMatrix);
				foreach (Component component2 in components2)
				{
					MirrorSurface mirrorSurface = (MirrorSurface)component2;
					if (mirrorSurface.Visible && mirrorSurface.OnRender != null)
					{
						mirrorSurface.OnRender();
					}
				}
				Draw.SpriteBatch.End();
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.target);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Engine.Graphics.GraphicsDevice.Textures[1] = GameplayBuffers.MirrorSources;
				GFX.FxMirrors.Parameters["pixel"].SetValue(new Vector2(1f / (float)GameplayBuffers.MirrorMasks.Width, 1f / (float)GameplayBuffers.MirrorMasks.Height));
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, GFX.FxMirrors, Matrix.Identity);
				Draw.SpriteBatch.Draw(GameplayBuffers.MirrorMasks, new Vector2(-32f, -32f), Color.White);
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x0005912C File Offset: 0x0005732C
		public override void Render()
		{
			if (this.hasReflections)
			{
				Vector2 position = this.FlooredCamera();
				Draw.SpriteBatch.Draw(this.target, position, Color.White * 0.5f);
			}
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x00059170 File Offset: 0x00057370
		private Vector2 FlooredCamera()
		{
			Vector2 position = (base.Scene as Level).Camera.Position;
			position.X = (float)((int)Math.Floor((double)position.X));
			position.Y = (float)((int)Math.Floor((double)position.Y));
			return position;
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x000591BE File Offset: 0x000573BE
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x000591CD File Offset: 0x000573CD
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x000591DC File Offset: 0x000573DC
		public void Dispose()
		{
			if (this.target != null && !this.target.IsDisposed)
			{
				this.target.Dispose();
			}
			this.target = null;
		}

		// Token: 0x04000D6F RID: 3439
		public const int MaxMirrorOffset = 32;

		// Token: 0x04000D70 RID: 3440
		private bool hasReflections;

		// Token: 0x04000D71 RID: 3441
		private VirtualRenderTarget target;
	}
}

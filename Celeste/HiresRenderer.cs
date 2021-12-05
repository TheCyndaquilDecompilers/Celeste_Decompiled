using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000218 RID: 536
	public class HiresRenderer : Renderer
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600115A RID: 4442 RVA: 0x00055637 File Offset: 0x00053837
		public static VirtualRenderTarget Buffer
		{
			get
			{
				return Celeste.HudTarget;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600115B RID: 4443 RVA: 0x0005563E File Offset: 0x0005383E
		public static bool DrawToBuffer
		{
			get
			{
				return HiresRenderer.Buffer != null && (Engine.ViewWidth < 1920 || Engine.ViewHeight < 1080);
			}
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x00055664 File Offset: 0x00053864
		public static void BeginRender(BlendState blend = null, SamplerState sampler = null)
		{
			if (blend == null)
			{
				blend = BlendState.AlphaBlend;
			}
			if (sampler == null)
			{
				sampler = SamplerState.LinearClamp;
			}
			Matrix transformMatrix = HiresRenderer.DrawToBuffer ? Matrix.Identity : Engine.ScreenMatrix;
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, blend, sampler, DepthStencilState.Default, RasterizerState.CullNone, null, transformMatrix);
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x000556B2 File Offset: 0x000538B2
		public static void EndRender()
		{
			Draw.SpriteBatch.End();
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x000556BE File Offset: 0x000538BE
		public override void BeforeRender(Scene scene)
		{
			if (HiresRenderer.DrawToBuffer)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(HiresRenderer.Buffer);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				this.RenderContent(scene);
			}
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x000556FC File Offset: 0x000538FC
		public override void Render(Scene scene)
		{
			if (HiresRenderer.DrawToBuffer)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Engine.ScreenMatrix);
				Draw.SpriteBatch.Draw(HiresRenderer.Buffer, new Vector2(-1f, -1f), Color.White);
				Draw.SpriteBatch.End();
				return;
			}
			this.RenderContent(scene);
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void RenderContent(Scene scene)
		{
		}
	}
}

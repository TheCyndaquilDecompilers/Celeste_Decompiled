using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002EA RID: 746
	[Tracked(false)]
	public class DustEdges : Entity
	{
		// Token: 0x060016F9 RID: 5881 RVA: 0x0008B3DC File Offset: 0x000895DC
		public DustEdges()
		{
			base.AddTag(Tags.Global | Tags.TransitionUpdate);
			base.Depth = -48;
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x0008B42C File Offset: 0x0008962C
		private void CreateTextures()
		{
			this.DustNoiseFrom = VirtualContent.CreateTexture("dust-noise-a", 128, 72, Color.White);
			this.DustNoiseTo = VirtualContent.CreateTexture("dust-noise-b", 128, 72, Color.White);
			Color[] array = new Color[this.DustNoiseFrom.Width * this.DustNoiseTo.Height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Color(Calc.Random.NextFloat(), 0f, 0f, 0f);
			}
			this.DustNoiseFrom.Texture.SetData<Color>(array);
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new Color(Calc.Random.NextFloat(), 0f, 0f, 0f);
			}
			this.DustNoiseTo.Texture.SetData<Color>(array);
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x0008B518 File Offset: 0x00089718
		public override void Update()
		{
			this.noiseEase = Calc.Approach(this.noiseEase, 1f, Engine.DeltaTime);
			if (this.noiseEase == 1f)
			{
				VirtualTexture dustNoiseFrom = this.DustNoiseFrom;
				this.DustNoiseFrom = this.DustNoiseTo;
				this.DustNoiseTo = dustNoiseFrom;
				this.noiseFromPos = this.noiseToPos;
				this.noiseToPos = new Vector2(Calc.Random.NextFloat(), Calc.Random.NextFloat());
				this.noiseEase = 0f;
			}
			DustEdges.DustGraphicEstabledCounter = 0;
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x0008B5A3 File Offset: 0x000897A3
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x0008B5B2 File Offset: 0x000897B2
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x0008B5C1 File Offset: 0x000897C1
		public override void HandleGraphicsReset()
		{
			base.HandleGraphicsReset();
			this.Dispose();
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0008B5CF File Offset: 0x000897CF
		private void Dispose()
		{
			if (this.DustNoiseFrom != null)
			{
				this.DustNoiseFrom.Dispose();
			}
			if (this.DustNoiseTo != null)
			{
				this.DustNoiseTo.Dispose();
			}
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x0008B5F8 File Offset: 0x000897F8
		public void BeforeRender()
		{
			List<Component> components = base.Scene.Tracker.GetComponents<DustEdge>();
			this.hasDust = (components.Count > 0);
			if (this.hasDust)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, (base.Scene as Level).Camera.Matrix);
				foreach (Component component in components)
				{
					DustEdge dustEdge = component as DustEdge;
					if (dustEdge.Visible && dustEdge.Entity.Visible)
					{
						dustEdge.RenderDust();
					}
				}
				Draw.SpriteBatch.End();
				if (this.DustNoiseFrom == null || this.DustNoiseFrom.IsDisposed)
				{
					this.CreateTextures();
				}
				Vector2 vector = this.FlooredCamera();
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.ResortDust);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Engine.Graphics.GraphicsDevice.Textures[1] = this.DustNoiseFrom.Texture;
				Engine.Graphics.GraphicsDevice.Textures[2] = this.DustNoiseTo.Texture;
				GFX.FxDust.Parameters["colors"].SetValue(DustStyles.Get(base.Scene).EdgeColors);
				GFX.FxDust.Parameters["noiseEase"].SetValue(this.noiseEase);
				GFX.FxDust.Parameters["noiseFromPos"].SetValue(this.noiseFromPos + new Vector2(vector.X / 320f, vector.Y / 180f));
				GFX.FxDust.Parameters["noiseToPos"].SetValue(this.noiseToPos + new Vector2(vector.X / 320f, vector.Y / 180f));
				GFX.FxDust.Parameters["pixel"].SetValue(new Vector2(0.003125f, 0.0055555557f));
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, GFX.FxDust, Matrix.Identity);
				Draw.SpriteBatch.Draw(GameplayBuffers.TempA, Vector2.Zero, Color.White);
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x0008B8D0 File Offset: 0x00089AD0
		public override void Render()
		{
			if (this.hasDust)
			{
				Vector2 position = this.FlooredCamera();
				Draw.SpriteBatch.Draw(GameplayBuffers.ResortDust, position, Color.White);
			}
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x0008B908 File Offset: 0x00089B08
		private Vector2 FlooredCamera()
		{
			Vector2 position = (base.Scene as Level).Camera.Position;
			position.X = (float)((int)Math.Floor((double)position.X));
			position.Y = (float)((int)Math.Floor((double)position.Y));
			return position;
		}

		// Token: 0x0400138B RID: 5003
		public static int DustGraphicEstabledCounter;

		// Token: 0x0400138C RID: 5004
		private bool hasDust;

		// Token: 0x0400138D RID: 5005
		private float noiseEase;

		// Token: 0x0400138E RID: 5006
		private Vector2 noiseFromPos;

		// Token: 0x0400138F RID: 5007
		private Vector2 noiseToPos;

		// Token: 0x04001390 RID: 5008
		private VirtualTexture DustNoiseFrom;

		// Token: 0x04001391 RID: 5009
		private VirtualTexture DustNoiseTo;
	}
}

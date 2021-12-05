using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000253 RID: 595
	public class Ring
	{
		// Token: 0x06001298 RID: 4760 RVA: 0x00062D4C File Offset: 0x00060F4C
		public Ring(float top, float bottom, float distance, float wavy, int steps, Color color, VirtualTexture texture, float texScale = 1f) : this(top, bottom, distance, wavy, steps, color, color, texture, texScale)
		{
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00062D70 File Offset: 0x00060F70
		public Ring(float top, float bottom, float distance, float wavy, int steps, Color topColor, Color botColor, VirtualTexture texture, float texScale = 1f)
		{
			this.Texture = texture;
			this.TopColor = topColor;
			this.BotColor = botColor;
			this.Verts = new VertexPositionColorTexture[steps * 24];
			float y = (1f - texScale) * 0.5f + 0.01f;
			float y2 = 1f - (1f - texScale) * 0.5f;
			for (int i = 0; i < steps; i++)
			{
				float num = (float)(i - 1) / (float)steps;
				float num2 = (float)i / (float)steps;
				Vector2 vector = Calc.AngleToVector(num * 6.2831855f, distance);
				Vector2 vector2 = Calc.AngleToVector(num2 * 6.2831855f, distance);
				float num3 = 0f;
				float num4 = 0f;
				if (wavy > 0f)
				{
					num3 = (float)Math.Sin((double)(num * 6.2831855f * 3f + wavy)) * Math.Abs(top - bottom) * 0.4f;
					num4 = (float)Math.Sin((double)(num2 * 6.2831855f * 3f + wavy)) * Math.Abs(top - bottom) * 0.4f;
				}
				int num5 = i * 6;
				this.Verts[num5].Color = topColor;
				this.Verts[num5].TextureCoordinate = new Vector2(num * texScale, y);
				this.Verts[num5].Position = new Vector3(vector.X, top + num3, vector.Y);
				this.Verts[num5 + 1].Color = topColor;
				this.Verts[num5 + 1].TextureCoordinate = new Vector2(num2 * texScale, y);
				this.Verts[num5 + 1].Position = new Vector3(vector2.X, top + num4, vector2.Y);
				this.Verts[num5 + 2].Color = botColor;
				this.Verts[num5 + 2].TextureCoordinate = new Vector2(num2 * texScale, y2);
				this.Verts[num5 + 2].Position = new Vector3(vector2.X, bottom + num4, vector2.Y);
				this.Verts[num5 + 3].Color = topColor;
				this.Verts[num5 + 3].TextureCoordinate = new Vector2(num * texScale, y);
				this.Verts[num5 + 3].Position = new Vector3(vector.X, top + num3, vector.Y);
				this.Verts[num5 + 4].Color = botColor;
				this.Verts[num5 + 4].TextureCoordinate = new Vector2(num2 * texScale, y2);
				this.Verts[num5 + 4].Position = new Vector3(vector2.X, bottom + num4, vector2.Y);
				this.Verts[num5 + 5].Color = botColor;
				this.Verts[num5 + 5].TextureCoordinate = new Vector2(num * texScale, y2);
				this.Verts[num5 + 5].Position = new Vector3(vector.X, bottom + num3, vector.Y);
			}
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x000630BC File Offset: 0x000612BC
		public void Rotate(float amount)
		{
			for (int i = 0; i < this.Verts.Length; i++)
			{
				VertexPositionColorTexture[] verts = this.Verts;
				int num = i;
				verts[num].TextureCoordinate.X = verts[num].TextureCoordinate.X + amount;
			}
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x000630F8 File Offset: 0x000612F8
		public void Draw(Matrix matrix, RasterizerState rstate = null, float alpha = 1f)
		{
			Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Engine.Graphics.GraphicsDevice.RasterizerState = ((rstate == null) ? MountainModel.CullCCRasterizer : rstate);
			Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
			Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Engine.Graphics.GraphicsDevice.Textures[0] = this.Texture.Texture;
			Color color = this.TopColor * alpha;
			Color color2 = this.BotColor * alpha;
			for (int i = 0; i < this.Verts.Length; i += 6)
			{
				this.Verts[i].Color = color;
				this.Verts[i + 1].Color = color;
				this.Verts[i + 2].Color = color2;
				this.Verts[i + 3].Color = color;
				this.Verts[i + 4].Color = color2;
				this.Verts[i + 5].Color = color2;
			}
			GFX.FxTexture.Parameters["World"].SetValue(matrix);
			foreach (EffectPass effectPass in GFX.FxTexture.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.Verts, 0, this.Verts.Length / 3);
			}
		}

		// Token: 0x04000E77 RID: 3703
		public VertexPositionColorTexture[] Verts;

		// Token: 0x04000E78 RID: 3704
		public VirtualTexture Texture;

		// Token: 0x04000E79 RID: 3705
		public Color TopColor;

		// Token: 0x04000E7A RID: 3706
		public Color BotColor;
	}
}

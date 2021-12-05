using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000252 RID: 594
	public class Skybox
	{
		// Token: 0x06001295 RID: 4757 RVA: 0x00062974 File Offset: 0x00060B74
		public Skybox(VirtualTexture texture, float size = 25f)
		{
			this.Texture = texture;
			this.Verts = new VertexPositionColorTexture[30];
			Vector3 vector = new Vector3(-size, size, -size);
			Vector3 vector2 = new Vector3(size, size, -size);
			Vector3 vector3 = new Vector3(size, size, size);
			Vector3 vector4 = new Vector3(-size, size, size);
			Vector3 vector5 = new Vector3(-size, -size, -size);
			Vector3 vector6 = new Vector3(size, -size, -size);
			Vector3 vector7 = new Vector3(size, -size, size);
			Vector3 vector8 = new Vector3(-size, -size, size);
			MTexture mtexture = new MTexture(texture);
			MTexture subtexture = mtexture.GetSubtexture(0, 0, 820, 820, null);
			MTexture subtexture2 = mtexture.GetSubtexture(820, 0, 820, 820, null);
			MTexture subtexture3 = mtexture.GetSubtexture(2460, 0, 820, 820, null);
			MTexture subtexture4 = mtexture.GetSubtexture(1640, 0, 820, 820, null);
			MTexture subtexture5 = mtexture.GetSubtexture(3280, 0, 819, 820, null);
			this.AddFace(this.Verts, 0, subtexture, vector, vector2, vector3, vector4);
			this.AddFace(this.Verts, 1, subtexture3, vector2, vector, vector5, vector6);
			this.AddFace(this.Verts, 2, subtexture2, vector4, vector3, vector7, vector8);
			this.AddFace(this.Verts, 3, subtexture5, vector3, vector2, vector6, vector7);
			this.AddFace(this.Verts, 4, subtexture4, vector, vector4, vector8, vector5);
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x00062ADC File Offset: 0x00060CDC
		private void AddFace(VertexPositionColorTexture[] verts, int face, MTexture tex, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			float x = (float)(tex.ClipRect.Left + 1) / (float)tex.Texture.Width;
			float y = (float)(tex.ClipRect.Top + 1) / (float)tex.Texture.Height;
			float x2 = (float)(tex.ClipRect.Right - 1) / (float)tex.Texture.Width;
			float y2 = (float)(tex.ClipRect.Bottom - 1) / (float)tex.Texture.Height;
			int num = face * 6;
			verts[num++] = new VertexPositionColorTexture(a, Color.White, new Vector2(x, y));
			verts[num++] = new VertexPositionColorTexture(b, Color.White, new Vector2(x2, y));
			verts[num++] = new VertexPositionColorTexture(c, Color.White, new Vector2(x2, y2));
			verts[num++] = new VertexPositionColorTexture(a, Color.White, new Vector2(x, y));
			verts[num++] = new VertexPositionColorTexture(c, Color.White, new Vector2(x2, y2));
			verts[num++] = new VertexPositionColorTexture(d, Color.White, new Vector2(x, y2));
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00062C30 File Offset: 0x00060E30
		public void Draw(Matrix matrix, Color color)
		{
			Engine.Graphics.GraphicsDevice.RasterizerState = MountainModel.CullNoneRasterizer;
			Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			Engine.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			Engine.Graphics.GraphicsDevice.Textures[0] = this.Texture.Texture;
			for (int i = 0; i < this.Verts.Length; i++)
			{
				this.Verts[i].Color = color;
			}
			GFX.FxTexture.Parameters["World"].SetValue(matrix);
			foreach (EffectPass effectPass in GFX.FxTexture.CurrentTechnique.Passes)
			{
				effectPass.Apply();
				Engine.Graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.Verts, 0, this.Verts.Length / 3);
			}
		}

		// Token: 0x04000E75 RID: 3701
		public VertexPositionColorTexture[] Verts;

		// Token: 0x04000E76 RID: 3702
		public VirtualTexture Texture;
	}
}

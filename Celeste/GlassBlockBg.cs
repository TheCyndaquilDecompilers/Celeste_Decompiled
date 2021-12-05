using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F7 RID: 503
	public class GlassBlockBg : Entity
	{
		// Token: 0x06001078 RID: 4216 RVA: 0x0004A284 File Offset: 0x00048484
		public GlassBlockBg()
		{
			base.Tag = Tags.Global;
			base.Add(new BeforeRenderHook(new Action(this.BeforeRender)));
			base.Add(new DisplacementRenderHook(new Action(this.OnDisplacementRender)));
			base.Depth = -9990;
			List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("particles/stars/");
			for (int i = 0; i < this.stars.Length; i++)
			{
				this.stars[i].Position.X = (float)Calc.Random.Next(320);
				this.stars[i].Position.Y = (float)Calc.Random.Next(180);
				this.stars[i].Texture = Calc.Random.Choose(atlasSubtextures);
				this.stars[i].Color = Calc.Random.Choose(GlassBlockBg.starColors);
				this.stars[i].Scroll = Vector2.One * Calc.Random.NextFloat(0.05f);
			}
			for (int j = 0; j < this.rays.Length; j++)
			{
				this.rays[j].Position.X = (float)Calc.Random.Next(320);
				this.rays[j].Position.Y = (float)Calc.Random.Next(180);
				this.rays[j].Width = Calc.Random.Range(4f, 16f);
				this.rays[j].Length = (float)Calc.Random.Choose(48, 96, 128);
				this.rays[j].Color = Color.White * Calc.Random.Range(0.2f, 0.4f);
			}
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x0004A4F0 File Offset: 0x000486F0
		private void BeforeRender()
		{
			List<Entity> entities = base.Scene.Tracker.GetEntities<GlassBlock>();
			if (this.hasBlocks = (entities.Count > 0))
			{
				Camera camera = (base.Scene as Level).Camera;
				int num = 320;
				int num2 = 180;
				if (this.starsTarget == null)
				{
					this.starsTarget = VirtualContent.CreateRenderTarget("glass-block-surfaces", 320, 180, false, true, 0);
				}
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.starsTarget);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
				Vector2 vector = new Vector2(8f, 8f);
				for (int i = 0; i < this.stars.Length; i++)
				{
					MTexture texture = this.stars[i].Texture;
					Color color = this.stars[i].Color;
					Vector2 scroll = this.stars[i].Scroll;
					Vector2 vector2 = new Vector2
					{
						X = this.Mod(this.stars[i].Position.X - camera.X * (1f - scroll.X), (float)num),
						Y = this.Mod(this.stars[i].Position.Y - camera.Y * (1f - scroll.Y), (float)num2)
					};
					texture.Draw(vector2, vector, color);
					if (vector2.X < vector.X)
					{
						texture.Draw(vector2 + new Vector2((float)num, 0f), vector, color);
					}
					else if (vector2.X > (float)num - vector.X)
					{
						texture.Draw(vector2 - new Vector2((float)num, 0f), vector, color);
					}
					if (vector2.Y < vector.Y)
					{
						texture.Draw(vector2 + new Vector2(0f, (float)num2), vector, color);
					}
					else if (vector2.Y > (float)num2 - vector.Y)
					{
						texture.Draw(vector2 - new Vector2(0f, (float)num2), vector, color);
					}
				}
				Draw.SpriteBatch.End();
				int vertexCount = 0;
				for (int j = 0; j < this.rays.Length; j++)
				{
					Vector2 vector3 = new Vector2
					{
						X = this.Mod(this.rays[j].Position.X - camera.X * 0.9f, (float)num),
						Y = this.Mod(this.rays[j].Position.Y - camera.Y * 0.9f, (float)num2)
					};
					this.DrawRay(vector3, ref vertexCount, ref this.rays[j]);
					if (vector3.X < 64f)
					{
						this.DrawRay(vector3 + new Vector2((float)num, 0f), ref vertexCount, ref this.rays[j]);
					}
					else if (vector3.X > (float)(num - 64))
					{
						this.DrawRay(vector3 - new Vector2((float)num, 0f), ref vertexCount, ref this.rays[j]);
					}
					if (vector3.Y < 64f)
					{
						this.DrawRay(vector3 + new Vector2(0f, (float)num2), ref vertexCount, ref this.rays[j]);
					}
					else if (vector3.Y > (float)(num2 - 64))
					{
						this.DrawRay(vector3 - new Vector2(0f, (float)num2), ref vertexCount, ref this.rays[j]);
					}
				}
				if (this.beamsTarget == null)
				{
					this.beamsTarget = VirtualContent.CreateRenderTarget("glass-block-beams", 320, 180, false, true, 0);
				}
				Engine.Graphics.GraphicsDevice.SetRenderTarget(this.beamsTarget);
				Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
				GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.verts, vertexCount, null, null);
			}
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x0004A96C File Offset: 0x00048B6C
		private void OnDisplacementRender()
		{
			foreach (Entity entity in base.Scene.Tracker.GetEntities<GlassBlock>())
			{
				Draw.Rect(entity.X, entity.Y, entity.Width, entity.Height, new Color(0.5f, 0.5f, 0.2f, 1f));
			}
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0004A9F8 File Offset: 0x00048BF8
		private void DrawRay(Vector2 position, ref int vertex, ref GlassBlockBg.Ray ray)
		{
			Vector2 value = new Vector2(-this.rayNormal.Y, this.rayNormal.X);
			Vector2 value2 = this.rayNormal * ray.Width * 0.5f;
			Vector2 value3 = value * ray.Length * 0.25f * 0.5f;
			Vector2 value4 = value * ray.Length * 0.5f * 0.5f;
			Vector2 v = position + value2 - value3 - value4;
			Vector2 v2 = position - value2 - value3 - value4;
			Vector2 vector = position + value2 - value3;
			Vector2 vector2 = position - value2 - value3;
			Vector2 vector3 = position + value2 + value3;
			Vector2 vector4 = position - value2 + value3;
			Vector2 v3 = position + value2 + value3 + value4;
			Vector2 v4 = position - value2 + value3 + value4;
			Color transparent = Color.Transparent;
			Color color = ray.Color;
			this.Quad(ref vertex, v, vector, vector2, v2, transparent, color, color, transparent);
			this.Quad(ref vertex, vector, vector3, vector4, vector2, color, color, color, color);
			this.Quad(ref vertex, vector3, v3, v4, vector4, color, transparent, transparent, color);
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x0004AB60 File Offset: 0x00048D60
		private void Quad(ref int vertex, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Color c0, Color c1, Color c2, Color c3)
		{
			this.verts[vertex].Position.X = v0.X;
			this.verts[vertex].Position.Y = v0.Y;
			VertexPositionColor[] array = this.verts;
			int num = vertex;
			vertex = num + 1;
			array[num].Color = c0;
			this.verts[vertex].Position.X = v1.X;
			this.verts[vertex].Position.Y = v1.Y;
			VertexPositionColor[] array2 = this.verts;
			num = vertex;
			vertex = num + 1;
			array2[num].Color = c1;
			this.verts[vertex].Position.X = v2.X;
			this.verts[vertex].Position.Y = v2.Y;
			VertexPositionColor[] array3 = this.verts;
			num = vertex;
			vertex = num + 1;
			array3[num].Color = c2;
			this.verts[vertex].Position.X = v0.X;
			this.verts[vertex].Position.Y = v0.Y;
			VertexPositionColor[] array4 = this.verts;
			num = vertex;
			vertex = num + 1;
			array4[num].Color = c0;
			this.verts[vertex].Position.X = v2.X;
			this.verts[vertex].Position.Y = v2.Y;
			VertexPositionColor[] array5 = this.verts;
			num = vertex;
			vertex = num + 1;
			array5[num].Color = c2;
			this.verts[vertex].Position.X = v3.X;
			this.verts[vertex].Position.Y = v3.Y;
			VertexPositionColor[] array6 = this.verts;
			num = vertex;
			vertex = num + 1;
			array6[num].Color = c3;
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0004AD74 File Offset: 0x00048F74
		public override void Render()
		{
			if (!this.hasBlocks)
			{
				return;
			}
			Vector2 position = (base.Scene as Level).Camera.Position;
			List<Entity> entities = base.Scene.Tracker.GetEntities<GlassBlock>();
			foreach (Entity entity in entities)
			{
				Draw.Rect(entity.X, entity.Y, entity.Width, entity.Height, this.bgColor);
			}
			if (this.starsTarget != null && !this.starsTarget.IsDisposed)
			{
				foreach (Entity entity2 in entities)
				{
					Rectangle value = new Rectangle((int)(entity2.X - position.X), (int)(entity2.Y - position.Y), (int)entity2.Width, (int)entity2.Height);
					Draw.SpriteBatch.Draw(this.starsTarget, entity2.Position, new Rectangle?(value), Color.White);
				}
			}
			if (this.beamsTarget != null && !this.beamsTarget.IsDisposed)
			{
				foreach (Entity entity3 in entities)
				{
					Rectangle value2 = new Rectangle((int)(entity3.X - position.X), (int)(entity3.Y - position.Y), (int)entity3.Width, (int)entity3.Height);
					Draw.SpriteBatch.Draw(this.beamsTarget, entity3.Position, new Rectangle?(value2), Color.White);
				}
			}
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x0004AF6C File Offset: 0x0004916C
		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			this.Dispose();
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0004AF7B File Offset: 0x0004917B
		public override void SceneEnd(Scene scene)
		{
			base.SceneEnd(scene);
			this.Dispose();
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x0004AF8C File Offset: 0x0004918C
		public void Dispose()
		{
			if (this.starsTarget != null && !this.starsTarget.IsDisposed)
			{
				this.starsTarget.Dispose();
			}
			if (this.beamsTarget != null && !this.beamsTarget.IsDisposed)
			{
				this.beamsTarget.Dispose();
			}
			this.starsTarget = null;
			this.beamsTarget = null;
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x00026894 File Offset: 0x00024A94
		private float Mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000BF9 RID: 3065
		private static readonly Color[] starColors = new Color[]
		{
			Calc.HexToColor("7f9fba"),
			Calc.HexToColor("9bd1cd"),
			Calc.HexToColor("bacae3")
		};

		// Token: 0x04000BFA RID: 3066
		private const int StarCount = 100;

		// Token: 0x04000BFB RID: 3067
		private const int RayCount = 50;

		// Token: 0x04000BFC RID: 3068
		private GlassBlockBg.Star[] stars = new GlassBlockBg.Star[100];

		// Token: 0x04000BFD RID: 3069
		private GlassBlockBg.Ray[] rays = new GlassBlockBg.Ray[50];

		// Token: 0x04000BFE RID: 3070
		private VertexPositionColor[] verts = new VertexPositionColor[2700];

		// Token: 0x04000BFF RID: 3071
		private Vector2 rayNormal = new Vector2(-5f, -8f).SafeNormalize();

		// Token: 0x04000C00 RID: 3072
		private Color bgColor = Calc.HexToColor("0d2e89");

		// Token: 0x04000C01 RID: 3073
		private VirtualRenderTarget beamsTarget;

		// Token: 0x04000C02 RID: 3074
		private VirtualRenderTarget starsTarget;

		// Token: 0x04000C03 RID: 3075
		private bool hasBlocks;

		// Token: 0x020004F6 RID: 1270
		private struct Star
		{
			// Token: 0x04002466 RID: 9318
			public Vector2 Position;

			// Token: 0x04002467 RID: 9319
			public MTexture Texture;

			// Token: 0x04002468 RID: 9320
			public Color Color;

			// Token: 0x04002469 RID: 9321
			public Vector2 Scroll;
		}

		// Token: 0x020004F7 RID: 1271
		private struct Ray
		{
			// Token: 0x0400246A RID: 9322
			public Vector2 Position;

			// Token: 0x0400246B RID: 9323
			public float Width;

			// Token: 0x0400246C RID: 9324
			public float Length;

			// Token: 0x0400246D RID: 9325
			public Color Color;
		}
	}
}

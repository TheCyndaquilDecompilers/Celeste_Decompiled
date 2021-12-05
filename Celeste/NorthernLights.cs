using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000210 RID: 528
	public class NorthernLights : Backdrop
	{
		// Token: 0x06001127 RID: 4391 RVA: 0x0005342C File Offset: 0x0005162C
		public NorthernLights()
		{
			for (int i = 0; i < 3; i++)
			{
				this.strands.Add(new NorthernLights.Strand());
			}
			for (int j = 0; j < this.particles.Length; j++)
			{
				this.particles[j].Position = new Vector2((float)Calc.Random.Range(0, 320), (float)Calc.Random.Range(0, 180));
				this.particles[j].Speed = (float)Calc.Random.Range(4, 14);
				this.particles[j].Color = Calc.Random.Choose(NorthernLights.colors);
			}
			Color color = Calc.HexToColor("020825");
			Color color2 = Calc.HexToColor("170c2f");
			this.gradient[0] = new VertexPositionColor(new Vector3(0f, 0f, 0f), color);
			this.gradient[1] = new VertexPositionColor(new Vector3(320f, 0f, 0f), color);
			this.gradient[2] = new VertexPositionColor(new Vector3(320f, 180f, 0f), color2);
			this.gradient[3] = new VertexPositionColor(new Vector3(0f, 0f, 0f), color);
			this.gradient[4] = new VertexPositionColor(new Vector3(320f, 180f, 0f), color2);
			this.gradient[5] = new VertexPositionColor(new Vector3(0f, 180f, 0f), color2);
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00053624 File Offset: 0x00051824
		public override void Update(Scene scene)
		{
			if (this.Visible)
			{
				this.timer += Engine.DeltaTime;
				foreach (NorthernLights.Strand strand in this.strands)
				{
					strand.Percent += Engine.DeltaTime / strand.Duration;
					strand.Alpha = Calc.Approach(strand.Alpha, (float)((strand.Percent < 1f) ? 1 : 0), Engine.DeltaTime);
					if (strand.Alpha <= 0f && strand.Percent >= 1f)
					{
						strand.Reset(0f);
					}
					foreach (NorthernLights.Node node in strand.Nodes)
					{
						node.SineOffset += Engine.DeltaTime;
					}
				}
				for (int i = 0; i < this.particles.Length; i++)
				{
					NorthernLights.Particle[] array = this.particles;
					int num = i;
					array[num].Position.Y = array[num].Position.Y + this.particles[i].Speed * Engine.DeltaTime;
				}
			}
			base.Update(scene);
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00053790 File Offset: 0x00051990
		public override void BeforeRender(Scene scene)
		{
			if (this.buffer == null)
			{
				this.buffer = VirtualContent.CreateRenderTarget("northern-lights", 320, 180, false, true, 0);
			}
			int vertexCount = 0;
			foreach (NorthernLights.Strand strand in this.strands)
			{
				NorthernLights.Node node = strand.Nodes[0];
				for (int i = 1; i < strand.Nodes.Count; i++)
				{
					NorthernLights.Node node2 = strand.Nodes[i];
					float num = Math.Min(1f, (float)i / 4f) * this.NorthernLightsAlpha;
					float num2 = Math.Min(1f, (float)(strand.Nodes.Count - i) / 4f) * this.NorthernLightsAlpha;
					float num3 = this.OffsetY + (float)Math.Sin((double)node.SineOffset) * 3f;
					float num4 = this.OffsetY + (float)Math.Sin((double)node2.SineOffset) * 3f;
					this.Set(ref vertexCount, node.Position.X, node.Position.Y + num3, node.TextureOffset, 1f, node.Color * (node.BottomAlpha * strand.Alpha * num));
					this.Set(ref vertexCount, node.Position.X, node.Position.Y - node.Height + num3, node.TextureOffset, 0.05f, node.Color * (node.TopAlpha * strand.Alpha * num));
					this.Set(ref vertexCount, node2.Position.X, node2.Position.Y - node2.Height + num4, node2.TextureOffset, 0.05f, node2.Color * (node2.TopAlpha * strand.Alpha * num2));
					this.Set(ref vertexCount, node.Position.X, node.Position.Y + num3, node.TextureOffset, 1f, node.Color * (node.BottomAlpha * strand.Alpha * num));
					this.Set(ref vertexCount, node2.Position.X, node2.Position.Y - node2.Height + num4, node2.TextureOffset, 0.05f, node2.Color * (node2.TopAlpha * strand.Alpha * num2));
					this.Set(ref vertexCount, node2.Position.X, node2.Position.Y + num4, node2.TextureOffset, 1f, node2.Color * (node2.BottomAlpha * strand.Alpha * num2));
					node = node2;
				}
			}
			Engine.Graphics.GraphicsDevice.SetRenderTarget(this.buffer);
			GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.gradient, this.gradient.Length, null, null);
			Engine.Graphics.GraphicsDevice.Textures[0] = GFX.Misc["northernlights"].Texture.Texture;
			Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
			GFX.DrawVertices<VertexPositionColorTexture>(Matrix.Identity, this.verts, vertexCount, GFX.FxTexture, null);
			bool clear = false;
			GaussianBlur.Blur(this.buffer, GameplayBuffers.TempA, this.buffer, 0f, clear, GaussianBlur.Samples.Five, 0.25f, GaussianBlur.Direction.Vertical, 1f);
			Draw.SpriteBatch.Begin();
			Camera camera = (scene as Level).Camera;
			for (int j = 0; j < this.particles.Length; j++)
			{
				Draw.Rect(new Vector2
				{
					X = this.mod(this.particles[j].Position.X - camera.X * 0.2f, 320f),
					Y = this.mod(this.particles[j].Position.Y - camera.Y * 0.2f, 180f)
				}, 1f, 1f, this.particles[j].Color);
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00053C40 File Offset: 0x00051E40
		public override void Ended(Scene scene)
		{
			if (this.buffer != null)
			{
				this.buffer.Dispose();
			}
			this.buffer = null;
			base.Ended(scene);
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00053C64 File Offset: 0x00051E64
		private void Set(ref int vert, float px, float py, float tx, float ty, Color color)
		{
			this.verts[vert].Color = color;
			this.verts[vert].Position.X = px;
			this.verts[vert].Position.Y = py;
			this.verts[vert].TextureCoordinate.X = tx;
			this.verts[vert].TextureCoordinate.Y = ty;
			vert++;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00053CED File Offset: 0x00051EED
		public override void Render(Scene scene)
		{
			Draw.SpriteBatch.Draw(this.buffer, Vector2.Zero, Color.White);
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00026894 File Offset: 0x00024A94
		private float mod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x04000CD3 RID: 3283
		private static readonly Color[] colors = new Color[]
		{
			Calc.HexToColor("2de079"),
			Calc.HexToColor("62f4f6"),
			Calc.HexToColor("45bc2e"),
			Calc.HexToColor("3856f0")
		};

		// Token: 0x04000CD4 RID: 3284
		private List<NorthernLights.Strand> strands = new List<NorthernLights.Strand>();

		// Token: 0x04000CD5 RID: 3285
		private NorthernLights.Particle[] particles = new NorthernLights.Particle[50];

		// Token: 0x04000CD6 RID: 3286
		private VertexPositionColorTexture[] verts = new VertexPositionColorTexture[1024];

		// Token: 0x04000CD7 RID: 3287
		private VertexPositionColor[] gradient = new VertexPositionColor[6];

		// Token: 0x04000CD8 RID: 3288
		private VirtualRenderTarget buffer;

		// Token: 0x04000CD9 RID: 3289
		private float timer;

		// Token: 0x04000CDA RID: 3290
		public float OffsetY;

		// Token: 0x04000CDB RID: 3291
		public float NorthernLightsAlpha = 1f;

		// Token: 0x0200051B RID: 1307
		private class Strand
		{
			// Token: 0x06002535 RID: 9525 RVA: 0x000F7BBB File Offset: 0x000F5DBB
			public Strand()
			{
				this.Reset(Calc.Random.NextFloat());
			}

			// Token: 0x06002536 RID: 9526 RVA: 0x000F7BE0 File Offset: 0x000F5DE0
			public void Reset(float startPercent)
			{
				this.Percent = startPercent;
				this.Duration = Calc.Random.Range(12f, 32f);
				this.Alpha = 0f;
				this.Nodes.Clear();
				Vector2 vector = new Vector2((float)Calc.Random.Range(-40, 60), (float)Calc.Random.Range(40, 90));
				float num = Calc.Random.NextFloat();
				Color value = Calc.Random.Choose(NorthernLights.colors);
				for (int i = 0; i < 40; i++)
				{
					NorthernLights.Node item = new NorthernLights.Node
					{
						Position = vector,
						TextureOffset = num,
						Height = (float)Calc.Random.Range(10, 80),
						TopAlpha = Calc.Random.Range(0.3f, 0.8f),
						BottomAlpha = Calc.Random.Range(0.5f, 1f),
						SineOffset = Calc.Random.NextFloat() * 6.2831855f,
						Color = Color.Lerp(value, Calc.Random.Choose(NorthernLights.colors), Calc.Random.Range(0f, 0.3f))
					};
					num += Calc.Random.Range(0.02f, 0.2f);
					vector += new Vector2((float)Calc.Random.Range(4, 20), (float)Calc.Random.Range(-15, 15));
					this.Nodes.Add(item);
				}
			}

			// Token: 0x04002509 RID: 9481
			public List<NorthernLights.Node> Nodes = new List<NorthernLights.Node>();

			// Token: 0x0400250A RID: 9482
			public float Duration;

			// Token: 0x0400250B RID: 9483
			public float Percent;

			// Token: 0x0400250C RID: 9484
			public float Alpha;
		}

		// Token: 0x0200051C RID: 1308
		private class Node
		{
			// Token: 0x0400250D RID: 9485
			public Vector2 Position;

			// Token: 0x0400250E RID: 9486
			public float TextureOffset;

			// Token: 0x0400250F RID: 9487
			public float Height;

			// Token: 0x04002510 RID: 9488
			public float TopAlpha;

			// Token: 0x04002511 RID: 9489
			public float BottomAlpha;

			// Token: 0x04002512 RID: 9490
			public float SineOffset;

			// Token: 0x04002513 RID: 9491
			public Color Color;
		}

		// Token: 0x0200051D RID: 1309
		private struct Particle
		{
			// Token: 0x04002514 RID: 9492
			public Vector2 Position;

			// Token: 0x04002515 RID: 9493
			public float Speed;

			// Token: 0x04002516 RID: 9494
			public Color Color;
		}
	}
}

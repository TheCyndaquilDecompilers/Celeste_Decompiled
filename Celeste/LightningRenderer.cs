using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020001F8 RID: 504
	[Tracked(false)]
	public class LightningRenderer : Entity
	{
		// Token: 0x06001083 RID: 4227 RVA: 0x0004B028 File Offset: 0x00049228
		public LightningRenderer()
		{
			base.Tag = (Tags.Global | Tags.TransitionUpdate);
			base.Depth = -1000100;
			this.electricityColorsLerped = new Color[this.electricityColors.Length];
			base.Add(new CustomBloom(new Action(this.OnRenderBloom)));
			base.Add(new BeforeRenderHook(new Action(this.OnBeforeRender)));
			base.Add(this.AmbientSfx = new SoundSource());
			this.AmbientSfx.DisposeOnTransition = false;
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0004B120 File Offset: 0x00049320
		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			for (int i = 0; i < 4; i++)
			{
				this.bolts.Add(new LightningRenderer.Bolt(this.electricityColors[0], 1f, 160, 160));
				this.bolts.Add(new LightningRenderer.Bolt(this.electricityColors[1], 0.35f, 160, 160));
			}
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x0004B196 File Offset: 0x00049396
		public void StartAmbience()
		{
			if (!this.AmbientSfx.Playing)
			{
				this.AmbientSfx.Play("event:/new_content/env/10_electricity", null, 0f);
			}
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x0004B1BC File Offset: 0x000493BC
		public void StopAmbience()
		{
			this.AmbientSfx.Stop(true);
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0004B1CB File Offset: 0x000493CB
		public void Reset()
		{
			this.UpdateSeeds = true;
			this.Fade = 0f;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x0004B1E0 File Offset: 0x000493E0
		public void Track(Lightning block)
		{
			this.list.Add(block);
			if (this.tiles == null)
			{
				this.levelTileBounds = (base.Scene as Level).TileBounds;
				this.tiles = new VirtualMap<bool>(this.levelTileBounds.Width, this.levelTileBounds.Height, false);
			}
			for (int i = (int)block.X / 8; i < ((int)block.X + block.VisualWidth) / 8; i++)
			{
				for (int j = (int)block.Y / 8; j < ((int)block.Y + block.VisualHeight) / 8; j++)
				{
					this.tiles[i - this.levelTileBounds.X, j - this.levelTileBounds.Y] = true;
				}
			}
			this.dirty = true;
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x0004B2AC File Offset: 0x000494AC
		public void Untrack(Lightning block)
		{
			this.list.Remove(block);
			if (this.list.Count <= 0)
			{
				this.tiles = null;
			}
			else
			{
				int num = (int)block.X / 8;
				while ((float)num < block.Right / 8f)
				{
					int num2 = (int)block.Y / 8;
					while ((float)num2 < block.Bottom / 8f)
					{
						this.tiles[num - this.levelTileBounds.X, num2 - this.levelTileBounds.Y] = false;
						num2++;
					}
					num++;
				}
			}
			this.dirty = true;
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x0004B34C File Offset: 0x0004954C
		public override void Update()
		{
			if (this.dirty)
			{
				this.RebuildEdges();
			}
			this.ToggleEdges(false);
			if (this.list.Count > 0)
			{
				foreach (LightningRenderer.Bolt bolt in this.bolts)
				{
					bolt.Update(base.Scene);
				}
				if (this.UpdateSeeds)
				{
					if (base.Scene.OnInterval(0.1f))
					{
						this.edgeSeed = (uint)Calc.Random.Next();
					}
					if (base.Scene.OnInterval(0.7f))
					{
						this.leapSeed = (uint)Calc.Random.Next();
					}
				}
			}
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0004B414 File Offset: 0x00049614
		public void ToggleEdges(bool immediate = false)
		{
			Camera camera = (base.Scene as Level).Camera;
			Rectangle rectangle = new Rectangle((int)camera.Left - 4, (int)camera.Top - 4, (int)(camera.Right - camera.Left) + 8, (int)(camera.Bottom - camera.Top) + 8);
			for (int i = 0; i < this.edges.Count; i++)
			{
				if (immediate)
				{
					this.edges[i].Visible = this.edges[i].InView(ref rectangle);
				}
				else if (!this.edges[i].Visible && base.Scene.OnInterval(0.05f, (float)i * 0.01f) && this.edges[i].InView(ref rectangle))
				{
					this.edges[i].Visible = true;
				}
				else if (this.edges[i].Visible && base.Scene.OnInterval(0.25f, (float)i * 0.01f) && !this.edges[i].InView(ref rectangle))
				{
					this.edges[i].Visible = false;
				}
			}
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x0004B560 File Offset: 0x00049760
		private void RebuildEdges()
		{
			this.dirty = false;
			this.edges.Clear();
			if (this.list.Count > 0)
			{
				Level level = base.Scene as Level;
				int left = level.TileBounds.Left;
				int top = level.TileBounds.Top;
				int right = level.TileBounds.Right;
				int bottom = level.TileBounds.Bottom;
				Point[] array = new Point[]
				{
					new Point(0, -1),
					new Point(0, 1),
					new Point(-1, 0),
					new Point(1, 0)
				};
				foreach (Lightning lightning in this.list)
				{
					int num = (int)lightning.X / 8;
					while ((float)num < lightning.Right / 8f)
					{
						int num2 = (int)lightning.Y / 8;
						while ((float)num2 < lightning.Bottom / 8f)
						{
							foreach (Point point in array)
							{
								Point point2 = new Point(-point.Y, point.X);
								if (!this.Inside(num + point.X, num2 + point.Y) && (!this.Inside(num - point2.X, num2 - point2.Y) || this.Inside(num + point.X - point2.X, num2 + point.Y - point2.Y)))
								{
									Point point3 = new Point(num, num2);
									Point point4 = new Point(num + point2.X, num2 + point2.Y);
									Vector2 value = new Vector2(4f) + new Vector2((float)(point.X - point2.X), (float)(point.Y - point2.Y)) * 4f;
									int num3 = 1;
									while (this.Inside(point4.X, point4.Y) && !this.Inside(point4.X + point.X, point4.Y + point.Y))
									{
										point4.X += point2.X;
										point4.Y += point2.Y;
										num3++;
										if (num3 > 8)
										{
											Vector2 a = new Vector2((float)point3.X, (float)point3.Y) * 8f + value - lightning.Position;
											Vector2 b = new Vector2((float)point4.X, (float)point4.Y) * 8f + value - lightning.Position;
											this.edges.Add(new LightningRenderer.Edge(lightning, a, b));
											num3 = 0;
											point3 = point4;
										}
									}
									if (num3 > 0)
									{
										Vector2 a = new Vector2((float)point3.X, (float)point3.Y) * 8f + value - lightning.Position;
										Vector2 b = new Vector2((float)point4.X, (float)point4.Y) * 8f + value - lightning.Position;
										this.edges.Add(new LightningRenderer.Edge(lightning, a, b));
									}
								}
							}
							num2++;
						}
						num++;
					}
				}
				if (this.edgeVerts == null)
				{
					this.edgeVerts = new VertexPositionColor[1024];
				}
			}
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x0004B960 File Offset: 0x00049B60
		private bool Inside(int tx, int ty)
		{
			return this.tiles[tx - this.levelTileBounds.X, ty - this.levelTileBounds.Y];
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x0004B988 File Offset: 0x00049B88
		private void OnRenderBloom()
		{
			Camera camera = (base.Scene as Level).Camera;
			new Rectangle((int)camera.Left, (int)camera.Top, (int)(camera.Right - camera.Left), (int)(camera.Bottom - camera.Top));
			Color color = Color.White * (0.25f + this.Fade * 0.75f);
			foreach (LightningRenderer.Edge edge in this.edges)
			{
				if (edge.Visible)
				{
					Draw.Line(edge.Parent.Position + edge.A, edge.Parent.Position + edge.B, color, 4f);
				}
			}
			foreach (Lightning lightning in this.list)
			{
				if (lightning.Visible)
				{
					Draw.Rect(lightning.X, lightning.Y, (float)lightning.VisualWidth, (float)lightning.VisualHeight, color);
				}
			}
			if (this.Fade > 0f)
			{
				Level level = base.Scene as Level;
				Draw.Rect(level.Camera.X, level.Camera.Y, 320f, 180f, Color.White * this.Fade);
			}
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x0004BB2C File Offset: 0x00049D2C
		private void OnBeforeRender()
		{
			if (this.list.Count > 0)
			{
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.Lightning);
				Engine.Graphics.GraphicsDevice.Clear(Color.Lerp(Calc.HexToColor("f7b262") * 0.1f, Color.White, this.Fade));
				Draw.SpriteBatch.Begin();
				foreach (LightningRenderer.Bolt bolt in this.bolts)
				{
					bolt.Render();
				}
				Draw.SpriteBatch.End();
			}
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x0004BBF0 File Offset: 0x00049DF0
		public override void Render()
		{
			if (this.list.Count <= 0)
			{
				return;
			}
			Camera camera = (base.Scene as Level).Camera;
			new Rectangle((int)camera.Left, (int)camera.Top, (int)(camera.Right - camera.Left), (int)(camera.Bottom - camera.Top));
			foreach (Lightning lightning in this.list)
			{
				if (lightning.Visible)
				{
					Draw.SpriteBatch.Draw(GameplayBuffers.Lightning, lightning.Position, new Rectangle?(new Rectangle((int)lightning.X, (int)lightning.Y, lightning.VisualWidth, lightning.VisualHeight)), Color.White);
				}
			}
			if (this.edges.Count > 0 && this.DrawEdges)
			{
				for (int i = 0; i < this.electricityColorsLerped.Length; i++)
				{
					this.electricityColorsLerped[i] = Color.Lerp(this.electricityColors[i], Color.White, this.Fade);
				}
				int num = 0;
				uint num2 = this.leapSeed;
				foreach (LightningRenderer.Edge edge in this.edges)
				{
					if (edge.Visible)
					{
						LightningRenderer.DrawSimpleLightning(ref num, ref this.edgeVerts, this.edgeSeed, edge.Parent.Position, edge.A, edge.B, this.electricityColorsLerped[0], 1f + this.Fade * 3f);
						LightningRenderer.DrawSimpleLightning(ref num, ref this.edgeVerts, this.edgeSeed + 1U, edge.Parent.Position, edge.A, edge.B, this.electricityColorsLerped[1], 1f + this.Fade * 3f);
						if (LightningRenderer.PseudoRand(ref num2) % 30U == 0U)
						{
							LightningRenderer.DrawBezierLightning(ref num, ref this.edgeVerts, this.edgeSeed, edge.Parent.Position, edge.A, edge.B, 24f, 10, this.electricityColorsLerped[1]);
						}
					}
				}
				if (num > 0)
				{
					GameplayRenderer.End();
					GFX.DrawVertices<VertexPositionColor>(camera.Matrix, this.edgeVerts, num, null, null);
					GameplayRenderer.Begin();
				}
			}
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x0004BEAC File Offset: 0x0004A0AC
		private static void DrawSimpleLightning(ref int index, ref VertexPositionColor[] verts, uint seed, Vector2 pos, Vector2 a, Vector2 b, Color color, float thickness = 1f)
		{
			seed += (uint)(a.GetHashCode() + b.GetHashCode());
			a += pos;
			b += pos;
			float num = (b - a).Length();
			Vector2 vector = (b - a) / num;
			Vector2 vector2 = vector.TurnRight();
			a += vector2;
			b += vector2;
			Vector2 vector3 = a;
			int num2 = (LightningRenderer.PseudoRand(ref seed) % 2U == 0U) ? -1 : 1;
			float num3 = LightningRenderer.PseudoRandRange(ref seed, 0f, 6.2831855f);
			float num4 = 0f;
			float num5 = (float)index + ((b - a).Length() / 4f + 1f) * 6f;
			while (num5 >= (float)verts.Length)
			{
				Array.Resize<VertexPositionColor>(ref verts, verts.Length * 2);
			}
			int num6 = index;
			while ((float)num6 < num5)
			{
				verts[num6].Color = color;
				num6++;
			}
			do
			{
				float num7 = LightningRenderer.PseudoRandRange(ref seed, 0f, 4f);
				num3 += 0.1f;
				num4 += 4f + num7;
				Vector2 vector4 = a + vector * num4;
				if (num4 < num)
				{
					vector4 += (float)num2 * vector2 * num7 - vector2;
				}
				else
				{
					vector4 = b;
				}
				VertexPositionColor[] array = verts;
				int num8 = index;
				index = num8 + 1;
				array[num8].Position = new Vector3(vector3 - vector2 * thickness, 0f);
				VertexPositionColor[] array2 = verts;
				num8 = index;
				index = num8 + 1;
				array2[num8].Position = new Vector3(vector4 - vector2 * thickness, 0f);
				VertexPositionColor[] array3 = verts;
				num8 = index;
				index = num8 + 1;
				array3[num8].Position = new Vector3(vector4 + vector2 * thickness, 0f);
				VertexPositionColor[] array4 = verts;
				num8 = index;
				index = num8 + 1;
				array4[num8].Position = new Vector3(vector3 - vector2 * thickness, 0f);
				VertexPositionColor[] array5 = verts;
				num8 = index;
				index = num8 + 1;
				array5[num8].Position = new Vector3(vector4 + vector2 * thickness, 0f);
				VertexPositionColor[] array6 = verts;
				num8 = index;
				index = num8 + 1;
				array6[num8].Position = new Vector3(vector3, 0f);
				vector3 = vector4;
				num2 = -num2;
			}
			while (num4 < num);
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x0004C148 File Offset: 0x0004A348
		private static void DrawBezierLightning(ref int index, ref VertexPositionColor[] verts, uint seed, Vector2 pos, Vector2 a, Vector2 b, float anchor, int steps, Color color)
		{
			seed += (uint)(a.GetHashCode() + b.GetHashCode());
			a += pos;
			b += pos;
			Vector2 vector = (b - a).SafeNormalize().TurnRight();
			SimpleCurve simpleCurve = new SimpleCurve(a, b, (b + a) / 2f + vector * anchor);
			int i = index + (steps + 2) * 6;
			while (i >= verts.Length)
			{
				Array.Resize<VertexPositionColor>(ref verts, verts.Length * 2);
			}
			Vector2 vector2 = simpleCurve.GetPoint(0f);
			for (int j = 0; j <= steps; j++)
			{
				Vector2 vector3 = simpleCurve.GetPoint((float)j / (float)steps);
				if (j != steps)
				{
					vector3 += new Vector2(LightningRenderer.PseudoRandRange(ref seed, -2f, 2f), LightningRenderer.PseudoRandRange(ref seed, -2f, 2f));
				}
				verts[index].Position = new Vector3(vector2 - vector, 0f);
				VertexPositionColor[] array = verts;
				int num = index;
				index = num + 1;
				array[num].Color = color;
				verts[index].Position = new Vector3(vector3 - vector, 0f);
				VertexPositionColor[] array2 = verts;
				num = index;
				index = num + 1;
				array2[num].Color = color;
				verts[index].Position = new Vector3(vector3, 0f);
				VertexPositionColor[] array3 = verts;
				num = index;
				index = num + 1;
				array3[num].Color = color;
				verts[index].Position = new Vector3(vector2 - vector, 0f);
				VertexPositionColor[] array4 = verts;
				num = index;
				index = num + 1;
				array4[num].Color = color;
				verts[index].Position = new Vector3(vector3, 0f);
				VertexPositionColor[] array5 = verts;
				num = index;
				index = num + 1;
				array5[num].Color = color;
				verts[index].Position = new Vector3(vector2, 0f);
				VertexPositionColor[] array6 = verts;
				num = index;
				index = num + 1;
				array6[num].Color = color;
				vector2 = vector3;
			}
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x0004C39C File Offset: 0x0004A59C
		private static void DrawFatLightning(uint seed, Vector2 a, Vector2 b, float size, float gap, Color color)
		{
			seed += (uint)(a.GetHashCode() + b.GetHashCode());
			float num = (b - a).Length();
			Vector2 vector = (b - a) / num;
			Vector2 value = vector.TurnRight();
			Vector2 vector2 = a;
			int num2 = 1;
			LightningRenderer.PseudoRandRange(ref seed, 0f, 6.2831855f);
			float num3 = 0f;
			do
			{
				num3 += LightningRenderer.PseudoRandRange(ref seed, 10f, 14f);
				Vector2 vector3 = a + vector * num3;
				if (num3 < num)
				{
					vector3 += (float)num2 * value * LightningRenderer.PseudoRandRange(ref seed, 0f, 6f);
				}
				else
				{
					vector3 = b;
				}
				Vector2 value2 = vector3;
				if (gap > 0f)
				{
					value2 = vector2 + (vector3 - vector2) * (1f - gap);
					Draw.Line(vector2, vector3 + vector, color, size * 0.5f);
				}
				Draw.Line(vector2, value2 + vector, color, size);
				vector2 = vector3;
				num2 = -num2;
			}
			while (num3 < num);
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x0002F85B File Offset: 0x0002DA5B
		private static uint PseudoRand(ref uint seed)
		{
			seed ^= seed << 13;
			seed ^= seed >> 17;
			return seed;
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x0004C4C6 File Offset: 0x0004A6C6
		public static float PseudoRandRange(ref uint seed, float min, float max)
		{
			return min + (LightningRenderer.PseudoRand(ref seed) & 1023U) / 1024f * (max - min);
		}

		// Token: 0x04000C04 RID: 3076
		private List<Lightning> list = new List<Lightning>();

		// Token: 0x04000C05 RID: 3077
		private List<LightningRenderer.Edge> edges = new List<LightningRenderer.Edge>();

		// Token: 0x04000C06 RID: 3078
		private List<LightningRenderer.Bolt> bolts = new List<LightningRenderer.Bolt>();

		// Token: 0x04000C07 RID: 3079
		private VertexPositionColor[] edgeVerts;

		// Token: 0x04000C08 RID: 3080
		private VirtualMap<bool> tiles;

		// Token: 0x04000C09 RID: 3081
		private Rectangle levelTileBounds;

		// Token: 0x04000C0A RID: 3082
		private uint edgeSeed;

		// Token: 0x04000C0B RID: 3083
		private uint leapSeed;

		// Token: 0x04000C0C RID: 3084
		private bool dirty;

		// Token: 0x04000C0D RID: 3085
		private Color[] electricityColors = new Color[]
		{
			Calc.HexToColor("fcf579"),
			Calc.HexToColor("8cf7e2")
		};

		// Token: 0x04000C0E RID: 3086
		private Color[] electricityColorsLerped;

		// Token: 0x04000C0F RID: 3087
		public float Fade;

		// Token: 0x04000C10 RID: 3088
		public bool UpdateSeeds = true;

		// Token: 0x04000C11 RID: 3089
		public const int BoltBufferSize = 160;

		// Token: 0x04000C12 RID: 3090
		public bool DrawEdges = true;

		// Token: 0x04000C13 RID: 3091
		public SoundSource AmbientSfx;

		// Token: 0x020004F8 RID: 1272
		private class Bolt
		{
			// Token: 0x060024AA RID: 9386 RVA: 0x000F4B60 File Offset: 0x000F2D60
			public Bolt(Color color, float scale, int width, int height)
			{
				this.color = color;
				this.width = width;
				this.height = height;
				this.scale = scale;
				this.routine = new Coroutine(this.Run(), true);
			}

			// Token: 0x060024AB RID: 9387 RVA: 0x000F4BAD File Offset: 0x000F2DAD
			public void Update(Scene scene)
			{
				this.routine.Update();
				this.flash = Calc.Approach(this.flash, 0f, Engine.DeltaTime * 2f);
			}

			// Token: 0x060024AC RID: 9388 RVA: 0x000F4BDB File Offset: 0x000F2DDB
			private IEnumerator Run()
			{
				yield return Calc.Random.Range(0f, 4f);
				for (;;)
				{
					List<Vector2> list = new List<Vector2>();
					for (int j = 0; j < 3; j++)
					{
						Vector2 vector = Calc.Random.Choose(new Vector2(0f, (float)Calc.Random.Range(8, this.height - 16)), new Vector2((float)Calc.Random.Range(8, this.width - 16), 0f), new Vector2((float)this.width, (float)Calc.Random.Range(8, this.height - 16)), new Vector2((float)Calc.Random.Range(8, this.width - 16), (float)this.height));
						Vector2 item = (vector.X <= 0f || vector.X >= (float)this.width) ? new Vector2((float)this.width - vector.X, vector.Y) : new Vector2(vector.X, (float)this.height - vector.Y);
						list.Add(vector);
						list.Add(item);
					}
					List<Vector2> list2 = new List<Vector2>();
					for (int k = 0; k < 3; k++)
					{
						list2.Add(new Vector2(Calc.Random.Range(0.25f, 0.75f) * (float)this.width, Calc.Random.Range(0.25f, 0.75f) * (float)this.height));
					}
					this.nodes.Clear();
					foreach (Vector2 vector2 in list)
					{
						this.nodes.Add(vector2);
						this.nodes.Add(list2.ClosestTo(vector2));
					}
					Vector2 item2 = list2[list2.Count - 1];
					foreach (Vector2 vector3 in list2)
					{
						this.nodes.Add(item2);
						this.nodes.Add(vector3);
						item2 = vector3;
					}
					this.flash = 1f;
					this.visible = true;
					this.size = 5f;
					this.gap = 0f;
					this.alpha = 1f;
					int num;
					for (int i = 0; i < 4; i = num + 1)
					{
						this.seed = (uint)Calc.Random.Next();
						yield return 0.1f;
						num = i;
					}
					for (int i = 0; i < 5; i = num + 1)
					{
						if (!Settings.Instance.DisableFlashes)
						{
							this.visible = false;
						}
						yield return 0.05f + (float)i * 0.02f;
						float num2 = (float)i / 5f;
						this.visible = true;
						this.size = (1f - num2) * 5f;
						this.gap = num2;
						this.alpha = 1f - num2;
						this.visible = true;
						this.seed = (uint)Calc.Random.Next();
						yield return 0.025f;
						num = i;
					}
					this.visible = false;
					yield return Calc.Random.Range(4f, 8f);
				}
				yield break;
			}

			// Token: 0x060024AD RID: 9389 RVA: 0x000F4BEC File Offset: 0x000F2DEC
			public void Render()
			{
				if (this.flash > 0f && !Settings.Instance.DisableFlashes)
				{
					Draw.Rect(0f, 0f, (float)this.width, (float)this.height, Color.White * this.flash * 0.15f * this.scale);
				}
				if (this.visible)
				{
					for (int i = 0; i < this.nodes.Count; i += 2)
					{
						LightningRenderer.DrawFatLightning(this.seed, this.nodes[i], this.nodes[i + 1], this.size * this.scale, this.gap, this.color * this.alpha);
					}
				}
			}

			// Token: 0x0400246E RID: 9326
			private List<Vector2> nodes = new List<Vector2>();

			// Token: 0x0400246F RID: 9327
			private Coroutine routine;

			// Token: 0x04002470 RID: 9328
			private bool visible;

			// Token: 0x04002471 RID: 9329
			private float size;

			// Token: 0x04002472 RID: 9330
			private float gap;

			// Token: 0x04002473 RID: 9331
			private float alpha;

			// Token: 0x04002474 RID: 9332
			private uint seed;

			// Token: 0x04002475 RID: 9333
			private float flash;

			// Token: 0x04002476 RID: 9334
			private readonly Color color;

			// Token: 0x04002477 RID: 9335
			private readonly float scale;

			// Token: 0x04002478 RID: 9336
			private readonly int width;

			// Token: 0x04002479 RID: 9337
			private readonly int height;
		}

		// Token: 0x020004F9 RID: 1273
		private class Edge
		{
			// Token: 0x060024AE RID: 9390 RVA: 0x000F4CBC File Offset: 0x000F2EBC
			public Edge(Lightning parent, Vector2 a, Vector2 b)
			{
				this.Parent = parent;
				this.Visible = true;
				this.A = a;
				this.B = b;
				this.Min = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
				this.Max = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
			}

			// Token: 0x060024AF RID: 9391 RVA: 0x000F4D48 File Offset: 0x000F2F48
			public bool InView(ref Rectangle view)
			{
				return (float)view.Left < this.Parent.X + this.Max.X && (float)view.Right > this.Parent.X + this.Min.X && (float)view.Top < this.Parent.Y + this.Max.Y && (float)view.Bottom > this.Parent.Y + this.Min.Y;
			}

			// Token: 0x0400247A RID: 9338
			public Lightning Parent;

			// Token: 0x0400247B RID: 9339
			public bool Visible;

			// Token: 0x0400247C RID: 9340
			public Vector2 A;

			// Token: 0x0400247D RID: 9341
			public Vector2 B;

			// Token: 0x0400247E RID: 9342
			public Vector2 Min;

			// Token: 0x0400247F RID: 9343
			public Vector2 Max;
		}
	}
}

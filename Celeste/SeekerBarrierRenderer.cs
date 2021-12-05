using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001FA RID: 506
	[Tracked(false)]
	public class SeekerBarrierRenderer : Entity
	{
		// Token: 0x0600109A RID: 4250 RVA: 0x0004C670 File Offset: 0x0004A870
		public SeekerBarrierRenderer()
		{
			base.Tag = (Tags.Global | Tags.TransitionUpdate);
			base.Depth = 0;
			base.Add(new CustomBloom(new Action(this.OnRenderBloom)));
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x0004C6D4 File Offset: 0x0004A8D4
		public void Track(SeekerBarrier block)
		{
			this.list.Add(block);
			if (this.tiles == null)
			{
				this.levelTileBounds = (base.Scene as Level).TileBounds;
				this.tiles = new VirtualMap<bool>(this.levelTileBounds.Width, this.levelTileBounds.Height, false);
			}
			int num = (int)block.X / 8;
			while ((float)num < block.Right / 8f)
			{
				int num2 = (int)block.Y / 8;
				while ((float)num2 < block.Bottom / 8f)
				{
					this.tiles[num - this.levelTileBounds.X, num2 - this.levelTileBounds.Y] = true;
					num2++;
				}
				num++;
			}
			this.dirty = true;
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x0004C79C File Offset: 0x0004A99C
		public void Untrack(SeekerBarrier block)
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

		// Token: 0x0600109D RID: 4253 RVA: 0x0004C83A File Offset: 0x0004AA3A
		public override void Update()
		{
			if (this.dirty)
			{
				this.RebuildEdges();
			}
			this.UpdateEdges();
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x0004C850 File Offset: 0x0004AA50
		public void UpdateEdges()
		{
			Camera camera = (base.Scene as Level).Camera;
			Rectangle rectangle = new Rectangle((int)camera.Left - 4, (int)camera.Top - 4, (int)(camera.Right - camera.Left) + 8, (int)(camera.Bottom - camera.Top) + 8);
			for (int i = 0; i < this.edges.Count; i++)
			{
				if (this.edges[i].Visible)
				{
					if (base.Scene.OnInterval(0.25f, (float)i * 0.01f) && !this.edges[i].InView(ref rectangle))
					{
						this.edges[i].Visible = false;
					}
				}
				else if (base.Scene.OnInterval(0.05f, (float)i * 0.01f) && this.edges[i].InView(ref rectangle))
				{
					this.edges[i].Visible = true;
				}
				if (this.edges[i].Visible && (base.Scene.OnInterval(0.05f, (float)i * 0.01f) || this.edges[i].Wave == null))
				{
					this.edges[i].UpdateWave(base.Scene.TimeActive * 3f);
				}
			}
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x0004C9BC File Offset: 0x0004ABBC
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
				foreach (SeekerBarrier seekerBarrier in this.list)
				{
					int num = (int)seekerBarrier.X / 8;
					while ((float)num < seekerBarrier.Right / 8f)
					{
						int num2 = (int)seekerBarrier.Y / 8;
						while ((float)num2 < seekerBarrier.Bottom / 8f)
						{
							foreach (Point point in array)
							{
								Point point2 = new Point(-point.Y, point.X);
								if (!this.Inside(num + point.X, num2 + point.Y) && (!this.Inside(num - point2.X, num2 - point2.Y) || this.Inside(num + point.X - point2.X, num2 + point.Y - point2.Y)))
								{
									Point point3 = new Point(num, num2);
									Point point4 = new Point(num + point2.X, num2 + point2.Y);
									Vector2 value = new Vector2(4f) + new Vector2((float)(point.X - point2.X), (float)(point.Y - point2.Y)) * 4f;
									while (this.Inside(point4.X, point4.Y) && !this.Inside(point4.X + point.X, point4.Y + point.Y))
									{
										point4.X += point2.X;
										point4.Y += point2.Y;
									}
									Vector2 a = new Vector2((float)point3.X, (float)point3.Y) * 8f + value - seekerBarrier.Position;
									Vector2 b = new Vector2((float)point4.X, (float)point4.Y) * 8f + value - seekerBarrier.Position;
									this.edges.Add(new SeekerBarrierRenderer.Edge(seekerBarrier, a, b));
								}
							}
							num2++;
						}
						num++;
					}
				}
			}
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x0004CD08 File Offset: 0x0004AF08
		private bool Inside(int tx, int ty)
		{
			return this.tiles[tx - this.levelTileBounds.X, ty - this.levelTileBounds.Y];
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0004CD30 File Offset: 0x0004AF30
		private void OnRenderBloom()
		{
			Camera camera = (base.Scene as Level).Camera;
			new Rectangle((int)camera.Left, (int)camera.Top, (int)(camera.Right - camera.Left), (int)(camera.Bottom - camera.Top));
			foreach (SeekerBarrier seekerBarrier in this.list)
			{
				if (seekerBarrier.Visible)
				{
					Draw.Rect(seekerBarrier.X, seekerBarrier.Y, seekerBarrier.Width, seekerBarrier.Height, Color.White);
				}
			}
			foreach (SeekerBarrierRenderer.Edge edge in this.edges)
			{
				if (edge.Visible)
				{
					Vector2 value = edge.Parent.Position + edge.A;
					edge.Parent.Position + edge.B;
					int num = 0;
					while ((float)num <= edge.Length)
					{
						Vector2 vector = value + edge.Normal * (float)num;
						Draw.Line(vector, vector + edge.Perpendicular * edge.Wave[num], Color.White);
						num++;
					}
				}
			}
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x0004CEBC File Offset: 0x0004B0BC
		public override void Render()
		{
			if (this.list.Count <= 0)
			{
				return;
			}
			Color color = Color.White * 0.15f;
			Color value = Color.White * 0.25f;
			foreach (SeekerBarrier seekerBarrier in this.list)
			{
				if (seekerBarrier.Visible)
				{
					Draw.Rect(seekerBarrier.Collider, color);
				}
			}
			if (this.edges.Count > 0)
			{
				foreach (SeekerBarrierRenderer.Edge edge in this.edges)
				{
					if (edge.Visible)
					{
						Vector2 value2 = edge.Parent.Position + edge.A;
						edge.Parent.Position + edge.B;
						Color.Lerp(value, Color.White, edge.Parent.Flash);
						int num = 0;
						while ((float)num <= edge.Length)
						{
							Vector2 vector = value2 + edge.Normal * (float)num;
							Draw.Line(vector, vector + edge.Perpendicular * edge.Wave[num], color);
							num++;
						}
					}
				}
			}
		}

		// Token: 0x04000C19 RID: 3097
		private List<SeekerBarrier> list = new List<SeekerBarrier>();

		// Token: 0x04000C1A RID: 3098
		private List<SeekerBarrierRenderer.Edge> edges = new List<SeekerBarrierRenderer.Edge>();

		// Token: 0x04000C1B RID: 3099
		private VirtualMap<bool> tiles;

		// Token: 0x04000C1C RID: 3100
		private Rectangle levelTileBounds;

		// Token: 0x04000C1D RID: 3101
		private bool dirty;

		// Token: 0x020004FC RID: 1276
		private class Edge
		{
			// Token: 0x060024BA RID: 9402 RVA: 0x000F5098 File Offset: 0x000F3298
			public Edge(SeekerBarrier parent, Vector2 a, Vector2 b)
			{
				this.Parent = parent;
				this.Visible = true;
				this.A = a;
				this.B = b;
				this.Min = new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
				this.Max = new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
				this.Normal = (b - a).SafeNormalize();
				this.Perpendicular = -this.Normal.Perpendicular();
				this.Length = (a - b).Length();
			}

			// Token: 0x060024BB RID: 9403 RVA: 0x000F5160 File Offset: 0x000F3360
			public void UpdateWave(float time)
			{
				if (this.Wave == null || (float)this.Wave.Length <= this.Length)
				{
					this.Wave = new float[(int)this.Length + 2];
				}
				int num = 0;
				while ((float)num <= this.Length)
				{
					this.Wave[num] = this.GetWaveAt(time, (float)num, this.Length);
					num++;
				}
			}

			// Token: 0x060024BC RID: 9404 RVA: 0x000F51C4 File Offset: 0x000F33C4
			private float GetWaveAt(float offset, float along, float length)
			{
				if (along <= 1f || along >= length - 1f)
				{
					return 0f;
				}
				if (this.Parent.Solidify >= 1f)
				{
					return 0f;
				}
				float num = offset + along * 0.25f;
				float num2 = (float)(Math.Sin((double)num) * 2.0 + Math.Sin((double)(num * 0.25f)));
				return (1f + num2 * Ease.SineInOut(Calc.YoYo(along / length))) * (1f - this.Parent.Solidify);
			}

			// Token: 0x060024BD RID: 9405 RVA: 0x000F5258 File Offset: 0x000F3458
			public bool InView(ref Rectangle view)
			{
				return (float)view.Left < this.Parent.X + this.Max.X && (float)view.Right > this.Parent.X + this.Min.X && (float)view.Top < this.Parent.Y + this.Max.Y && (float)view.Bottom > this.Parent.Y + this.Min.Y;
			}

			// Token: 0x04002488 RID: 9352
			public SeekerBarrier Parent;

			// Token: 0x04002489 RID: 9353
			public bool Visible;

			// Token: 0x0400248A RID: 9354
			public Vector2 A;

			// Token: 0x0400248B RID: 9355
			public Vector2 B;

			// Token: 0x0400248C RID: 9356
			public Vector2 Min;

			// Token: 0x0400248D RID: 9357
			public Vector2 Max;

			// Token: 0x0400248E RID: 9358
			public Vector2 Normal;

			// Token: 0x0400248F RID: 9359
			public Vector2 Perpendicular;

			// Token: 0x04002490 RID: 9360
			public float[] Wave;

			// Token: 0x04002491 RID: 9361
			public float Length;
		}
	}
}

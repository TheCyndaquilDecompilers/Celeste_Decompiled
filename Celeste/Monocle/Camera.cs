using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x02000123 RID: 291
	public class Camera
	{
		// Token: 0x06000A4D RID: 2637 RVA: 0x000198B4 File Offset: 0x00017AB4
		public Camera()
		{
			this.Viewport = default(Viewport);
			this.Viewport.Width = Engine.Width;
			this.Viewport.Height = Engine.Height;
			this.UpdateMatrices();
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x00019930 File Offset: 0x00017B30
		public Camera(int width, int height)
		{
			this.Viewport = default(Viewport);
			this.Viewport.Width = width;
			this.Viewport.Height = height;
			this.UpdateMatrices();
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x000199A4 File Offset: 0x00017BA4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Camera:\n\tViewport: { ",
				this.Viewport.X,
				", ",
				this.Viewport.Y,
				", ",
				this.Viewport.Width,
				", ",
				this.Viewport.Height,
				" }\n\tPosition: { ",
				this.position.X,
				", ",
				this.position.Y,
				" }\n\tOrigin: { ",
				this.origin.X,
				", ",
				this.origin.Y,
				" }\n\tZoom: { ",
				this.zoom.X,
				", ",
				this.zoom.Y,
				" }\n\tAngle: ",
				this.angle
			});
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x00019AF0 File Offset: 0x00017CF0
		private void UpdateMatrices()
		{
			this.matrix = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-new Vector2((float)((int)Math.Floor((double)this.position.X)), (float)((int)Math.Floor((double)this.position.Y))), 0f)) * Matrix.CreateRotationZ(this.angle) * Matrix.CreateScale(new Vector3(this.zoom, 1f)) * Matrix.CreateTranslation(new Vector3(new Vector2((float)((int)Math.Floor((double)this.origin.X)), (float)((int)Math.Floor((double)this.origin.Y))), 0f));
			this.inverse = Matrix.Invert(this.matrix);
			this.changed = false;
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x00019BCD File Offset: 0x00017DCD
		public void CopyFrom(Camera other)
		{
			this.position = other.position;
			this.origin = other.origin;
			this.angle = other.angle;
			this.zoom = other.zoom;
			this.changed = true;
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000A52 RID: 2642 RVA: 0x00019C06 File Offset: 0x00017E06
		public Matrix Matrix
		{
			get
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				return this.matrix;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000A53 RID: 2643 RVA: 0x00019C1C File Offset: 0x00017E1C
		public Matrix Inverse
		{
			get
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				return this.inverse;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000A54 RID: 2644 RVA: 0x00019C32 File Offset: 0x00017E32
		// (set) Token: 0x06000A55 RID: 2645 RVA: 0x00019C3A File Offset: 0x00017E3A
		public Vector2 Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.changed = true;
				this.position = value;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000A56 RID: 2646 RVA: 0x00019C4A File Offset: 0x00017E4A
		// (set) Token: 0x06000A57 RID: 2647 RVA: 0x00019C52 File Offset: 0x00017E52
		public Vector2 Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.changed = true;
				this.origin = value;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x00019C62 File Offset: 0x00017E62
		// (set) Token: 0x06000A59 RID: 2649 RVA: 0x00019C6F File Offset: 0x00017E6F
		public float X
		{
			get
			{
				return this.position.X;
			}
			set
			{
				this.changed = true;
				this.position.X = value;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x00019C84 File Offset: 0x00017E84
		// (set) Token: 0x06000A5B RID: 2651 RVA: 0x00019C91 File Offset: 0x00017E91
		public float Y
		{
			get
			{
				return this.position.Y;
			}
			set
			{
				this.changed = true;
				this.position.Y = value;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000A5C RID: 2652 RVA: 0x00019CA6 File Offset: 0x00017EA6
		// (set) Token: 0x06000A5D RID: 2653 RVA: 0x00019CB4 File Offset: 0x00017EB4
		public float Zoom
		{
			get
			{
				return this.zoom.X;
			}
			set
			{
				this.changed = true;
				this.zoom.Y = value;
				this.zoom.X = value;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000A5E RID: 2654 RVA: 0x00019CE2 File Offset: 0x00017EE2
		// (set) Token: 0x06000A5F RID: 2655 RVA: 0x00019CEA File Offset: 0x00017EEA
		public float Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				this.changed = true;
				this.angle = value;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000A60 RID: 2656 RVA: 0x00019CFA File Offset: 0x00017EFA
		// (set) Token: 0x06000A61 RID: 2657 RVA: 0x00019D1F File Offset: 0x00017F1F
		public float Left
		{
			get
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				return Vector2.Transform(Vector2.Zero, this.Inverse).X;
			}
			set
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				this.X = Vector2.Transform(Vector2.UnitX * value, this.Matrix).X;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000A62 RID: 2658 RVA: 0x00019D50 File Offset: 0x00017F50
		// (set) Token: 0x06000A63 RID: 2659 RVA: 0x00007EAB File Offset: 0x000060AB
		public float Right
		{
			get
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				return Vector2.Transform(Vector2.UnitX * (float)this.Viewport.Width, this.Inverse).X;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000A64 RID: 2660 RVA: 0x00019D86 File Offset: 0x00017F86
		// (set) Token: 0x06000A65 RID: 2661 RVA: 0x00019DAB File Offset: 0x00017FAB
		public float Top
		{
			get
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				return Vector2.Transform(Vector2.Zero, this.Inverse).Y;
			}
			set
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				this.Y = Vector2.Transform(Vector2.UnitY * value, this.Matrix).Y;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000A66 RID: 2662 RVA: 0x00019DDC File Offset: 0x00017FDC
		// (set) Token: 0x06000A67 RID: 2663 RVA: 0x00007EAB File Offset: 0x000060AB
		public float Bottom
		{
			get
			{
				if (this.changed)
				{
					this.UpdateMatrices();
				}
				return Vector2.Transform(Vector2.UnitY * (float)this.Viewport.Height, this.Inverse).Y;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00019E12 File Offset: 0x00018012
		public void CenterOrigin()
		{
			this.origin = new Vector2((float)this.Viewport.Width / 2f, (float)this.Viewport.Height / 2f);
			this.changed = true;
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00019E4C File Offset: 0x0001804C
		public void RoundPosition()
		{
			this.position.X = (float)Math.Round((double)this.position.X);
			this.position.Y = (float)Math.Round((double)this.position.Y);
			this.changed = true;
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x00019E9A File Offset: 0x0001809A
		public Vector2 ScreenToCamera(Vector2 position)
		{
			return Vector2.Transform(position, this.Inverse);
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x00019EA8 File Offset: 0x000180A8
		public Vector2 CameraToScreen(Vector2 position)
		{
			return Vector2.Transform(position, this.Matrix);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00019EB6 File Offset: 0x000180B6
		public void Approach(Vector2 position, float ease)
		{
			this.Position += (position - this.Position) * ease;
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00019EDC File Offset: 0x000180DC
		public void Approach(Vector2 position, float ease, float maxDistance)
		{
			Vector2 vector = (position - this.Position) * ease;
			if (vector.Length() > maxDistance)
			{
				this.Position += Vector2.Normalize(vector) * maxDistance;
				return;
			}
			this.Position += vector;
		}

		// Token: 0x04000633 RID: 1587
		private Matrix matrix = Matrix.Identity;

		// Token: 0x04000634 RID: 1588
		private Matrix inverse = Matrix.Identity;

		// Token: 0x04000635 RID: 1589
		private bool changed;

		// Token: 0x04000636 RID: 1590
		private Vector2 position = Vector2.Zero;

		// Token: 0x04000637 RID: 1591
		private Vector2 zoom = Vector2.One;

		// Token: 0x04000638 RID: 1592
		private Vector2 origin = Vector2.Zero;

		// Token: 0x04000639 RID: 1593
		private float angle;

		// Token: 0x0400063A RID: 1594
		public Viewport Viewport;
	}
}

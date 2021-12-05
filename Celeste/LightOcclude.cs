using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x0200033A RID: 826
	[Tracked(false)]
	public class LightOcclude : Component
	{
		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060019DE RID: 6622 RVA: 0x000A6700 File Offset: 0x000A4900
		public int Left
		{
			get
			{
				if (this.bounds != null)
				{
					return (int)base.Entity.X + this.bounds.Value.Left;
				}
				return (int)base.Entity.Collider.AbsoluteLeft;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060019DF RID: 6623 RVA: 0x000A674C File Offset: 0x000A494C
		public int Width
		{
			get
			{
				if (this.bounds != null)
				{
					return this.bounds.Value.Width;
				}
				return (int)base.Entity.Collider.Width;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060019E0 RID: 6624 RVA: 0x000A6780 File Offset: 0x000A4980
		public int Top
		{
			get
			{
				if (this.bounds != null)
				{
					return (int)base.Entity.Y + this.bounds.Value.Top;
				}
				return (int)base.Entity.Collider.AbsoluteTop;
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060019E1 RID: 6625 RVA: 0x000A67CC File Offset: 0x000A49CC
		public int Height
		{
			get
			{
				if (this.bounds != null)
				{
					return this.bounds.Value.Height;
				}
				return (int)base.Entity.Collider.Height;
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060019E2 RID: 6626 RVA: 0x000A67FD File Offset: 0x000A49FD
		public int Right
		{
			get
			{
				return this.Left + this.Width;
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060019E3 RID: 6627 RVA: 0x000A680C File Offset: 0x000A4A0C
		public int Bottom
		{
			get
			{
				return this.Top + this.Height;
			}
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x000A681B File Offset: 0x000A4A1B
		public LightOcclude(float alpha = 1f) : base(true, true)
		{
			this.Alpha = alpha;
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x000A6837 File Offset: 0x000A4A37
		public LightOcclude(Rectangle bounds, float alpha = 1f) : this(alpha)
		{
			this.bounds = new Rectangle?(bounds);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x000A684C File Offset: 0x000A4A4C
		public override void Update()
		{
			base.Update();
			bool flag = this.Visible && base.Entity.Visible;
			Rectangle b = new Rectangle(this.Left, this.Top, this.Width, this.Height);
			if (this.lastSize != b || this.lastVisible != flag || this.lastAlpha != this.Alpha)
			{
				this.MakeLightsDirty();
				this.lastVisible = flag;
				this.lastSize = b;
				this.lastAlpha = this.Alpha;
			}
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x000A68DA File Offset: 0x000A4ADA
		public override void Removed(Entity entity)
		{
			this.MakeLightsDirty();
			base.Removed(entity);
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x000A68E9 File Offset: 0x000A4AE9
		public override void EntityRemoved(Scene scene)
		{
			this.MakeLightsDirty();
			base.EntityRemoved(scene);
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x000A68F8 File Offset: 0x000A4AF8
		private void MakeLightsDirty()
		{
			Rectangle rectangle = new Rectangle(this.Left, this.Top, this.Width, this.Height);
			foreach (Component component in base.Entity.Scene.Tracker.GetComponents<VertexLight>())
			{
				VertexLight vertexLight = (VertexLight)component;
				if (!vertexLight.Dirty)
				{
					Rectangle value = new Rectangle((int)(vertexLight.Center.X - vertexLight.EndRadius), (int)(vertexLight.Center.Y - vertexLight.EndRadius), (int)vertexLight.EndRadius * 2, (int)vertexLight.EndRadius * 2);
					if (rectangle.Intersects(value) || this.lastSize.Intersects(value))
					{
						vertexLight.Dirty = true;
					}
				}
			}
		}

		// Token: 0x04001692 RID: 5778
		public float Alpha = 1f;

		// Token: 0x04001693 RID: 5779
		private Rectangle? bounds;

		// Token: 0x04001694 RID: 5780
		public Rectangle RenderBounds;

		// Token: 0x04001695 RID: 5781
		private Rectangle lastSize;

		// Token: 0x04001696 RID: 5782
		private bool lastVisible;

		// Token: 0x04001697 RID: 5783
		private float lastAlpha;
	}
}

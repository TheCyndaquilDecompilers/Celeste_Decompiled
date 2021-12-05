using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000337 RID: 823
	[Tracked(false)]
	public class EffectCutout : Component
	{
		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060019C3 RID: 6595 RVA: 0x000A5F6E File Offset: 0x000A416E
		public int Left
		{
			get
			{
				return (int)base.Entity.Collider.AbsoluteLeft;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060019C4 RID: 6596 RVA: 0x000A5F81 File Offset: 0x000A4181
		public int Right
		{
			get
			{
				return (int)base.Entity.Collider.AbsoluteRight;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060019C5 RID: 6597 RVA: 0x000A5F94 File Offset: 0x000A4194
		public int Top
		{
			get
			{
				return (int)base.Entity.Collider.AbsoluteTop;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060019C6 RID: 6598 RVA: 0x000A5FA7 File Offset: 0x000A41A7
		public int Bottom
		{
			get
			{
				return (int)base.Entity.Collider.AbsoluteBottom;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060019C7 RID: 6599 RVA: 0x000A5FBA File Offset: 0x000A41BA
		public Rectangle Bounds
		{
			get
			{
				return base.Entity.Collider.Bounds;
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x000A5FCC File Offset: 0x000A41CC
		public EffectCutout() : base(true, true)
		{
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x000A5FE4 File Offset: 0x000A41E4
		public override void Update()
		{
			bool flag = this.Visible && base.Entity.Visible;
			Rectangle bounds = this.Bounds;
			if (this.lastSize != bounds || this.lastAlpha != this.Alpha || this.lastVisible != flag)
			{
				this.MakeLightsDirty();
				this.lastSize = bounds;
				this.lastAlpha = this.Alpha;
				this.lastVisible = flag;
			}
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x000A6054 File Offset: 0x000A4254
		public override void Removed(Entity entity)
		{
			this.MakeLightsDirty();
			base.Removed(entity);
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x000A6063 File Offset: 0x000A4263
		public override void EntityRemoved(Scene scene)
		{
			this.MakeLightsDirty();
			base.EntityRemoved(scene);
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x000A6074 File Offset: 0x000A4274
		private void MakeLightsDirty()
		{
			Rectangle bounds = this.Bounds;
			foreach (Component component in base.Entity.Scene.Tracker.GetComponents<VertexLight>())
			{
				VertexLight vertexLight = (VertexLight)component;
				if (!vertexLight.Dirty)
				{
					Rectangle value = new Rectangle((int)(vertexLight.Center.X - vertexLight.EndRadius), (int)(vertexLight.Center.Y - vertexLight.EndRadius), (int)vertexLight.EndRadius * 2, (int)vertexLight.EndRadius * 2);
					if (bounds.Intersects(value) || this.lastSize.Intersects(value))
					{
						vertexLight.Dirty = true;
					}
				}
			}
		}

		// Token: 0x04001680 RID: 5760
		public float Alpha = 1f;

		// Token: 0x04001681 RID: 5761
		private Rectangle lastSize;

		// Token: 0x04001682 RID: 5762
		private bool lastVisible;

		// Token: 0x04001683 RID: 5763
		private float lastAlpha;
	}
}

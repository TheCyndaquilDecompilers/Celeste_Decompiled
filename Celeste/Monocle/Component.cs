using System;

namespace Monocle
{
	// Token: 0x020000F0 RID: 240
	public class Component
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x0000915B File Offset: 0x0000735B
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x00009163 File Offset: 0x00007363
		public Entity Entity { get; private set; }

		// Token: 0x0600060E RID: 1550 RVA: 0x0000916C File Offset: 0x0000736C
		public Component(bool active, bool visible)
		{
			this.Active = active;
			this.Visible = visible;
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00009182 File Offset: 0x00007382
		public virtual void Added(Entity entity)
		{
			this.Entity = entity;
			if (this.Scene != null)
			{
				this.Scene.Tracker.ComponentAdded(this);
			}
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x000091A4 File Offset: 0x000073A4
		public virtual void Removed(Entity entity)
		{
			if (this.Scene != null)
			{
				this.Scene.Tracker.ComponentRemoved(this);
			}
			this.Entity = null;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x000091C6 File Offset: 0x000073C6
		public virtual void EntityAdded(Scene scene)
		{
			scene.Tracker.ComponentAdded(this);
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x000091D4 File Offset: 0x000073D4
		public virtual void EntityRemoved(Scene scene)
		{
			scene.Tracker.ComponentRemoved(this);
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void SceneEnd(Scene scene)
		{
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void EntityAwake()
		{
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Update()
		{
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void Render()
		{
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void DebugRender(Camera camera)
		{
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void HandleGraphicsReset()
		{
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void HandleGraphicsCreate()
		{
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x000091E4 File Offset: 0x000073E4
		public void RemoveSelf()
		{
			if (this.Entity != null)
			{
				this.Entity.Remove(this);
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x000091FA File Offset: 0x000073FA
		public T SceneAs<T>() where T : Scene
		{
			return this.Scene as T;
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0000920C File Offset: 0x0000740C
		public T EntityAs<T>() where T : Entity
		{
			return this.Entity as T;
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x0000921E File Offset: 0x0000741E
		public Scene Scene
		{
			get
			{
				if (this.Entity == null)
				{
					return null;
				}
				return this.Entity.Scene;
			}
		}

		// Token: 0x040004AB RID: 1195
		public bool Active;

		// Token: 0x040004AC RID: 1196
		public bool Visible;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
	// Token: 0x02000113 RID: 275
	public class ComponentList : IEnumerable<Component>, IEnumerable
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x00013128 File Offset: 0x00011328
		// (set) Token: 0x06000881 RID: 2177 RVA: 0x00013130 File Offset: 0x00011330
		public Entity Entity { get; internal set; }

		// Token: 0x06000882 RID: 2178 RVA: 0x0001313C File Offset: 0x0001133C
		internal ComponentList(Entity entity)
		{
			this.Entity = entity;
			this.components = new List<Component>();
			this.toAdd = new List<Component>();
			this.toRemove = new List<Component>();
			this.current = new HashSet<Component>();
			this.adding = new HashSet<Component>();
			this.removing = new HashSet<Component>();
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000883 RID: 2179 RVA: 0x00013198 File Offset: 0x00011398
		// (set) Token: 0x06000884 RID: 2180 RVA: 0x000131A0 File Offset: 0x000113A0
		internal ComponentList.LockModes LockMode
		{
			get
			{
				return this.lockMode;
			}
			set
			{
				this.lockMode = value;
				if (this.toAdd.Count > 0)
				{
					foreach (Component component in this.toAdd)
					{
						if (!this.current.Contains(component))
						{
							this.current.Add(component);
							this.components.Add(component);
							component.Added(this.Entity);
						}
					}
					this.adding.Clear();
					this.toAdd.Clear();
				}
				if (this.toRemove.Count > 0)
				{
					foreach (Component component2 in this.toRemove)
					{
						if (this.current.Contains(component2))
						{
							this.current.Remove(component2);
							this.components.Remove(component2);
							component2.Removed(this.Entity);
						}
					}
					this.removing.Clear();
					this.toRemove.Clear();
				}
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x000132E0 File Offset: 0x000114E0
		public void Add(Component component)
		{
			switch (this.lockMode)
			{
			case ComponentList.LockModes.Open:
				if (!this.current.Contains(component))
				{
					this.current.Add(component);
					this.components.Add(component);
					component.Added(this.Entity);
					return;
				}
				break;
			case ComponentList.LockModes.Locked:
				if (!this.current.Contains(component) && !this.adding.Contains(component))
				{
					this.adding.Add(component);
					this.toAdd.Add(component);
					return;
				}
				break;
			case ComponentList.LockModes.Error:
				throw new Exception("Cannot add or remove Entities at this time!");
			default:
				return;
			}
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x0001337C File Offset: 0x0001157C
		public void Remove(Component component)
		{
			switch (this.lockMode)
			{
			case ComponentList.LockModes.Open:
				if (this.current.Contains(component))
				{
					this.current.Remove(component);
					this.components.Remove(component);
					component.Removed(this.Entity);
					return;
				}
				break;
			case ComponentList.LockModes.Locked:
				if (this.current.Contains(component) && !this.removing.Contains(component))
				{
					this.removing.Add(component);
					this.toRemove.Add(component);
					return;
				}
				break;
			case ComponentList.LockModes.Error:
				throw new Exception("Cannot add or remove Entities at this time!");
			default:
				return;
			}
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x0001341C File Offset: 0x0001161C
		public void Add(IEnumerable<Component> components)
		{
			foreach (Component component in components)
			{
				this.Add(component);
			}
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00013464 File Offset: 0x00011664
		public void Remove(IEnumerable<Component> components)
		{
			foreach (Component component in components)
			{
				this.Remove(component);
			}
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x000134AC File Offset: 0x000116AC
		public void RemoveAll<T>() where T : Component
		{
			this.Remove(this.GetAll<T>());
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x000134BC File Offset: 0x000116BC
		public void Add(params Component[] components)
		{
			foreach (Component component in components)
			{
				this.Add(component);
			}
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x000134E4 File Offset: 0x000116E4
		public void Remove(params Component[] components)
		{
			foreach (Component component in components)
			{
				this.Remove(component);
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600088C RID: 2188 RVA: 0x0001350C File Offset: 0x0001170C
		public int Count
		{
			get
			{
				return this.components.Count;
			}
		}

		// Token: 0x170000CB RID: 203
		public Component this[int index]
		{
			get
			{
				if (index < 0 || index >= this.components.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return this.components[index];
			}
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x0001353F File Offset: 0x0001173F
		public IEnumerator<Component> GetEnumerator()
		{
			return this.components.GetEnumerator();
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x00013551 File Offset: 0x00011751
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x00013559 File Offset: 0x00011759
		public Component[] ToArray()
		{
			return this.components.ToArray<Component>();
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x00013568 File Offset: 0x00011768
		internal void Update()
		{
			this.LockMode = ComponentList.LockModes.Locked;
			foreach (Component component in this.components)
			{
				if (component.Active)
				{
					component.Update();
				}
			}
			this.LockMode = ComponentList.LockModes.Open;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x000135D0 File Offset: 0x000117D0
		internal void Render()
		{
			this.LockMode = ComponentList.LockModes.Error;
			foreach (Component component in this.components)
			{
				if (component.Visible)
				{
					component.Render();
				}
			}
			this.LockMode = ComponentList.LockModes.Open;
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x00013638 File Offset: 0x00011838
		internal void DebugRender(Camera camera)
		{
			this.LockMode = ComponentList.LockModes.Error;
			foreach (Component component in this.components)
			{
				component.DebugRender(camera);
			}
			this.LockMode = ComponentList.LockModes.Open;
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x00013698 File Offset: 0x00011898
		internal void HandleGraphicsReset()
		{
			this.LockMode = ComponentList.LockModes.Error;
			foreach (Component component in this.components)
			{
				component.HandleGraphicsReset();
			}
			this.LockMode = ComponentList.LockModes.Open;
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x000136F8 File Offset: 0x000118F8
		internal void HandleGraphicsCreate()
		{
			this.LockMode = ComponentList.LockModes.Error;
			foreach (Component component in this.components)
			{
				component.HandleGraphicsCreate();
			}
			this.LockMode = ComponentList.LockModes.Open;
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00013758 File Offset: 0x00011958
		public T Get<T>() where T : Component
		{
			foreach (Component component in this.components)
			{
				if (component is T)
				{
					return component as T;
				}
			}
			return default(T);
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x000137C8 File Offset: 0x000119C8
		public IEnumerable<T> GetAll<T>() where T : Component
		{
			foreach (Component component in this.components)
			{
				if (component is T)
				{
					yield return component as T;
				}
			}
			List<Component>.Enumerator enumerator = default(List<Component>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x040005BC RID: 1468
		private List<Component> components;

		// Token: 0x040005BD RID: 1469
		private List<Component> toAdd;

		// Token: 0x040005BE RID: 1470
		private List<Component> toRemove;

		// Token: 0x040005BF RID: 1471
		private HashSet<Component> current;

		// Token: 0x040005C0 RID: 1472
		private HashSet<Component> adding;

		// Token: 0x040005C1 RID: 1473
		private HashSet<Component> removing;

		// Token: 0x040005C2 RID: 1474
		private ComponentList.LockModes lockMode;

		// Token: 0x020003AC RID: 940
		public enum LockModes
		{
			// Token: 0x04001F30 RID: 7984
			Open,
			// Token: 0x04001F31 RID: 7985
			Locked,
			// Token: 0x04001F32 RID: 7986
			Error
		}
	}
}

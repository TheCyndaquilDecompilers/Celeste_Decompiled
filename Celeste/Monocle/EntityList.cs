using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
	// Token: 0x02000114 RID: 276
	public class EntityList : IEnumerable<Entity>, IEnumerable
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x000137D8 File Offset: 0x000119D8
		// (set) Token: 0x06000899 RID: 2201 RVA: 0x000137E0 File Offset: 0x000119E0
		public Scene Scene { get; private set; }

		// Token: 0x0600089A RID: 2202 RVA: 0x000137EC File Offset: 0x000119EC
		internal EntityList(Scene scene)
		{
			this.Scene = scene;
			this.entities = new List<Entity>();
			this.toAdd = new List<Entity>();
			this.toAwake = new List<Entity>();
			this.toRemove = new List<Entity>();
			this.current = new HashSet<Entity>();
			this.adding = new HashSet<Entity>();
			this.removing = new HashSet<Entity>();
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x00013853 File Offset: 0x00011A53
		internal void MarkUnsorted()
		{
			this.unsorted = true;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0001385C File Offset: 0x00011A5C
		public void UpdateLists()
		{
			if (this.toAdd.Count > 0)
			{
				for (int i = 0; i < this.toAdd.Count; i++)
				{
					Entity entity = this.toAdd[i];
					if (!this.current.Contains(entity))
					{
						this.current.Add(entity);
						this.entities.Add(entity);
						if (this.Scene != null)
						{
							this.Scene.TagLists.EntityAdded(entity);
							this.Scene.Tracker.EntityAdded(entity);
							entity.Added(this.Scene);
						}
					}
				}
				this.unsorted = true;
			}
			if (this.toRemove.Count > 0)
			{
				for (int j = 0; j < this.toRemove.Count; j++)
				{
					Entity entity2 = this.toRemove[j];
					if (this.entities.Contains(entity2))
					{
						this.current.Remove(entity2);
						this.entities.Remove(entity2);
						if (this.Scene != null)
						{
							entity2.Removed(this.Scene);
							this.Scene.TagLists.EntityRemoved(entity2);
							this.Scene.Tracker.EntityRemoved(entity2);
							Engine.Pooler.EntityRemoved(entity2);
						}
					}
				}
				this.toRemove.Clear();
				this.removing.Clear();
			}
			if (this.unsorted)
			{
				this.unsorted = false;
				this.entities.Sort(EntityList.CompareDepth);
			}
			if (this.toAdd.Count > 0)
			{
				this.toAwake.AddRange(this.toAdd);
				this.toAdd.Clear();
				this.adding.Clear();
				foreach (Entity entity3 in this.toAwake)
				{
					if (entity3.Scene == this.Scene)
					{
						entity3.Awake(this.Scene);
					}
				}
				this.toAwake.Clear();
			}
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00013A74 File Offset: 0x00011C74
		public void Add(Entity entity)
		{
			if (!this.adding.Contains(entity) && !this.current.Contains(entity))
			{
				this.adding.Add(entity);
				this.toAdd.Add(entity);
			}
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00013AAB File Offset: 0x00011CAB
		public void Remove(Entity entity)
		{
			if (!this.removing.Contains(entity) && this.current.Contains(entity))
			{
				this.removing.Add(entity);
				this.toRemove.Add(entity);
			}
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x00013AE4 File Offset: 0x00011CE4
		public void Add(IEnumerable<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				this.Add(entity);
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x00013B2C File Offset: 0x00011D2C
		public void Remove(IEnumerable<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				this.Remove(entity);
			}
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x00013B74 File Offset: 0x00011D74
		public void Add(params Entity[] entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				this.Add(entities[i]);
			}
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00013B98 File Offset: 0x00011D98
		public void Remove(params Entity[] entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				this.Remove(entities[i]);
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00013BBC File Offset: 0x00011DBC
		public int Count
		{
			get
			{
				return this.entities.Count;
			}
		}

		// Token: 0x170000CE RID: 206
		public Entity this[int index]
		{
			get
			{
				if (index < 0 || index >= this.entities.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return this.entities[index];
			}
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x00013BF0 File Offset: 0x00011DF0
		public int AmountOf<T>() where T : Entity
		{
			int num = 0;
			using (List<Entity>.Enumerator enumerator = this.entities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is T)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00013C4C File Offset: 0x00011E4C
		public T FindFirst<T>() where T : Entity
		{
			foreach (Entity entity in this.entities)
			{
				if (entity is T)
				{
					return entity as T;
				}
			}
			return default(T);
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00013CBC File Offset: 0x00011EBC
		public List<T> FindAll<T>() where T : Entity
		{
			List<T> list = new List<T>();
			foreach (Entity entity in this.entities)
			{
				if (entity is T)
				{
					list.Add(entity as T);
				}
			}
			return list;
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00013D28 File Offset: 0x00011F28
		public void With<T>(Action<T> action) where T : Entity
		{
			foreach (Entity entity in this.entities)
			{
				if (entity is T)
				{
					action(entity as T);
				}
			}
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x00013D90 File Offset: 0x00011F90
		public IEnumerator<Entity> GetEnumerator()
		{
			return this.entities.GetEnumerator();
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00013DA2 File Offset: 0x00011FA2
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x00013DAA File Offset: 0x00011FAA
		public Entity[] ToArray()
		{
			return this.entities.ToArray<Entity>();
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x00013DB8 File Offset: 0x00011FB8
		public bool HasVisibleEntities(int matchTags)
		{
			foreach (Entity entity in this.entities)
			{
				if (entity.Visible && entity.TagCheck(matchTags))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x00013E1C File Offset: 0x0001201C
		internal void Update()
		{
			foreach (Entity entity in this.entities)
			{
				if (entity.Active)
				{
					entity.Update();
				}
			}
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x00013E78 File Offset: 0x00012078
		public void Render()
		{
			foreach (Entity entity in this.entities)
			{
				if (entity.Visible)
				{
					entity.Render();
				}
			}
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x00013ED4 File Offset: 0x000120D4
		public void RenderOnly(int matchTags)
		{
			foreach (Entity entity in this.entities)
			{
				if (entity.Visible && entity.TagCheck(matchTags))
				{
					entity.Render();
				}
			}
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x00013F38 File Offset: 0x00012138
		public void RenderOnlyFullMatch(int matchTags)
		{
			foreach (Entity entity in this.entities)
			{
				if (entity.Visible && entity.TagFullCheck(matchTags))
				{
					entity.Render();
				}
			}
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x00013F9C File Offset: 0x0001219C
		public void RenderExcept(int excludeTags)
		{
			foreach (Entity entity in this.entities)
			{
				if (entity.Visible && !entity.TagCheck(excludeTags))
				{
					entity.Render();
				}
			}
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x00014000 File Offset: 0x00012200
		public void DebugRender(Camera camera)
		{
			foreach (Entity entity in this.entities)
			{
				entity.DebugRender(camera);
			}
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x00014054 File Offset: 0x00012254
		internal void HandleGraphicsReset()
		{
			foreach (Entity entity in this.entities)
			{
				entity.HandleGraphicsReset();
			}
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x000140A4 File Offset: 0x000122A4
		internal void HandleGraphicsCreate()
		{
			foreach (Entity entity in this.entities)
			{
				entity.HandleGraphicsCreate();
			}
		}

		// Token: 0x040005C4 RID: 1476
		private List<Entity> entities;

		// Token: 0x040005C5 RID: 1477
		private List<Entity> toAdd;

		// Token: 0x040005C6 RID: 1478
		private List<Entity> toAwake;

		// Token: 0x040005C7 RID: 1479
		private List<Entity> toRemove;

		// Token: 0x040005C8 RID: 1480
		private HashSet<Entity> current;

		// Token: 0x040005C9 RID: 1481
		private HashSet<Entity> adding;

		// Token: 0x040005CA RID: 1482
		private HashSet<Entity> removing;

		// Token: 0x040005CB RID: 1483
		private bool unsorted;

		// Token: 0x040005CC RID: 1484
		public static Comparison<Entity> CompareDepth = (Entity a, Entity b) => Math.Sign(b.actualDepth - a.actualDepth);
	}
}

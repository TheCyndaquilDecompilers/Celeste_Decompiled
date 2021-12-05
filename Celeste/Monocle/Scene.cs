using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x0200011E RID: 286
	public class Scene : IEnumerable<Entity>, IEnumerable
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0001553C File Offset: 0x0001373C
		// (set) Token: 0x060008F4 RID: 2292 RVA: 0x00015544 File Offset: 0x00013744
		public bool Focused { get; private set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0001554D File Offset: 0x0001374D
		// (set) Token: 0x060008F6 RID: 2294 RVA: 0x00015555 File Offset: 0x00013755
		public EntityList Entities { get; private set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x0001555E File Offset: 0x0001375E
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x00015566 File Offset: 0x00013766
		public TagLists TagLists { get; private set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0001556F File Offset: 0x0001376F
		// (set) Token: 0x060008FA RID: 2298 RVA: 0x00015577 File Offset: 0x00013777
		public RendererList RendererList { get; private set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x00015580 File Offset: 0x00013780
		// (set) Token: 0x060008FC RID: 2300 RVA: 0x00015588 File Offset: 0x00013788
		public Entity HelperEntity { get; private set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060008FD RID: 2301 RVA: 0x00015591 File Offset: 0x00013791
		// (set) Token: 0x060008FE RID: 2302 RVA: 0x00015599 File Offset: 0x00013799
		public Tracker Tracker { get; private set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060008FF RID: 2303 RVA: 0x000155A4 File Offset: 0x000137A4
		// (remove) Token: 0x06000900 RID: 2304 RVA: 0x000155DC File Offset: 0x000137DC
		public event Action OnEndOfFrame;

		// Token: 0x06000901 RID: 2305 RVA: 0x00015614 File Offset: 0x00013814
		public Scene()
		{
			this.Tracker = new Tracker();
			this.Entities = new EntityList(this);
			this.TagLists = new TagLists();
			this.RendererList = new RendererList(this);
			this.actualDepthLookup = new Dictionary<int, double>();
			this.HelperEntity = new Entity();
			this.Entities.Add(this.HelperEntity);
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0001567C File Offset: 0x0001387C
		public virtual void Begin()
		{
			this.Focused = true;
			foreach (Entity entity in this.Entities)
			{
				entity.SceneBegin(this);
			}
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x000156D0 File Offset: 0x000138D0
		public virtual void End()
		{
			this.Focused = false;
			foreach (Entity entity in this.Entities)
			{
				entity.SceneEnd(this);
			}
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00015724 File Offset: 0x00013924
		public virtual void BeforeUpdate()
		{
			if (!this.Paused)
			{
				this.TimeActive += Engine.DeltaTime;
			}
			this.RawTimeActive += Engine.RawDeltaTime;
			this.Entities.UpdateLists();
			this.TagLists.UpdateLists();
			this.RendererList.UpdateLists();
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0001577E File Offset: 0x0001397E
		public virtual void Update()
		{
			if (!this.Paused)
			{
				this.Entities.Update();
				this.RendererList.Update();
			}
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0001579E File Offset: 0x0001399E
		public virtual void AfterUpdate()
		{
			if (this.OnEndOfFrame != null)
			{
				this.OnEndOfFrame();
				this.OnEndOfFrame = null;
			}
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x000157BA File Offset: 0x000139BA
		public virtual void BeforeRender()
		{
			this.RendererList.BeforeRender();
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x000157C7 File Offset: 0x000139C7
		public virtual void Render()
		{
			this.RendererList.Render();
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x000157D4 File Offset: 0x000139D4
		public virtual void AfterRender()
		{
			this.RendererList.AfterRender();
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x000157E1 File Offset: 0x000139E1
		public virtual void HandleGraphicsReset()
		{
			this.Entities.HandleGraphicsReset();
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x000157EE File Offset: 0x000139EE
		public virtual void HandleGraphicsCreate()
		{
			this.Entities.HandleGraphicsCreate();
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void GainFocus()
		{
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x000091E2 File Offset: 0x000073E2
		public virtual void LoseFocus()
		{
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x000157FB File Offset: 0x000139FB
		public bool OnInterval(float interval)
		{
			return (int)((this.TimeActive - Engine.DeltaTime) / interval) < (int)(this.TimeActive / interval);
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00015817 File Offset: 0x00013A17
		public bool OnInterval(float interval, float offset)
		{
			return Math.Floor((double)((this.TimeActive - offset - Engine.DeltaTime) / interval)) < Math.Floor((double)((this.TimeActive - offset) / interval));
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00015841 File Offset: 0x00013A41
		public bool BetweenInterval(float interval)
		{
			return Calc.BetweenInterval(this.TimeActive, interval);
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0001584F File Offset: 0x00013A4F
		public bool OnRawInterval(float interval)
		{
			return (int)((this.RawTimeActive - Engine.RawDeltaTime) / interval) < (int)(this.RawTimeActive / interval);
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x0001586B File Offset: 0x00013A6B
		public bool OnRawInterval(float interval, float offset)
		{
			return Math.Floor((double)((this.RawTimeActive - offset - Engine.RawDeltaTime) / interval)) < Math.Floor((double)((this.RawTimeActive - offset) / interval));
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x00015895 File Offset: 0x00013A95
		public bool BetweenRawInterval(float interval)
		{
			return Calc.BetweenInterval(this.RawTimeActive, interval);
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x000158A4 File Offset: 0x00013AA4
		public bool CollideCheck(Vector2 point, int tag)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x000158F0 File Offset: 0x00013AF0
		public bool CollideCheck(Vector2 from, Vector2 to, int tag)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x0001593C File Offset: 0x00013B3C
		public bool CollideCheck(Rectangle rect, int tag)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00015987 File Offset: 0x00013B87
		public bool CollideCheck(Rectangle rect, Entity entity)
		{
			return entity.Collidable && entity.CollideRect(rect);
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x0001599C File Offset: 0x00013B9C
		public Entity CollideFirst(Vector2 point, int tag)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					return list[i];
				}
			}
			return null;
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x000159F0 File Offset: 0x00013BF0
		public Entity CollideFirst(Vector2 from, Vector2 to, int tag)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					return list[i];
				}
			}
			return null;
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x00015A44 File Offset: 0x00013C44
		public Entity CollideFirst(Rectangle rect, int tag)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					return list[i];
				}
			}
			return null;
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00015A98 File Offset: 0x00013C98
		public void CollideInto(Vector2 point, int tag, List<Entity> hits)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					hits.Add(list[i]);
				}
			}
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00015AF0 File Offset: 0x00013CF0
		public void CollideInto(Vector2 from, Vector2 to, int tag, List<Entity> hits)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					hits.Add(list[i]);
				}
			}
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00015B48 File Offset: 0x00013D48
		public void CollideInto(Rectangle rect, int tag, List<Entity> hits)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					list.Add(list[i]);
				}
			}
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00015BA0 File Offset: 0x00013DA0
		public List<Entity> CollideAll(Vector2 point, int tag)
		{
			List<Entity> list = new List<Entity>();
			this.CollideInto(point, tag, list);
			return list;
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x00015BC0 File Offset: 0x00013DC0
		public List<Entity> CollideAll(Vector2 from, Vector2 to, int tag)
		{
			List<Entity> list = new List<Entity>();
			this.CollideInto(from, to, tag, list);
			return list;
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x00015BE0 File Offset: 0x00013DE0
		public List<Entity> CollideAll(Rectangle rect, int tag)
		{
			List<Entity> list = new List<Entity>();
			this.CollideInto(rect, tag, list);
			return list;
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x00015C00 File Offset: 0x00013E00
		public void CollideDo(Vector2 point, int tag, Action<Entity> action)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					action(list[i]);
				}
			}
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x00015C58 File Offset: 0x00013E58
		public void CollideDo(Vector2 from, Vector2 to, int tag, Action<Entity> action)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					action(list[i]);
				}
			}
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x00015CB0 File Offset: 0x00013EB0
		public void CollideDo(Rectangle rect, int tag, Action<Entity> action)
		{
			List<Entity> list = this.TagLists[tag];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					action(list[i]);
				}
			}
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x00015D08 File Offset: 0x00013F08
		public Vector2 LineWalkCheck(Vector2 from, Vector2 to, int tag, float precision)
		{
			Vector2 vector = to - from;
			vector.Normalize();
			vector *= precision;
			int num = (int)Math.Floor((double)((from - to).Length() / precision));
			Vector2 result = from;
			Vector2 vector2 = from + vector;
			for (int i = 0; i <= num; i++)
			{
				if (this.CollideCheck(vector2, tag))
				{
					return result;
				}
				result = vector2;
				vector2 += vector;
			}
			return to;
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x00015D7C File Offset: 0x00013F7C
		public bool CollideCheck<T>(Vector2 point) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x00015DD8 File Offset: 0x00013FD8
		public bool CollideCheck<T>(Vector2 from, Vector2 to) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x00015E34 File Offset: 0x00014034
		public bool CollideCheck<T>(Rectangle rect) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x00015E90 File Offset: 0x00014090
		public T CollideFirst<T>(Vector2 point) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					return list[i] as T;
				}
			}
			return default(T);
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x00015F04 File Offset: 0x00014104
		public T CollideFirst<T>(Vector2 from, Vector2 to) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					return list[i] as T;
				}
			}
			return default(T);
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x00015F78 File Offset: 0x00014178
		public T CollideFirst<T>(Rectangle rect) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					return list[i] as T;
				}
			}
			return default(T);
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x00015FEC File Offset: 0x000141EC
		public void CollideInto<T>(Vector2 point, List<Entity> hits) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					hits.Add(list[i]);
				}
			}
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x00016050 File Offset: 0x00014250
		public void CollideInto<T>(Vector2 from, Vector2 to, List<Entity> hits) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					hits.Add(list[i]);
				}
			}
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x000160B4 File Offset: 0x000142B4
		public void CollideInto<T>(Rectangle rect, List<Entity> hits) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					list.Add(list[i]);
				}
			}
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00016118 File Offset: 0x00014318
		public void CollideInto<T>(Vector2 point, List<T> hits) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					hits.Add(list[i] as T);
				}
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00016188 File Offset: 0x00014388
		public void CollideInto<T>(Vector2 from, Vector2 to, List<T> hits) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					hits.Add(list[i] as T);
				}
			}
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x000161F8 File Offset: 0x000143F8
		public void CollideInto<T>(Rectangle rect, List<T> hits) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					hits.Add(list[i] as T);
				}
			}
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x00016268 File Offset: 0x00014468
		public List<T> CollideAll<T>(Vector2 point) where T : Entity
		{
			List<T> list = new List<T>();
			this.CollideInto<T>(point, list);
			return list;
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x00016284 File Offset: 0x00014484
		public List<T> CollideAll<T>(Vector2 from, Vector2 to) where T : Entity
		{
			List<T> list = new List<T>();
			this.CollideInto<T>(from, to, list);
			return list;
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x000162A4 File Offset: 0x000144A4
		public List<T> CollideAll<T>(Rectangle rect) where T : Entity
		{
			List<T> list = new List<T>();
			this.CollideInto<T>(rect, list);
			return list;
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x000162C0 File Offset: 0x000144C0
		public void CollideDo<T>(Vector2 point, Action<T> action) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollidePoint(point))
				{
					action(list[i] as T);
				}
			}
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00016330 File Offset: 0x00014530
		public void CollideDo<T>(Vector2 from, Vector2 to, Action<T> action) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideLine(from, to))
				{
					action(list[i] as T);
				}
			}
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x000163A0 File Offset: 0x000145A0
		public void CollideDo<T>(Rectangle rect, Action<T> action) where T : Entity
		{
			List<Entity> list = this.Tracker.Entities[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Collidable && list[i].CollideRect(rect))
				{
					action(list[i] as T);
				}
			}
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x00016410 File Offset: 0x00014610
		public Vector2 LineWalkCheck<T>(Vector2 from, Vector2 to, float precision) where T : Entity
		{
			Vector2 vector = to - from;
			vector.Normalize();
			vector *= precision;
			int num = (int)Math.Floor((double)((from - to).Length() / precision));
			Vector2 result = from;
			Vector2 vector2 = from + vector;
			for (int i = 0; i <= num; i++)
			{
				if (this.CollideCheck<T>(vector2))
				{
					return result;
				}
				result = vector2;
				vector2 += vector;
			}
			return to;
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x00016480 File Offset: 0x00014680
		public bool CollideCheckByComponent<T>(Vector2 point) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x000164E4 File Offset: 0x000146E4
		public bool CollideCheckByComponent<T>(Vector2 from, Vector2 to) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00016548 File Offset: 0x00014748
		public bool CollideCheckByComponent<T>(Rectangle rect) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x000165AC File Offset: 0x000147AC
		public T CollideFirstByComponent<T>(Vector2 point) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
				{
					return list[i] as T;
				}
			}
			return default(T);
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x00016628 File Offset: 0x00014828
		public T CollideFirstByComponent<T>(Vector2 from, Vector2 to) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
				{
					return list[i] as T;
				}
			}
			return default(T);
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x000166A4 File Offset: 0x000148A4
		public T CollideFirstByComponent<T>(Rectangle rect) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
				{
					return list[i] as T;
				}
			}
			return default(T);
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x00016720 File Offset: 0x00014920
		public void CollideIntoByComponent<T>(Vector2 point, List<Component> hits) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
				{
					hits.Add(list[i]);
				}
			}
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x00016790 File Offset: 0x00014990
		public void CollideIntoByComponent<T>(Vector2 from, Vector2 to, List<Component> hits) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
				{
					hits.Add(list[i]);
				}
			}
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00016800 File Offset: 0x00014A00
		public void CollideIntoByComponent<T>(Rectangle rect, List<Component> hits) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
				{
					list.Add(list[i]);
				}
			}
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00016870 File Offset: 0x00014A70
		public void CollideIntoByComponent<T>(Vector2 point, List<T> hits) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
				{
					hits.Add(list[i] as T);
				}
			}
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x000168E8 File Offset: 0x00014AE8
		public void CollideIntoByComponent<T>(Vector2 from, Vector2 to, List<T> hits) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
				{
					hits.Add(list[i] as T);
				}
			}
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00016960 File Offset: 0x00014B60
		public void CollideIntoByComponent<T>(Rectangle rect, List<T> hits) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
				{
					list.Add(list[i] as T);
				}
			}
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x000169DC File Offset: 0x00014BDC
		public List<T> CollideAllByComponent<T>(Vector2 point) where T : Component
		{
			List<T> list = new List<T>();
			this.CollideIntoByComponent<T>(point, list);
			return list;
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x000169F8 File Offset: 0x00014BF8
		public List<T> CollideAllByComponent<T>(Vector2 from, Vector2 to) where T : Component
		{
			List<T> list = new List<T>();
			this.CollideIntoByComponent<T>(from, to, list);
			return list;
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x00016A18 File Offset: 0x00014C18
		public List<T> CollideAllByComponent<T>(Rectangle rect) where T : Component
		{
			List<T> list = new List<T>();
			this.CollideIntoByComponent<T>(rect, list);
			return list;
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00016A34 File Offset: 0x00014C34
		public void CollideDoByComponent<T>(Vector2 point, Action<T> action) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
				{
					action(list[i] as T);
				}
			}
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00016AAC File Offset: 0x00014CAC
		public void CollideDoByComponent<T>(Vector2 from, Vector2 to, Action<T> action) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
				{
					action(list[i] as T);
				}
			}
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x00016B24 File Offset: 0x00014D24
		public void CollideDoByComponent<T>(Rectangle rect, Action<T> action) where T : Component
		{
			List<Component> list = this.Tracker.Components[typeof(T)];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
				{
					action(list[i] as T);
				}
			}
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x00016B9C File Offset: 0x00014D9C
		public Vector2 LineWalkCheckByComponent<T>(Vector2 from, Vector2 to, float precision) where T : Component
		{
			Vector2 vector = to - from;
			vector.Normalize();
			vector *= precision;
			int num = (int)Math.Floor((double)((from - to).Length() / precision));
			Vector2 result = from;
			Vector2 vector2 = from + vector;
			for (int i = 0; i <= num; i++)
			{
				if (this.CollideCheckByComponent<T>(vector2))
				{
					return result;
				}
				result = vector2;
				vector2 += vector;
			}
			return to;
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x00016C0C File Offset: 0x00014E0C
		internal void SetActualDepth(Entity entity)
		{
			double num = 0.0;
			if (this.actualDepthLookup.TryGetValue(entity.depth, out num))
			{
				Dictionary<int, double> dictionary = this.actualDepthLookup;
				int depth = entity.depth;
				dictionary[depth] += 9.999999974752427E-07;
			}
			else
			{
				this.actualDepthLookup.Add(entity.depth, 9.999999974752427E-07);
			}
			entity.actualDepth = (double)entity.depth - num;
			this.Entities.MarkUnsorted();
			for (int i = 0; i < BitTag.TotalTags; i++)
			{
				if (entity.TagCheck(1 << i))
				{
					this.TagLists.MarkUnsorted(i);
				}
			}
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x00016CC0 File Offset: 0x00014EC0
		public T CreateAndAdd<T>() where T : Entity, new()
		{
			T t = Engine.Pooler.Create<T>();
			this.Add(t);
			return t;
		}

		// Token: 0x170000D6 RID: 214
		public List<Entity> this[BitTag tag]
		{
			get
			{
				return this.TagLists[tag.ID];
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x00016CF8 File Offset: 0x00014EF8
		public void Add(Entity entity)
		{
			this.Entities.Add(entity);
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x00016D06 File Offset: 0x00014F06
		public void Remove(Entity entity)
		{
			this.Entities.Remove(entity);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x00016D14 File Offset: 0x00014F14
		public void Add(IEnumerable<Entity> entities)
		{
			this.Entities.Add(entities);
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00016D22 File Offset: 0x00014F22
		public void Remove(IEnumerable<Entity> entities)
		{
			this.Entities.Remove(entities);
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x00016D30 File Offset: 0x00014F30
		public void Add(params Entity[] entities)
		{
			this.Entities.Add(entities);
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00016D3E File Offset: 0x00014F3E
		public void Remove(params Entity[] entities)
		{
			this.Entities.Remove(entities);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00016D4C File Offset: 0x00014F4C
		public IEnumerator<Entity> GetEnumerator()
		{
			return this.Entities.GetEnumerator();
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00016D59 File Offset: 0x00014F59
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00016D64 File Offset: 0x00014F64
		public List<Entity> GetEntitiesByTagMask(int mask)
		{
			List<Entity> list = new List<Entity>();
			foreach (Entity entity in this.Entities)
			{
				if ((entity.Tag & mask) != 0)
				{
					list.Add(entity);
				}
			}
			return list;
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00016DC4 File Offset: 0x00014FC4
		public List<Entity> GetEntitiesExcludingTagMask(int mask)
		{
			List<Entity> list = new List<Entity>();
			foreach (Entity entity in this.Entities)
			{
				if ((entity.Tag & mask) == 0)
				{
					list.Add(entity);
				}
			}
			return list;
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x00016E24 File Offset: 0x00015024
		public void Add(Renderer renderer)
		{
			this.RendererList.Add(renderer);
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00016E32 File Offset: 0x00015032
		public void Remove(Renderer renderer)
		{
			this.RendererList.Remove(renderer);
		}

		// Token: 0x0400060C RID: 1548
		public bool Paused;

		// Token: 0x0400060D RID: 1549
		public float TimeActive;

		// Token: 0x0400060E RID: 1550
		public float RawTimeActive;

		// Token: 0x04000615 RID: 1557
		private Dictionary<int, double> actualDepthLookup;
	}
}

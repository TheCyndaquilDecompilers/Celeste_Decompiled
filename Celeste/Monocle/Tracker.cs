using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x02000138 RID: 312
	public class Tracker
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x0001FAE0 File Offset: 0x0001DCE0
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x0001FAE7 File Offset: 0x0001DCE7
		public static Dictionary<Type, List<Type>> TrackedEntityTypes { get; private set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x0001FAEF File Offset: 0x0001DCEF
		// (set) Token: 0x06000B42 RID: 2882 RVA: 0x0001FAF6 File Offset: 0x0001DCF6
		public static Dictionary<Type, List<Type>> TrackedComponentTypes { get; private set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000B43 RID: 2883 RVA: 0x0001FAFE File Offset: 0x0001DCFE
		// (set) Token: 0x06000B44 RID: 2884 RVA: 0x0001FB05 File Offset: 0x0001DD05
		public static HashSet<Type> StoredEntityTypes { get; private set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000B45 RID: 2885 RVA: 0x0001FB0D File Offset: 0x0001DD0D
		// (set) Token: 0x06000B46 RID: 2886 RVA: 0x0001FB14 File Offset: 0x0001DD14
		public static HashSet<Type> StoredComponentTypes { get; private set; }

		// Token: 0x06000B47 RID: 2887 RVA: 0x0001FB1C File Offset: 0x0001DD1C
		public static void Initialize()
		{
			Tracker.TrackedEntityTypes = new Dictionary<Type, List<Type>>();
			Tracker.TrackedComponentTypes = new Dictionary<Type, List<Type>>();
			Tracker.StoredEntityTypes = new HashSet<Type>();
			Tracker.StoredComponentTypes = new HashSet<Type>();
			foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(Tracked), false);
				if (customAttributes.Length != 0)
				{
					bool inherited = (customAttributes[0] as Tracked).Inherited;
					if (typeof(Entity).IsAssignableFrom(type))
					{
						if (!type.IsAbstract)
						{
							if (!Tracker.TrackedEntityTypes.ContainsKey(type))
							{
								Tracker.TrackedEntityTypes.Add(type, new List<Type>());
							}
							Tracker.TrackedEntityTypes[type].Add(type);
						}
						Tracker.StoredEntityTypes.Add(type);
						if (!inherited)
						{
							goto IL_216;
						}
						using (List<Type>.Enumerator enumerator = Tracker.GetSubclasses(type).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Type type2 = enumerator.Current;
								if (!type2.IsAbstract)
								{
									if (!Tracker.TrackedEntityTypes.ContainsKey(type2))
									{
										Tracker.TrackedEntityTypes.Add(type2, new List<Type>());
									}
									Tracker.TrackedEntityTypes[type2].Add(type);
								}
							}
							goto IL_216;
						}
					}
					if (typeof(Component).IsAssignableFrom(type))
					{
						if (!type.IsAbstract)
						{
							if (!Tracker.TrackedComponentTypes.ContainsKey(type))
							{
								Tracker.TrackedComponentTypes.Add(type, new List<Type>());
							}
							Tracker.TrackedComponentTypes[type].Add(type);
						}
						Tracker.StoredComponentTypes.Add(type);
						if (!inherited)
						{
							goto IL_216;
						}
						using (List<Type>.Enumerator enumerator = Tracker.GetSubclasses(type).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Type type3 = enumerator.Current;
								if (!type3.IsAbstract)
								{
									if (!Tracker.TrackedComponentTypes.ContainsKey(type3))
									{
										Tracker.TrackedComponentTypes.Add(type3, new List<Type>());
									}
									Tracker.TrackedComponentTypes[type3].Add(type);
								}
							}
							goto IL_216;
						}
					}
					throw new Exception("Type '" + type.Name + "' cannot be Tracked because it does not derive from Entity or Component");
				}
				IL_216:;
			}
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0001FD68 File Offset: 0x0001DF68
		private static List<Type> GetSubclasses(Type type)
		{
			List<Type> list = new List<Type>();
			foreach (Type type2 in Assembly.GetEntryAssembly().GetTypes())
			{
				if (type != type2 && type.IsAssignableFrom(type2))
				{
					list.Add(type2);
				}
			}
			return list;
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0001FDB2 File Offset: 0x0001DFB2
		// (set) Token: 0x06000B4A RID: 2890 RVA: 0x0001FDBA File Offset: 0x0001DFBA
		public Dictionary<Type, List<Entity>> Entities { get; private set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000B4B RID: 2891 RVA: 0x0001FDC3 File Offset: 0x0001DFC3
		// (set) Token: 0x06000B4C RID: 2892 RVA: 0x0001FDCB File Offset: 0x0001DFCB
		public Dictionary<Type, List<Component>> Components { get; private set; }

		// Token: 0x06000B4D RID: 2893 RVA: 0x0001FDD4 File Offset: 0x0001DFD4
		public Tracker()
		{
			this.Entities = new Dictionary<Type, List<Entity>>(Tracker.TrackedEntityTypes.Count);
			foreach (Type key in Tracker.StoredEntityTypes)
			{
				this.Entities.Add(key, new List<Entity>());
			}
			this.Components = new Dictionary<Type, List<Component>>(Tracker.TrackedComponentTypes.Count);
			foreach (Type key2 in Tracker.StoredComponentTypes)
			{
				this.Components.Add(key2, new List<Component>());
			}
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x0001FEAC File Offset: 0x0001E0AC
		public bool IsEntityTracked<T>() where T : Entity
		{
			return this.Entities.ContainsKey(typeof(T));
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x0001FEC3 File Offset: 0x0001E0C3
		public bool IsComponentTracked<T>() where T : Component
		{
			return this.Components.ContainsKey(typeof(T));
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x0001FEDC File Offset: 0x0001E0DC
		public T GetEntity<T>() where T : Entity
		{
			List<Entity> list = this.Entities[typeof(T)];
			if (list.Count == 0)
			{
				return default(T);
			}
			return list[0] as T;
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x0001FF24 File Offset: 0x0001E124
		public T GetNearestEntity<T>(Vector2 nearestTo) where T : Entity
		{
			List<Entity> entities = this.GetEntities<T>();
			T t = default(T);
			float num = 0f;
			foreach (Entity entity in entities)
			{
				T t2 = (T)((object)entity);
				float num2 = Vector2.DistanceSquared(nearestTo, t2.Position);
				if (t == null || num2 < num)
				{
					t = t2;
					num = num2;
				}
			}
			return t;
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x0001FFAC File Offset: 0x0001E1AC
		public List<Entity> GetEntities<T>() where T : Entity
		{
			return this.Entities[typeof(T)];
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0001FFC3 File Offset: 0x0001E1C3
		public List<Entity> GetEntitiesCopy<T>() where T : Entity
		{
			return new List<Entity>(this.GetEntities<T>());
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0001FFD0 File Offset: 0x0001E1D0
		public IEnumerator<T> EnumerateEntities<T>() where T : Entity
		{
			foreach (Entity entity in this.Entities[typeof(T)])
			{
				yield return entity as T;
			}
			List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0001FFDF File Offset: 0x0001E1DF
		public int CountEntities<T>() where T : Entity
		{
			return this.Entities[typeof(T)].Count;
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0001FFFC File Offset: 0x0001E1FC
		public T GetComponent<T>() where T : Component
		{
			List<Component> list = this.Components[typeof(T)];
			if (list.Count == 0)
			{
				return default(T);
			}
			return list[0] as T;
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x00020044 File Offset: 0x0001E244
		public T GetNearestComponent<T>(Vector2 nearestTo) where T : Component
		{
			List<Component> components = this.GetComponents<T>();
			T t = default(T);
			float num = 0f;
			foreach (Component component in components)
			{
				T t2 = (T)((object)component);
				float num2 = Vector2.DistanceSquared(nearestTo, t2.Entity.Position);
				if (t == null || num2 < num)
				{
					t = t2;
					num = num2;
				}
			}
			return t;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x000200D0 File Offset: 0x0001E2D0
		public List<Component> GetComponents<T>() where T : Component
		{
			return this.Components[typeof(T)];
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x000200E7 File Offset: 0x0001E2E7
		public List<Component> GetComponentsCopy<T>() where T : Component
		{
			return new List<Component>(this.GetComponents<T>());
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x000200F4 File Offset: 0x0001E2F4
		public IEnumerator<T> EnumerateComponents<T>() where T : Component
		{
			foreach (Component component in this.Components[typeof(T)])
			{
				yield return component as T;
			}
			List<Component>.Enumerator enumerator = default(List<Component>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x00020103 File Offset: 0x0001E303
		public int CountComponents<T>() where T : Component
		{
			return this.Components[typeof(T)].Count;
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x00020120 File Offset: 0x0001E320
		internal void EntityAdded(Entity entity)
		{
			Type type = entity.GetType();
			List<Type> list;
			if (Tracker.TrackedEntityTypes.TryGetValue(type, out list))
			{
				foreach (Type key in list)
				{
					this.Entities[key].Add(entity);
				}
			}
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x00020190 File Offset: 0x0001E390
		internal void EntityRemoved(Entity entity)
		{
			Type type = entity.GetType();
			List<Type> list;
			if (Tracker.TrackedEntityTypes.TryGetValue(type, out list))
			{
				foreach (Type key in list)
				{
					this.Entities[key].Remove(entity);
				}
			}
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x00020200 File Offset: 0x0001E400
		internal void ComponentAdded(Component component)
		{
			Type type = component.GetType();
			List<Type> list;
			if (Tracker.TrackedComponentTypes.TryGetValue(type, out list))
			{
				foreach (Type key in list)
				{
					this.Components[key].Add(component);
				}
			}
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x00020270 File Offset: 0x0001E470
		internal void ComponentRemoved(Component component)
		{
			Type type = component.GetType();
			List<Type> list;
			if (Tracker.TrackedComponentTypes.TryGetValue(type, out list))
			{
				foreach (Type key in list)
				{
					this.Components[key].Remove(component);
				}
			}
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x000202E0 File Offset: 0x0001E4E0
		public void LogEntities()
		{
			foreach (KeyValuePair<Type, List<Entity>> keyValuePair in this.Entities)
			{
				string obj = keyValuePair.Key.Name + " : " + keyValuePair.Value.Count;
				Engine.Commands.Log(obj);
			}
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00020360 File Offset: 0x0001E560
		public void LogComponents()
		{
			foreach (KeyValuePair<Type, List<Component>> keyValuePair in this.Components)
			{
				string obj = keyValuePair.Key.Name + " : " + keyValuePair.Value.Count;
				Engine.Commands.Log(obj);
			}
		}
	}
}

using System;
using System.Collections.Generic;

namespace Monocle
{
	// Token: 0x02000120 RID: 288
	public static class Cache
	{
		// Token: 0x0600095E RID: 2398 RVA: 0x00016ED8 File Offset: 0x000150D8
		private static void Init<T>() where T : Entity, new()
		{
			if (Cache.cache == null)
			{
				Cache.cache = new Dictionary<Type, Stack<Entity>>();
			}
			if (!Cache.cache.ContainsKey(typeof(T)))
			{
				Cache.cache.Add(typeof(T), new Stack<Entity>());
			}
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x00016F25 File Offset: 0x00015125
		public static void Store<T>(T instance) where T : Entity, new()
		{
			Cache.Init<T>();
			Cache.cache[typeof(T)].Push(instance);
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00016F4C File Offset: 0x0001514C
		public static T Create<T>() where T : Entity, new()
		{
			Cache.Init<T>();
			if (Cache.cache[typeof(T)].Count > 0)
			{
				return Cache.cache[typeof(T)].Pop() as T;
			}
			return Activator.CreateInstance<T>();
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x00016FA3 File Offset: 0x000151A3
		public static void Clear<T>() where T : Entity, new()
		{
			if (Cache.cache != null && Cache.cache.ContainsKey(typeof(T)))
			{
				Cache.cache[typeof(T)].Clear();
			}
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x00016FDC File Offset: 0x000151DC
		public static void ClearAll()
		{
			if (Cache.cache != null)
			{
				foreach (KeyValuePair<Type, Stack<Entity>> keyValuePair in Cache.cache)
				{
					keyValuePair.Value.Clear();
				}
			}
		}

		// Token: 0x0400061D RID: 1565
		public static Dictionary<Type, Stack<Entity>> cache;
	}
}

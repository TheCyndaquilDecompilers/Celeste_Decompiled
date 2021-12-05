using System;
using System.Collections.Generic;
using System.Reflection;

namespace Monocle
{
	// Token: 0x02000131 RID: 305
	public class Pooler
	{
		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0001E486 File Offset: 0x0001C686
		// (set) Token: 0x06000B0B RID: 2827 RVA: 0x0001E48E File Offset: 0x0001C68E
		internal Dictionary<Type, Queue<Entity>> Pools { get; private set; }

		// Token: 0x06000B0C RID: 2828 RVA: 0x0001E498 File Offset: 0x0001C698
		public Pooler()
		{
			this.Pools = new Dictionary<Type, Queue<Entity>>();
			foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
			{
				if (type.GetCustomAttributes(typeof(Pooled), false).Length != 0)
				{
					if (!typeof(Entity).IsAssignableFrom(type))
					{
						throw new Exception("Type '" + type.Name + "' cannot be Pooled because it doesn't derive from Entity");
					}
					if (type.GetConstructor(Type.EmptyTypes) == null)
					{
						throw new Exception("Type '" + type.Name + "' cannot be Pooled because it doesn't have a parameterless constructor");
					}
					this.Pools.Add(type, new Queue<Entity>());
				}
			}
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x0001E55C File Offset: 0x0001C75C
		public T Create<T>() where T : Entity, new()
		{
			if (!this.Pools.ContainsKey(typeof(T)))
			{
				return Activator.CreateInstance<T>();
			}
			Queue<Entity> queue = this.Pools[typeof(T)];
			if (queue.Count == 0)
			{
				return Activator.CreateInstance<T>();
			}
			return queue.Dequeue() as T;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x0001E5BC File Offset: 0x0001C7BC
		internal void EntityRemoved(Entity entity)
		{
			Type type = entity.GetType();
			if (this.Pools.ContainsKey(type))
			{
				this.Pools[type].Enqueue(entity);
			}
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x0001E5F0 File Offset: 0x0001C7F0
		public void Log()
		{
			if (this.Pools.Count == 0)
			{
				Engine.Commands.Log("No Entity types are marked as Pooled!");
			}
			foreach (KeyValuePair<Type, Queue<Entity>> keyValuePair in this.Pools)
			{
				string obj = keyValuePair.Key.Name + " : " + keyValuePair.Value.Count;
				Engine.Commands.Log(obj);
			}
		}
	}
}

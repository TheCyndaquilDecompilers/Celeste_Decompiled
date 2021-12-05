using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste
{
	// Token: 0x020001BD RID: 445
	public static class SpawnManager
	{
		// Token: 0x06000F56 RID: 3926 RVA: 0x0003E468 File Offset: 0x0003C668
		public static void Init()
		{
			foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
			{
				if (type.GetCustomAttribute(typeof(SpawnableAttribute)) != null)
				{
					foreach (MethodInfo methodInfo in type.GetMethods())
					{
						SpawnerAttribute spawnerAttribute = methodInfo.GetCustomAttribute(typeof(SpawnerAttribute)) as SpawnerAttribute;
						if (methodInfo.IsStatic && spawnerAttribute != null)
						{
							string name = spawnerAttribute.Name;
							if (name == null)
							{
								name = type.Name;
							}
							SpawnManager.SpawnActions.Add(name, (Spawn)methodInfo.CreateDelegate(typeof(Spawn)));
						}
					}
				}
			}
		}

		// Token: 0x04000ABB RID: 2747
		public static Dictionary<string, Spawn> SpawnActions = new Dictionary<string, Spawn>(StringComparer.InvariantCultureIgnoreCase);
	}
}

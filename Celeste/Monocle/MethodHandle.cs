using System;
using System.Reflection;

namespace Monocle
{
	// Token: 0x0200012C RID: 300
	public class MethodHandle<T> where T : Entity
	{
		// Token: 0x06000ADB RID: 2779 RVA: 0x0001D241 File Offset: 0x0001B441
		public MethodHandle(string methodName)
		{
			this.info = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0001D261 File Offset: 0x0001B461
		public void Call(T instance)
		{
			this.info.Invoke(instance, null);
		}

		// Token: 0x04000686 RID: 1670
		private MethodInfo info;
	}
}
